CREATE OR REPLACE FUNCTION "getById_sp_TaskTemplate_escalationdetails"(
												 pvar_TaskTemplateid Varchar(50)
											 )
                                             RETURNS TABLE("TaskTemplateid" uuid,"TaskTemplate_escalationdetailsid" uuid ,priority Varchar
,notifyto uuid
,emailid Varchar
,mobilenumber Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 TaskTemplate_escalationdetails.TaskTemplateid
                                             ,TaskTemplate_escalationdetails.TaskTemplate_escalationdetailsid   
											 ,TaskTemplate_escalationdetails.priority
,TaskTemplate_escalationdetails.notifyto
,TaskTemplate_escalationdetails.emailid
,TaskTemplate_escalationdetails.mobilenumber
 
											 
											 FROM TaskTemplate_escalationdetails
											 WHERE 
											 CAST(TaskTemplate_escalationdetails.TaskTemplateid AS VARCHAR)=pvar_TaskTemplateid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

