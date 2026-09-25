
CREATE OR REPLACE FUNCTION public."Update_Shift_Planning"(
	pvar_shiftplanningid uuid,
	pvar_tenantid uuid,
	pvar_shiftname uuid,
	pvar_shiftstarttime character varying,
	pvar_shiftendtime character varying,
	pvar_validfrom date,
	pvar_validto date,
	pvar_people json,
	pvar_modifieduser uuid,
	pvar_shiftplanning_people_bulkupload json,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);DECLARE pvar_ShiftPlanning_people_uploadid uuid;

              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20*/
			  IF "Check_Authorization"(pvar_modifieduser, 'ShiftPlanning', 'edit') THEN

			  pvar_returnMessage:='';

			  -- Check for overlapping date range excluding the current record
IF EXISTS (
SELECT 1
FROM ShiftPlanning sp
WHERE sp.tenantid = pvar_tenantid
AND sp.shiftplanningid <> pvar_shiftplanningid
AND COALESCE(sp.isdeleted, false) = false
AND sp.shiftname = pvar_shiftname
AND pvar_validfrom <= sp.validto
AND pvar_validto >= sp.validfrom
) THEN
pvar_returnmessage := 'Shift Planning already exists for the selected shift and date range';
RETURN;
END IF;
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('ShiftPlanning', NOW(),
(SELECT query_to_xml('SELECT * FROM ShiftPlanning WHERE ShiftPlanning.ShiftPlanningid= '''||pvar_ShiftPlanningid||'''', true, false, '')));

                    
                    UPDATE ShiftPlanning SET
                    shiftname=pvar_shiftname
,shiftstarttime=pvar_shiftstarttime
,shiftendtime=pvar_shiftendtime
,validfrom=pvar_validfrom
,validto=pvar_validto

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE ShiftPlanningid=pvar_ShiftPlanningid;

                    

                    INSERT INTO history
VALUES('ShiftPlanning_people', NOW(),
(SELECT query_to_xml('SELECT * FROM ShiftPlanning_people WHERE ShiftPlanning_people.ShiftPlanningid= '''||pvar_ShiftPlanningid||'''', true, false, '')));

								DELETE FROM  ShiftPlanning_people WHERE ShiftPlanningid=pvar_ShiftPlanningid;
								
								
								INSERT INTO ShiftPlanning_people (
									ShiftPlanningid
									,ShiftPlanning_peopleid 
                                    ,record_order  
									,personname
,workprofile
,coveragetype

									
									)
									SELECT 
									pvar_ShiftPlanningid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'personname' AS uuid) as personname
,CAST(j->>'workprofile' AS  uuid) as workprofile
,j->>'coveragetype' as coveragetype

									
                                    FROM json_array_elements(pvar_people) as j;
									
								pvar_ShiftPlanning_people_uploadid := gen_random_uuid();
								insert into  ShiftPlanning_people_upload (ShiftPlanning_people_uploadid ,
									ShiftPlanning_people_uploadfile ,
									createddate,
									createdby )

									values(pvar_ShiftPlanning_people_uploadid, '',now(),pvar_modifieduser);
								update ShiftPlanning_people_bulkupload  
						set isdeleted = true where ShiftPlanningid = pvar_ShiftPlanningid;
									INSERT INTO ShiftPlanning_people_bulkupload (
									ShiftPlanningid,  ShiftPlanning_peopleid, ShiftPlanning_people_uploadfileid
									
                                    ,record_order  ,action_date, action_by, action
									,personname
,workprofile
,coveragetype

									,unalteredjson
									)
									SELECT 
									pvar_ShiftPlanningid, gen_random_uuid(), pvar_ShiftPlanning_people_uploadid
                                    
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,NOW(),pvar_modifieduser,'Added'
,CASE WHEN j->>'personname'  = '' THEN NULL ELSE j->>'personname' END as personname

,CASE WHEN j->>'workprofile'  = '' THEN NULL ELSE j->>'workprofile' END as workprofile

,CASE WHEN j->>'coveragetype'  = '' THEN NULL ELSE j->>'coveragetype' END as coveragetype

									,to_jsonb(json_build_object(
        'ShiftPlanningid', pvar_ShiftPlanningid,
        'ShiftPlanning_peopleid', gen_random_uuid(),
        'ShiftPlanning_people_uploadfileid', pvar_ShiftPlanning_people_uploadid,
        'record_order', CAST(coalesce(j->>'record_order', '0') AS INT)
        ,'personname', j->>'personname'
,'workprofile', j->>'workprofile'
,'coveragetype', j->>'coveragetype'

    ))
                                    FROM json_array_elements(pvar_ShiftPlanning_people_bulkupload) as j;
                   pvar_returnMessage := validate_ShiftPlanning_people_bulkupload(pvar_ShiftPlanningid, pvar_ShiftPlanning_people_uploadid );
									

					
							
					
    if pvar_returnMessage IS NULL or pvar_returnMessage = '' then 
        pvar_returnMessage := '201.1';
    else 
        pvar_returnMessage := CONCAT('result:', pvar_ShiftPlanningid);
    end if;
			
			  END IF;

			  
																ELSE
																

															
																INSERT INTO system_logging
																(
																Log_code
																,system_logging_guid
																,log_application
																,log_date
																,log_level
																,log_logger
																,log_message
																,log_user_name
																)
																VALUES
																('401.1'
																,gen_random_uuid()
																,'Store Proc Authorization Check'
																,NOW()
																,'Critical'
																,'Update_Shift_Planning'
																,'Authorization Failed Update_Shift_Planning'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';
																
																END IF;

			  			 /* EXCEPTION WHEN OTHERS THEN
			 
						INSERT INTO system_logging
						(
						Log_code
						,system_logging_guid
						,log_application
						,log_date
						,log_level
						,log_logger
						,log_message
						)
						VALUES
						('16'
						,gen_random_uuid()
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Update_Shift_Planning'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Shift_Planning - update failed';*/
			  	
			  END
              
$BODY$;

