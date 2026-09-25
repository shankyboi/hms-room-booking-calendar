 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_ClinicalTask"
			  (
				  pvar_ClinicalTaskid Varchar
			  )
			  RETURNS TABLE(
                workprofile uuid
,competency uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,ClinicalTaskid uuid
                ,taskduration JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:36*/
               
              RETURN QUERY
			  SELECT 
				 ClinicalTask.workprofile
,ClinicalTask.competency

				 ,ClinicalTask.createduser,ClinicalTask.createddate,ClinicalTask.modifieduser,ClinicalTask.modifieddate
				 ,ClinicalTask.tenantid
                 ,ClinicalTask.ClinicalTaskid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 ClinicalTask_taskduration.ClinicalTaskid
                                             ,ClinicalTask_taskduration.ClinicalTask_taskdurationid   
											 ,ClinicalTask_taskduration.workprofile
,ClinicalTask_taskduration.tasktype
,ClinicalTask_taskduration.taskname
,ClinicalTask_taskduration.durationinminutes
,ClinicalTask_taskduration.overbookingcount
 
											  
											 FROM ClinicalTask_taskduration
											 WHERE 
											 ClinicalTask_taskduration.ClinicalTaskid=ClinicalTask.ClinicalTaskid
                                             
                                             ORDER BY record_order DESC
											) J) as taskduration
   
			  FROM ClinicalTask
			  WHERE CAST(ClinicalTask.ClinicalTaskid AS Varchar)=pvar_ClinicalTaskid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

