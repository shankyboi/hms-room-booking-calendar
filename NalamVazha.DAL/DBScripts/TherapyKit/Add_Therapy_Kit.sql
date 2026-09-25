
			  CREATE OR REPLACE FUNCTION  "Add_Therapy_Kit"
			  (
				  pvar_TherapyKitid uuid
,pvar_tenantid uuid
,
pvar_therapykitname Varchar(128)
,
pvar_kitprice Varchar(256)
,pvar_kititems json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:45*/
		

			  
                                                                                    if pvar_TherapyKitid is null then
                                                                                    pvar_TherapyKitid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TherapyKit', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from TherapyKit where upper(TherapyKit.therapykitname::varchar) = upper(pvar_therapykitname::varchar) and TherapyKit.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Therapy Kit Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TherapyKit(
				 therapykitname
,kitprice

				 ,createduser
				 ,TherapyKitid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_therapykitname
,pvar_kitprice

				 ,pvar_createduser
				 ,pvar_TherapyKitid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
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
																,'Add_Therapy_Kit'
																,'Authorization Failed Add_Therapy_Kit'
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
						,'Add_Therapy_Kit'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Therapy_Kit - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

