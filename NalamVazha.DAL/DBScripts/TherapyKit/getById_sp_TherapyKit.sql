 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TherapyKit"
			  (
				  pvar_TherapyKitid Varchar
			  )
			  RETURNS TABLE(
                therapykitname Varchar
,kitprice Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TherapyKitid uuid
                ,kititems JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:45*/
               
              RETURN QUERY
			  SELECT 
				 TherapyKit.therapykitname
,TherapyKit.kitprice

				 ,TherapyKit.createduser,TherapyKit.createddate,TherapyKit.modifieduser,TherapyKit.modifieddate
				 ,TherapyKit.tenantid
                 ,TherapyKit.TherapyKitid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 TherapyKit_kititems.TherapyKitid
                                             ,TherapyKit_kititems.TherapyKit_kititemsid   
											 ,TherapyKit_kititems.therapyitem
,TherapyKit_kititems.price
,TherapyKit_kititems.count
,TherapyKit_kititems.linetotal
 
											  
											 FROM TherapyKit_kititems
											 WHERE 
											 TherapyKit_kititems.TherapyKitid=TherapyKit.TherapyKitid
                                             
                                             ORDER BY record_order DESC
											) J) as kititems
   
			  FROM TherapyKit
			  WHERE CAST(TherapyKit.TherapyKitid AS Varchar)=pvar_TherapyKitid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

