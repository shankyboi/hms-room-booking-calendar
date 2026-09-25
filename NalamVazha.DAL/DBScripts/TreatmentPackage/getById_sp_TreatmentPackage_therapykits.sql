CREATE OR REPLACE FUNCTION "getById_sp_TreatmentPackage_therapykits"(
												 pvar_TreatmentPackageid Varchar(50)
											 )
                                             RETURNS TABLE("TreatmentPackageid" uuid,"TreatmentPackage_therapykitsid" uuid ,therapykitname uuid
,kitprice Varchar
,numberofkits int
,therapykitcost Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TreatmentPackage_therapykits.TreatmentPackageid
                                             ,TreatmentPackage_therapykits.TreatmentPackage_therapykitsid   
											 ,TreatmentPackage_therapykits.therapykitname
,TreatmentPackage_therapykits.kitprice
,TreatmentPackage_therapykits.numberofkits
,TreatmentPackage_therapykits.therapykitcost
 
											 
											 FROM TreatmentPackage_therapykits
											 WHERE 
											 CAST(TreatmentPackage_therapykits.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

