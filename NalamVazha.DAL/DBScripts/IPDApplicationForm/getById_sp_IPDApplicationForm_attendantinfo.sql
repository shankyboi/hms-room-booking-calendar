CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_attendantinfo"(
												 pvar_IPDApplicationFormid Varchar(50)
											 )
                                             RETURNS TABLE("IPDApplicationFormid" uuid,"IPDApplicationForm_attendantinfoid" uuid ,attendantname Varchar
,age int
,gender Varchar
,phonenumber Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 IPDApplicationForm_attendantinfo.IPDApplicationFormid
                                             ,IPDApplicationForm_attendantinfo.IPDApplicationForm_attendantinfoid   
											 ,IPDApplicationForm_attendantinfo.attendantname
,IPDApplicationForm_attendantinfo.age
,IPDApplicationForm_attendantinfo.gender
,IPDApplicationForm_attendantinfo.phonenumber
 
											 
											 FROM IPDApplicationForm_attendantinfo
											 WHERE 
											 CAST(IPDApplicationForm_attendantinfo.IPDApplicationFormid AS VARCHAR)=pvar_IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_attendantinfo.isdeleted,false) = false   
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

