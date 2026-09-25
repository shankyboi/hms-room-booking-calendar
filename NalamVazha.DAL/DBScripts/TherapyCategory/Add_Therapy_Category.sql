
			  CREATE OR REPLACE FUNCTION  "Add_Therapy_Category"
			  (
				  pvar_TherapyCategoryid uuid
,pvar_tenantid uuid
,
pvar_categoryname Varchar(128)
,
pvar_categorydescription Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:28*/
		

			  
                                                                                    if pvar_TherapyCategoryid is null then
                                                                                    pvar_TherapyCategoryid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TherapyCategory', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from TherapyCategory where upper(TherapyCategory.categoryname::varchar) = upper(pvar_categoryname::varchar) and TherapyCategory.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Category Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TherapyCategory(
				 categoryname
,categorydescription

				 ,createduser
				 ,TherapyCategoryid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_categoryname
,pvar_categorydescription

				 ,pvar_createduser
				 ,pvar_TherapyCategoryid
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
																,'Add_Therapy_Category'
																,'Authorization Failed Add_Therapy_Category'
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
						,'Add_Therapy_Category'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Therapy_Category - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

