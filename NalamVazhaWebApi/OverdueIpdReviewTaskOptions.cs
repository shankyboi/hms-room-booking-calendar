namespace NalamVazhaWebApi
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;

    public sealed class OverdueIpdReviewTaskOptions
    {
        public bool Enabled { get; set; } = true;
        public string SystemUserId { get; set; }
        public int RunHour { get; set; } = 0;
        public int RunMinute { get; set; } = 5;
        public bool RunOnStartup { get; set; } = true;

        public static OverdueIpdReviewTaskOptions FromEnvironment()
        {
            return new OverdueIpdReviewTaskOptions
            {
                Enabled = ReadBool("IPD_WORKFLOW_TASK_ENABLED", true),
                SystemUserId = Environment.GetEnvironmentVariable("IPD_WORKFLOW_TASK_SYSTEM_USER_ID"),
                RunHour = ReadInt("IPD_WORKFLOW_TASK_RUN_HOUR", 0, 0, 23),
                RunMinute = ReadInt("IPD_WORKFLOW_TASK_RUN_MINUTE", 5, 0, 59),
                RunOnStartup = ReadBool("IPD_WORKFLOW_TASK_RUN_ON_STARTUP", true)
            };
        }

        public string GetTemplateNamesJson()
        {
            var names = new Dictionary<string, string>
            {
                ["new_ipd_arrived"] = ReadText("IPD_TASK_TEMPLATE_NEW_IPD_ARRIVED", "New IPD Arrived"),
                ["doctor_review_followup"] = ReadText("IPD_TASK_TEMPLATE_DOCTOR_REVIEW_FOLLOWUP", "IPD Follow-Up"),
                ["waiting_list_followup"] = ReadText("IPD_TASK_TEMPLATE_WAITING_LIST", "Waitlist Follow-Up"),
                ["booking_deposit_pending"] = ReadText("IPD_TASK_TEMPLATE_BOOKING_DEPOSIT", "Booking Deposit Payment"),
                ["assessment_not_added"] = ReadText("IPD_TASK_TEMPLATE_ASSESSMENT_NOT_ADDED", "Pending Assessment"),
                ["assessment_draft"] = ReadText("IPD_TASK_TEMPLATE_ASSESSMENT_DRAFT", "Pending Assessment"),
                ["assessment_review_pending"] = ReadText("IPD_TASK_TEMPLATE_ASSESSMENT_REVIEW", "Assessment Follow-UP"),
                ["opd_assessment_review_pending"] = ReadText("OPD_TASK_TEMPLATE_ASSESSMENT_REVIEW", "Assessment Follow-UP"),
                ["doctor_not_allotted"] = ReadText("IPD_TASK_TEMPLATE_DOCTOR_NOT_ALLOTTED", "Allot Screening Doctor"),
                ["arrival_confirmation_pending"] = ReadText("IPD_TASK_TEMPLATE_ARRIVAL_CONFIRMATION", "Screening Follow-Up"),
                ["arrival_confirmation_reminder"] = ReadText("IPD_TASK_TEMPLATE_ARRIVAL_REMINDER", "Arrival Confirmation"),
                ["consultation_doctor_pending"] = ReadText("IPD_TASK_TEMPLATE_CONSULTATION_DOCTOR", "Allot Consultation Doctor"),
                ["consultation_approval_pending"] = ReadText("IPD_TASK_TEMPLATE_CONSULTATION_APPROVAL", "Consultation Follow-Up"),
                ["patient_admission_pending"] = ReadText("IPD_TASK_TEMPLATE_PATIENT_ADMISSION", "Admit"),
                ["feedback_followup"] = ReadText("IPD_TASK_TEMPLATE_FEEDBACK_FOLLOWUP", "FeedBack Form Follow-Up"),
                ["discharge_checklist"] = ReadText("IPD_TASK_TEMPLATE_DISCHARGE_CHECKLIST", "Discharge Checklist Confirm"),
                ["ipd_cancellation_refund"] = ReadText("IPD_TASK_TEMPLATE_IPD_CANCELLATION_REFUND", "Cancellation Approve And Refund"),
                ["opd_cancellation_refund"] = ReadText("IPD_TASK_TEMPLATE_OPD_CANCELLATION_REFUND", "Cancellation Approve And Refund"),
                ["cancelled_room_release"] = ReadText("IPD_TASK_TEMPLATE_CANCELLED_ROOM_RELEASE", "Release Cancellation Room"),
                ["screening_ineligible_room_release"] = ReadText("IPD_TASK_TEMPLATE_SCREENING_INELIGIBLE_ROOM_RELEASE", "Release Ineligible HS Room"),
                ["consultation_ineligible_room_release"] = ReadText("IPD_TASK_TEMPLATE_CONSULTATION_INELIGIBLE_ROOM_RELEASE", "Release Ineligible HS Room"),
                ["discharged_room_release"] = ReadText("IPD_TASK_TEMPLATE_DISCHARGED_ROOM_RELEASE", "Release Discharge Room")
            };
            return JsonSerializer.Serialize(names);
        }

        private static string ReadText(string name, string defaultValue)
        {
            string value = Environment.GetEnvironmentVariable(name);
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value.Trim();
        }

        private static bool ReadBool(string name, bool defaultValue)
        {
            return bool.TryParse(Environment.GetEnvironmentVariable(name), out bool value) ? value : defaultValue;
        }

        private static int ReadInt(string name, int defaultValue, int minimum, int maximum)
        {
            return int.TryParse(Environment.GetEnvironmentVariable(name), out int value)
                && value >= minimum && value <= maximum
                    ? value
                    : defaultValue;
        }
    }
}
