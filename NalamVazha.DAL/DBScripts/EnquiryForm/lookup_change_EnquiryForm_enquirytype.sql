
								CREATE OR REPLACE FUNCTION  "lookup_change_EnquiryForm_enquirytype"(
								pvar_EnquiryTypeid Varchar(50)=null
                                )
								RETURNS TABLE("EnquiryTypeid" Varchar
,enquiryname Varchar
,isroombookingrelated Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
										
                                RETURN QUERY
								SELECT  
									CAST(EnquiryType.EnquiryTypeid AS Varchar) as EnquiryTypeid
,CAST(EnquiryType.enquiryname AS Varchar) as enquiryname
,CAST(EnquiryType.isroombookingrelated AS Varchar) as isroombookingrelated

								FROM EnquiryType
							   WHERE (CAST(EnquiryType.EnquiryTypeid AS VARCHAR) = pvar_EnquiryTypeid)
;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

