CREATE OR REPLACE FUNCTION "getById_sp_TreatmentPackage_roomtypes"(
												 pvar_TreatmentPackageid Varchar(50)
											 )
                                             RETURNS TABLE("TreatmentPackageid" uuid,"TreatmentPackage_roomtypesid" uuid ,roomtype uuid
,costperday decimal
,percentagecovered decimal
,roomcost Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TreatmentPackage_roomtypes.TreatmentPackageid
                                             ,TreatmentPackage_roomtypes.TreatmentPackage_roomtypesid   
											 ,TreatmentPackage_roomtypes.roomtype
,TreatmentPackage_roomtypes.costperday
,TreatmentPackage_roomtypes.percentagecovered
,TreatmentPackage_roomtypes.roomcost
 
											 
											 FROM TreatmentPackage_roomtypes
											 WHERE 
											 CAST(TreatmentPackage_roomtypes.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

