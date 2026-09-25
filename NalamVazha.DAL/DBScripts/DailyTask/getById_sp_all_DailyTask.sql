
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_DailyTask"
              (
			  pvar_DailyTaskid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"DailyTaskid" uuid
,taskno Varchar
,dateandtime Varchar
,tasktype Varchar
,taskname Varchar
,patientname Varchar
,patientcategory Varchar
,ipdreferencenumber Varchar
,opdreferencenumber Varchar
,status Varchar
,amount int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40*/
			  		 
              RETURN QUERY
			  SELECT  
				 DailyTask.tenantid
,tenant.businessname as _tenantname
,DailyTask.DailyTaskid
,DailyTask.taskno
,CAST(COALESCE(to_char(DailyTask.dateandtime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as dateandtime
,CAST(_TypeofTask.tasktype AS VARCHAR) as tasktype
,CAST(__TaskTemplate.taskname AS VARCHAR) as taskname
,CAST(___PatientProfile.firstname||' '||___PatientProfile.lastname AS VARCHAR) as patientname
,DailyTask.patientcategory
,CAST(____IPDApplicationForm.bookingreferencenumber AS VARCHAR) as ipdreferencenumber
,CAST(_____OPDForm.bookingreferencenumber AS VARCHAR) as opdreferencenumber
,DailyTask.status
,DailyTask.amount

				 ,DailyTask.createduser,DailyTask.createddate,DailyTask.modifieduser,DailyTask.modifieddate
                 
                 
				 
			  FROM  DailyTask 
 LEFT OUTER JOIN tenant ON DailyTask.tenantid=tenant.tenantid
INNER JOIN TypeofTask _TypeofTask ON DailyTask.tasktype=_TypeofTask.TypeofTaskid
LEFT OUTER JOIN TaskTemplate __TaskTemplate ON DailyTask.taskname=__TaskTemplate.TaskTemplateid
LEFT OUTER JOIN PatientProfile ___PatientProfile ON DailyTask.patientname=___PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm ____IPDApplicationForm ON DailyTask.ipdreferencenumber=____IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm _____OPDForm ON DailyTask.opdreferencenumber=_____OPDForm.OPDFormid

			  WHERE CAST(DailyTask.DailyTaskid AS Varchar)=pvar_DailyTaskid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

