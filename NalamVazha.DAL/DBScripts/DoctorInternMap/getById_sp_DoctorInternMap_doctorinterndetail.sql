CREATE OR REPLACE FUNCTION "getById_sp_DoctorInternMap_doctorinterndetail"(
												 pvar_DoctorInternMapid Varchar(50)
											 )
                                             RETURNS TABLE("DoctorInternMapid" uuid,"DoctorInternMap_doctorinterndetailid" uuid ,interndoctor uuid
,isactive Boolean
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 DoctorInternMap_doctorinterndetail.DoctorInternMapid
                                             ,DoctorInternMap_doctorinterndetail.DoctorInternMap_doctorinterndetailid   
											 ,DoctorInternMap_doctorinterndetail.interndoctor
,DoctorInternMap_doctorinterndetail.isactive
 
											 
											 FROM DoctorInternMap_doctorinterndetail
											 WHERE 
											 CAST(DoctorInternMap_doctorinterndetail.DoctorInternMapid AS VARCHAR)=pvar_DoctorInternMapid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

