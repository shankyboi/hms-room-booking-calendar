CREATE OR REPLACE FUNCTION "getById_sp_EnquiryForm_medicalinfo"(
												 pvar_EnquiryFormid Varchar(50)
											 )
                                             RETURNS TABLE("EnquiryFormid" uuid,"EnquiryForm_medicalinfoid" uuid ,medicalcondition uuid
,conditionname Varchar
,duration Varchar
,severity Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 EnquiryForm_medicalinfo.EnquiryFormid
                                             ,EnquiryForm_medicalinfo.EnquiryForm_medicalinfoid   
											 ,EnquiryForm_medicalinfo.medicalcondition
,EnquiryForm_medicalinfo.conditionname
,EnquiryForm_medicalinfo.duration
,EnquiryForm_medicalinfo.severity
 
											 
											 FROM EnquiryForm_medicalinfo
											 WHERE 
											 CAST(EnquiryForm_medicalinfo.EnquiryFormid AS VARCHAR)=pvar_EnquiryFormid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

