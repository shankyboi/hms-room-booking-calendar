CREATE OR REPLACE FUNCTION "getById_sp_ClinicalTask_taskduration"(
												 pvar_ClinicalTaskid Varchar(50)
											 )
                                             RETURNS TABLE("ClinicalTaskid" uuid,"ClinicalTask_taskdurationid" uuid ,workprofile uuid
,tasktype uuid
,taskname uuid
,durationinminutes int
,overbookingcount int
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
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
											 CAST(ClinicalTask_taskduration.ClinicalTaskid AS VARCHAR)=pvar_ClinicalTaskid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

