CREATE OR REPLACE FUNCTION "getById_sp_People_clinicaltaskinfo"(
												 pvar_Peopleid Varchar(50)
											 )
                                             RETURNS TABLE("Peopleid" uuid,"People_clinicaltaskinfoid" uuid ,consultations Varchar
,workprofile uuid
,tasktype uuid
,taskname uuid
,durationinminutes int
,overbookingcount int
,availableon Varchar
,workhourstarts Varchar
,workhourends Varchar
,priority Varchar
,feesamount decimal
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 People_clinicaltaskinfo.Peopleid
                                             ,People_clinicaltaskinfo.People_clinicaltaskinfoid   
											 ,People_clinicaltaskinfo.consultations
,People_clinicaltaskinfo.workprofile
,People_clinicaltaskinfo.tasktype
,People_clinicaltaskinfo.taskname
,People_clinicaltaskinfo.durationinminutes
,People_clinicaltaskinfo.overbookingcount
,People_clinicaltaskinfo.availableon
,People_clinicaltaskinfo.workhourstarts
,People_clinicaltaskinfo.workhourends
,People_clinicaltaskinfo.priority
,People_clinicaltaskinfo.feesamount
 
											 
											 FROM People_clinicaltaskinfo
											 WHERE 
											 CAST(People_clinicaltaskinfo.Peopleid AS VARCHAR)=pvar_Peopleid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

