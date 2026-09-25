CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_medicationinfo"(
												 pvar_IPDApplicationFormid Varchar(50)
											 )
                                             RETURNS TABLE("IPDApplicationFormid" uuid,"IPDApplicationForm_medicationinfoid" uuid ,medicinename Varchar
,frequencyinaday Varchar
,medicationduration Varchar
,quantity numeric
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 IPDApplicationForm_medicationinfo.IPDApplicationFormid
                                             ,IPDApplicationForm_medicationinfo.IPDApplicationForm_medicationinfoid   
											 ,IPDApplicationForm_medicationinfo.medicinename
,IPDApplicationForm_medicationinfo.frequencyinaday
,IPDApplicationForm_medicationinfo.medicationduration
,IPDApplicationForm_medicationinfo.quantity
 
											 
											 FROM IPDApplicationForm_medicationinfo
											 WHERE 
											 CAST(IPDApplicationForm_medicationinfo.IPDApplicationFormid AS VARCHAR)=pvar_IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_medicationinfo.isdeleted,false) = false   
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

