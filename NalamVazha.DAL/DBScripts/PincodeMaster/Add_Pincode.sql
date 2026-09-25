
			  CREATE OR REPLACE FUNCTION  "Add_Pincode"
			  (
				  pvar_PincodeMasterid uuid
,
pvar_circlename Varchar(256)
,
pvar_regionname Varchar(256)
,
pvar_divisionname Varchar(256)
,
pvar_officename Varchar(256)
,
pvar_pincode Varchar(256)
,
pvar_officetype Varchar(256)
,
pvar_delivery Varchar(256)
,
pvar_district Varchar(256)
,
pvar_statename Varchar(256)
,
pvar_latitude Varchar(256)
,
pvar_longitude Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/16/2026 09:42:32*/
		

			  
                                                                                    if pvar_PincodeMasterid is null then
                                                                                    pvar_PincodeMasterid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'PincodeMaster', 'create') THEN
			  pvar_returnMessage:='';
			  
                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PincodeMaster(
				 circlename
,regionname
,divisionname
,officename
,pincode
,officetype
,delivery
,district
,statename
,latitude
,longitude

				 ,createduser
				 ,PincodeMasterid
				 
                
			  )
			  VALUES (
 				 pvar_circlename
,pvar_regionname
,pvar_divisionname
,pvar_officename
,pvar_pincode
,pvar_officetype
,pvar_delivery
,pvar_district
,pvar_statename
,pvar_latitude
,pvar_longitude

				 ,pvar_createduser
				 ,pvar_PincodeMasterid
				 
                   
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
																,'Add_Pincode'
																,'Authorization Failed Add_Pincode'
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
						,'Add_Pincode'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Pincode - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

