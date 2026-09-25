
			  CREATE OR REPLACE FUNCTION  "Add_Patient_Visit"
			  (
				  pvar_PatientVisitid uuid
,pvar_tenantid uuid
,
pvar_visitnumber Varchar(256)
,
pvar_visitdatetime Timestamp(3)
,
pvar_patientname  uuid
,
pvar_visittype   Varchar(1024)
,
pvar_ipdnumber  uuid
,
pvar_opdnumber  uuid
,
pvar_consultingdoctor  uuid
,
pvar_visitstatus  Varchar(1024)
,
pvar_notes Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_visitnumber Varchar(10);lvar_val_visitnumber int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:59*/
		

			  
                                                                                    if pvar_PatientVisitid is null then
                                                                                    pvar_PatientVisitid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                        || RIGHT('00' ||cast(to_char(NOW(),'dd') as Varchar(2)),2) INTO lvar_curday_visitnumber;
                                        select COALESCE(max(RIGHT(PatientVisit.visitnumber,5)),'0') INTO lvar_val_visitnumber from
                                        PatientVisit where substring(PatientVisit.visitnumber,1,8) = lvar_curday_visitnumber and (PatientVisit.visitnumber) NOT LIKE '%/%';
                                        lvar_val_visitnumber:=lvar_val_visitnumber + 1;
                                        pvar_visitnumber:= (lvar_curday_visitnumber||'-'|| cast(to_char(lvar_val_visitnumber,'fm00000') as Varchar(5)));

			  IF "Check_Authorization"(pvar_createduser, 'PatientVisit', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from PatientVisit where upper(PatientVisit.visitnumber::varchar) = upper(pvar_visitnumber::varchar) and PatientVisit.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Visit Number Already Exists.';

																END IF;

              IF(pvar_visitstatus is not null AND pvar_visitstatus!='0' AND LENGTH(pvar_visitstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_visitstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='visitstatus'
                                                                and entityname='PatientVisit' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_visitstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'visitstatus value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_visittype is not null AND pvar_visittype!='0' AND LENGTH(pvar_visittype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_visittype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='visittype'
                                                                and entityname='PatientVisit' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_visittype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'visittype value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO PatientVisit(
				 visitnumber
,visitdatetime
,patientname
,visittype
,ipdnumber
,opdnumber
,consultingdoctor
,visitstatus
,notes

				 ,createduser
				 ,PatientVisitid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_visitnumber
,pvar_visitdatetime
,pvar_patientname
,pvar_visittype
,pvar_ipdnumber
,pvar_opdnumber
,pvar_consultingdoctor
,pvar_visitstatus
,pvar_notes

				 ,pvar_createduser
				 ,pvar_PatientVisitid
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
																,'Add_Patient_Visit'
																,'Authorization Failed Add_Patient_Visit'
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
						,'Add_Patient_Visit'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Patient_Visit - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

