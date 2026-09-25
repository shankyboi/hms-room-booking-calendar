(function (window) {
    'use strict';

    function normalizeKey(value) {
        return (value || '').toString().toLowerCase().replace(/[\s\-_]/g, '');
    }

    function hashColor(key) {
        var hash = 0;
        for (var i = 0; i < key.length; i++) {
            hash = key.charCodeAt(i) + ((hash << 5) - hash);
        }
        var h = (hash & 0x7fffffff) % 360;
        return 'hsl(' + h + ', 58%, 38%)';
    }

    var taskTypePalette = {
        ipdscreening: '#0f766e',
        ipscreening: '#0f766e',
        screening: '#0f766e',
        ipdconsultation: '#2563eb',
        consultation: '#2563eb',
        consult: '#2563eb',
        iprounds: '#7c3aed',
        rounds: '#7c3aed',
        followup: '#16a34a',
        therapy: '#9333ea',
        physiotherapy: '#0891b2',
        assessment: '#ca8a04',
        checkup: '#ea580c',
        procedure: '#dc2626',
        surgery: '#b91c1c',
        emergency: '#dc2626',
        review: '#4f46e5'
    };

    window.NVClinicalCalendarColors = window.NVClinicalCalendarColors || {};
    window.NVClinicalCalendarColors.taskTypeColor = function (taskType) {
        var key = normalizeKey(taskType);
        if (!key) return '#2563eb';
        for (var paletteKey in taskTypePalette) {
            if (Object.prototype.hasOwnProperty.call(taskTypePalette, paletteKey) && key.indexOf(paletteKey) !== -1) {
                return taskTypePalette[paletteKey];
            }
        }
        return hashColor(key);
    };

    window.NVClinicalCalendarColors.normalizePriority = function (priority) {
        var value = (priority || 'Medium').toString().trim().toLowerCase();
        if (value === 'high') return 'High';
        if (value === 'low') return 'Low';
        return 'Medium';
    };

    window.NVClinicalCalendarColors.priorityColors = function (priority) {
        var value = window.NVClinicalCalendarColors.normalizePriority(priority);
        if (value === 'High') return { bg: '#FCEBEB', fg: '#A32D2D', border: '#E24B4A', dot: '#E24B4A' };
        if (value === 'Low') return { bg: '#EAF3DE', fg: '#3B6D11', border: '#639922', dot: '#639922' };
        return { bg: '#FAEEDA', fg: '#854F0B', border: '#BA7517', dot: '#BA7517' };
    };
})(window);
