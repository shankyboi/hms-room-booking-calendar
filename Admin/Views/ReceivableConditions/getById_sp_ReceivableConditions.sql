 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_ReceivableConditions"
			  (
				  pvar_ReceivableConditionsid Varchar
			  )
			  RETURNS TABLE(
                receivablefor Varchar
,amount decimal
,ismandatory Boolean
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,ReceivableConditionsid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/23/2026 09:15:10*/
               
              RETURN QUERY
			  SELECT 
				 ReceivableConditions.receivablefor
,ReceivableConditions.amount
,COALESCE(ReceivableConditions.ismandatory,true) as ismandatory

				 ,ReceivableConditions.createduser,ReceivableConditions.createddate,ReceivableConditions.modifieduser,ReceivableConditions.modifieddate
				 ,ReceivableConditions.tenantid
                 ,ReceivableConditions.ReceivableConditionsid
                    
			  FROM ReceivableConditions
			  WHERE CAST(ReceivableConditions.ReceivableConditionsid AS Varchar)=pvar_ReceivableConditionsid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

