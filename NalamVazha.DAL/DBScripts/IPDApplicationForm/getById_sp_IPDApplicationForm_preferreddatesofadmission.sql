CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_preferreddatesofadmission"(
												 pvar_IPDApplicationFormid Varchar(50)
											 )
                                             RETURNS TABLE("IPDApplicationFormid" uuid,"IPDApplicationForm_preferreddatesofadmissionid" uuid ,dateofarrival date
,dateofdeparture date
,daysofstay int
,record_order int
)
											 AS $BODY$
											 BEGIN

											 RETURN QUERY
											 SELECT
											 IPDApplicationForm_preferreddatesofadmission.IPDApplicationFormid
                                             ,IPDApplicationForm_preferreddatesofadmission.IPDApplicationForm_preferreddatesofadmissionid
											 ,IPDApplicationForm_preferreddatesofadmission.dateofarrival
,IPDApplicationForm_preferreddatesofadmission.dateofdeparture
,IPDApplicationForm_preferreddatesofadmission.daysofstay
,IPDApplicationForm_preferreddatesofadmission.record_order

											 FROM IPDApplicationForm_preferreddatesofadmission
											 WHERE
											 CAST(IPDApplicationForm_preferreddatesofadmission.IPDApplicationFormid AS VARCHAR)=pvar_IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_preferreddatesofadmission.isdeleted,false) = false
                                             ORDER BY record_order DESC;

											 END
                                             $BODY$
                                             LANGUAGE plpgsql;
