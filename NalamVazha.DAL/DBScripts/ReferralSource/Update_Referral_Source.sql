
			  CREATE OR REPLACE FUNCTION  "Update_Referral_Source"
			  (
				  pvar_ReferralSourceid uuid
,pvar_tenantid uuid
,
pvar_referralsourcename Varchar(128)
,
pvar_contactnumber Varchar(10)
,
pvar_websiteurl Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:40*/
			  IF "Check_Authorization"(pvar_modifieduser, 'ReferralSource', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from ReferralSource where upper(ReferralSource.referralsourcename) = upper(pvar_referralsourcename) and ReferralSource.tenantid=pvar_tenantid  and ReferralSource.ReferralSourceid <> pvar_ReferralSourceid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Referral Source Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('ReferralSource', NOW(),
(SELECT query_to_xml('SELECT * FROM ReferralSource WHERE ReferralSource.ReferralSourceid= '''||pvar_ReferralSourceid||'''', true, false, '')));

                    
                    UPDATE ReferralSource SET
                    referralsourcename=pvar_referralsourcename
,contactnumber=pvar_contactnumber
,websiteurl=pvar_websiteurl

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE ReferralSourceid=pvar_ReferralSourceid;

                    

                    


					
							
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
																,'Update_Referral_Source'
																,'Authorization Failed Update_Referral_Source'
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
						,'Update_Referral_Source'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Referral_Source - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

