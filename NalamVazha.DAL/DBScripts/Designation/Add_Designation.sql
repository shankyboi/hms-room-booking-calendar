
			  CREATE OR REPLACE FUNCTION  "Add_Designation"
			  (
				  pvar_Designationid uuid
,pvar_tenantid uuid
,
pvar_workprofile  uuid
,
pvar_designation Varchar(128)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:33*/
		

			  
                                                                                    if pvar_Designationid is null then
                                                                                    pvar_Designationid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Designation', 'create') THEN
			  pvar_returnMessage:='';

			  IF "DesignationAlreadyExists"(pvar_tenantid, pvar_workprofile, pvar_designation, NULL) THEN
				  pvar_returnMessage := 'Designation already exists for selected Work Profile';
			  END IF;
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Designation(
				 workprofile
,designation

				 ,createduser
				 ,Designationid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_workprofile
,pvar_designation

				 ,pvar_createduser
				 ,pvar_Designationid
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
																,'Add_Designation'
																,'Authorization Failed Add_Designation'
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
						,'Add_Designation'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Designation - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

