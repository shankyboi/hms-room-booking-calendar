
CREATE OR REPLACE FUNCTION public."Update_Assessment"(
	pvar_assessmentid uuid,
	pvar_tenantid uuid,
	pvar_patientname uuid,
	pvar_patientvisit uuid,
	pvar_assessedby character varying,
	pvar_doctorname uuid,
	pvar_ipdform uuid,
	pvar_opdform uuid,	
	pvar_assessmentdate timestamp without time zone,
	pvar_questionnairetemplate uuid,
	pvar_assessmentnotes character varying,
	pvar_assessmentquestions json,
	pvar_modifieduser uuid,
	pvar_eligibleforfinaladmission character varying,
	pvar_taskname character varying,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
			  DECLARE lv_viewactionroles Varchar(128);
			  DECLARE lv_bookingstatus varchar(100);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 16:41:42*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Assessment', 'edit') THEN
			  SELECT userrole INTO lv_viewactionroles FROM users WHERE usersid = pvar_modifieduser;

			  pvar_returnMessage:='';

			  
               IF(pvar_assessedby is not null AND pvar_assessedby!='0' AND LENGTH(pvar_assessedby)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_assessedby, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='assessedby'
                                                                and entityname='Assessment' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_assessedby, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'assessedby value is invalid';

                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Assessment', NOW(),
(SELECT query_to_xml('SELECT * FROM Assessment WHERE Assessment.Assessmentid= '''||pvar_Assessmentid||'''', true, false, '')));

	SELECT bookingstatus into lv_bookingstatus FROM ipdapplicationform
   WHERE ipdapplicationformid=pvar_ipdform;

		   -- Preserve the task explicitly supplied by the caller. This is required
		   -- for IP Rounds/IP Discharge, which must not advance the IPD booking.
		   -- Retain the legacy inference only for callers that provide no task.
		   IF NULLIF(TRIM(pvar_taskname), '') IS NULL THEN
			   IF(lv_bookingstatus IN ('Admission Approved', 'Admission confirmed', 'Rejected'))
			   THEN
				   -- An assessment/history update must not undo a completed admission
				   -- decision when no workflow task was supplied by the caller.
				   pvar_taskname := 'Preserve Booking Status';
			   ELSIF(lv_bookingstatus='Screening Scheduled'
				  OR lv_bookingstatus='Awaiting IPD Consultation'
				  OR lv_bookingstatus='Consultation Scheduled')
			   THEN
				   pvar_taskname := 'IPD Consultation';
			   ELSE
				   pvar_taskname := 'IPD Screening';
			   END IF;
		   END IF;

						  
                    
                    UPDATE Assessment SET
                   assessmentnotes=pvar_assessmentnotes,assessedby=pvar_assessedby

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Assessmentid=pvar_Assessmentid;

   
                if(LOWER(TRIM(COALESCE(pvar_taskname, ''))) IN ('ipd screening', 'assessment review', 'screening'))
				   THEN
			   		--IF lv_viewactionroles = 'Intern Doctor' THEN
					   IF LOWER(TRIM(COALESCE(lv_viewactionroles, ''))) IN ('doctor', 'intern doctor') THEN
						IF lv_bookingstatus NOT IN ('Admission Approved', 'Admission confirmed', 'Rejected') THEN
							UPDATE ipdapplicationform
							--SET bookingstatus='Screening completed by the patient'
							SET bookingstatus='Assessment Form Reviewed'
							WHERE ipdapplicationformid=pvar_ipdform;
						END IF;

						if(pvar_eligibleforfinaladmission ='screeningapproved'	AND LOWER(TRIM(COALESCE(lv_bookingstatus, ''))) IN (
								'screening scheduled',
								'awaiting doctor screening',
								'screening completed by the patient'
							))
					 THEN
							UPDATE ipdapplicationform
							--SET bookingstatus='Approved for Admission'
							SET bookingstatus='Admission Approved'
							WHERE ipdapplicationformid=pvar_ipdform;
					 END IF;
	if(pvar_eligibleforfinaladmission ='screeningrejected'	AND LOWER(TRIM(COALESCE(lv_bookingstatus, ''))) IN (
			'screening scheduled',
			'awaiting doctor screening',
			'screening completed by the patient'
		))
					 THEN
							UPDATE ipdapplicationform
							--SET bookingstatus='Not Eligible for Admission'
							SET bookingstatus='Rejected'
							WHERE ipdapplicationformid=pvar_ipdform;
					 END IF;

					 
					ELSE
               		UPDATE ipdapplicationform
			   	--	SET bookingstatus='Screening completed by the patient - Intern Doctor Review Pending'
			  			--SET bookingstatus='Screening completed by the patient - Doctor Review Pending'
					 SET bookingstatus='Assessment Form - Review Pending'
					 WHERE ipdapplicationformid=pvar_ipdform;

					if(pvar_eligibleforfinaladmission ='patient-saveasdraft' OR
					  pvar_eligibleforfinaladmission ='doctor-saveasdraft' OR
					  pvar_eligibleforfinaladmission ='fdesk-saveasdraft')
					 THEN
							UPDATE ipdapplicationform
							--SET bookingstatus='Screening completed by the patient - In Draft'
							SET bookingstatus='Assessment Form - In Draft'
							WHERE ipdapplicationformid=pvar_ipdform;
					 END IF;

					 
					
					 
					END IF;

			 END IF;
 

		   if(pvar_taskname ='IPD Consultation')
			THEN

			Update ClinicalAppointment set status='Completed'
WHERE ClinicalAppointmentid IN (

					  SELECT ca.ClinicalAppointmentid   
  FROM ClinicalAppointment ca
  INNER JOIN IPDApplicationForm ipd ON ipd.IPDApplicationFormid = pvar_ipdform
    AND COALESCE(ipd.isdeleted, false) = false
  WHERE COALESCE(ca.isdeleted, false) = false
    AND ca.bookingid IS NOT NULL
    AND LENGTH(TRIM(ca.bookingid)) > 0
    AND (ca.tasktype ILIKE '%IP Screening%'
	OR   ca.tasktype ILIKE '%Consultation%')
    AND (
      TRIM(CAST(ca.bookingid AS VARCHAR)) = TRIM(CAST(ipd.IPDApplicationFormid AS VARCHAR))
      OR TRIM(CAST(ca.bookingid AS VARCHAR)) = TRIM(ipd.bookingreferencenumber)
    ));

	
		           if(lv_viewactionroles ='Doctor')
					  THEN
					   
						    if(pvar_eligibleforfinaladmission ='rejected')
							  THEN
							   
				               UPDATE ipdapplicationform
							  -- set bookingstatus='Rejected in Final Consultation'
							   set bookingstatus='Rejected'
							   where ipdapplicationformid=pvar_ipdform;
							   
				             END IF;
		
							  if(pvar_eligibleforfinaladmission ='approved')
								  THEN
								   
					               UPDATE ipdapplicationform
								  -- set bookingstatus='Eligible for Admission'
								  set bookingstatus='Admission confirmed'
								   where ipdapplicationformid=pvar_ipdform;
								   
					             END IF;
					    
					   	  
				 
					   END IF;
		   END IF;

 
			   

			    
                    

                    INSERT INTO history
VALUES('Assessment_assessmentquestions', NOW(),
(SELECT query_to_xml('SELECT * FROM Assessment_assessmentquestions WHERE Assessment_assessmentquestions.Assessmentid= '''||pvar_Assessmentid||'''', true, false, '')));

								DELETE FROM  Assessment_assessmentquestions WHERE Assessmentid=pvar_Assessmentid;
								
								
								INSERT INTO Assessment_assessmentquestions (
									Assessmentid
									,Assessment_assessmentquestionsid 
                                    ,record_order  
									,questions
,optiontext
,optionvalue
,defaultvalue
,questioncategory
,questionsub
,question
,answertype
,scalerangemin
,scalerangemax
,isrequired
,scorevalue

									
									)
									SELECT 
									pvar_Assessmentid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,coalesce(j->>'questions',gen_random_uuid()::varchar) as questions
,j->>'optiontext' as optiontext
,j->>'optionvalue' as optionvalue
,j->>'defaultvalue' as defaultvalue
,CAST(j->>'questioncategory' AS uuid) as questioncategory
,CAST(j->>'questionsub' AS uuid) as questionsub
,CAST(j->>'question' AS uuid) as question
,j->>'answertype' as answertype
,CAST(j->>'scalerangemin' AS int) as scalerangemin
,CAST(j->>'scalerangemax' AS int) as scalerangemax
,CAST(j->>'isrequired' AS Boolean) as isrequired
,CAST(j->>'scorevalue' AS int) as scorevalue

									
                                    FROM json_array_elements(pvar_assessmentquestions) as j;
									

					
							
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
																,'Update_Assessment'
																,'Authorization Failed Update_Assessment'
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
						,'Update_Assessment'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Assessment - update failed';*/
			  	
			  END
              
$BODY$;


