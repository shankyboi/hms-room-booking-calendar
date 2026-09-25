
CREATE OR REPLACE FUNCTION public.validate_shiftplanning_people_bulkupload(
	pvar_shiftplanningid uuid,
	pvar_shiftplanning_people_uploadfileid uuid,
	pvar_tenantid uuid DEFAULT NULL::uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    
AS $BODY$
							DECLARE
							successcount INTEGER := 0;
							Errorcount INTEGER := 0;
							BEGIN
							
                            UPDATE ShiftPlanning_people_bulkupload AS pcpb
                            SET errordescription = 'workprofile value is empty.'
                            WHERE pcpb.workprofile IS NULL
                            AND COALESCE(pcpb.errordescription, NULL) IS NULL
                            AND pcpb.ShiftPlanningid = pvar_ShiftPlanningid
                            AND pcpb.ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;
 
                            
                            UPDATE ShiftPlanning_people_bulkupload AS pcpb
                    SET errordescription = 'workprofile not found: ' || pcpb.workprofile
                    WHERE pcpb.workprofile IS NOT NULL
                    AND COALESCE(pcpb.errordescription, NULL) IS NULL
                    AND pcpb.workprofile NOT IN (
                    SELECT workprofilename FROM WorkProfile
                    )
                    AND pcpb.ShiftPlanningid =pvar_ShiftPlanningid
                AND pcpb.ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;



UPDATE ShiftPlanning_people_bulkupload AS pcpb
        SET errordescription = 'personname cannot be empty.'
        WHERE (pcpb.personname IS NULL OR LENGTH(pcpb.personname) = 0)
        AND COALESCE(pcpb.errordescription, NULL) IS NULL
        AND pcpb.ShiftPlanningid = pvar_ShiftPlanningid
        AND pcpb.ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;

        UPDATE ShiftPlanning_people_bulkupload AS pcpb
        SET errordescription = 'personname not found: ' || pcpb.personname
        WHERE pcpb.personname IS NOT NULL
        AND LENGTH(pcpb.personname) > 0
        AND COALESCE(pcpb.errordescription, NULL) IS NULL
        AND LOWER(pcpb.personname) NOT IN (
            SELECT LOWER(firstname || ' ' || COALESCE(lastname, '')) FROM people
        )
        AND pcpb.ShiftPlanningid = pvar_ShiftPlanningid
        AND pcpb.ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;

        UPDATE ShiftPlanning_people_bulkupload AS pcpb
        SET personname = (
            SELECT peopleid::varchar
            FROM people
            WHERE LOWER(firstname || ' ' || COALESCE(lastname, '')) = LOWER(pcpb.personname)
            LIMIT 1
        )
        WHERE pcpb.personname IS NOT NULL
        AND COALESCE(pcpb.errordescription, NULL) IS NULL
        AND pcpb.ShiftPlanningid = pvar_ShiftPlanningid
        AND pcpb.ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;






            UPDATE ShiftPlanning_people_bulkupload AS pcpb
            SET workprofile = (
                SELECT WorkProfileid
                FROM WorkProfile
                WHERE LOWER(WorkProfile.workprofilename) = LOWER(pcpb.workprofile)
                LIMIT 1
                )
    WHERE pcpb.ShiftPlanningid =pvar_ShiftPlanningid
    AND pcpb.ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid
    AND COALESCE(pcpb.errordescription, NULL) IS NULL;

										UPDATE ShiftPlanning_people_bulkupload pcpb
										SET errordescription = 'coveragetype value is not matched.' 
										WHERE pcpb.ShiftPlanningid = pvar_ShiftPlanningid
										AND NOT EXISTS (
    SELECT 1 FROM lookups
    WHERE entityname = 'ShiftPlanning'
    AND fieldname = 'coveragetype'
    AND pcpb.coveragetype = ANY(string_to_array(fielddesc, ','))
)
										AND COALESCE(errordescription, NULL) IS NULL
										AND ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;

							
                                    UPDATE ShiftPlanning_people_bulkupload
                                    SET errordescription = CASE
WHEN coveragetype IS NULL OR length(coveragetype) = 0 THEN 'coveragetype cannot be empty.'

                                    ELSE NULL END
                                    WHERE ShiftPlanningid = pvar_ShiftPlanningid
                                    AND COALESCE(errordescription, NULL) IS NULL
                                    AND ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;
							
							
                                SELECT COUNT(*) INTO successcount
                                FROM ShiftPlanning_people_bulkupload
                                WHERE COALESCE(errordescription, NULL) IS NULL
                                AND ShiftPlanningid =pvar_ShiftPlanningid
                                AND ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;
 
                                SELECT COUNT(*) INTO Errorcount
                                FROM ShiftPlanning_people_bulkupload
                                WHERE COALESCE(errordescription, NULL) IS NOT NULL
                                AND ShiftPlanningid = pvar_ShiftPlanningid
                                AND ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;
							
							INSERT INTO ShiftPlanning_people (
							ShiftPlanningid,ShiftPlanning_peopleid
	,personname
	,workprofile
	,coveragetype

							)
							SELECT 
							ShiftPlanningid,ShiftPlanning_peopleid
	,personname ::uuid
	,workprofile ::uuid
	,coveragetype ::Varchar

							FROM ShiftPlanning_people_bulkupload 
							WHERE COALESCE(errordescription, NULL) IS NULL
							AND ShiftPlanningid =pvar_ShiftPlanningid
							AND ShiftPlanning_people_uploadfileid = pvar_ShiftPlanning_people_uploadfileid;
							
							INSERT INTO system_logging (
							log_code, 
							system_logging_guid,
							log_application, 
							log_date, 
							log_level, 
							log_logger, 
							log_message
							) VALUES (
							'200', 
							gen_random_uuid(), 
							'Bulk Upload Validation', 
							NOW(), 
							'Info', 
							'validate_ShiftPlanning_people_bulkupload', 
							format('Validation completed: Success=%s, Errors=%s', successcount, Errorcount)
							);
							
                        pvar_returnMessage := '';
if (Errorcount > 0) then
     pvar_returnMessage := 'The file has error';
end if;
							END
              $BODY$
              LANGUAGE plpgsql;



