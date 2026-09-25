 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TaskTemplate"
			  (
				  pvar_TaskTemplateid Varchar
			  )
			  RETURNS TABLE(
                tasktype uuid
,taskname Varchar
,priority Varchar
,duration int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TaskTemplateid uuid
                ,escalationdetails JSON,nextactiondetails JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
               
              RETURN QUERY
			  SELECT 
				 TaskTemplate.tasktype
,TaskTemplate.taskname
,TaskTemplate.priority
,TaskTemplate.duration

				 ,TaskTemplate.createduser,TaskTemplate.createddate,TaskTemplate.modifieduser,TaskTemplate.modifieddate
				 ,TaskTemplate.tenantid
                 ,TaskTemplate.TaskTemplateid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 TaskTemplate_escalationdetails.TaskTemplateid
                                             ,TaskTemplate_escalationdetails.TaskTemplate_escalationdetailsid   
											 ,TaskTemplate_escalationdetails.priority
,TaskTemplate_escalationdetails.notifyto
,TaskTemplate_escalationdetails.emailid
,TaskTemplate_escalationdetails.mobilenumber
 
											  
											 FROM TaskTemplate_escalationdetails
											 WHERE 
											 TaskTemplate_escalationdetails.TaskTemplateid=TaskTemplate.TaskTemplateid
                                             
                                             ORDER BY record_order DESC
											) J) as escalationdetails
,(SELECT json_agg(J) FROM (
											 SELECT 
											 TaskTemplate_nextactiondetails.TaskTemplateid
                                             ,TaskTemplate_nextactiondetails.TaskTemplate_nextactiondetailsid   
											 ,TaskTemplate_nextactiondetails.actiontype
,TaskTemplate_nextactiondetails.actionname
 
											  
											 FROM TaskTemplate_nextactiondetails
											 WHERE 
											 TaskTemplate_nextactiondetails.TaskTemplateid=TaskTemplate.TaskTemplateid
                                             
                                             ORDER BY record_order DESC
											) J) as nextactiondetails
   
			  FROM TaskTemplate
			  WHERE CAST(TaskTemplate.TaskTemplateid AS Varchar)=pvar_TaskTemplateid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

