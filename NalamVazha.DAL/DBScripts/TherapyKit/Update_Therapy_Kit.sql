
			  CREATE OR REPLACE FUNCTION  "Update_Therapy_Kit"
			  (
				  pvar_TherapyKitid uuid
,pvar_tenantid uuid
,
pvar_therapykitname Varchar(128)
,
pvar_kitprice Varchar(256)
,pvar_kititems json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:45*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TherapyKit', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from TherapyKit where upper(TherapyKit.therapykitname) = upper(pvar_therapykitname) and TherapyKit.tenantid=pvar_tenantid  and TherapyKit.TherapyKitid <> pvar_TherapyKitid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Therapy Kit Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TherapyKit', NOW(),
(SELECT query_to_xml('SELECT * FROM TherapyKit WHERE TherapyKit.TherapyKitid= '''||pvar_TherapyKitid||'''', true, false, '')));

                    
                    UPDATE TherapyKit SET
                    therapykitname=pvar_therapykitname
,kitprice=pvar_kitprice

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TherapyKitid=pvar_TherapyKitid;

                    

                    INSERT INTO history
VALUES('TherapyKit_kititems', NOW(),
(SELECT query_to_xml('SELECT * FROM TherapyKit_kititems WHERE TherapyKit_kititems.TherapyKitid= '''||pvar_TherapyKitid||'''', true, false, '')));

								DELETE FROM  TherapyKit_kititems WHERE TherapyKitid=pvar_TherapyKitid;
								
								
								INSERT INTO TherapyKit_kititems (
									TherapyKitid
									,TherapyKit_kititemsid 
                                    ,record_order  
									,therapyitem
,price
,count
,linetotal

									
									)
									SELECT 
									pvar_TherapyKitid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'therapyitem' AS uuid) as therapyitem
,CAST(j->>'price' AS decimal(18,2)) as price
,CAST(j->>'count' AS int) as count
,j->>'linetotal' as linetotal

									
                                    FROM json_array_elements(pvar_kititems) as j;
									



					
							
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
																,'Update_Therapy_Kit'
																,'Authorization Failed Update_Therapy_Kit'
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
						,'Update_Therapy_Kit'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Therapy_Kit - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

