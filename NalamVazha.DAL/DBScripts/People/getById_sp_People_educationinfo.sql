CREATE OR REPLACE FUNCTION "getById_sp_People_educationinfo"(
												 pvar_Peopleid Varchar(50)
											 )
                                             RETURNS TABLE("Peopleid" uuid,"People_educationinfoid" uuid ,fieldofstudy Varchar
,degree Varchar
,educationinstitution Varchar
,certificationnumber Varchar
,yearofgraduation int
,degreestatus Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 People_educationinfo.Peopleid
                                             ,People_educationinfo.People_educationinfoid   
											 ,People_educationinfo.fieldofstudy
,People_educationinfo.degree
,People_educationinfo.educationinstitution
,People_educationinfo.certificationnumber
,People_educationinfo.yearofgraduation
,People_educationinfo.degreestatus
 
											 
											 FROM People_educationinfo
											 WHERE 
											 CAST(People_educationinfo.Peopleid AS VARCHAR)=pvar_Peopleid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

