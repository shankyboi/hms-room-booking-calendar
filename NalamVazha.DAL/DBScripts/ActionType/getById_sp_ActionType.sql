 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_ActionType"
			  (
				  pvar_ActionTypeid Varchar
			  )
			  RETURNS TABLE(
                actiontype Varchar
,description Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,ActionTypeid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:14:11*/
               
              RETURN QUERY
			  SELECT 
				 ActionType.actiontype
,ActionType.description

				 ,ActionType.createduser,ActionType.createddate,ActionType.modifieduser,ActionType.modifieddate
				 ,ActionType.tenantid
                 ,ActionType.ActionTypeid
                    
			  FROM ActionType
			  WHERE CAST(ActionType.ActionTypeid AS Varchar)=pvar_ActionTypeid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

