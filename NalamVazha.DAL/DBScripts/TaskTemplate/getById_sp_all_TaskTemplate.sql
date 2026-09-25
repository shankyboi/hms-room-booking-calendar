
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TaskTemplate"
              (
			  pvar_TaskTemplateid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TaskTemplateid" uuid
,tasktype Varchar
,taskname Varchar
,priority Varchar
,duration int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_TaskTemplate_escalationdetails" json,"automaton_TaskTemplate_nextactiondetails" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:27:26*/
			  		 
              RETURN QUERY
			  SELECT  
				 TaskTemplate.tenantid
,tenant.businessname as _tenantname
,TaskTemplate.TaskTemplateid
,CAST(_TypeofTask.tasktype AS VARCHAR) as tasktype
,TaskTemplate.taskname
,TaskTemplate.priority
,TaskTemplate.duration

				 ,TaskTemplate.createduser,TaskTemplate.createddate,TaskTemplate.modifieduser,TaskTemplate.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						TaskTemplate_escalationdetails.priority as "Priority"
,CAST(_users.firstname||' '||_users.lastname AS VARCHAR) as "Notify To "
,TaskTemplate_escalationdetails.emailid as "Email ID"
,TaskTemplate_escalationdetails.mobilenumber as "Mobile Number"

							
						FROM  TaskTemplate_escalationdetails 
LEFT OUTER JOIN users _users ON TaskTemplate_escalationdetails.notifyto=_users.usersid

						WHERE TaskTemplate.TaskTemplateid =TaskTemplate_escalationdetails.TaskTemplateid
) J)
						as automaton_TaskTemplate_escalationdetails
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_ActionType.actiontype AS VARCHAR) as "Action Type"
,CAST(__Actions.actionname AS VARCHAR) as "Action Name"

							
						FROM  TaskTemplate_nextactiondetails 
LEFT OUTER JOIN ActionType _ActionType ON TaskTemplate_nextactiondetails.actiontype=_ActionType.ActionTypeid
LEFT OUTER JOIN Actions __Actions ON TaskTemplate_nextactiondetails.actionname=__Actions.Actionsid

						WHERE TaskTemplate.TaskTemplateid =TaskTemplate_nextactiondetails.TaskTemplateid
) J)
						as automaton_TaskTemplate_nextactiondetails

                 
				 
			  FROM  TaskTemplate 
 LEFT OUTER JOIN tenant ON TaskTemplate.tenantid=tenant.tenantid
LEFT OUTER JOIN TypeofTask _TypeofTask ON TaskTemplate.tasktype=_TypeofTask.TypeofTaskid

			  WHERE CAST(TaskTemplate.TaskTemplateid AS Varchar)=pvar_TaskTemplateid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

