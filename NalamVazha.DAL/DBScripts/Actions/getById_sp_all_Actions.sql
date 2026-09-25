
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Actions"
              (
			  pvar_Actionsid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Actionsid" uuid
,actiontype Varchar
,actionname Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:16:42*/
			  		 
              RETURN QUERY
			  SELECT  
				 Actions.tenantid
,tenant.businessname as _tenantname
,Actions.Actionsid
,CAST(_ActionType.actiontype AS VARCHAR) as actiontype
,Actions.actionname
,Actions.description

				 ,Actions.createduser,Actions.createddate,Actions.modifieduser,Actions.modifieddate
                 
                 
				 
			  FROM  Actions 
 LEFT OUTER JOIN tenant ON Actions.tenantid=tenant.tenantid
LEFT OUTER JOIN ActionType _ActionType ON Actions.actiontype=_ActionType.ActionTypeid

			  WHERE CAST(Actions.Actionsid AS Varchar)=pvar_Actionsid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

