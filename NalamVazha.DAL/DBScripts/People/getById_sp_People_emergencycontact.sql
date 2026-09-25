CREATE OR REPLACE FUNCTION "getById_sp_People_emergencycontact"(
												 pvar_Peopleid Varchar(50)
											 )
                                             RETURNS TABLE("Peopleid" uuid,"People_emergencycontactid" uuid ,personname Varchar
,relationship Varchar
,phonenumber Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 People_emergencycontact.Peopleid
                                             ,People_emergencycontact.People_emergencycontactid   
											 ,People_emergencycontact.personname
,People_emergencycontact.relationship
,People_emergencycontact.phonenumber
 
											 
											 FROM People_emergencycontact
											 WHERE 
											 CAST(People_emergencycontact.Peopleid AS VARCHAR)=pvar_Peopleid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

