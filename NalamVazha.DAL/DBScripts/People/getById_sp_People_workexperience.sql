CREATE OR REPLACE FUNCTION "getById_sp_People_workexperience"(
												 pvar_Peopleid Varchar(50)
											 )
                                             RETURNS TABLE("Peopleid" uuid,"People_workexperienceid" uuid ,designation Varchar
,institutionname Varchar
,fromdate date
,todate date
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 People_workexperience.Peopleid
                                             ,People_workexperience.People_workexperienceid   
											 ,People_workexperience.designation
,People_workexperience.institutionname
,People_workexperience.fromdate
,People_workexperience.todate
 
											 
											 FROM People_workexperience
											 WHERE 
											 CAST(People_workexperience.Peopleid AS VARCHAR)=pvar_Peopleid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

