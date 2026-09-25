 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_MailLogs"
			  (
				  pvar_MailLogsid Varchar
			  )
			  RETURNS TABLE(
                entityname Varchar
,entityid Varchar
,mailfor Varchar
,mailsubject Varchar
,mailto Varchar
,mailbody text
,issent Boolean
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,MailLogsid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:27*/
               
              RETURN QUERY
			  SELECT 
				 MailLogs.entityname
,MailLogs.entityid
,MailLogs.mailfor
,MailLogs.mailsubject
,MailLogs.mailto
,MailLogs.mailbody
,COALESCE(MailLogs.issent,true) as issent

				 ,MailLogs.createduser,MailLogs.createddate,MailLogs.modifieduser,MailLogs.modifieddate
				 
                 ,MailLogs.MailLogsid
                    
			  FROM MailLogs
			  WHERE CAST(MailLogs.MailLogsid AS Varchar)=pvar_MailLogsid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

