
			  CREATE OR REPLACE FUNCTION  "Add_Therapy_Item"
			  (
				  pvar_TherapyItemid uuid
,pvar_tenantid uuid
,
pvar_therapyitemcategory  uuid
,
pvar_therapyitemname Varchar(128)
,
pvar_price decimal(18,2)
,
pvar_therapyitemimage Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:42*/
		

			  
                                                                                    if pvar_TherapyItemid is null then
                                                                                    pvar_TherapyItemid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TherapyItem', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from TherapyItem where upper(TherapyItem.therapyitemname::varchar) = upper(pvar_therapyitemname::varchar) and TherapyItem.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Therapy Item Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TherapyItem(
				 therapyitemcategory
,therapyitemname
,price
,therapyitemimage

				 ,createduser
				 ,TherapyItemid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_therapyitemcategory
,pvar_therapyitemname
,pvar_price
,pvar_therapyitemimage

				 ,pvar_createduser
				 ,pvar_TherapyItemid
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
																,'Add_Therapy_Item'
																,'Authorization Failed Add_Therapy_Item'
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
						,'Add_Therapy_Item'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Therapy_Item - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

