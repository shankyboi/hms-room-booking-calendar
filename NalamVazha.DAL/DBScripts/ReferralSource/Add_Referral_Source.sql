
			  CREATE OR REPLACE FUNCTION  "Add_Referral_Source"
			  (
				  pvar_ReferralSourceid uuid
,pvar_tenantid uuid
,
pvar_referralsourcename Varchar(128)
,
pvar_contactnumber Varchar(10)
,
pvar_websiteurl Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:40*/
		

			  
                                                                                    if pvar_ReferralSourceid is null then
                                                                                    pvar_ReferralSourceid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'ReferralSource', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from ReferralSource where upper(ReferralSource.referralsourcename::varchar) = upper(pvar_referralsourcename::varchar) and ReferralSource.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Referral Source Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO ReferralSource(
				 referralsourcename
,contactnumber
,websiteurl

				 ,createduser
				 ,ReferralSourceid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_referralsourcename
,pvar_contactnumber
,pvar_websiteurl

				 ,pvar_createduser
				 ,pvar_ReferralSourceid
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
																,'Add_Referral_Source'
																,'Authorization Failed Add_Referral_Source'
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
						,'Add_Referral_Source'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Referral_Source - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

