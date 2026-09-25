
								CREATE OR REPLACE FUNCTION "lookup_change_escalationdetails_TaskTemplate_notifyto"
								(
                                pvar_usersid Varchar(50)=null
								)
                                RETURNS TABLE("usersid" Varchar
,firstname Varchar
,lastname Varchar
,emailid Varchar
,mobilenumber Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
										
                                RETURN QUERY
								SELECT  
									CAST(users.usersid AS Varchar) as usersid
,CAST(users.firstname AS Varchar) as firstname
,CAST(users.lastname AS Varchar) as lastname
,CAST (users.emailid AS VARCHAR) as emailid
,CAST (users.mobilenumber AS VARCHAR) as mobilenumber

								FROM users
							    WHERE (CAST(users.usersid AS VARCHAR) = pvar_usersid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

