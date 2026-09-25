CREATE OR REPLACE FUNCTION "getById_sp_TreatmentPackage_therapy"(
												 pvar_TreatmentPackageid Varchar(50)
											 )
                                             RETURNS TABLE("TreatmentPackageid" uuid,"TreatmentPackage_therapyid" uuid ,therapyname uuid
,therapycost decimal
,numberoftimes int
,therapyprice Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TreatmentPackage_therapy.TreatmentPackageid
                                             ,TreatmentPackage_therapy.TreatmentPackage_therapyid   
											 ,TreatmentPackage_therapy.therapyname
,TreatmentPackage_therapy.therapycost
,TreatmentPackage_therapy.numberoftimes
,TreatmentPackage_therapy.therapyprice
 
											 
											 FROM TreatmentPackage_therapy
											 WHERE 
											 CAST(TreatmentPackage_therapy.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

