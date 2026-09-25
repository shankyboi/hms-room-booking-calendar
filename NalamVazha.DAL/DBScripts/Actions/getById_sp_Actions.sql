 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Actions"
			  (
				  pvar_Actionsid Varchar
			  )
			  RETURNS TABLE(
                actiontype uuid
,actionname Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Actionsid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:16:42*/
               
              RETURN QUERY
			  SELECT 
				 Actions.actiontype
,Actions.actionname
,Actions.description

				 ,Actions.createduser,Actions.createddate,Actions.modifieduser,Actions.modifieddate
				 ,Actions.tenantid
                 ,Actions.Actionsid
                    
			  FROM Actions
			  WHERE CAST(Actions.Actionsid AS Varchar)=pvar_Actionsid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

