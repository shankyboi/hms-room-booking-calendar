
			  CREATE OR REPLACE FUNCTION  "Add_Daily_Task"
			  (
				  pvar_DailyTaskid uuid
,pvar_tenantid uuid
,
pvar_taskno Varchar(256)
,
pvar_tasktype  uuid
,
pvar_taskname  uuid
,
pvar_patientname  uuid
,
pvar_ipdreferencenumber  uuid
,
pvar_opdreferencenumber  uuid
,
pvar_status Varchar(128)
,
pvar_amount int
,pvar_dateandtime Timestamp(3)
,pvar_activityname Varchar(256)
,pvar_description Text
,pvar_priority Varchar(32)
,pvar_assignedto uuid

				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_taskno Varchar(10);lvar_val_taskno int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/03/2026 08:39:40*/
		

			  
                                                                                    if pvar_DailyTaskid is null then
                                                                                    pvar_DailyTaskid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) ||'-'|| RIGHT('000' ||cast(DATE_PART('doy',NOW()) as Varchar(3)),3) 
                                          INTO lvar_curday_taskno;
                                        select COALESCE(max(RIGHT(DailyTask.taskno,5)),'0') INTO lvar_val_taskno from
                                        DailyTask where substring(DailyTask.taskno,1,8) = lvar_curday_taskno and (DailyTask.taskno) NOT LIKE '%/%';
                                        lvar_val_taskno:=lvar_val_taskno + 1;
                                        pvar_taskno:= (lvar_curday_taskno||'-'|| cast(to_char(lvar_val_taskno,'fm00000') as Varchar(5)));

			  IF "Check_Authorization"(pvar_createduser, 'DailyTask', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from DailyTask where upper(DailyTask.taskno::varchar) = upper(pvar_taskno::varchar) and DailyTask.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Task No  Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO DailyTask(
				 taskno
,tasktype
,taskname
,patientname
,ipdreferencenumber
,opdreferencenumber
,status
,amount
,dateandtime
,activityname
,description
,priority
,assignedto

				 ,createduser
				 ,DailyTaskid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_taskno
,pvar_tasktype
,pvar_taskname
,pvar_patientname
,pvar_ipdreferencenumber
,pvar_opdreferencenumber
,pvar_status
,pvar_amount
,COALESCE(pvar_dateandtime, NOW())
,NULLIF(btrim(pvar_activityname), '')
,NULLIF(btrim(pvar_description), '')
,NULLIF(btrim(pvar_priority), '')
,COALESCE(pvar_assignedto, pvar_createduser)

				 ,pvar_createduser
				 ,pvar_DailyTaskid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
			  pvar_returnMessage :='201.1';
               
              END IF;
			   

			  
																ELSE
																

															
																INSERT INTO system_logging
																(
																Log_code
																,system_logging_guid
																,log_application
																,log_date
																,log_level
																,log_logger
																,log_message
																,log_user_name
																)
																VALUES
																('401.1'
																,gen_random_uuid()
																,'Store Proc Authorization Check'
																,NOW()
																,'Critical'
																,'Add_Daily_Task'
																,'Authorization Failed Add_Daily_Task'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
						INSERT INTO system_logging
						(
						Log_code
						,system_logging_guid
						,log_application
						,log_date
						,log_level
						,log_logger
						,log_message
						)
						VALUES
						('16'
						,gen_random_uuid()
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Daily_Task'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Daily_Task - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

