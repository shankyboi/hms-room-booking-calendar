
			  CREATE OR REPLACE FUNCTION  "Update_Clinical_Task"
			  (
				  pvar_ClinicalTaskid uuid
,pvar_tenantid uuid
,
pvar_workprofile  uuid
,
pvar_competency  uuid
,pvar_taskduration json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36*/
			  IF "Check_Authorization"(pvar_modifieduser, 'ClinicalTask', 'edit') THEN


			  pvar_returnMessage:='';

			  IF EXISTS (
			      SELECT 1 FROM ClinicalTask
			      WHERE ClinicalTask.workprofile = pvar_workprofile
			        AND ClinicalTask.competency = pvar_competency
			        AND ClinicalTask.ClinicalTaskid <> pvar_ClinicalTaskid
			        AND COALESCE(cast(ClinicalTask.tenantid as varchar),'') = COALESCE(cast(pvar_tenantid as varchar),'')
			        AND COALESCE(ClinicalTask.isdeleted,false) = false
			  ) THEN
			      pvar_returnMessage := 'A Clinical Task already exists for this Work Profile and Competency.';
			  END IF;


			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('ClinicalTask', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalTask WHERE ClinicalTask.ClinicalTaskid= '''||pvar_ClinicalTaskid||'''', true, false, '')));

                    
                    UPDATE ClinicalTask SET
                    workprofile=pvar_workprofile
,competency=pvar_competency

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE ClinicalTaskid=pvar_ClinicalTaskid;

                    

                    INSERT INTO history
VALUES('ClinicalTask_taskduration', NOW(),
(SELECT query_to_xml('SELECT * FROM ClinicalTask_taskduration WHERE ClinicalTask_taskduration.ClinicalTaskid= '''||pvar_ClinicalTaskid||'''', true, false, '')));

								DELETE FROM  ClinicalTask_taskduration WHERE ClinicalTaskid=pvar_ClinicalTaskid;
								
								
								INSERT INTO ClinicalTask_taskduration (
									ClinicalTaskid
									,ClinicalTask_taskdurationid 
                                    ,record_order  
									,workprofile
,tasktype
,taskname
,durationinminutes
,overbookingcount

									
									)
									SELECT 
									pvar_ClinicalTaskid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'workprofile' AS uuid) as workprofile
,CAST(j->>'tasktype' AS uuid) as tasktype
,CAST(j->>'taskname' AS  uuid) as taskname
,CAST(j->>'durationinminutes' AS int) as durationinminutes
,CAST(j->>'overbookingcount' AS int) as overbookingcount

									
                                    FROM json_array_elements(pvar_taskduration) as j;
									



					
							
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
																,'Update_Clinical_Task'
																,'Authorization Failed Update_Clinical_Task'
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
						,'Update_Clinical_Task'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Clinical_Task - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

