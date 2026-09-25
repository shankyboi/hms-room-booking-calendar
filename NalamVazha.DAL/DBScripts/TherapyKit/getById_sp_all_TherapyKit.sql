
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TherapyKit"
              (
			  pvar_TherapyKitid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TherapyKitid" uuid
,therapykitname Varchar
,kitprice Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_TherapyKit_kititems" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:45*/
			  		 
              RETURN QUERY
			  SELECT  
				 TherapyKit.tenantid
,tenant.businessname as _tenantname
,TherapyKit.TherapyKitid
,TherapyKit.therapykitname
,TherapyKit.kitprice

				 ,TherapyKit.createduser,TherapyKit.createddate,TherapyKit.modifieduser,TherapyKit.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_TherapyItem.therapyitemname AS VARCHAR) as "Therapy Item"
,TherapyKit_kititems.price as "Price"
,TherapyKit_kititems.count as "count"
,TherapyKit_kititems.linetotal as "Line Total"

							
						FROM  TherapyKit_kititems 
INNER JOIN TherapyItem _TherapyItem ON TherapyKit_kititems.therapyitem=_TherapyItem.TherapyItemid

						WHERE TherapyKit.TherapyKitid =TherapyKit_kititems.TherapyKitid
) J)
						as automaton_TherapyKit_kititems

                 
				 
			  FROM  TherapyKit 
 LEFT OUTER JOIN tenant ON TherapyKit.tenantid=tenant.tenantid

			  WHERE CAST(TherapyKit.TherapyKitid AS Varchar)=pvar_TherapyKitid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

