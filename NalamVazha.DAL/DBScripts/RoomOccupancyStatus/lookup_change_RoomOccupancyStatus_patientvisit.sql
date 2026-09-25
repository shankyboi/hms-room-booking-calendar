
								CREATE OR REPLACE FUNCTION  "lookup_change_RoomOccupancyStatus_patientvisit"(
								pvar_PatientVisitid Varchar(50)=null
                                )
								RETURNS TABLE("PatientVisitid" Varchar
,visitnumber Varchar
,patientname Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:03*/
										
                                RETURN QUERY
								SELECT  
									CAST(PatientVisit.PatientVisitid AS Varchar) as PatientVisitid
,CAST(PatientVisit.visitnumber AS Varchar) as visitnumber
,CAST(PatientVisit.patientname AS Varchar) as patientname

								FROM PatientVisit
							   WHERE (CAST(PatientVisit.PatientVisitid AS VARCHAR) = pvar_PatientVisitid)
;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

