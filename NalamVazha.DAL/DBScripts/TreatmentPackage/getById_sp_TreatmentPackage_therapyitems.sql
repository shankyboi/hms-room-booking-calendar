CREATE OR REPLACE FUNCTION "getById_sp_TreatmentPackage_therapyitems"(
												 pvar_TreatmentPackageid Varchar(50)
											 )
                                             RETURNS TABLE("TreatmentPackageid" uuid,"TreatmentPackage_therapyitemsid" uuid ,therapyitem uuid
,price decimal
,therapyitemcount int
,therapyitemcost Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TreatmentPackage_therapyitems.TreatmentPackageid
                                             ,TreatmentPackage_therapyitems.TreatmentPackage_therapyitemsid   
											 ,TreatmentPackage_therapyitems.therapyitem
,TreatmentPackage_therapyitems.price
,TreatmentPackage_therapyitems.therapyitemcount
,TreatmentPackage_therapyitems.therapyitemcost
 
											 
											 FROM TreatmentPackage_therapyitems
											 WHERE 
											 CAST(TreatmentPackage_therapyitems.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

