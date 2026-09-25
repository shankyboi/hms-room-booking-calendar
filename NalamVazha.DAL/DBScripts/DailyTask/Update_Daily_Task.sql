
			  CREATE OR REPLACE FUNCTION  "Update_Daily_Task"
			  (
				  pvar_DailyTaskid uuid
,pvar_tenantid uuid
,
pvar_tasktype  uuid
,
pvar_taskname  uuid
,
pvar_patientname  uuid
,
pvar_ipdreferencenumber  uuid
,
pvar_opdreferencenumber  uuid
,
pvar_status Varchar(128)
,
pvar_amount int

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40*/
			  IF "Check_Authorization"(pvar_modifieduser, 'DailyTask', 'edit') THEN


			  pvar_returnMessage:='';

			  
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('DailyTask', NOW(),
(SELECT query_to_xml('SELECT * FROM DailyTask WHERE DailyTask.DailyTaskid= '''||pvar_DailyTaskid||'''', true, false, '')));

                    
                    UPDATE DailyTask SET
                    tasktype=pvar_tasktype
,taskname=pvar_taskname
,patientname=pvar_patientname
,ipdreferencenumber=pvar_ipdreferencenumber
,opdreferencenumber=pvar_opdreferencenumber
,status=pvar_status
,amount=pvar_amount

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE DailyTaskid=pvar_DailyTaskid;

                    

                    


					
							
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
																,'Update_Daily_Task'
																,'Authorization Failed Update_Daily_Task'
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
						,'Update_Daily_Task'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Daily_Task - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

