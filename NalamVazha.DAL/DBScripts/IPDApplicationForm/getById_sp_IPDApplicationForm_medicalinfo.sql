CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_medicalinfo"(
												 pvar_IPDApplicationFormid Varchar(50)
											 )
                                             RETURNS TABLE("IPDApplicationFormid" uuid,"IPDApplicationForm_medicalinfoid" uuid ,medicalconditionname uuid
,duration decimal
,unit Varchar
,severitylevel Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 IPDApplicationForm_medicalinfo.IPDApplicationFormid
                                             ,IPDApplicationForm_medicalinfo.IPDApplicationForm_medicalinfoid   
											 ,IPDApplicationForm_medicalinfo.medicalconditionname
,IPDApplicationForm_medicalinfo.duration
,IPDApplicationForm_medicalinfo.unit
,IPDApplicationForm_medicalinfo.severitylevel
 
											 
											 FROM IPDApplicationForm_medicalinfo
											 WHERE 
											 CAST(IPDApplicationForm_medicalinfo.IPDApplicationFormid AS VARCHAR)=pvar_IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_medicalinfo.isdeleted,false) = false   
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

