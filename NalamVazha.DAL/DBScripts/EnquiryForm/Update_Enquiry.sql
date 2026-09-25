
			  CREATE OR REPLACE FUNCTION  "Update_Enquiry"
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

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
			  IF "Check_Authorization"(pvar_modifieduser, 'EnquiryForm', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from EnquiryForm where upper(EnquiryForm.enquirynumber) = upper(pvar_enquirynumber) and EnquiryForm.tenantid=pvar_tenantid  and EnquiryForm.EnquiryFormid <> pvar_EnquiryFormid)
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
 
			  IF(pvar_returnMessage='')
			  THEN
                IF EXISTS (
                SELECT 1
                FROM EnquiryForm
                WHERE EnquiryFormid = pvar_EnquiryFormid
                  AND lower(coalesce(verifiedstatus,'')) IN ('approved','rejected')
            ) THEN
                pvar_returnMessage := 'Cannot update: Already Approved/Rejected';
                RETURN;
            END IF;
                    INSERT INTO history
VALUES('EnquiryForm', NOW(),
(SELECT query_to_xml('SELECT * FROM EnquiryForm WHERE EnquiryForm.EnquiryFormid= '''||pvar_EnquiryFormid||'''', true, false, '')));

                    pvar_verifiedstatus:='Ready For Review';
                    UPDATE EnquiryForm SET
                    enquirynumber=pvar_enquirynumber
,enquirydate=pvar_enquirydate
,enquirytype=pvar_enquirytype
,isroombookingrelated=pvar_isroombookingrelated
,patientname=pvar_patientname
,firstname=pvar_firstname
,lastname=pvar_lastname
,gender=pvar_gender
,age=pvar_age
,phonenumber=pvar_phonenumber
,emailaddress=pvar_emailaddress
,preferredcontactmethod=pvar_preferredcontactmethod
,enquiryreason=pvar_enquiryreason
,enquiredvia=pvar_enquiredvia
,preferredroomtype=pvar_preferredroomtype
,preferreddateofarrival=pvar_preferreddateofarrival
,preferreddateofdeparture=pvar_preferreddateofdeparture
,joinwaitinglist=pvar_joinwaitinglist
,enquirystatus=pvar_enquirystatus
,verifiedstatus=pvar_verifiedstatus

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE EnquiryFormid=pvar_EnquiryFormid;

                    

                    INSERT INTO history
VALUES('EnquiryForm_medicalinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM EnquiryForm_medicalinfo WHERE EnquiryForm_medicalinfo.EnquiryFormid= '''||pvar_EnquiryFormid||'''', true, false, '')));

								DELETE FROM  EnquiryForm_medicalinfo WHERE EnquiryFormid=pvar_EnquiryFormid;
								
								
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
																,'Update_Enquiry'
																,'Authorization Failed Update_Enquiry'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';
																
																END IF;

			  			 /* EXCEPTION WHEN OTHERS THEN
			 
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
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Update_Enquiry'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Enquiry - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

