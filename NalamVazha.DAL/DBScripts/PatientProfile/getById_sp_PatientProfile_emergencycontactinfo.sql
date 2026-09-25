CREATE OR REPLACE FUNCTION "getById_sp_PatientProfile_emergencycontactinfo"(
												 pvar_PatientProfileid Varchar(50)
											 )
                                             RETURNS TABLE("PatientProfileid" uuid,"PatientProfile_emergencycontactinfoid" uuid ,personname Varchar
,relationship Varchar
,phonenumber Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 PatientProfile_emergencycontactinfo.PatientProfileid
                                             ,PatientProfile_emergencycontactinfo.PatientProfile_emergencycontactinfoid   
											 ,PatientProfile_emergencycontactinfo.personname
,PatientProfile_emergencycontactinfo.relationship
,PatientProfile_emergencycontactinfo.phonenumber
 
											 
											 FROM PatientProfile_emergencycontactinfo
											 WHERE 
											 CAST(PatientProfile_emergencycontactinfo.PatientProfileid AS VARCHAR)=pvar_PatientProfileid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

