(function (window, $) {
    "use strict";

    var rules = [
        {
            names: ["pan card", "pan"],
            regex: /^[A-Z]{5}[0-9]{4}[A-Z]{1}$/,
            message: "Invalid PAN format (ABCDE1234F)",
            maxLength: 10,
            sanitize: function (value) {
                return sanitizeByPattern(value, ["A", "A", "A", "A", "A", "9", "9", "9", "9", "A"]);
            }
        },
        {
            names: ["adhaar card", "adhaar", "aadhar card", "aadhaar card", "aadhar", "aadhaar"],
            regex: /^[0-9]{12}$/,
            message: "Aadhar must be 12 digits",
            maxLength: 12,
            sanitize: function (value) {
                return onlyDigits(value).substring(0, 12);
            }
        },
        {
            names: ["passport"],
            regex: /^[A-Z]{1}[0-9]{7}$/,
            message: "Invalid Passport format (A1234567)",
            maxLength: 8,
            sanitize: function (value) {
                return sanitizeByPattern(value, ["A", "9", "9", "9", "9", "9", "9", "9"]);
            }
        },
        {
            names: ["voter id", "voter card"],
            regex: /^[A-Z]{3}[0-9]{7}$/,
            message: "Invalid Voter ID format (ABC1234567)",
            maxLength: 10,
            sanitize: function (value) {
                return sanitizeByPattern(value, ["A", "A", "A", "9", "9", "9", "9", "9", "9", "9"]);
            }
        },
        {
            names: ["driving license", "driving licence", "driver license", "driver licence", "dl"],
            regex: /^[A-Z]{2}[0-9]{13}$/,
            message: "Invalid Driving License format",
            maxLength: 15,
            sanitize: function (value) {
                return sanitizeByPattern(value, ["A", "A", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9", "9"]);
            }
        }
    ];

    function normalize(value) {
        return (value || "").toString().trim().toLowerCase();
    }

    function getRule(idType) {
        var type = normalize(idType);
        for (var i = 0; i < rules.length; i++) {
            if (rules[i].names.indexOf(type) > -1) {
                return rules[i];
            }
        }
        return null;
    }

    function getMessage(idType) {
        var rule = getRule(idType);
        return rule ? rule.message : "";
    }

    function onlyDigits(value) {
        return (value || "").toString().replace(/\D/g, "");
    }

    function sanitizeByPattern(value, pattern) {
        var raw = (value || "").toString().toUpperCase().replace(/[^A-Z0-9]/g, "");
        var result = "";
        var rawIndex = 0;

        for (var i = 0; i < pattern.length && rawIndex < raw.length; i++) {
            var expected = pattern[i];
            while (rawIndex < raw.length) {
                var ch = raw.charAt(rawIndex++);
                if ((expected === "A" && /^[A-Z]$/.test(ch)) || (expected === "9" && /^[0-9]$/.test(ch))) {
                    result += ch;
                    break;
                }
            }
        }

        return result;
    }

    function sanitizeValue(idType, value) {
        var rule = getRule(idType);
        if (rule && rule.sanitize) {
            return rule.sanitize(value);
        }
        return (value || "").toString().toUpperCase();
    }

    function validate(idType, idNumber) {
        var rule = getRule(idType);
        var value = sanitizeValue(idType, idNumber).trim();
        if (!value) {
            return {
                isValid: true,
                value: value,
                message: ""
            };
        }
        return {
            isValid: !rule || rule.regex.test(value),
            value: value,
            message: rule ? rule.message : ""
        };
    }

    function getValidationSpan($number) {
        var fieldName = $number.attr("name") || $number.attr("id");
        var $span = $('[data-valmsg-for="' + fieldName + '"]');
        if (!$span.length && fieldName && fieldName.indexOf(".") > -1) {
            $span = $('[data-valmsg-for="' + fieldName.replace(/\./g, "\\.") + '"]');
        }
        return $span;
    }

    function clearInlineResult($number) {
        var $span = getValidationSpan($number);
        $number.removeClass("input-validation-error");
        $span.removeClass("field-validation-error red-important")
            .addClass("field-validation-valid")
            .text("");
    }

    function attach(options) {
        var typeSelector = options.typeSelector;
        var numberSelector = options.numberSelector;
        var $type = $(typeSelector);
        var $number = $(numberSelector);

        if (!$type.length || !$number.length) {
            return;
        }

        function applyRuleAttributes() {
            var rule = getRule($type.val());
            if (rule && rule.maxLength) {
                $number.attr("maxlength", rule.maxLength);
            } else {
                $number.removeAttr("maxlength");
            }
        }

        function run(triggerJqueryValidation) {
            applyRuleAttributes();
            var result = validate($type.val(), $number.val());
            if ($number.val() !== result.value) {
                $number.val(result.value);
            }
            if (!triggerJqueryValidation || result.isValid) {
                clearInlineResult($number);
            }
            if (triggerJqueryValidation && $number.valid) {
                $number.valid();
            }
            return result.isValid;
        }

        $number.off(".idProofValidation")
            .on("input.idProofValidation keyup.idProofValidation", function () {
                run(false);
            })
            .on("change.idProofValidation blur.idProofValidation", function () {
                run(true);
            });

        $type.off(".idProofValidation").on("change.idProofValidation", function () {
            run(true);
        });

        if ($.validator && $.validator.addMethod) {
            $.validator.addMethod("idProofNumberFormat", function (value, element, selector) {
                return validate($(selector).val(), value).isValid;
            }, function (selector) {
                return getMessage($(selector).val()) || "Invalid ID Proof Number";
            });

            $number.each(function () {
                var $field = $(this);
                if ($field.rules && this.form && $.data(this.form, "validator")) {
                    $field.rules("add", {
                        idProofNumberFormat: typeSelector
                    });
                }
            });
        }

        run(false);
    }

    function autoAttach() {
        attach({ typeSelector: "#idtype", numberSelector: "#idnumber" });
        attach({ typeSelector: "#idprooftype", numberSelector: "#idproofnumber" });
        attach({ typeSelector: "#pscQaIdProofType", numberSelector: "#pscQaIdProofNumber" });
    }

    window.IdProofValidation = {
        attach: attach,
        validate: validate,
        getMessage: getMessage,
        autoAttach: autoAttach
    };

    $(autoAttach);
})(window, jQuery);
