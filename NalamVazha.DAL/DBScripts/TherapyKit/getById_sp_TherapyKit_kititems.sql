CREATE OR REPLACE FUNCTION "getById_sp_TherapyKit_kititems"(
												 pvar_TherapyKitid Varchar(50)
											 )
                                             RETURNS TABLE("TherapyKitid" uuid,"TherapyKit_kititemsid" uuid ,therapyitem uuid
,price decimal
,count int
,linetotal Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TherapyKit_kititems.TherapyKitid
                                             ,TherapyKit_kititems.TherapyKit_kititemsid   
											 ,TherapyKit_kititems.therapyitem
,TherapyKit_kititems.price
,TherapyKit_kititems.count
,TherapyKit_kititems.linetotal
 
											 
											 FROM TherapyKit_kititems
											 WHERE 
											 CAST(TherapyKit_kititems.TherapyKitid AS VARCHAR)=pvar_TherapyKitid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

