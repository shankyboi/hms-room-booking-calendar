(function () {
    'use strict';

    function isTelephoneInput(element) {
        return element instanceof HTMLInputElement &&
            element.type === 'tel' &&
            !element.readOnly &&
            !element.disabled;
    }

    function digitsOnly(value) {
        return String(value || '').replace(/\D/g, '');
    }

    function sanitize(element) {
        if (!isTelephoneInput(element)) {
            return;
        }

        var sanitizedValue = digitsOnly(element.value);
        if (element.value !== sanitizedValue) {
            element.value = sanitizedValue;
            element.dispatchEvent(new Event('change', { bubbles: true }));
        }
    }

    function configure(element) {
        if (!(element instanceof HTMLInputElement) || element.type !== 'tel') {
            return;
        }

        element.setAttribute('inputmode', 'numeric');
        element.setAttribute('pattern', '[0-9]*');
        sanitize(element);
    }

    document.addEventListener('beforeinput', function (event) {
        if (!isTelephoneInput(event.target) ||
            !event.inputType ||
            event.inputType.indexOf('insert') !== 0 ||
            event.data === null) {
            return;
        }

        if (/\D/.test(event.data)) {
            event.preventDefault();
        }
    }, true);

    document.addEventListener('keydown', function (event) {
        if (!isTelephoneInput(event.target) ||
            event.ctrlKey ||
            event.metaKey ||
            event.altKey) {
            return;
        }

        var allowedKeys = [
            'Backspace', 'Delete', 'Tab', 'Enter', 'Escape',
            'ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown',
            'Home', 'End'
        ];

        if (allowedKeys.indexOf(event.key) === -1 && !/^[0-9]$/.test(event.key)) {
            event.preventDefault();
        }
    }, true);

    document.addEventListener('input', function (event) {
        sanitize(event.target);
    }, true);

    document.addEventListener('paste', function (event) {
        if (!isTelephoneInput(event.target)) {
            return;
        }

        var clipboard = event.clipboardData;
        if (!clipboard) {
            return;
        }

        event.preventDefault();
        var input = event.target;
        var pastedDigits = digitsOnly(clipboard.getData('text'));
        var start = input.selectionStart === null ? input.value.length : input.selectionStart;
        var end = input.selectionEnd === null ? start : input.selectionEnd;
        input.setRangeText(pastedDigits, start, end, 'end');
        input.dispatchEvent(new Event('input', { bubbles: true }));
    }, true);

    document.addEventListener('drop', function (event) {
        if (isTelephoneInput(event.target)) {
            event.preventDefault();
        }
    }, true);

    function initialize(root) {
        if (root instanceof HTMLInputElement) {
            configure(root);
        }

        if (root.querySelectorAll) {
            root.querySelectorAll('input[type="tel"]').forEach(configure);
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            initialize(document);
        });
    } else {
        initialize(document);
    }

    new MutationObserver(function (mutations) {
        mutations.forEach(function (mutation) {
            mutation.addedNodes.forEach(initialize);
        });
    }).observe(document.documentElement, { childList: true, subtree: true });
})();
