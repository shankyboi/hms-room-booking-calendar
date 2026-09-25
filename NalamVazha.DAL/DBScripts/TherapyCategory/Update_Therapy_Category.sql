
			  CREATE OR REPLACE FUNCTION  "Update_Therapy_Category"
			  (
				  pvar_TherapyCategoryid uuid
,pvar_tenantid uuid
,
pvar_categoryname Varchar(128)
,
pvar_categorydescription Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:28*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TherapyCategory', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from TherapyCategory where upper(TherapyCategory.categoryname) = upper(pvar_categoryname) and TherapyCategory.tenantid=pvar_tenantid  and TherapyCategory.TherapyCategoryid <> pvar_TherapyCategoryid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Category Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TherapyCategory', NOW(),
(SELECT query_to_xml('SELECT * FROM TherapyCategory WHERE TherapyCategory.TherapyCategoryid= '''||pvar_TherapyCategoryid||'''', true, false, '')));

                    
                    UPDATE TherapyCategory SET
                    categoryname=pvar_categoryname
,categorydescription=pvar_categorydescription

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TherapyCategoryid=pvar_TherapyCategoryid;

                    

                    


					
							
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
																,'Update_Therapy_Category'
																,'Authorization Failed Update_Therapy_Category'
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
						,'Update_Therapy_Category'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Therapy_Category - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

