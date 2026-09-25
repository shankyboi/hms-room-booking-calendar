
			  CREATE OR REPLACE FUNCTION  "Add_Therapy_Item_Category"
			  (
				  pvar_TherapyItemCategoryid uuid
,pvar_tenantid uuid
,
pvar_itemcategory Varchar(128)
,
pvar_description Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:39*/
		

			  
                                                                                    if pvar_TherapyItemCategoryid is null then
                                                                                    pvar_TherapyItemCategoryid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TherapyItemCategory', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from TherapyItemCategory where upper(TherapyItemCategory.itemcategory::varchar) = upper(pvar_itemcategory::varchar) and TherapyItemCategory.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Item Category Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TherapyItemCategory(
				 itemcategory
,description

				 ,createduser
				 ,TherapyItemCategoryid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_itemcategory
,pvar_description

				 ,pvar_createduser
				 ,pvar_TherapyItemCategoryid
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
																,'Add_Therapy_Item_Category'
																,'Authorization Failed Add_Therapy_Item_Category'
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
						,'Add_Therapy_Item_Category'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Therapy_Item_Category - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

