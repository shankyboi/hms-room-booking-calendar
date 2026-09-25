CREATE OR REPLACE FUNCTION "getById_sp_TaskTemplate_nextactiondetails"(
												 pvar_TaskTemplateid Varchar(50)
											 )
                                             RETURNS TABLE("TaskTemplateid" uuid,"TaskTemplate_nextactiondetailsid" uuid ,actiontype uuid
,actionname uuid
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TaskTemplate_nextactiondetails.TaskTemplateid
                                             ,TaskTemplate_nextactiondetails.TaskTemplate_nextactiondetailsid   
											 ,TaskTemplate_nextactiondetails.actiontype
,TaskTemplate_nextactiondetails.actionname
 
											 
											 FROM TaskTemplate_nextactiondetails
											 WHERE 
											 CAST(TaskTemplate_nextactiondetails.TaskTemplateid AS VARCHAR)=pvar_TaskTemplateid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

