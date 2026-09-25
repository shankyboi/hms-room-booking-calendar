
			  CREATE OR REPLACE FUNCTION  "Add_Enquiry"
			  (
				  pvar_EnquiryFormid uuid
,pvar_tenantid uuid
,
pvar_enquirynumber Varchar(256)
,
pvar_enquirydate date
,
pvar_enquirytype  uuid
,
pvar_isroombookingrelated  Varchar(1024)
,
pvar_patientname  uuid
,
pvar_firstname Varchar(128)
,
pvar_lastname Varchar(128)
,
pvar_gender  Varchar(1024)
,
pvar_age Bigint
,
pvar_phonenumber Varchar(10)
,
pvar_emailaddress Varchar(128)
,
pvar_preferredcontactmethod  Varchar(1024)
,
pvar_enquiryreason text
,
pvar_enquiredvia  Varchar(1024)
,
pvar_preferredroomtype  uuid
,
pvar_preferreddateofarrival date
,
pvar_preferreddateofdeparture date
,
pvar_joinwaitinglist Boolean
,
pvar_enquirystatus  Varchar(1024)
,
pvar_verifiedstatus  Varchar(1024)
,pvar_medicalinfo json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_enquirynumber Varchar(10);lvar_val_enquirynumber int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
		

			  
                                                                                    if pvar_EnquiryFormid is null then
                                                                                    pvar_EnquiryFormid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) || RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                        || RIGHT('00' ||cast(to_char(NOW(),'dd') as Varchar(2)),2) INTO lvar_curday_enquirynumber;
                                        select COALESCE(max(RIGHT(EnquiryForm.enquirynumber,4)),'0') INTO lvar_val_enquirynumber from
                                        EnquiryForm where substring(EnquiryForm.enquirynumber,1,8) = lvar_curday_enquirynumber and (EnquiryForm.enquirynumber) NOT LIKE '%/%';
                                        lvar_val_enquirynumber:=lvar_val_enquirynumber + 1;
                                        pvar_enquirynumber:= (lvar_curday_enquirynumber||'-'|| cast(to_char(lvar_val_enquirynumber,'fm0000') as Varchar(4)));

			  IF "Check_Authorization"(pvar_createduser, 'EnquiryForm', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from EnquiryForm where upper(EnquiryForm.enquirynumber::varchar) = upper(pvar_enquirynumber::varchar) and EnquiryForm.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Enquiry Number Already Exists.';

																END IF;

              IF(pvar_enquiredvia is not null AND pvar_enquiredvia!='0' AND LENGTH(pvar_enquiredvia)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_enquiredvia, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='enquiredvia'
                                                                and entityname='EnquiryForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_enquiredvia, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'enquiredvia value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_enquirystatus is not null AND pvar_enquirystatus!='0' AND LENGTH(pvar_enquirystatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_enquirystatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='enquirystatus'
                                                                and entityname='EnquiryForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_enquirystatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'enquirystatus value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_gender is not null AND pvar_gender!='0' AND LENGTH(pvar_gender)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_gender, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='gender'
                                                                and entityname='EnquiryForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_gender, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'gender value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_isroombookingrelated is not null AND pvar_isroombookingrelated!='0' AND LENGTH(pvar_isroombookingrelated)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_isroombookingrelated, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='isroombookingrelated'
                                                                and entityname='EnquiryForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_isroombookingrelated, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'isroombookingrelated value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_preferredcontactmethod is not null AND pvar_preferredcontactmethod!='0' AND LENGTH(pvar_preferredcontactmethod)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_preferredcontactmethod, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='preferredcontactmethod'
                                                                and entityname='EnquiryForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_preferredcontactmethod, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'preferredcontactmethod value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_verifiedstatus is not null AND pvar_verifiedstatus!='0' AND LENGTH(pvar_verifiedstatus)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_verifiedstatus, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='verifiedstatus'
                                                                and entityname='EnquiryForm' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_verifiedstatus, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'verifiedstatus value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  pvar_verifiedstatus:='Ready For Review';

			  INSERT INTO EnquiryForm(
				 enquirynumber
,enquirydate
,enquirytype
,isroombookingrelated
,patientname
,firstname
,lastname
,gender
,age
,phonenumber
,emailaddress
,preferredcontactmethod
,enquiryreason
,enquiredvia
,preferredroomtype
,preferreddateofarrival
,preferreddateofdeparture
,joinwaitinglist
,enquirystatus
,verifiedstatus

				 ,createduser
				 ,EnquiryFormid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_enquirynumber
,pvar_enquirydate
,pvar_enquirytype
,pvar_isroombookingrelated
,pvar_patientname
,pvar_firstname
,pvar_lastname
,pvar_gender
,pvar_age
,pvar_phonenumber
,pvar_emailaddress
,pvar_preferredcontactmethod
,pvar_enquiryreason
,pvar_enquiredvia
,pvar_preferredroomtype
,pvar_preferreddateofarrival
,pvar_preferreddateofdeparture
,pvar_joinwaitinglist
,pvar_enquirystatus
,pvar_verifiedstatus

				 ,pvar_createduser
				 ,pvar_EnquiryFormid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
								INSERT INTO EnquiryForm_medicalinfo (
									EnquiryFormid
									,EnquiryForm_medicalinfoid 
                                    ,record_order  
									,medicalcondition
,conditionname
,duration
,severity

									
									)
									SELECT 
									pvar_EnquiryFormid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'medicalcondition' AS uuid) as medicalcondition
,j->>'conditionname' as conditionname
,j->>'duration' as duration
,j->>'severity' as severity

									
                                    FROM json_array_elements(pvar_medicalinfo) as j;
									

					 
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
																,'Add_Enquiry'
																,'Authorization Failed Add_Enquiry'
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
						,'Add_Enquiry'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Enquiry - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

