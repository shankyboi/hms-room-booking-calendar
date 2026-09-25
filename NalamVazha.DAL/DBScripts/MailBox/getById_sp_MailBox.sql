 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_MailBox"
			  (
				  pvar_MailBoxid Varchar
			  )
			  RETURNS TABLE(
                senderdisplayname Varchar
,senderemail Varchar
,password Varchar
,emailhostname Varchar
,portnumber int
,applicableservice Varchar
,emailfooter text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,MailBoxid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:14*/
               
              RETURN QUERY
			  SELECT 
				 MailBox.senderdisplayname
,MailBox.senderemail
,MailBox.password
,MailBox.emailhostname
,MailBox.portnumber
,MailBox.applicableservice
,MailBox.emailfooter

				 ,MailBox.createduser,MailBox.createddate,MailBox.modifieduser,MailBox.modifieddate
				 ,MailBox.tenantid
                 ,MailBox.MailBoxid
                    
			  FROM MailBox
			  WHERE CAST(MailBox.MailBoxid AS Varchar)=pvar_MailBoxid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

