
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_AlertTemplates"
              (
			  pvar_AlertTemplatesid Varchar
			  )
              RETURNS TABLE(
                "AlertTemplatesid" uuid
,entityname Varchar
,entityaction Varchar
,sendasbatch Varchar
,alerttype Varchar
,alertsubject Varchar
,alertcopyto Varchar
,alertcarboncopyto Varchar
,alertcontent text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:25*/
			  		 
              RETURN QUERY
			  SELECT  
				 AlertTemplates.AlertTemplatesid
,AlertTemplates.entityname
,AlertTemplates.entityaction
,CAST(case when AlertTemplates.sendasbatch=true then 'Yes' else 'No' End AS Varchar) as sendasbatch
,AlertTemplates.alerttype
,AlertTemplates.alertsubject
,AlertTemplates.alertcopyto
,AlertTemplates.alertcarboncopyto
,AlertTemplates.alertcontent

				 ,AlertTemplates.createduser,AlertTemplates.createddate,AlertTemplates.modifieduser,AlertTemplates.modifieddate
                 
                 
				 
			  FROM  AlertTemplates 

			  WHERE CAST(AlertTemplates.AlertTemplatesid AS Varchar)=pvar_AlertTemplatesid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

