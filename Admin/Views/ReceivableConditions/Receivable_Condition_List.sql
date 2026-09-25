
			  CREATE OR REPLACE FUNCTION  "Receivable_Condition_List"
              (pvar_tenantid Varchar
,pvar_receivablefor Varchar(1024)
)
			  RETURNS TABLE(tenantid uuid
,_tenantName Varchar(128)
,ReceivableConditionsid uuid
,receivablefor Varchar,amount decimal,ismandatory Varchar,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/23/2026 09:15:10*/
			  		
                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;
		        
                if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')	
				then
                    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
				    FROM users where users.usersid::varchar=lstr_usersid;	
                    if(lvar_tenantid is NULL)
					then 
						SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
               
					end if;
                else 
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;
                lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];



              
                RETURN QUERY
				SELECT  
				ReceivableConditions.tenantid
,tenant.businessname as _tenantName
,ReceivableConditions.ReceivableConditionsid
,ReceivableConditions.receivablefor
,ReceivableConditions.amount
,CAST(case when ReceivableConditions.ismandatory=true then 'Yes' else 'No' End AS Varchar)as ismandatory

				
				,ReceivableConditions.createduser,ReceivableConditions.createddate,ReceivableConditions.modifieduser,ReceivableConditions.modifieddate
				FROM  ReceivableConditions 
 LEFT OUTER JOIN tenant ON ReceivableConditions.tenantid=tenant.tenantid

				WHERE (lvar_tenantid is null or COALESCE(cast(ReceivableConditions.tenantid as varchar), '') = Any(lvar_tenantid)) AND ReceivableConditions.isdeleted=false
AND (pvar_receivablefor is null or pvar_receivablefor ='0' or LENGTH(CAST(pvar_receivablefor as Varchar))=0 or CAST(ReceivableConditions.receivablefor as VARCHAR)=pvar_receivablefor)

				 ORDER BY ReceivableConditions.createddate DESC;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

