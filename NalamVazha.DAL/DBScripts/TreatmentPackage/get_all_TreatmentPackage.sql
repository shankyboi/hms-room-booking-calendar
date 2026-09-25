 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_TreatmentPackage"
              (
			  pvar_tenantid Varchar=null
              )
			 RETURNS TABLE(
                packagename Varchar
,noofdays int
,roomtypeamountaverage Varchar
,therapyamount Varchar
,therapykitamount Varchar
,therapyitemamount Varchar
,medicineamount Varchar
,calculatedpackagecost Varchar
,packagecost decimal
,packagebookingdeposit decimal
,packagebookingadvance decimal
,billingwaiverfordelayedstart Varchar
,waiverpercentage decimal
,roomtransfercost Varchar
,packagedescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"TreatmentPackageid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
              
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

				 TreatmentPackage.packagename
,TreatmentPackage.noofdays
,TreatmentPackage.roomtypeamountaverage
,TreatmentPackage.therapyamount
,TreatmentPackage.therapykitamount
,TreatmentPackage.therapyitemamount
,TreatmentPackage.medicineamount
,TreatmentPackage.calculatedpackagecost
,TreatmentPackage.packagecost
,TreatmentPackage.packagebookingdeposit
,TreatmentPackage.packagebookingadvance
,TreatmentPackage.billingwaiverfordelayedstart
,TreatmentPackage.waiverpercentage
,TreatmentPackage.roomtransfercost
,TreatmentPackage.packagedescription

				 ,TreatmentPackage.createduser,TreatmentPackage.createddate,TreatmentPackage.modifieduser,TreatmentPackage.modifieddate
				,TreatmentPackage.tenantid 
                ,TreatmentPackage.TreatmentPackageid
				 
			  FROM TreatmentPackage
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(TreatmentPackage.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND TreatmentPackage.isdeleted=false
			  ;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

