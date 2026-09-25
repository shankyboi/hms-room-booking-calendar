
			  CREATE OR REPLACE FUNCTION  "Add_Task_Type"
			  (
				  pvar_TaskTypeid uuid
,pvar_tenantid uuid
,
pvar_tasktypename Varchar(128)
,
pvar_description Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:21*/
		

			  
                                                                                    if pvar_TaskTypeid is null then
                                                                                    pvar_TaskTypeid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TaskType', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from TaskType where upper(TaskType.tasktypename::varchar) = upper(pvar_tasktypename::varchar) and TaskType.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Task Type Name  Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TaskType(
				 tasktypename
,description

				 ,createduser
				 ,TaskTypeid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_tasktypename
,pvar_description

				 ,pvar_createduser
				 ,pvar_TaskTypeid
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
																,'Add_Task_Type'
																,'Authorization Failed Add_Task_Type'
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
						,'Add_Task_Type'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Task_Type - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

