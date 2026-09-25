
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Therapies"
              (
			  pvar_Therapiesid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Therapiesid" uuid
,therapycategory Varchar
,therapyname Varchar
,therapycost decimal
,standarddurationinmins int
,therapyimage Varchar
,therapyvideourl Varchar
,therapyinstructions text
,isgrouptherapyallowed Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:30*/
			  		 
              RETURN QUERY
			  SELECT  
				 Therapies.tenantid
,tenant.businessname as _tenantname
,Therapies.Therapiesid
,CAST(_TherapyCategory.categoryname AS VARCHAR) as therapycategory
,Therapies.therapyname
,Therapies.therapycost
,Therapies.standarddurationinmins
,Therapies.therapyimage
,Therapies.therapyvideourl
,Therapies.therapyinstructions
,CAST(case when Therapies.isgrouptherapyallowed=true then 'Yes' else 'No' End AS Varchar) as isgrouptherapyallowed

				 ,Therapies.createduser,Therapies.createddate,Therapies.modifieduser,Therapies.modifieddate
                 
                 
				 
			  FROM  Therapies 
 LEFT OUTER JOIN tenant ON Therapies.tenantid=tenant.tenantid
INNER JOIN TherapyCategory _TherapyCategory ON Therapies.therapycategory=_TherapyCategory.TherapyCategoryid

			  WHERE CAST(Therapies.Therapiesid AS Varchar)=pvar_Therapiesid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

