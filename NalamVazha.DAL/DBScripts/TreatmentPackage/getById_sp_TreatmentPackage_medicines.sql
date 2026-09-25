CREATE OR REPLACE FUNCTION "getById_sp_TreatmentPackage_medicines"(
												 pvar_TreatmentPackageid Varchar(50)
											 )
                                             RETURNS TABLE("TreatmentPackageid" uuid,"TreatmentPackage_medicinesid" uuid ,medicinename uuid
,price decimal
,medicinecount int
,medicinecost Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TreatmentPackage_medicines.TreatmentPackageid
                                             ,TreatmentPackage_medicines.TreatmentPackage_medicinesid   
											 ,TreatmentPackage_medicines.medicinename
,TreatmentPackage_medicines.price
,TreatmentPackage_medicines.medicinecount
,TreatmentPackage_medicines.medicinecost
 
											 
											 FROM TreatmentPackage_medicines
											 WHERE 
											 CAST(TreatmentPackage_medicines.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

