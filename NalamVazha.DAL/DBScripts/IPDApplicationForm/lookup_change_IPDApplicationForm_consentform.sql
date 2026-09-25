
								CREATE OR REPLACE FUNCTION  "lookup_change_IPDApplicationForm_consentform"(
								pvar_PatientConsentid Varchar(50)=null
                                )
								RETURNS TABLE("PatientConsentid" Varchar
,consenttype Varchar
,consentlanguage Varchar
,consentfile Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
										
                                RETURN QUERY
								SELECT  
									CAST(PatientConsent.PatientConsentid AS Varchar) as PatientConsentid
,CAST(PatientConsent.consenttype AS Varchar) as consenttype
,CAST(PatientConsent.consentlanguage AS Varchar) as consentlanguage
,CAST(PatientConsent.consentfile AS Varchar) as consentfile

								FROM PatientConsent
							   WHERE (CAST(PatientConsent.PatientConsentid AS VARCHAR) = pvar_PatientConsentid)
;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

