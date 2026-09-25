CREATE OR REPLACE FUNCTION "getById_sp_TreatmentPackage_refundpolicy"(
												 pvar_TreatmentPackageid Varchar(50)
											 )
                                             RETURNS TABLE("TreatmentPackageid" uuid,"TreatmentPackage_refundpolicyid" uuid ,refundtype Varchar
,cancellationby Varchar
,cancellationwindowdays int
,refundpercentage decimal
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TreatmentPackage_refundpolicy.TreatmentPackageid
                                             ,TreatmentPackage_refundpolicy.TreatmentPackage_refundpolicyid   
											 ,TreatmentPackage_refundpolicy.refundtype
,TreatmentPackage_refundpolicy.cancellationby
,TreatmentPackage_refundpolicy.cancellationwindowdays
,TreatmentPackage_refundpolicy.refundpercentage
 
											 
											 FROM TreatmentPackage_refundpolicy
											 WHERE 
											 CAST(TreatmentPackage_refundpolicy.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

