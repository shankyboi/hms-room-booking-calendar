DROP FUNCTION IF EXISTS "getById_sp_OPDForm_medicationinfo"(Varchar);
CREATE OR REPLACE FUNCTION "getById_sp_OPDForm_medicationinfo"(
												 pvar_OPDFormid Varchar(50)
											 )
                                             RETURNS TABLE("OPDFormid" uuid,"OPDForm_medicationinfoid" uuid ,medicinename Varchar
,frequencyinaday Varchar
,medicationduration Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 OPDForm_medicationinfo.OPDFormid
                                             ,OPDForm_medicationinfo.OPDForm_medicationinfoid   
											 ,OPDForm_medicationinfo.medicinename
,OPDForm_medicationinfo.frequencyinaday
,OPDForm_medicationinfo.medicationduration
 
											 
											 FROM OPDForm_medicationinfo
											 WHERE 
											 CAST(OPDForm_medicationinfo.OPDFormid AS VARCHAR)=pvar_OPDFormid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

