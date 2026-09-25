
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_ReceivableConditions"
              (
			  pvar_ReceivableConditionsid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"ReceivableConditionsid" uuid
,receivablefor Varchar
,amount decimal
,ismandatory Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/23/2026 09:15:10*/
			  		 
              RETURN QUERY
			  SELECT  
				 ReceivableConditions.tenantid
,tenant.businessname as _tenantname
,ReceivableConditions.ReceivableConditionsid
,ReceivableConditions.receivablefor
,ReceivableConditions.amount
,CAST(case when ReceivableConditions.ismandatory=true then 'Yes' else 'No' End AS Varchar) as ismandatory

				 ,ReceivableConditions.createduser,ReceivableConditions.createddate,ReceivableConditions.modifieduser,ReceivableConditions.modifieddate
                 
                 
				 
			  FROM  ReceivableConditions 
 LEFT OUTER JOIN tenant ON ReceivableConditions.tenantid=tenant.tenantid

			  WHERE CAST(ReceivableConditions.ReceivableConditionsid AS Varchar)=pvar_ReceivableConditionsid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

