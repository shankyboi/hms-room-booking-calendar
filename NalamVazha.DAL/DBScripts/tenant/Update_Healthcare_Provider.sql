
			  DROP FUNCTION IF EXISTS public."Update_Healthcare_Provider"(uuid, character varying, character varying, character varying, character varying, character varying, character varying, integer, boolean, boolean, character varying, character varying, character varying, character varying, uuid, uuid, uuid);

			  CREATE OR REPLACE FUNCTION  "Update_Healthcare_Provider"
			  (
				  pvar_tenantid uuid
,
pvar_businessname Varchar(50)
,
pvar_natureofbusiness  Varchar(1024)
,
pvar_businessemail Varchar(128)
,
pvar_businessphone Varchar(20)
,
pvar_businesswebsite Varchar(256)
,
pvar_organizationlogo Varchar(4000)
,
pvar_numberofemployees int
,
pvar_enablepatientautologin Boolean
,
pvar_allowdoctortoadmitpatients Boolean
,
pvar_preadmissionnoticedays int
,
pvar_addressline1 Varchar(256)
,
pvar_addressline2 Varchar(256)
,
pvar_zip Varchar(256)
,
pvar_statename Varchar(256)
,
pvar_country  uuid
,
pvar_parentid  uuid

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:17*/
			  


			  pvar_returnMessage:='';

			  
               IF(pvar_natureofbusiness is not null AND pvar_natureofbusiness!='0' AND LENGTH(pvar_natureofbusiness)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_natureofbusiness, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='natureofbusiness'
                                                                and entityname='tenant' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_natureofbusiness, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'natureofbusiness value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('tenant', NOW(),
(SELECT query_to_xml('SELECT * FROM tenant WHERE tenant.tenantid= '''||pvar_tenantid||'''', true, false, '')));

                    
                    UPDATE tenant SET
                    businessname=pvar_businessname
,natureofbusiness=pvar_natureofbusiness
,businessemail=pvar_businessemail
,businessphone=pvar_businessphone
,businesswebsite=pvar_businesswebsite
,organizationlogo=pvar_organizationlogo
,numberofemployees=pvar_numberofemployees
,enablepatientautologin=pvar_enablepatientautologin
,allowdoctortoadmitpatients=pvar_allowdoctortoadmitpatients
,preadmissionnoticedays=pvar_preadmissionnoticedays
,addressline1=pvar_addressline1
,addressline2=pvar_addressline2
,zip=pvar_zip
,statename=pvar_statename
,country=pvar_country
,parentid=pvar_parentid

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE tenantid=pvar_tenantid;

                    IF(pvar_parentid is not null)
THEN
 

update users set viewertenantids=tenantid::varchar||','|| sub.tenantids FROM (SELECT STRING_AGG(tenantid::varchar, ',') AS tenantids
FROM tenant
WHERE parentid =pvar_parentid)sub
where tenantid=pvar_parentid;

END IF ;

                    


					
							
					pvar_returnMessage :='201.1';
			
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
						,'Update_Healthcare_Provider'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Healthcare_Provider - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;
