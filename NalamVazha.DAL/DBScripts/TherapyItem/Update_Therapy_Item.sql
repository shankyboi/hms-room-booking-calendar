
			  CREATE OR REPLACE FUNCTION  "Update_Therapy_Item"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:42*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TherapyItem', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from TherapyItem where upper(TherapyItem.therapyitemname) = upper(pvar_therapyitemname) and TherapyItem.tenantid=pvar_tenantid  and TherapyItem.TherapyItemid <> pvar_TherapyItemid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Therapy Item Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TherapyItem', NOW(),
(SELECT query_to_xml('SELECT * FROM TherapyItem WHERE TherapyItem.TherapyItemid= '''||pvar_TherapyItemid||'''', true, false, '')));

                    
                    UPDATE TherapyItem SET
                    therapyitemcategory=pvar_therapyitemcategory
,therapyitemname=pvar_therapyitemname
,price=pvar_price
,therapyitemimage=pvar_therapyitemimage

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TherapyItemid=pvar_TherapyItemid;

                    

                    


					
							
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
																,'Update_Therapy_Item'
																,'Authorization Failed Update_Therapy_Item'
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
						,'Update_Therapy_Item'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Therapy_Item - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

