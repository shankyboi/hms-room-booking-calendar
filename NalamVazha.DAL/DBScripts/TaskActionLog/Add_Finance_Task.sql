
			  CREATE OR REPLACE FUNCTION  "Add_Finance_Task"
			  (
				  pvar_TaskActionLogid uuid
,pvar_tenantid uuid
,
pvar_taskname  uuid
,
pvar_tasktype  uuid
,
pvar_summary Varchar(128)
,
pvar_description Varchar(256)
,pvar_actiondate Timestamp(3)
,pvar_actionby uuid
,pvar_comments Varchar(1028)
,pvar_escalateto uuid
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:40:35*/
		

			  
                                                                                    if pvar_TaskActionLogid is null then
                                                                                    pvar_TaskActionLogid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TaskActionLog', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TaskActionLog(
				 taskname
,tasktype
,summary
,description
,actiondate
,actionby
,comments
,escalateto

				 ,createduser
				 ,TaskActionLogid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_taskname
,pvar_tasktype
,pvar_summary
,pvar_description
,COALESCE(pvar_actiondate, NOW())
,COALESCE(pvar_actionby, pvar_createduser)
,pvar_comments
,pvar_escalateto

				 ,pvar_createduser
				 ,pvar_TaskActionLogid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
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
																,'Add_Finance_Task'
																,'Authorization Failed Add_Finance_Task'
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
						,'Add_Finance_Task'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Finance_Task - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;
