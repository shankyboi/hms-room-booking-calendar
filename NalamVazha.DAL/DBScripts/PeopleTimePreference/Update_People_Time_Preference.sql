
CREATE OR REPLACE FUNCTION public."Update_People_Time_Preference"(
	pvar_peopletimepreferenceid uuid,
	pvar_tenantid uuid,
	pvar_shiftname uuid,
pvar_workprofile  uuid,
	pvar_people uuid,
	pvar_timepreference json,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 14:00:31*/
			  IF "Check_Authorization"(pvar_modifieduser, 'PeopleTimePreference', 'edit') THEN

			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('PeopleTimePreference', NOW(),
(SELECT query_to_xml('SELECT * FROM PeopleTimePreference WHERE PeopleTimePreference.PeopleTimePreferenceid= '''||pvar_PeopleTimePreferenceid||'''', true, false, '')));

                    
                    UPDATE PeopleTimePreference SET
                    shiftname=pvar_shiftname
					,workprofile=pvar_workprofile
,people=pvar_people

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE PeopleTimePreferenceid=pvar_PeopleTimePreferenceid;

                    

                    INSERT INTO history
VALUES('PeopleTimePreference_timepreference', NOW(),
(SELECT query_to_xml('SELECT * FROM PeopleTimePreference_timepreference WHERE PeopleTimePreference_timepreference.PeopleTimePreferenceid= '''||pvar_PeopleTimePreferenceid||'''', true, false, '')));

								DELETE FROM  PeopleTimePreference_timepreference WHERE PeopleTimePreferenceid=pvar_PeopleTimePreferenceid;
								
								
								INSERT INTO PeopleTimePreference_timepreference (
									PeopleTimePreferenceid
									,PeopleTimePreference_timepreferenceid 
                                    ,record_order  
									,clinicaltask
,tasktype
,taskname
,availableon
,taskstarttime
,taskendtime
,taskhours

									
									)
									SELECT 
									pvar_PeopleTimePreferenceid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,coalesce(j->>'clinicaltask',gen_random_uuid()::varchar) as clinicaltask
,CAST(j->>'tasktype' AS uuid) as tasktype
,CAST(j->>'taskname' AS  uuid) as taskname
,j->>'availableon' as availableon
,j->>'taskstarttime' as taskstarttime
,j->>'taskendtime' as taskendtime
,CAST(j->>'taskhours' AS numeric(18,2))

									
                                    FROM json_array_elements(pvar_timepreference) as j;
									

					
							
					pvar_returnMessage :='201.1';
			
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
																,'Update_People_Time_Preference'
																,'Authorization Failed Update_People_Time_Preference'
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
						,'Update_People_Time_Preference'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_People_Time_Preference - update failed';*/
			  	
			  END
              
$BODY$;

