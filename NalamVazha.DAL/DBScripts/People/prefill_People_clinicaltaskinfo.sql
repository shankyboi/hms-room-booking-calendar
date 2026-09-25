
								CREATE OR REPLACE FUNCTION  "prefill_People_clinicaltaskinfo"
                                (								
                                pvar_clinicaltask Varchar(50)=null

                                )
								RETURNS TABLE(workprofile Varchar
,tasktype Varchar
,taskname Varchar
,durationinminutes Varchar
,overbookingcount Varchar
,consultations Varchar
) 
						 		AS $BODY$
                                    
								BEGIN
                                /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
										


                                RETURN QUERY
								SELECT  
									CAST(clinicaltask_taskduration.workprofile AS Varchar) as workprofile,CAST(clinicaltask_taskduration.tasktype AS Varchar) as tasktype,CAST(clinicaltask_taskduration.taskname AS Varchar) as taskname,CAST(clinicaltask_taskduration.durationinminutes AS Varchar) as durationinminutes,CAST(clinicaltask_taskduration.overbookingcount AS Varchar) as overbookingcount,CAST(clinicaltask_taskduration.clinicaltask_taskdurationid AS Varchar) as consultationsid
								FROM clinicaltask_taskduration INNER JOIN ClinicalTask ON clinicaltask_taskduration.clinicaltaskid=ClinicalTask.clinicaltaskid
								 WHERE (CAST(ClinicalTask.ClinicalTaskid AS Varchar) like pvar_clinicaltask)
;
								
											
								END
                                $BODY$
                                LANGUAGE plpgsql;

