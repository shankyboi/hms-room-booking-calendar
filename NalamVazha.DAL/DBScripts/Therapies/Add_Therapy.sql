
			  CREATE OR REPLACE FUNCTION  "Add_Therapy"
			  (
				  pvar_Therapiesid uuid
,pvar_tenantid uuid
,
pvar_therapycategory  uuid
,
pvar_therapyname Varchar(128)
,
pvar_therapycost decimal(18,2)
,
pvar_standarddurationinmins int
,
pvar_therapyimage Varchar(256)
,
pvar_therapyvideourl Varchar(256)
,
pvar_therapyinstructions text
,
pvar_isgrouptherapyallowed Boolean
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:30*/
		

			  
                                                                                    if pvar_Therapiesid is null then
                                                                                    pvar_Therapiesid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Therapies', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Therapies where upper(Therapies.therapyname::varchar) = upper(pvar_therapyname::varchar) and Therapies.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Therapy Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Therapies(
				 therapycategory
,therapyname
,therapycost
,standarddurationinmins
,therapyimage
,therapyvideourl
,therapyinstructions
,isgrouptherapyallowed

				 ,createduser
				 ,Therapiesid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_therapycategory
,pvar_therapyname
,pvar_therapycost
,pvar_standarddurationinmins
,pvar_therapyimage
,pvar_therapyvideourl
,pvar_therapyinstructions
,pvar_isgrouptherapyallowed

				 ,pvar_createduser
				 ,pvar_Therapiesid
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
																,'Add_Therapy'
																,'Authorization Failed Add_Therapy'
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
						,'Add_Therapy'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Therapy - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

