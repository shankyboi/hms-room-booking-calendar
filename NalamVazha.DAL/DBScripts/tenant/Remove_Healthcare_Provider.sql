
			  CREATE OR REPLACE FUNCTION  "Remove_Healthcare_Provider"
			  (
				  pvar_tenantid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:17*/
			  
			  
            INSERT INTO history
VALUES('tenant', NOW(),
(SELECT query_to_xml('SELECT * FROM tenant WHERE CAST(tenant.tenantid AS VARCHAR)= '''||pvar_tenantid||'''', true, false, '')));

			 
			 UPDATE tenant SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(tenant.tenantid AS VARCHAR)=pvar_tenantid;
					 
				  pvar_returnMessage := '201.1';
					 
			  
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
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Remove_Healthcare_Provider'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

