CREATE OR REPLACE FUNCTION public."Add_Assessment_Template"(
	pvar_assessmenttemplateid uuid,
	pvar_tenantid uuid,
	pvar_templatename character varying,
	pvar_taskname character varying,
	pvar_isdefaulttemplate boolean,
	pvar_templatequestions json,
	pvar_templateapplicability json,
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:35:09*/
		
			  
                                                                                    if pvar_AssessmentTemplateid is null then
                                                                                    pvar_AssessmentTemplateid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'AssessmentTemplate', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from AssessmentTemplate where upper(AssessmentTemplate.templatename::varchar) = upper(pvar_templatename::varchar) and AssessmentTemplate.tenantid=pvar_tenantid and COALESCE(AssessmentTemplate.isdeleted, false) = false)
																THEN
																pvar_returnMessage := pvar_returnMessage||'Template Name Already Exists.';
																END IF;

			  IF pvar_isdefaulttemplate = true AND pvar_returnMessage = '' THEN
				IF EXISTS (SELECT 1 FROM AssessmentTemplate WHERE upper(taskname::varchar) = upper(pvar_taskname::varchar) AND isdefaulttemplate = true AND tenantid = pvar_tenantid) THEN
					pvar_returnMessage := 'DefaultTemplateExists';
				END IF;
			  END IF;
                
			  if(pvar_returnMessage='')
			  THEN
			  
			INSERT INTO AssessmentTemplate(
			templatename
			,taskname
			,isdefaulttemplate
			,createduser
			,AssessmentTemplateid
			,tenantid
			
			)
			VALUES (
			pvar_templatename
			,pvar_taskname
			,pvar_isdefaulttemplate
			,pvar_createduser
			,pvar_AssessmentTemplateid
			,pvar_tenantid
			
			);
			   
               
			  
			  
								
								
								INSERT INTO AssessmentTemplate_templatequestions (
									AssessmentTemplateid
									,AssessmentTemplate_templatequestionsid 
                                    ,record_order  
									,questioncategory
,questionsub
,question
,optiontext
,optionvalue
,defaultvalue
,answertype
,scalerangemin
,scalerangemax
,isrequired
,scorevalue
									
									)
									SELECT 
									pvar_AssessmentTemplateid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'questioncategory' AS uuid) as questioncategory
,CAST(j->>'questionsub' AS uuid) as questionsub
,CAST(j->>'question' AS uuid) as question
,j->>'optiontext' as optiontext
,j->>'optionvalue' as optionvalue
,j->>'defaultvalue' as defaultvalue
,j->>'answertype' as answertype
,CAST(j->>'scalerangemin' AS int) as scalerangemin
,CAST(j->>'scalerangemax' AS int) as scalerangemax
,CAST(j->>'isrequired' AS Boolean) as isrequired
,CAST(j->>'scorevalue' AS int) as scorevalue
									
                                    FROM json_array_elements(pvar_templatequestions) as j;
									
								
								
								INSERT INTO AssessmentTemplate_templateapplicability (
									AssessmentTemplateid
									,AssessmentTemplate_templateapplicabilityid 
                                    ,record_order  
									,patientcategory
,gender
,relationshipstatus
,medicalcondition
,agegroup
									
									)
									SELECT 
									pvar_AssessmentTemplateid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'patientcategory' AS uuid) as patientcategory
,j->>'gender' as gender
,j->>'relationshipstatus' as relationshipstatus
,CAST(j->>'medicalcondition' AS  uuid) as medicalcondition
,j->>'agegroup' as agegroup
									
                                    FROM json_array_elements(pvar_templateapplicability) as j;
									
					 
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
																,'Add_Assessment_Template'
																,'Authorization Failed Add_Assessment_Template'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  	
			  END
              
$BODY$;
ALTER FUNCTION public."Add_Assessment_Template"(uuid, uuid, character varying, character varying, boolean, json, json, uuid)
    OWNER TO md_nalamvazha;
GRANT EXECUTE ON FUNCTION public."Add_Assessment_Template"(uuid, uuid, character varying, character varying, boolean, json, json, uuid) TO PUBLIC;
GRANT EXECUTE ON FUNCTION public."Add_Assessment_Template"(uuid, uuid, character varying, character varying, boolean, json, json, uuid) TO md_nalamvazha;
