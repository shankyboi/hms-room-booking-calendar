
CREATE OR REPLACE FUNCTION public."Update_Appointment_Status"(
	pvar_clinicalappointmentid uuid,
	pvar_status character varying,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
BEGIN
    IF "Check_Authorization"(pvar_modifieduser, 'ClinicalAppointment', 'edit') THEN

        -- Validate appointment exists
        IF NOT EXISTS (
            SELECT 1 FROM ClinicalAppointment
            WHERE ClinicalAppointmentid = pvar_clinicalappointmentid
              AND COALESCE(isdeleted, false) = false
        ) THEN
            pvar_returnMessage := 'Appointment not found.';
            RETURN;
        END IF;

        -- Validate status value against lookups
        IF pvar_status IS NOT NULL AND LENGTH(pvar_status) > 0 THEN
            IF NOT EXISTS (
                SELECT 1 FROM lookups
                WHERE fieldname  = 'status'
                  AND entityname = 'ClinicalAppointment'
                  AND fielddesc ILIKE ('%' || pvar_status || '%')
            ) THEN
                pvar_returnMessage := 'status value is invalid.';
                RETURN;
            END IF;
        END IF;

        -- insert into history table
        INSERT INTO history
        VALUES (
            'ClinicalAppointment',
            NOW(),
            (SELECT query_to_xml(
                'SELECT * FROM ClinicalAppointment WHERE ClinicalAppointmentid = ''' || pvar_clinicalappointmentid || '''',
                true, false, ''
            ))
        );

        UPDATE ClinicalAppointment
        SET    status       = pvar_status,
               modifieduser = pvar_modifieduser,
               modifieddate = NOW()
        WHERE  ClinicalAppointmentid = pvar_clinicalappointmentid;
		
        IF pvar_status IN ('Completed', 'Cancelled', 'In Progress', 'In-Progress') THEN
            PERFORM public."SyncOPDPatientVisitStatus"(
                pvar_clinicalappointmentid,
                CASE WHEN pvar_status = 'In Progress' THEN 'In-Progress' ELSE pvar_status END,
                pvar_modifieduser
            );
        END IF;
        pvar_returnMessage := '201.1';

    ELSE

        INSERT INTO system_logging
            (Log_code, system_logging_guid, log_application, log_date, log_level, log_logger, log_message, log_user_name)
        VALUES
            ('401.1', gen_random_uuid(), 'Store Proc Authorization Check', NOW(), 'Critical',
             'Update_Appointment_Status', 'Authorization Failed Update_Appointment_Status', pvar_modifieduser);

        pvar_returnMessage := '401.1';

    END IF;
END;
$BODY$;
