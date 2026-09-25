# Room Booking Calendar — User Stories and Expected Behaviour

# 1. Complete Feature List

1. **Calendar drag selection and card manipulation**
   - Drag-select a room and date range.
   - Move an editable booking or suggestion card.
   - Resize an editable card to change the stay dates.
   - Select eligible cards for batch actions.

2. **Room blocking and manual availability**
   - Block a room for a selected date range.
   - View or update an existing manual block.
   - Change a blocked room back to available.
   - Prevent blocked periods from being used for bookings or Direct IPD.

3. **Drag-time availability highlighting**
   - Highlight valid destinations while dragging from the Review Queue.
   - Use bright yellow for exact preference matches.
   - Use dull yellow for valid flexible placements.
   - Apply the same highlighting while moving a system-suggested card already placed on the calendar.
   - Accept Review Queue drops only on highlighted destinations.

4. **Direct IPD from the calendar**
   - Start Direct IPD room selection from the calendar.
   - Select one room or a continuous split stay.
   - Validate live room availability.
   - Carry selected rooms into the embedded Direct IPD form.
   - Select a separate attendant room when required.
   - Refresh the calendar after the admission is saved.

5. **Take to Next System Suggestion**
   - Navigate directly to each system-suggested placement.
   - Expand collapsed room groups automatically.
   - Scroll to the suggested room and dates.
   - Cycle through all suggestions and show the current position.

6. **Review Queue management**
   - Search and filter Review Queue requests.
   - Expand a card using only its green header.
   - Open the booking popup from the expanded white card area.
   - Show a traffic-light indicator beside the patient name: green for Full Stay, yellow for Split Stay, and red for Allocation Not Available.
   - Drag patient and attendant tiles independently.
   - Hide the queue while dragging and restore it after a failed drop.
   - Keep the queue closed after a successful drop.
   - Return eligible calendar suggestions to the Review Queue.
   - Perform Rework, Doctor Review, Waitlist, and Reject actions.

---

# 2. Feature-wise Changes

## Feature 1 — Calendar Drag Selection and Card Manipulation

- Added horizontal drag selection across a room row and one or more calendar dates.
- Connected normal calendar range selection to the Room Availability workflow.
- Routed range selection to Direct IPD when Direct IPD mode is active.
- Enabled unlocked calendar cards to be moved to another room/date.
- Enabled unlocked calendar cards to be resized to adjust the stay period.
- Added validation for room conflicts, preferred room types, preferred dates, flexibility, gender policies, and attendant stay limits.
- Recalculated the preference colour/legend after every accepted move or resize.
- Protected locked workflow states from movement, resizing, and batch selection.
- Added checkbox and Ctrl/Command-click selection for eligible Pending and Waitlisted applications.
- Counted a patient and attendant belonging to the same application as one batch item.

## Feature 2 — Room Blocking and Manual Availability

- Added a Room Availability modal that opens from a dragged calendar range.
- Prepopulated the selected room, From Date, and To Date.
- Added `Blocked` and `Available` status handling.
- Added mandatory `Blocked for` information and an optional description.
- Added date-range validation before saving.
- Connected manual availability changes to the server API.
- Rendered manual blocks as blocked calendar cards.
- Allowed an existing block to be opened and updated from the calendar.
- Reloaded calendar availability after a successful save.
- Included manual blocks in booking, drag/drop, and Direct IPD conflict checks.

## Feature 3 — Drag-Time Availability Highlighting

- Added whole-stay calendar highlighting when a Review Queue tile is dragged.
- Added **bright yellow** highlighting when both room type and dates match preferences.
- Added **dull yellow** highlighting when a placement is allowed through room/date flexibility.
- Calculated highlights using availability, stay duration, preferences, flexibility, gender, patient/attendant role, and existing occupancy.
- Prevented invalid cells from being highlighted.
- Added the same highlighting when an unlocked system-suggested card already on the calendar is moved.
- Ignored the moving card's existing occupancy while calculating its alternative destinations.
- Cleared all temporary highlighting after a completed, cancelled, or invalid drag.
- Restricted Review Queue drops to highlighted room/date destinations.

## Feature 4 — Direct IPD from the Calendar

- Added the `Add Direct IPD` action to start calendar-based room selection.
- Added temporary green selection cards and removable selection chips.
- Added live server-side room availability verification before accepting each segment.
- Prevented past-date selection, overlaps, duplicate checks, and unavailable-room selection.
- Added support for a split stay across multiple rooms when the segments are continuous and non-overlapping.
- Added validation that rejects gaps between split-stay segments.
- Stored selected patient rooms using a temporary server-side token.
- Opened the Direct IPD admission form inside an embedded modal with the selected rooms preloaded.
- Added a return-to-calendar flow for selecting a separate attendant room.
- Restricted attendant selections to the permitted attendant date range.
- Passed patient and attendant room selections back to the embedded form.
- Cleared temporary selection state on completion or cancellation.
- Reloaded the calendar after a successful Direct IPD admission.

## Feature 5 — Take to Next System Suggestion

- Added a suggestion count to the navigation button.
- Added sequential navigation through the month's system suggestions.
- Automatically expanded a collapsed room-type group containing the target suggestion.
- Automatically changed a hidden-suggestion filter so the target can be displayed.
- Scrolled horizontally to the suggestion dates and vertically to the room row.
- Highlighted/focused the selected system suggestion.
- Updated the button text with the current suggestion position.
- Wrapped navigation back to the first suggestion after the final item.
- Reset the navigation cursor when the suggestion set changes.
- Disabled the button when there are no suggestions.
- Prevented suggestion navigation while Direct IPD room selection is active.

## Feature 6 — Review Queue Management

- Added queue counts and All, Solo, Attendant, search, and status filters.
- Made only the green card header responsible for expansion and collapse.
- Made the white region of an expanded card open the standard booking popup.
- Prevented internal controls, dropdowns, action buttons, and draggable tiles from accidentally opening the popup.
- Added a traffic-light availability indicator immediately beside the patient name.
- Added green for Full Stay Available, yellow for Split Stay Available, and red for Allocation Not Available.
- Calculated the indicator using the patient's complete required stay duration; a shorter partial stay never qualifies.
- Included preferred placements and placements permitted through room-type/date flexibility.
- Added independent draggable tiles for patients and attendants.
- Disabled a person's Review Queue tile after that person has already been manually placed.
- Hid the Review Queue as soon as a drag begins so the full calendar is available as a drop surface.
- Reopened the Review Queue after cancellation or an invalid/unavailable drop.
- Kept the Review Queue closed after a successful placement.
- Added validation that accepts drops only on highlighted destinations.
- Kept a request available when the patient is placed but its attendant still needs a room.
- Added the ability to return eligible manual/system suggestions from the calendar to the Review Queue without creating duplicates.
- Added per-request workflow actions for Rework, Doctor Review, Waitlist, and Reject.

---

# 3. User Stories, Scenarios, and Expected Behaviour

# Feature 1 — Calendar Drag Selection and Card Manipulation

## User story F1.1 — Select a room and date range

**As a Front Desk user, I want to drag across dates in a room row so that I can perform an action for that room and period without entering the dates separately.**

| Scenario | Expected behaviour |
|---|---|
| Drag across one or more date cells in a normal calendar state | The selected room and date range are identified and the Room Availability modal opens. |
| Drag across a room-type header instead of an actual room row | No booking or room block is created; the selection is cleared. |
| Select a range while Direct IPD selection is active | The range is routed to the Direct IPD workflow instead of opening the Room Availability modal. |
| Complete or cancel the modal action | The temporary calendar selection is cleared. |

## User story F1.2 — Move an existing editable card

**As a Front Desk user, I want to drag an editable calendar card to another room or date so that I can correct or refine a proposed placement.**

| Scenario | Expected behaviour |
|---|---|
| Move an unlocked card to an available permitted room/date | The card is reassigned, its room and dates are updated, its preference legend is recalculated, and a success message is shown. |
| Move a locked card | The movement is rejected and the calendar refreshes to its previous state. |
| Move a card to an unknown room or invalid calendar resource | The change is rejected, the original placement is restored, and an informational message is shown. |
| Move a card into an occupied, booked, blocked, or otherwise overlapping range | The change is rejected, the card returns to its original position, and the user is told that the room is unavailable. |
| Move to a room type that is outside the recorded preferences when room-type flexibility is not allowed | The change is rejected and the user is told to use a preferred room type. |
| Move to dates outside the recorded preferences when date flexibility is not allowed | The change is rejected and the user is told to use a preferred date range. |
| Move outside both room and date preferences when neither form of flexibility is allowed | The change is rejected and the user is told that both the room type and date must match preferences. |
| Move a patient or attendant to a room whose gender rule is incompatible | The change is rejected and the gender restriction is explained. |
| Move an attendant outside the associated patient's stay dates | The change is rejected; an attendant must remain fully within the patient's stay range. |
| Move an attendant into the same room as the patient | The placement is not allowed because the patient and attendant require separate rooms/beds. |

## User story F1.3 — Resize an editable card

**As a Front Desk user, I want to resize an editable card so that I can adjust the proposed stay dates directly on the calendar.**

| Scenario | Expected behaviour |
|---|---|
| Resize an unlocked card to a valid available period | The same room/date, flexibility, gender, attendant-range, and overlap validations used for movement are applied before accepting the new dates. |
| Resize a locked card or resize into an invalid period | The change is rejected and the original card is restored. |

## User story F1.4 — Select cards for batch actions

**As a Front Desk user, I want to select eligible calendar cards so that I can perform a batch workflow action.**

| Scenario | Expected behaviour |
|---|---|
| Select an eligible Pending or Waitlisted card using its checkbox | The card becomes selected and the batch count is updated. |
| Ctrl-click or Command-click an eligible card | The card selection is toggled without opening the booking popup. |
| Try to select a locked or ineligible card | It is not added to the batch selection. |
| Deselect the last selected card | The batch-action button is disabled and any open batch menu closes. |
| Patient and attendant cards belong to the same application | They count as one application in the batch count. |

---

# Feature 2 — Room Blocking and Manual Availability

## User story F2.1 — Block a room for a date range

**As a Front Desk user, I want to mark a room as blocked for specific dates so that it cannot be assigned during maintenance or another operational restriction.**

| Scenario | Expected behaviour |
|---|---|
| Drag across an available room row while not in Direct IPD mode | The Room Availability modal opens with the room and selected dates populated. |
| Choose `Blocked`, enter a reason, and save a valid range | A manual block is saved through the room-availability API, a success message is shown, and the calendar reloads to display the block. |
| Leave `Blocked for` empty | Saving is stopped and the field is marked as required. |
| Enter a To Date earlier than the From Date | Saving is stopped and a valid-date-range message is displayed. |
| The server rejects the request | The modal remains open and the API error, or a generic save error, is displayed. |

## User story F2.2 — Review or change an existing manual block

**As a Front Desk user, I want to click an existing blocked card so that I can inspect or update its availability details.**

| Scenario | Expected behaviour |
|---|---|
| Click a manual blocked card | The Room Availability modal opens with its room, dates, reason, and description populated. |
| Change an existing block to `Available` and save | The same allocation record is updated, the calendar refreshes, and the room becomes available for the affected dates according to the server response. |
| Close or cancel the modal | No availability change is saved and the temporary selection is cleared. |

## User story F2.3 — Prevent use of blocked rooms

**As a Front Desk user, I want blocked periods to participate in availability checks so that patients are not placed into unavailable rooms.**

| Scenario | Expected behaviour |
|---|---|
| Move or drop a booking onto a blocked period | The placement is rejected as a room conflict. |
| Select a blocked period during Direct IPD | Live availability validation rejects the range and asks the user to choose another room or dates. |

---

# Feature 3 — Drag-Time Availability Highlighting

## User story F3.1 — Highlight placements while dragging from the Review Queue

**As a Front Desk user, I want valid calendar cells highlighted while dragging a patient or attendant so that I can see where the complete stay may be placed.**

| Scenario | Expected behaviour |
|---|---|
| Start dragging a Review Queue patient tile | Valid room/date cells are highlighted for the complete stay duration. |
| The room type and date range both match recorded preferences | The complete proposed stay is shown in **bright yellow**. |
| Placement is valid only because room type, dates, or both are flexible | The complete proposed stay is shown in **dull/light yellow**. |
| The request is flexible with dates only | Preferred-room cells matching preferred dates are bright; other available date ranges in permitted room types are dull. Non-preferred room types are not highlighted. |
| The request is flexible with room type only | Preferred room types on preferred dates are bright; other permitted room types on the same preferred dates are dull. Dates outside preferences are not highlighted. |
| The request is flexible with both room type and dates | Exact preference matches are bright; other valid available combinations are dull. |
| A room/date range conflicts with another calendar card | It is not highlighted. |
| A room violates the person's gender rule | It is not highlighted. |
| An attendant placement falls outside the patient stay or uses the patient's room | It is not highlighted. |
| The drag ends or is cancelled | All temporary yellow highlighting is removed. |

## User story F3.2 — Highlight placements while moving a system suggestion already on the calendar

**As a Front Desk user, I want the same guidance when moving a placed system suggestion so that rescheduling is as clear as the original Review Queue placement.**

| Scenario | Expected behaviour |
|---|---|
| Press and begin moving an unlocked system-suggestion card | The card's own current occupancy is ignored for validation, and bright/dull yellow placement guidance appears across valid cells. |
| Move a placed patient suggestion | Highlighting uses the patient's room, date, gender, flexibility, duration, and conflict rules. |
| Move a placed attendant suggestion | Highlighting uses attendant preferences and additionally restricts placement to the patient's stay and a separate room. |
| Press a locked, non-suggestion, or interactive control within a card | Drag guidance is not activated. |
| Complete, cancel, or release the calendar drag | Highlighting is removed and the normal calendar styling returns. |

## User story F3.3 — Enforce highlighted placement guidance

**As a Front Desk user, I want Review Queue drops accepted only on highlighted cells so that visually invalid placements cannot be saved accidentally.**

| Scenario | Expected behaviour |
|---|---|
| Drop on a bright or dull highlighted room/date | The application is evaluated and placed if all validations pass. |
| Drop on a non-highlighted or unavailable location | The placement is rejected, a message asks the user to drop on a highlighted room/date, and the Review Queue reopens. |

---

# Feature 4 — Direct IPD from the Calendar

## User story F4.1 — Start a Direct IPD admission from the calendar

**As a Front Desk user, I want to start Direct IPD from the Room Calendar so that I can select rooms before completing the admission form.**

| Scenario | Expected behaviour |
|---|---|
| Click `Add Direct IPD` | Direct IPD selection mode starts, previous temporary room selections are cleared, and guidance asks the user to drag across an available room/date range. |
| Select a valid patient room/date range | Live availability is checked; after success, the range appears as a temporary green Direct IPD selection card and as a removable selection chip. |
| Select a past date | The selection is rejected with a message that past dates cannot be used. |
| Select a range that conflicts with a calendar card or another temporary selection | The range is rejected as unavailable. |
| The live room-availability API reports the room unavailable | The range is not added and the user is asked to choose another room or date range. |
| The live availability request fails | No range is added and the user is asked to retry. |
| The same availability check is already in progress | A duplicate request is not sent; the user is told that availability is already being checked. |

## User story F4.2 — Build a split but continuous patient stay

**As a Front Desk user, I want to select consecutive segments in different rooms so that Direct IPD can support a continuous split stay.**

| Scenario | Expected behaviour |
|---|---|
| Add consecutive ranges where each new segment starts the day after the previous segment ends | The segments are accepted and displayed as one continuous stay. |
| Add ranges that overlap | The new range is rejected. |
| Add ranges with a gap between them | The new range is rejected because the stay must be continuous. |
| Remove a selected segment using its chip | That segment is removed and the temporary calendar selection refreshes. |
| Clear patient selections | All temporary patient room selections are removed. |

## User story F4.3 — Continue to the embedded Direct IPD form

**As a Front Desk user, I want my calendar selections carried into the Direct IPD form so that I do not need to re-enter the room plan.**

| Scenario | Expected behaviour |
|---|---|
| Click Continue without a patient room | The form does not open and the user is asked to select at least one patient room. |
| Continue with a valid continuous patient stay | The selected rooms are stored against a temporary server-side token and the embedded Direct IPD form opens with the selection. |
| The login session has expired | Preparation is rejected as unauthorized and the Direct IPD form is not opened. |
| The server cannot prepare the form | The user remains on the calendar and sees an error. |

## User story F4.4 — Select a separate attendant room

**As a Front Desk user, I want to select an attendant room from the same calendar so that the attendant's stay is included in the Direct IPD admission.**

| Scenario | Expected behaviour |
|---|---|
| The embedded form requests attendant-room selection | The form is hidden temporarily, selection mode changes to Attendant, and the permitted date range is displayed. |
| Select an available attendant room within the permitted dates | The selection is accepted after live availability validation. |
| Select attendant dates outside the permitted attendant range | The selection is rejected. |
| Return without choosing an attendant room | The user is asked to select an attendant room. |
| Return with a valid attendant room | Patient and attendant selections are passed back to the embedded Direct IPD form. |

## User story F4.5 — Finish or cancel Direct IPD

**As a Front Desk user, I want the calendar to refresh after completing Direct IPD so that the saved admission is immediately reflected.**

| Scenario | Expected behaviour |
|---|---|
| Direct IPD admission is saved | The embedded modal closes, temporary selections are cleared, the calendar reloads from the server, and a success message is shown. |
| The Direct IPD modal is closed/cancelled | The embedded page is unloaded and all temporary Direct IPD state is reset. |
| Try to navigate system suggestions while Direct IPD selection is active | Navigation is prevented until Direct IPD selection is completed or cancelled. |

---

# Feature 5 — Take to Next System Suggestion

## User story F5.1 — Navigate to a system-suggested placement

**As a Front Desk user, I want the calendar to take me to the next system suggestion so that I can review suggested placements efficiently.**

| Scenario | Expected behaviour |
|---|---|
| System suggestions exist and the button is clicked for the first time | The first suggestion becomes focused, its room-type section is expanded if necessary, and the calendar scrolls horizontally and vertically to its room/date. |
| Click the button again | The next suggestion is focused and the button text indicates the current position, such as `Next System Suggestion 2/7`. |
| Reach the final suggestion and click again | Navigation wraps to the first suggestion. |
| Suggestions were hidden by the filter | The filter changes to show the focused suggestion before scrolling. |
| The suggestion list changes because of filtering or data refresh | The navigation cursor resets so it cannot point to a stale suggestion. |
| No suggestions exist for the displayed month | The button is disabled and its tooltip explains that no suggestions are available. |
| Direct IPD selection is active | Suggestion navigation is stopped and the user is asked to finish or cancel Direct IPD first. |
| Suggestion navigation succeeds | A message identifies the suggestion number, patient, room, and date range. |

---

# Feature 6 — Review Queue Behaviour

## User story F6.1 — Find and inspect Review Queue requests

**As a Front Desk user, I want to filter and inspect requests that need manual attention so that I can resolve them efficiently.**

| Scenario | Expected behaviour |
|---|---|
| Open the Review Queue | The drawer opens and shows the current queue count. |
| Filter by All, Solo, or Attendant | Only matching requests are shown and the filter counts remain visible. |
| Search by patient, attendant, or IPD/reference details | The list narrows to matching requests. |
| Apply a status filter | Only requests in the chosen workflow status are shown. |
| Click the green card header | Only that header action expands or collapses the card details. |
| Click the white area of an expanded card | The same booking popup used by calendar cards opens. |
| Click an input, dropdown, action button, or draggable person tile inside the card | The control performs its own action and does not unintentionally open the booking popup. |

## User story F6.2 — Understand booking availability before dragging

**As a Front Desk user, I want a traffic-light indicator beside the patient name so that I can understand complete-stay allocation feasibility without expanding the card.**

| Scenario | Expected behaviour |
|---|---|
| One preferred room can accommodate the complete required duration across a preferred date range | A green circle is shown with `Full Stay Available`. |
| One room can accommodate the complete required duration outside the preferred room type, dates, or both, and the corresponding flexibility is enabled | A green circle is shown with `Full Stay Available`. |
| No single room covers the full duration, but consecutive rooms cover every day of the duration within the preferred room type/date criteria | A yellow circle is shown with `Split Stay Available`. |
| No single room covers the full duration, but consecutive rooms cover every day through allowed room-type flexibility, date flexibility, or both | A yellow circle is shown with `Split Stay Available`. |
| No valid single-room or multi-room combination covers every day of the required duration | A red circle is shown with `Allocation Not Available`. |
| A shorter stay is possible but the complete duration is not (for example, 7 of 10 required days) | A red circle remains shown; partial-duration availability is not considered sufficient. |
| The attendant tab is selected | The circle beside the patient name continues to represent the patient's complete-stay availability. |

## User story F6.3 — Drag a patient or attendant from the Review Queue

**As a Front Desk user, I want to drag patient and attendant tiles independently so that I can place each person in an appropriate room.**

| Scenario | Expected behaviour |
|---|---|
| Begin dragging an eligible patient or attendant tile | The Review Queue hides immediately, the calendar becomes the active drop surface, and valid cells are highlighted. |
| Release without completing a drop | Temporary highlighting is removed and the Review Queue reopens. |
| Drop on an unavailable or non-highlighted location | The placement is rejected, an informational message is shown, and the Review Queue reopens. |
| Drop successfully | The new card appears on the calendar, temporary highlighting is removed, and the Review Queue stays closed. |
| Try to drag a person already manually placed | The drag is stopped and the user is told that the patient or attendant is already placed. |
| Patient placement succeeds but an attendant still requires placement | The request remains available for attendant placement; patient and attendant can be managed independently. |
| Attendant placement is attempted before a valid patient placement or outside the patient range | Invalid locations are not highlighted and the drop is rejected. |

## User story F6.4 — Move a manual or suggested placement back to review

**As a Front Desk user, I want to return an eligible calendar suggestion to the Review Queue so that it can be reconsidered.**

| Scenario | Expected behaviour |
|---|---|
| Click the remove/return control on an eligible suggestion card | The card is removed from the calendar, its temporary Review Queue state is reset, and the request is added to the Review Queue if not already present. |
| The request is already in the Review Queue | A duplicate queue entry is not created. |
| The return succeeds | The Review Queue opens and the calendar refreshes. |

## User story F6.5 — Perform Review Queue workflow actions

**As a Front Desk user, I want to take workflow actions from a request card so that manual review can progress without leaving the calendar.**

| Scenario | Expected behaviour |
|---|---|
| Choose an enabled action such as Rework, Doctor Review, Waitlist, or Reject | The appropriate comments/workflow dialog opens for that application. |
| Choose Waitlist when the application is not eligible to join the waiting list | The action is not completed. |
| Choose a disabled action | No workflow change occurs. |
| Open one request's action menu | Other Review Queue action menus close to avoid multiple overlapping menus. |

---

# 4. Cross-feature Acceptance Rules

1. **No overlapping occupancy:** A room cannot accept a patient, attendant, Direct IPD selection, or moved suggestion over an existing unavailable range.
2. **Preference rules remain authoritative:** Flexibility flags determine whether non-preferred room types or dates may be used.
3. **Gender restrictions remain authoritative:** A visually available room is still invalid when its room-type gender policy does not allow the person.
4. **Attendant constraints remain authoritative:** The attendant must use a separate room and stay within the patient's dates.
5. **Locked workflow states are protected:** Locked cards cannot be moved, resized, or batch-selected.
6. **Visual guidance and validation must agree:** A Review Queue drop is accepted only where the calendar showed a valid yellow highlight.
7. **Temporary UI state is cleaned up:** Highlights, drag state, temporary selections, and overlays are cleared after success, cancellation, or validation failure.
8. **User feedback is provided:** Successful actions show confirmation; rejected actions explain why the requested placement or workflow could not proceed.
9. **Availability is duration-complete:** Green or yellow is shown only when every day of the patient's complete required duration can be allocated. Availability for a shorter period does not change a red result.

---

# 5. Scope and Glossary

This document describes the Room Booking Calendar functionality currently integrated into Nalam. It is intended for product review, development, and functional testing.

The primary persona is a **Front Desk user**: a logged-in hospital user who reviews room requests, blocks rooms, creates Direct IPD admissions, and manages proposed room placements.

| Term | Meaning |
|---|---|
| Calendar card/tile | A patient, attendant, booking, suggestion, or room-block entry displayed across one or more dates. |
| System suggestion | A room placement generated by the allocation engine and displayed on the calendar for review. |
| Review Queue | Requests that require manual review or manual placement. |
| Preferred placement | The room type and date range match the patient's or attendant's recorded preferences. |
| Flexible placement | The placement is allowed because the request permits a different room type, different dates, or both. |
| Locked card | A card whose workflow/status prevents movement or selection, including booked, occupied, blocked, admitted, confirmed, rework, or doctor-review states. |

---

# 6. Implementation Code Reference

The following sections contain the principal production code for each feature. The excerpts focus on feature entry points, state transitions, and validation. Supporting rendering, data-mapping, and API code remains in the referenced source files.

## 6.1 Source Files

| Source file | Responsibility |
|---|---|
| `Admin/Views/RoomOccupancyStatus/Booking_Calendar.cshtml` | Calendar UI, DayPilot events, room blocking, suggestion navigation, highlights, and Review Queue behaviour. |
| `Admin/wwwroot/js/booking-calendar-direct-ipd.js` | Direct IPD calendar-selection workflow and live room-availability validation. |
| `Admin/Controllers/IPDApplicationFormController.cs` | Creates the Direct IPD selection token and serves/saves the admission form. |

## 6.2 Feature 1 Code — Calendar Drag Selection and Card Manipulation

### Calendar range selection

```javascript
onTimeRangeSelected: function (args) {
    var roomId = args.resource;
    var targetRoom = byId(state.rooms, roomId);

    if (!targetRoom) {
        dp.clearSelection();
        return;
    }

    var startDp = new DayPilot.Date(args.start);
    var endDp = new DayPilot.Date(args.end).addDays(-1);
    var fromDate = startDp.toString("yyyy-MM-dd");
    var toDate = endDp.toString("yyyy-MM-dd");

    if (calendarDirectIpd.isActive()) {
        calendarDirectIpd.addRange(targetRoom, fromDate, toDate);
        dp.clearSelection();
        return;
    }

    openRoomAvailabilityModal(targetRoom, fromDate, toDate);
}
```

### Move an existing calendar card

```javascript
onEventMoved: function (args) {
    finishCalendarSuggestionDragGuidance();

    var tags = args.e.data.tags;
    if (!tags || !tags.booking || isLockedCalendarCard(tags.booking)) {
        refreshScheduler();
        return;
    }

    var newStartDate = new DayPilot.Date(args.newStart)
        .toString("yyyy-MM-dd");
    var newEndDate = new DayPilot.Date(args.newEnd)
        .addDays(-1)
        .toString("yyyy-MM-dd");

    handleBookingDropReassign(
        tags.booking.id,
        args.newResource,
        newStartDate,
        newEndDate,
        !!tags.isAttendant
    );
}
```

### Resize an existing calendar card

```javascript
onEventResized: function (args) {
    var tags = args.e.data.tags;
    if (!tags || !tags.booking || isLockedCalendarCard(tags.booking)) {
        refreshScheduler();
        return;
    }

    var newStartDate = new DayPilot.Date(args.newStart)
        .toString("yyyy-MM-dd");
    var newEndDate = new DayPilot.Date(args.newEnd)
        .addDays(-1)
        .toString("yyyy-MM-dd");

    handleBookingDropReassign(
        tags.booking.id,
        args.e.data.resource || tags.booking.roomId,
        newStartDate,
        newEndDate,
        !!tags.isAttendant
    );
}
```

### Preference, flexibility, and conflict validation

```javascript
var roomTypeViolation =
    !flexibleWithRoomType && !targetRoomTypeAllowed;
var dateViolation =
    !flexibleWithDates && !targetDateAllowed;

if (roomTypeViolation || dateViolation) {
    refreshScheduler();
    showToast(
        roomTypeViolation && dateViolation
            ? "Move the card only to a preferred room type and date range."
            : roomTypeViolation
                ? "Move the card only to a preferred room type."
                : "Move the card only within a preferred date range.",
        "info"
    );
    return;
}

var hasConflict = (state.calendarCards || []).some(function (otherCard) {
    if (String(otherCard.id) === String(card.id)) return false;
    if (String(otherCard.roomId) !== String(newRoomId)) return false;

    return rangesOverlap(
        newStartDate,
        newEndDate,
        otherCard.startDate,
        otherCard.endDate
    );
});

if (hasConflict) {
    refreshScheduler();
    showToast("The room is not available for the selected dates.", "info");
    return;
}
```

## 6.3 Feature 2 Code — Room Blocking and Manual Availability

### Open the room-availability modal

```javascript
function openRoomAvailabilityModal(room, fromDate, toDate, existingBlock) {
    selectedRoomAvailability = {
        room: room,
        fromDate: fromDate,
        toDate: toDate,
        allocationId: existingBlock ? existingBlock.id : null
    };

    var details = splitManualBlockDetails(
        existingBlock && existingBlock.bookedFor
    );

    $("#roomAvailabilityRoom")
        .val(room.roomNumber || room.roomCode || room.id);
    $("#roomAvailabilityFrom").val(fromDate);
    $("#roomAvailabilityTo").val(toDate);
    $("#roomAvailabilityStatus")
        .val(existingBlock ? "Available" : "Blocked");
    $("#roomAvailabilityBlockedFor").val(details.blockedFor);
    $("#roomAvailabilityDescription").val(details.description);
    $("#modalRoomAvailabilityOverlay").removeClass("hidden");
}
```

### Validate and save room availability

```javascript
function saveRoomAvailability(event) {
    event.preventDefault();

    var selection = selectedRoomAvailability;
    if (!selection) return;

    var room = selection.room;
    var fromDate = $("#roomAvailabilityFrom").val();
    var toDate = $("#roomAvailabilityTo").val();
    var status = $("#roomAvailabilityStatus").val();
    var blockedFor = $.trim($("#roomAvailabilityBlockedFor").val());
    var description = $.trim($("#roomAvailabilityDescription").val());

    if (!fromDate || !toDate || toDate < fromDate) {
        $("#roomAvailabilityError")
            .text("Choose a valid date range.")
            .show();
        return;
    }

    if (!blockedFor) {
        $("#roomAvailabilityError")
            .text("Blocked for is required.")
            .show();
        return;
    }

    $.ajax({
        url: '@Url.Action("SetManualRoomAvailability", "RoomOccupancyStatus")',
        type: "POST",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify({
            allocationid: selection.allocationId,
            room: room.id,
            fromdate: fromDate,
            todate: toDate,
            status: status,
            blockedfor: blockedFor,
            description: description
        })
    }).done(function () {
        closeRoomAvailabilityModal();
        showToast("Room availability updated.", "success");
        loadCalendarMonth(true);
    }).fail(function (xhr) {
        $("#roomAvailabilityError")
            .text(xhr.responseText || "The room availability could not be saved.")
            .show();
    });
}
```

### Open an existing blocked card

```javascript
if (
    booking.isManualBlock &&
    String(booking.status).toLowerCase() === "blocked"
) {
    var blockedRoom = byId(state.rooms, booking.roomId);
    if (blockedRoom) {
        openRoomAvailabilityModal(
            blockedRoom,
            booking.startDate,
            booking.endDate,
            booking
        );
    }
    return;
}
```

## 6.4 Feature 3 Code — Drag-Time Availability Highlighting

### Bright and dull yellow styles

```css
#nlmRoot .scheduler_default_cell.nlm-review-drop-preferred,
#nlmRoot .scheduler_default_cell_inner.nlm-review-drop-preferred {
    background: #fde047 !important;
    box-shadow: inset 0 0 0 1px #eab308;
}

#nlmRoot .scheduler_default_cell.nlm-review-drop-flexible,
#nlmRoot .scheduler_default_cell_inner.nlm-review-drop-flexible {
    background: #fef9c3 !important;
    box-shadow: inset 0 0 0 1px #fde68a;
}
```

### Determine whether a destination is preferred or flexible

```javascript
function getReviewDragHighlightLevel(resourceId, startDate) {
    if (!reviewDragState || !startDate) return "";

    var app = getReviewDragApplication();
    var room = byId(state.rooms || [], resourceId);
    if (!app || !room) return "";

    var isAttendant = reviewDragState.kind === "attendant";
    var durationDays = Number(reviewDragState.durationDays) ||
        getReviewDragDuration(app, reviewDragState.kind);
    var endDate = addDaysToIso(startDate, durationDays);

    if (!isReviewRoomGenderCompatible(room, app, isAttendant)) return "";

    var preferredRoomTypeIds = getRoomPreferenceIds(app, isAttendant);
    var preferredDates = getReviewDatePreferences(app, isAttendant);
    var roomMatchesPreference = preferredRoomTypeIds.some(function (id) {
        return String(id) === String(room.roomTypeId);
    });
    var dateMatchesPreference = preferredDates.some(function (dates) {
        return dates.startDate === startDate && dates.endDate === endDate;
    });

    if (!roomMatchesPreference && !isTrueValue(app.flexibleWithRoomType)) {
        return "";
    }
    if (!dateMatchesPreference && !isTrueValue(app.flexibleWithDates)) {
        return "";
    }

    if (!isReviewRoomAvailable(
        room.id,
        startDate,
        endDate,
        app.applicationId,
        isAttendant,
        reviewDragState.ignoreCalendarCardId
    )) {
        return "";
    }

    return roomMatchesPreference && dateMatchesPreference
        ? "preferred"
        : "flexible";
}
```

### Paint the complete stay range

```javascript
function paintReviewDragHighlights() {
    clearReviewDragHighlights();
    if (!reviewDragState) return;

    document.querySelectorAll(
        "#dp .scheduler_default_cell:not(.category-row-cell)"
    ).forEach(function (cell) {
        var rect = cell.getBoundingClientRect();
        if (!rect.width || !rect.height) return;

        var room = getReviewDropRoomFromPoint(
            rect.left + Math.min(4, rect.width / 2),
            rect.top + rect.height / 2
        );
        var date = getReviewDropDateFromPoint(rect.left + rect.width / 2);
        if (!room || !date) return;

        var level = getReviewDragCellHighlightLevel(room.id, date);
        if (!level) return;

        cell.classList.add(
            level === "preferred"
                ? "nlm-review-drop-preferred"
                : "nlm-review-drop-flexible"
        );
    });
}
```

### Enable highlighting for a placed system suggestion

```javascript
function beginCalendarSuggestionDragGuidance(card) {
    if (!card || !isSystemSuggestionCard(card) || isLockedCalendarCard(card)) {
        return;
    }

    var application = card.pendingApplication ||
        (state.pendingApplications || []).find(function (app) {
            return String(app.applicationId) === String(card.applicationId);
        });
    if (!application) return;

    var startDate = normalizeCalendarDate(card.startDate);
    var endDate = normalizeCalendarDate(card.endDate);
    var durationDays = Math.max(
        1,
        Math.round(
            (new Date(endDate + "T00:00:00") -
             new Date(startDate + "T00:00:00")) / 86400000
        )
    );

    reviewDragState = {
        applicationId: application.applicationId,
        application: application,
        kind: card.isAttendant === true ? "attendant" : "patient",
        source: "calendar",
        durationDays: durationDays,
        ignoreCalendarCardId: card.id
    };

    buildReviewDragHighlightCache();
    paintReviewDragHighlights();
    $(".nlm-scheduler-holder").addClass("review-drag-active");
}

document.addEventListener("pointerdown", function (event) {
    var element = event.target.closest("[data-calendar-card-id]");
    if (!element || event.target.closest("button, input, select, textarea, a")) {
        return;
    }

    var cardId = element.getAttribute("data-calendar-card-id");
    var card = (state.calendarCards || []).find(function (item) {
        return String(item.id) === String(cardId);
    });

    beginCalendarSuggestionDragGuidance(card);
}, true);
```

## 6.5 Feature 4 Code — Direct IPD from the Calendar

### Start Direct IPD selection

```javascript
function start() {
    selection.active = true;
    selection.role = "Patient";
    selection.patientRooms = [];
    selection.attendantRooms = [];
    renderSelection();
    options.showToast(
        "Direct IPD selection started. Drag across an available room and date range.",
        "info"
    );
}
```

### Validate the selected range and check live availability

```javascript
function addRange(room, fromDate, toDate) {
    if (fromDate < options.getTodayIso()) {
        options.showToast("Past dates cannot be selected for Direct IPD.", "info");
        return;
    }

    if (
        selection.role === "Attendant" &&
        (fromDate < selection.attendantFrom || toDate > selection.attendantTo)
    ) {
        options.showToast(
            "Attendant room dates must stay within the attendant date range.",
            "info"
        );
        return;
    }

    var hasConflict = allRooms().some(function (selected) {
        return String(selected.roomid) === String(room.id) &&
            dateRangesOverlap(
                fromDate,
                toDate,
                selected.fromdate,
                selected.todate
            );
    });

    if (hasConflict) {
        options.showToast("This room is not available for the selected dates.", "info");
        return;
    }

    $.ajax({
        url: options.availabilityUrl,
        dataType: "json",
        data: {
            tenantid: options.tenantId,
            roomtype: room.roomTypeId,
            occupancystatus: "Empty",
            nextdaycheckin: fromDate,
            nextdaycheckout: toDate
        }
    }).done(function (response) {
        var selectedRoomId = String(room.id).toLowerCase();
        var isAvailable = rowsFromResponse(response).some(function (row) {
            return roomIdFromAvailability(row) === selectedRoomId;
        });

        if (!isAvailable) {
            options.showToast(
                "The room is already blocked or booked. Choose another room or date range.",
                "info"
            );
            return;
        }

        commitRange(room, fromDate, toDate);
    });
}
```

### Enforce a continuous split stay

```javascript
function validateContinuity(rooms) {
    var ordered = rooms.slice().sort(function (a, b) {
        return a.fromdate.localeCompare(b.fromdate);
    });

    for (var index = 1; index < ordered.length; index++) {
        if (ordered[index].fromdate <= ordered[index - 1].todate) {
            return "Selected date ranges cannot overlap.";
        }

        if (
            ordered[index].fromdate !==
            options.addDaysToIso(ordered[index - 1].todate, 1)
        ) {
            return "The selected rooms must form one continuous stay without missing dates.";
        }
    }

    return "";
}
```

### Prepare the embedded Direct IPD form

```csharp
[HttpPost]
public virtual IActionResult Prepare_Calendar_Direct_IPD(
    [FromBody] CalendarDirectIPDSelectionModel selection)
{
    var loginUserId = HttpContext.Session.GetString("NalamVazhaloginUserID");
    if (string.IsNullOrWhiteSpace(loginUserId))
        return Unauthorized("Session Expired");

    if (selection?.rooms == null || selection.rooms.Count == 0)
        return BadRequest("Select at least one patient room before continuing.");

    var token = Guid.NewGuid().ToString("N");
    HttpContext.Session.SetString(
        "CalendarDirectIPDSelection:" + token,
        JsonConvert.SerializeObject(selection.rooms)
    );

    return Json(new
    {
        success = true,
        token,
        url = Url.Action(
            "Direct_IPD_Admission",
            "IPDApplicationForm",
            new { calendarSelectionToken = token, embedded = "Y" }
        )
    });
}
```

## 6.6 Feature 5 Code — Take to Next System Suggestion

```javascript
function takeToNextSystemSuggestion() {
    if (calendarDirectIpd.isActive()) {
        showToast(
            "Finish or cancel Direct IPD room selection before navigating suggestions.",
            "info"
        );
        return;
    }

    var candidates = getSystemSuggestionCandidates();
    if (candidates.length === 0) {
        showToast("No system suggestions are available in this month.", "info");
        return;
    }

    var cursorKey = getSystemSuggestionCursorKey(candidates);
    if (state.systemSuggestionCursorKey !== cursorKey) {
        state.systemSuggestionCursor = -1;
        state.systemSuggestionCursorKey = cursorKey;
    }

    state.systemSuggestionCursor =
        (state.systemSuggestionCursor + 1) % candidates.length;

    var card = candidates[state.systemSuggestionCursor];
    var room = byId(state.rooms, card.roomId);

    if (room && state.collapsedRoomTypes[room.roomTypeId]) {
        state.collapsedRoomTypes[room.roomTypeId] = false;
    }
    if (state.systemSuggestionFilter === "hidden") {
        state.systemSuggestionFilter = "all";
    }

    state.focusedSystemSuggestionApplicationId =
        String(card.applicationId || card.id || "");

    renderToolbar();
    refreshScheduler();

    requestAnimationFrame(function () {
        requestAnimationFrame(function () {
            scrollToSystemSuggestion(card);
        });
    });
}
```

### Disable or update the navigation button

```javascript
$("#btnNextSystemSuggestion")
    .prop("disabled", suggestionCandidates.length === 0)
    .attr(
        "title",
        suggestionCandidates.length === 0
            ? "No system suggestions in this month"
            : "Go to the next system-suggested room placement"
    );

$("#systemSuggestionButtonText").text(
    hasActiveSuggestionCursor
        ? "Next System Suggestion " +
            (state.systemSuggestionCursor + 1) +
            "/" + suggestionCandidates.length
        : "Take to System Suggestion"
);
```

## 6.7 Feature 6 Code — Review Queue Behaviour

### Calculate the complete-duration availability status

```javascript
function getReviewBookingAvailability(app, isAttendant) {
    var preferredRoomTypeIds = getRoomPreferenceIds(app, isAttendant);
    var flexibleRoom = isTrueValue(app.flexibleWithRoomType);
    var dateRanges = getReviewAvailabilityDateRanges(app, isAttendant);
    var candidateRooms = (state.rooms || []).filter(function (room) {
        var roomMatches = preferredRoomTypeIds.some(function (id) {
            return String(id) === String(room.roomTypeId);
        });

        return (roomMatches || flexibleRoom) &&
            isReviewRoomGenderCompatible(room, app, isAttendant);
    });
    var hasSplitStay = false;

    for (var rangeIndex = 0; rangeIndex < dateRanges.length; rangeIndex++) {
        var range = dateRanges[rangeIndex];
        var fullStayRoom = candidateRooms.some(function (room) {
            return isReviewRoomAvailable(
                room.id,
                range.startDate,
                range.endDate,
                app.applicationId,
                isAttendant
            );
        });

        if (fullStayRoom) {
            return {
                label: "Full Stay Available",
                className: "full",
                description: "A single room is available for the complete required duration."
            };
        }

        var allNightsCovered = true;
        for (
            var dateCursor = range.startDate;
            dateCursor < range.endDate;
            dateCursor = addDaysToIso(dateCursor, 1)
        ) {
            var nightEnd = addDaysToIso(dateCursor, 1);
            var hasRoomForNight = candidateRooms.some(function (room) {
                return isReviewRoomAvailable(
                    room.id,
                    dateCursor,
                    nightEnd,
                    app.applicationId,
                    isAttendant
                );
            });

            if (!hasRoomForNight) {
                allNightsCovered = false;
                break;
            }
        }

        hasSplitStay = hasSplitStay || allNightsCovered;
    }

    if (hasSplitStay) {
        return {
            label: "Split Stay Available",
            className: "split",
            description: "The complete required duration can be covered across multiple rooms."
        };
    }

    return {
        label: "Allocation Not Available",
        className: "none",
        description: "The complete required duration cannot be allocated."
    };
}
```

### Render the traffic light beside the patient name

```javascript
var patientBookingAvailability = showingAttendant
    ? getReviewBookingAvailability(app, false)
    : bookingAvailability;

var patientAvailabilityLightHtml =
    '<span class="review-availability-light ' +
        patientBookingAvailability.className +
        '" role="img" aria-label="' +
        patientBookingAvailability.label +
        '" title="' +
        patientBookingAvailability.label + ': ' +
        patientBookingAvailability.description +
    '"></span>';

var patientNameHtml =
    '<div class="nlm-app-name-row">' +
        '<h3 class="nlm-app-name">' + app.patientName + '</h3>' +
        patientAvailabilityLightHtml +
    '</div>';
```

```css
.review-availability-light {
    display: inline-block;
    width: 11px;
    height: 11px;
    border: 2px solid rgba(255, 255, 255, .92);
    border-radius: 50%;
}

.review-availability-light.full  { background: #22c55e; }
.review-availability-light.split { background: #facc15; }
.review-availability-light.none  { background: #ef4444; }
```

### Expand only from the green header

```javascript
$(document).on(
    "click keydown",
    ".review-card-summary[data-review-toggle]",
    function (event) {
        if (
            event.type === "keydown" &&
            event.key !== "Enter" &&
            event.key !== " "
        ) {
            return;
        }

        event.preventDefault();
        event.stopPropagation();
        toggleReviewCard($(this).data("review-toggle"));
    }
);
```

### Open the booking popup from the expanded white area

```javascript
$(document).on(
    "click",
    ".nlm-app-card[data-review-app]",
    function (event) {
        if ($(event.target).closest(
            ".nlm-app-card-top, button, input, select, textarea, a, " +
            ".review-drag-person, .nlm-review-actions-wrap"
        ).length) {
            return;
        }

        var $card = $(this);
        var applicationId = $card.data("review-app");
        var isExpanded =
            $card.find(".review-card-summary[data-review-toggle]")
                .first()
                .attr("aria-expanded") === "true";

        if (!isExpanded) return;

        event.preventDefault();
        event.stopPropagation();
        openReviewQueueBookingModal(applicationId);
    }
);
```

### Hide the queue and highlight the calendar when dragging starts

```javascript
$(document).on(
    "dragstart",
    ".review-drag-person[data-review-drag='true']",
    function (event) {
        var applicationId = $(this).data("app");
        var kind = String($(this).data("kind") || "patient");
        var queueItem = getReviewQueueItem(applicationId);

        reviewDragState = {
            applicationId: applicationId,
            kind: kind,
            source: "reviewQueue",
            application: queueItem ? queueItem.application : null,
            queueWasOpen: state.isReviewQueueOpen === true
        };

        closeDrawer();
        buildReviewDragHighlightCache();
        paintReviewDragHighlights();
        $("body").addClass("nlm-review-dragging");
        $(".nlm-scheduler-holder").addClass("review-drag-active");

        if (event.originalEvent && event.originalEvent.dataTransfer) {
            event.originalEvent.dataTransfer.effectAllowed = "move";
            event.originalEvent.dataTransfer.setData(
                "text/plain",
                String(applicationId)
            );
        }
    }
);
```

### Reopen the queue after a failed drop and keep it closed after success

```javascript
function finishReviewQueueDrag(dropSucceeded) {
    if (!reviewDragState || reviewDragState.source === "calendar") return;

    var reopenQueue =
        reviewDragState.queueWasOpen === true && dropSucceeded !== true;

    $(".review-drag-person.is-dragging").removeClass("is-dragging");
    $(".nlm-scheduler-holder").removeClass("review-drag-active");
    $("body").removeClass("nlm-review-dragging");

    clearReviewDragHighlights();
    reviewDragState = null;
    reviewDragHighlightCache = {};

    if (reopenQueue) openDrawer();
    else closeDrawer();
}
```

### Accept drops only on highlighted destinations

```javascript
$(document).on("drop", "#dp", function (event) {
    if (!reviewDragState) return;

    event.preventDefault();
    event.stopPropagation();

    var sourceEvent = event.originalEvent || event;
    var targetRoom = getReviewDropRoomFromPoint(
        sourceEvent.clientX,
        sourceEvent.clientY
    );
    var startDate = getReviewDropDateFromPoint(sourceEvent.clientX);

    if (
        !targetRoom ||
        !startDate ||
        !getReviewDragHighlightLevel(targetRoom.id, startDate)
    ) {
        showToast(
            "Drop the card on a highlighted room and date.",
            "info"
        );
        finishReviewQueueDrag(false);
        return;
    }

    var activeDrag = reviewDragState;
    var placedBefore = (state.calendarCards || []).filter(function (card) {
        return String(card.applicationId) ===
                String(activeDrag.applicationId) &&
            (card.isAttendant === true) ===
                (activeDrag.kind === "attendant") &&
            card.source === "ManualReview";
    }).length;

    handleReviewQueueDropResolved(targetRoom, startDate);

    var placedAfter = (state.calendarCards || []).filter(function (card) {
        return String(card.applicationId) ===
                String(activeDrag.applicationId) &&
            (card.isAttendant === true) ===
                (activeDrag.kind === "attendant") &&
            card.source === "ManualReview";
    }).length;
    finishReviewQueueDrag(placedAfter > placedBefore);
});
```
