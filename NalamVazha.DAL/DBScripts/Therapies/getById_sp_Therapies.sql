 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Therapies"
			  (
				  pvar_Therapiesid Varchar
			  )
			  RETURNS TABLE(
                therapycategory uuid
,therapyname Varchar
,therapycost decimal
,standarddurationinmins int
,therapyimage Varchar
,therapyvideourl Varchar
,therapyinstructions text
,isgrouptherapyallowed Boolean
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Therapiesid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:30*/
               
              RETURN QUERY
			  SELECT 
				 Therapies.therapycategory
,Therapies.therapyname
,Therapies.therapycost
,Therapies.standarddurationinmins
,Therapies.therapyimage
,Therapies.therapyvideourl
,Therapies.therapyinstructions
,COALESCE(Therapies.isgrouptherapyallowed,true) as isgrouptherapyallowed

				 ,Therapies.createduser,Therapies.createddate,Therapies.modifieduser,Therapies.modifieddate
				 ,Therapies.tenantid
                 ,Therapies.Therapiesid
                    
			  FROM Therapies
			  WHERE CAST(Therapies.Therapiesid AS Varchar)=pvar_Therapiesid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

