CREATE OR REPLACE FUNCTION "getById_sp_OPDForm_medicalrecords"(
												 pvar_OPDFormid Varchar(50)
											 )
                                             RETURNS TABLE("OPDFormid" uuid,"OPDForm_medicalrecordsid" uuid ,medicalrecordname Varchar
,medicalrecordfile Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 OPDForm_medicalrecords.OPDFormid
                                             ,OPDForm_medicalrecords.OPDForm_medicalrecordsid   
											 ,OPDForm_medicalrecords.medicalrecordname
,OPDForm_medicalrecords.medicalrecordfile
 
											 
											 FROM OPDForm_medicalrecords
											 WHERE 
											 CAST(OPDForm_medicalrecords.OPDFormid AS VARCHAR)=pvar_OPDFormid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

