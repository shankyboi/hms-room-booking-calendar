
			  CREATE OR REPLACE FUNCTION  "Alert_Templates_List"
              (pvar_entityname Varchar(1024)
,pvar_entityaction Varchar(1024)
,pvar_alerttype Varchar(1024)
)
			  RETURNS TABLE(actionmethodname character varying
,AlertTemplatesid uuid
,entityname Varchar,entityaction Varchar,sendasbatch Varchar,alerttype Varchar,alertsubject Varchar,alertcopyto Varchar,alertcarboncopyto Varchar,alertcontent text,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:25*/
			  		
              
                RETURN QUERY
				SELECT  
				(
    SELECT roleauthorization.actionmethodname
    FROM roleauthorization
    WHERE controllername = pvar_entityname
      AND actionname IN ('Detail', 'CheckerDetail')
    ORDER BY CASE 
                 WHEN actionname LIKE '%CheckerDetail%' THEN 1
                 ELSE 2
             END
    LIMIT 1
) AS actionmethodname
,AlertTemplates.AlertTemplatesid
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

				WHERE AlertTemplates.isdeleted=false 
AND (pvar_entityname is null or pvar_entityname ='0' or LENGTH(CAST(pvar_entityname as Varchar))=0 or CAST(AlertTemplates.entityname as VARCHAR)=pvar_entityname)
AND (pvar_entityaction is null or pvar_entityaction ='0' or LENGTH(CAST(pvar_entityaction as Varchar))=0 or CAST(AlertTemplates.entityaction as VARCHAR)=pvar_entityaction)
AND (pvar_alerttype is null or pvar_alerttype ='0' or LENGTH(CAST(pvar_alerttype as Varchar))=0 or CAST(AlertTemplates.alerttype as VARCHAR)=pvar_alerttype)

				 ORDER BY AlertTemplates.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

