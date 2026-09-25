
			  CREATE OR REPLACE FUNCTION  "Add_Task"
			  (
				  pvar_Taskid uuid
,pvar_tenantid uuid
,
pvar_tasktype  uuid
,
pvar_taskname Varchar(128)
,
pvar_taskdesc Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:24*/
		

			  
                                                                                    if pvar_Taskid is null then
                                                                                    pvar_Taskid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Task', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Task where upper(Task.taskname::varchar) = upper(pvar_taskname::varchar) and Task.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Task Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Task(
				 tasktype
,taskname
,taskdesc

				 ,createduser
				 ,Taskid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_tasktype
,pvar_taskname
,pvar_taskdesc

				 ,pvar_createduser
				 ,pvar_Taskid
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
																,'Add_Task'
																,'Authorization Failed Add_Task'
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
						,'Add_Task'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Task - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

