CREATE OR REPLACE FUNCTION "getById_sp_People_preferredlanguageinfo"(
												 pvar_Peopleid Varchar(50)
											 )
                                             RETURNS TABLE("Peopleid" uuid,"People_preferredlanguageinfoid" uuid ,languagesknown Varchar
,proficiency Varchar
,ability Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 People_preferredlanguageinfo.Peopleid
                                             ,People_preferredlanguageinfo.People_preferredlanguageinfoid   
											 ,People_preferredlanguageinfo.languagesknown
,People_preferredlanguageinfo.proficiency
,People_preferredlanguageinfo.ability
 
											 
											 FROM People_preferredlanguageinfo
											 WHERE 
											 CAST(People_preferredlanguageinfo.Peopleid AS VARCHAR)=pvar_Peopleid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

