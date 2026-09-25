
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_ClinicalTask"
              (
			  pvar_ClinicalTaskid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"ClinicalTaskid" uuid
,workprofile Varchar
,competency Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_ClinicalTask_taskduration" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36*/
			  		 
              RETURN QUERY
			  SELECT  
				 ClinicalTask.tenantid
,tenant.businessname as _tenantname
,ClinicalTask.ClinicalTaskid
,CAST(_WorkProfile.workprofilename AS VARCHAR) as workprofile
,CAST(__Competency.competencyname AS VARCHAR) as competency

				 ,ClinicalTask.createduser,ClinicalTask.createddate,ClinicalTask.modifieduser,ClinicalTask.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_WorkProfile.workprofilename AS VARCHAR) as "Work Profile"
,CAST(__TaskType.tasktypename AS VARCHAR) as "Task Type"
,CAST(___Task.taskname AS VARCHAR) as "Task Name"
,ClinicalTask_taskduration.durationinminutes as "Duration in Minutes"
,ClinicalTask_taskduration.overbookingcount as "Over Booking Count"

							
						FROM  ClinicalTask_taskduration 
INNER JOIN WorkProfile _WorkProfile ON ClinicalTask_taskduration.workprofile=_WorkProfile.WorkProfileid
INNER JOIN TaskType __TaskType ON ClinicalTask_taskduration.tasktype=__TaskType.TaskTypeid
INNER JOIN Task ___Task ON ClinicalTask_taskduration.taskname=___Task.Taskid

						WHERE ClinicalTask.ClinicalTaskid =ClinicalTask_taskduration.ClinicalTaskid
) J)
						as automaton_ClinicalTask_taskduration

                 
				 
			  FROM  ClinicalTask 
 LEFT OUTER JOIN tenant ON ClinicalTask.tenantid=tenant.tenantid
INNER JOIN WorkProfile _WorkProfile ON ClinicalTask.workprofile=_WorkProfile.WorkProfileid
INNER JOIN Competency __Competency ON ClinicalTask.competency=__Competency.Competencyid

			  WHERE CAST(ClinicalTask.ClinicalTaskid AS Varchar)=pvar_ClinicalTaskid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

