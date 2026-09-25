(function (global) {
    'use strict';

    function dateRangesOverlap(start1, end1, start2, end2) {
        return String(start1) <= String(end2) && String(end1) >= String(start2);
    }

    // Compatibility for a page already compiled before this workflow was extracted.
    // The rebuilt Razor view no longer relies on this global.
    if (typeof global.rangesOverlap !== 'function') {
        global.rangesOverlap = dateRangesOverlap;
    }

    function create(options) {
        // This module is referenced near the top of the Razor view so it is available to
        // the calendar script, while the host layout/fallback may provide jQuery later.
        // Resolve jQuery when the workflow is created instead of capturing an undefined
        // value when this file is first evaluated.
        var $ = global.jQuery;
        if (!$) {
            throw new Error('The Direct IPD calendar workflow requires jQuery.');
        }
        var state = options.state;
        var selection = {
            active: false,
            role: 'Patient',
            patientRooms: [],
            attendantRooms: [],
            attendantFrom: null,
            attendantTo: null,
            modalUrl: null
        };
        var pendingAvailabilityChecks = {};

        function rowsFromResponse(data) {
            if (typeof data === 'string') {
                try { data = JSON.parse(data); } catch (ignore) { return []; }
            }
            if (Array.isArray(data)) return data;
            if (!data) return [];
            var candidates = [data.detail, data.data, data.rows, data.result];
            for (var index = 0; index < candidates.length; index++) {
                if (typeof candidates[index] === 'string') {
                    try { candidates[index] = JSON.parse(candidates[index]); } catch (ignore) { candidates[index] = []; }
                }
                if (Array.isArray(candidates[index])) return candidates[index];
            }
            return [];
        }

        function roomIdFromAvailability(row) {
            return String((row && (row.Roomid || row.roomid || row.RoomID)) || '').toLowerCase();
        }

        function roomsForRole() {
            return selection.role === 'Attendant' ? selection.attendantRooms : selection.patientRooms;
        }

        function allRooms() {
            return selection.patientRooms.concat(selection.attendantRooms);
        }

        function validateContinuity(rooms) {
            var ordered = rooms.slice().sort(function (a, b) { return a.fromdate.localeCompare(b.fromdate); });
            for (var index = 1; index < ordered.length; index++) {
                if (ordered[index].fromdate <= ordered[index - 1].todate) return 'Selected date ranges cannot overlap.';
                if (ordered[index].fromdate !== options.addDaysToIso(ordered[index - 1].todate, 1)) {
                    return 'The selected rooms must form one continuous stay without missing dates.';
                }
            }
            return '';
        }

        function buildEvents() {
            return allRooms().map(function (segment, index) {
                return {
                    id: 'direct-ipd::' + segment.role + '::' + index + '::' + segment.roomid + '::' + segment.fromdate,
                    text: segment.role + ' · ' + segment.roomnumber,
                    start: segment.fromdate + 'T00:00:00',
                    end: options.isoEndExclusive(segment.todate),
                    resource: segment.roomid,
                    moveDisabled: true,
                    resizeDisabled: true,
                    cssClass: 'nlm-direct-selection-event',
                    tags: { isDirectSelection: true, directSegment: segment }
                };
            });
        }

        function renderEvent(args) {
            var tags = args.data.tags;
            if (!tags || !tags.isDirectSelection) return false;
            var segment = tags.directSegment || {};
            args.data.cssClass = 'nlm-direct-selection-event';
            args.data.backColor = 'rgba(34,197,94,.14)';
            args.data.borderColor = '#16a34a';
            args.data.fontColor = '#166534';
            args.data.html = '<div style="height:100%;display:flex;align-items:center;padding:0 8px;box-sizing:border-box;font-size:10px;font-weight:800;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;">' +
                $('<div>').text((segment.role || 'Patient') + ' · ' + (segment.roomnumber || 'Room')).html() + '</div>';
            return true;
        }

        function renderSelection() {
            var rooms = roomsForRole();
            $('#btnStartDirectIPD').toggleClass('active', selection.active && selection.role === 'Patient');
            $('#directIpdSelectionBar').toggleClass('hidden', !selection.active);
            $('#directIpdSelectionTitle').text(selection.role === 'Attendant'
                ? 'Select attendant room (' + selection.attendantFrom + ' to ' + selection.attendantTo + ')'
                : 'Patient room selections');
            $('#btnContinueDirectIPD').text(selection.role === 'Attendant' ? 'Return to Direct IPD' : 'Continue to Direct IPD');
            $('#directIpdSelectionList').html(rooms.map(function (room, index) {
                return '<span class="nlm-direct-selection-chip">' +
                    $('<div>').text((room.roomnumber || 'Room') + ' · ' + room.fromdate + ' to ' + room.todate).html() +
                    '<button type="button" data-direct-room-index="' + index + '" aria-label="Remove selection"><i class="fa-solid fa-xmark"></i></button></span>';
            }).join('') || '<span style="font-size:11px;color:#64748b;">Drag horizontally across an available room row.</span>');
            options.refreshScheduler();
        }

        function start() {
            selection.active = true;
            selection.role = 'Patient';
            selection.patientRooms = [];
            selection.attendantRooms = [];
            renderSelection();
            options.showToast('Direct IPD selection started. Drag across an available room and date range.', 'info');
        }

        function commitRange(room, fromDate, toDate) {
            var roomType = options.byId(state.roomTypes, room.roomTypeId);
            var segment = {
                roomid: room.id,
                roomtypeid: room.roomTypeId,
                roomnumber: room.roomNumber || room.roomCode || room.id,
                roomtypename: roomType ? roomType.name : '',
                role: selection.role,
                fromdate: fromDate,
                todate: toDate
            };
            var rooms = roomsForRole().concat([segment]);
            var continuityError = validateContinuity(rooms);
            if (continuityError) {
                options.showToast(continuityError, 'info');
                return;
            }
            if (selection.role === 'Attendant') selection.attendantRooms = rooms;
            else selection.patientRooms = rooms;
            renderSelection();

            swal({
                title: 'Room dates selected',
                text: segment.roomnumber + ' · ' + fromDate + ' to ' + toDate,
                type: 'success',
                showCancelButton: true,
                confirmButtonText: 'Continue selection',
                cancelButtonText: selection.role === 'Attendant' ? 'Return to Direct IPD' : 'Go to booking',
                allowOutsideClick: false
            }).then(function (result) {
                var keepSelecting = result === true || (result && result.value === true);
                if (!keepSelecting) continueSelection();
            }, continueSelection);
        }

        function addRange(room, fromDate, toDate) {
            if (fromDate < options.getTodayIso()) {
                options.showToast('Past dates cannot be selected for Direct IPD.', 'info');
                return;
            }
            if (selection.role === 'Attendant' &&
                (fromDate < selection.attendantFrom || toDate > selection.attendantTo)) {
                options.showToast('Attendant room dates must stay within the attendant date range.', 'info');
                return;
            }
            var hasCalendarConflict = (state.calendarCards || []).some(function (card) {
                return String(card.roomId) === String(room.id) &&
                    dateRangesOverlap(fromDate, toDate, card.startDate, card.endDate);
            });
            var hasSelectionConflict = allRooms().some(function (selected) {
                return String(selected.roomid) === String(room.id) &&
                    dateRangesOverlap(fromDate, toDate, selected.fromdate, selected.todate);
            });
            if (hasCalendarConflict || hasSelectionConflict) {
                options.showToast('This room is not available for the selected dates.', 'info');
                return;
            }

            var requestKey = [room.id, fromDate, toDate].join('|');
            if (pendingAvailabilityChecks[requestKey]) {
                options.showToast('Room availability is already being checked.', 'info');
                return;
            }
            pendingAvailabilityChecks[requestKey] = true;
            var requestedRole = selection.role;
            options.showToast('Checking live room availability...', 'info');

            $.ajax({
                url: options.availabilityUrl || '/IPDApplicationForm/get_Available_Rooms',
                dataType: 'json',
                data: {
                    tenantid: options.tenantId || global.NLM_TENANT_ID || '',
                    roomtype: room.roomTypeId,
                    occupancystatus: 'Empty',
                    nextdaycheckin: fromDate,
                    nextdaycheckout: toDate
                }
            }).done(function (response) {
                if (!selection.active || selection.role !== requestedRole) return;
                var selectedRoomId = String(room.id || '').toLowerCase();
                var isAvailable = rowsFromResponse(response).some(function (row) {
                    return roomIdFromAvailability(row) === selectedRoomId;
                });
                if (!isAvailable) {
                    options.showToast(
                        (room.roomNumber || room.roomCode || 'This room') +
                        ' is already blocked or booked for ' + fromDate + ' to ' + toDate +
                        '. Choose another room or date range.',
                        'info'
                    );
                    return;
                }
                commitRange(room, fromDate, toDate);
            }).fail(function () {
                options.showToast('Unable to verify live room availability. Please retry the selection.', 'info');
            }).always(function () {
                delete pendingAvailabilityChecks[requestKey];
            });
        }

        function openModal() {
            if (!selection.patientRooms.length) {
                options.showToast('Select at least one patient room.', 'info');
                return;
            }
            $.ajax({
                url: options.prepareUrl,
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                data: JSON.stringify({ rooms: selection.patientRooms })
            }).done(function (response) {
                if (typeof response === 'string') {
                    try { response = JSON.parse(response); } catch (ignore) { response = null; }
                }
                if (!response || !response.url) {
                    options.showToast('Direct IPD could not be opened.', 'info');
                    return;
                }
                selection.active = false;
                selection.modalUrl = response.url;
                $('#directIpdSelectionBar').addClass('hidden');
                $('#calendarDirectIPDFrame').attr('src', response.url);
                $('#modalCalendarDirectIPDOverlay').removeClass('hidden');
                options.refreshScheduler();
            }).fail(function (xhr) {
                options.showToast(xhr.responseText || 'Direct IPD could not be opened.', 'info');
            });
        }

        function continueSelection() {
            var continuityError = validateContinuity(roomsForRole());
            if (continuityError) {
                options.showToast(continuityError, 'info');
                return;
            }
            if (selection.role === 'Attendant') {
                if (!selection.attendantRooms.length) {
                    options.showToast('Select an attendant room before returning.', 'info');
                    return;
                }
                selection.active = false;
                $('#directIpdSelectionBar').addClass('hidden');
                var frame = document.getElementById('calendarDirectIPDFrame');
                if (frame && frame.contentWindow && typeof frame.contentWindow.NLM_applyCalendarRoomSelections === 'function') {
                    frame.contentWindow.NLM_applyCalendarRoomSelections(allRooms());
                }
                $('#modalCalendarDirectIPDOverlay').removeClass('hidden');
                options.refreshScheduler();
                return;
            }
            openModal();
        }

        function closeModal() {
            $('#modalCalendarDirectIPDOverlay').addClass('hidden');
            $('#calendarDirectIPDFrame').attr('src', 'about:blank');
            selection.active = false;
            selection.role = 'Patient';
            selection.patientRooms = [];
            selection.attendantRooms = [];
            options.refreshScheduler();
        }

        global.NLM_selectAttendantRoom = function (dateRange) {
            selection.role = 'Attendant';
            selection.attendantRooms = [];
            selection.attendantFrom = dateRange.fromDate;
            selection.attendantTo = dateRange.toDate;
            selection.active = true;
            $('#modalCalendarDirectIPDOverlay').addClass('hidden');
            renderSelection();
            options.showToast('Select the attendant room within the permitted dates.', 'info');
        };
        global.NLM_closeDirectIpdModal = closeModal;
        global.NLM_directIpdAdmissionSaved = function () {
            closeModal();
            options.loadCalendarMonth(true);
            options.showToast('Direct IPD admission saved and calendar availability updated.', 'success');
        };

        $(document)
            .off('.nlmDirectIpd')
            .on('click.nlmDirectIpd', '#btnStartDirectIPD', start)
            .on('click.nlmDirectIpd', '#btnContinueDirectIPD', continueSelection)
            .on('click.nlmDirectIpd', '#btnClearDirectIPDSelection', function () {
                if (selection.role === 'Attendant') selection.attendantRooms = [];
                else selection.patientRooms = [];
                renderSelection();
            })
            .on('click.nlmDirectIpd', '[data-direct-room-index]', function () {
                var index = parseInt($(this).attr('data-direct-room-index'), 10);
                if (selection.role === 'Attendant') selection.attendantRooms.splice(index, 1);
                else selection.patientRooms.splice(index, 1);
                renderSelection();
            });

        return {
            isActive: function () { return selection.active; },
            addRange: addRange,
            buildEvents: buildEvents,
            renderEvent: renderEvent,
            getSelection: function () { return selection; }
        };
    }

    global.NLMCalendarDirectIpd = { create: create };
})(window);
