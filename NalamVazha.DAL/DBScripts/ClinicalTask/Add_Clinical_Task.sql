
			  CREATE OR REPLACE FUNCTION  "Add_Clinical_Task"
			  (
				  pvar_ClinicalTaskid uuid
,pvar_tenantid uuid
,
pvar_workprofile  uuid
,
pvar_competency  uuid
,pvar_taskduration json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36*/
		

			  
                                                                                    if pvar_ClinicalTaskid is null then
                                                                                    pvar_ClinicalTaskid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'ClinicalTask', 'create') THEN
			  pvar_returnMessage:='';

			  IF EXISTS (
			      SELECT 1 FROM ClinicalTask
			      WHERE ClinicalTask.workprofile = pvar_workprofile
			        AND ClinicalTask.competency = pvar_competency
			        AND COALESCE(cast(ClinicalTask.tenantid as varchar),'') = COALESCE(cast(pvar_tenantid as varchar),'')
			        AND COALESCE(ClinicalTask.isdeleted,false) = false
			  ) THEN
			      pvar_returnMessage := 'A Clinical Task already exists for this Work Profile and Competency.';
			  END IF;


			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO ClinicalTask(
				 workprofile
,competency

				 ,createduser
				 ,ClinicalTaskid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_workprofile
,pvar_competency

				 ,pvar_createduser
				 ,pvar_ClinicalTaskid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
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
																,'Add_Clinical_Task'
																,'Authorization Failed Add_Clinical_Task'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
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
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Clinical_Task'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Clinical_Task - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

