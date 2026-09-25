CREATE OR REPLACE FUNCTION "getById_sp_OPDForm_medicalinfo"(
												 pvar_OPDFormid Varchar(50)
											 )
                                             RETURNS TABLE("OPDFormid" uuid,"OPDForm_medicalinfoid" uuid ,medicalconditionname uuid
,duration decimal
,unit Varchar
,severitylevel Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 OPDForm_medicalinfo.OPDFormid
                                             ,OPDForm_medicalinfo.OPDForm_medicalinfoid   
											 ,OPDForm_medicalinfo.medicalconditionname
,OPDForm_medicalinfo.duration
,OPDForm_medicalinfo.unit
,OPDForm_medicalinfo.severitylevel
 
											 
											 FROM OPDForm_medicalinfo
											 WHERE 
											 CAST(OPDForm_medicalinfo.OPDFormid AS VARCHAR)=pvar_OPDFormid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

