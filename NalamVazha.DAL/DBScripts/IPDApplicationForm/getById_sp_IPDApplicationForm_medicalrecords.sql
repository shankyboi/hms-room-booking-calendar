CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_medicalrecords"(
												 pvar_IPDApplicationFormid Varchar(50)
											 )
                                             RETURNS TABLE("IPDApplicationFormid" uuid,"IPDApplicationForm_medicalrecordsid" uuid ,medicalrecordname Varchar
,medicalrecordfile Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 IPDApplicationForm_medicalrecords.IPDApplicationFormid
                                             ,IPDApplicationForm_medicalrecords.IPDApplicationForm_medicalrecordsid   
											 ,IPDApplicationForm_medicalrecords.medicalrecordname
,IPDApplicationForm_medicalrecords.medicalrecordfile
 
											 
											 FROM IPDApplicationForm_medicalrecords
											 WHERE 
											 CAST(IPDApplicationForm_medicalrecords.IPDApplicationFormid AS VARCHAR)=pvar_IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_medicalrecords.isdeleted,false) = false   
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

