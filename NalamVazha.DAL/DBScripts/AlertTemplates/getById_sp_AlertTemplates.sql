 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_AlertTemplates"
			  (
				  pvar_AlertTemplatesid Varchar
			  )
			  RETURNS TABLE(
                entityname Varchar
,entityaction Varchar
,sendasbatch Boolean
,alerttype Varchar
,alertsubject Varchar
,alertcopyto Varchar
,alertcarboncopyto Varchar
,alertcontent text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,AlertTemplatesid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:25*/
               
              RETURN QUERY
			  SELECT 
				 AlertTemplates.entityname
,AlertTemplates.entityaction
,COALESCE(AlertTemplates.sendasbatch,true) as sendasbatch
,AlertTemplates.alerttype
,AlertTemplates.alertsubject
,AlertTemplates.alertcopyto
,AlertTemplates.alertcarboncopyto
,AlertTemplates.alertcontent

				 ,AlertTemplates.createduser,AlertTemplates.createddate,AlertTemplates.modifieduser,AlertTemplates.modifieddate
				 
                 ,AlertTemplates.AlertTemplatesid
                    
			  FROM AlertTemplates
			  WHERE CAST(AlertTemplates.AlertTemplatesid AS Varchar)=pvar_AlertTemplatesid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

