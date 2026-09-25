CREATE OR REPLACE FUNCTION public."Update_Assessment_Template"(
	pvar_assessmenttemplateid uuid,
	pvar_tenantid uuid,
	pvar_templatename character varying,
	pvar_taskname character varying,
	pvar_isdefaulttemplate boolean,
	pvar_templatequestions json,
	pvar_templateapplicability json,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/23/2026 13:35:09*/
			  IF "Check_Authorization"(pvar_modifieduser, 'AssessmentTemplate', 'edit') THEN
			  pvar_returnMessage:='';
			  if EXISTS (SELECT * from AssessmentTemplate where upper(AssessmentTemplate.templatename) = upper(pvar_templatename) and AssessmentTemplate.tenantid=pvar_tenantid and COALESCE(AssessmentTemplate.isdeleted, false) = false and AssessmentTemplate.AssessmentTemplateid <> pvar_AssessmentTemplateid)
																THEN
																  pvar_returnMessage := pvar_returnMessage||'Template Name Already Exists.';
																END IF;

			  IF pvar_isdefaulttemplate = true AND pvar_returnMessage = '' THEN
				IF EXISTS (SELECT 1 FROM AssessmentTemplate WHERE upper(taskname::varchar) = upper(pvar_taskname::varchar) AND isdefaulttemplate = true AND tenantid = pvar_tenantid AND AssessmentTemplateid <> pvar_AssessmentTemplateid) THEN
					pvar_returnMessage := 'DefaultTemplateExists';
				END IF;
			  END IF;
                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('AssessmentTemplate', NOW(),
(SELECT query_to_xml('SELECT * FROM AssessmentTemplate WHERE AssessmentTemplate.AssessmentTemplateid= '''||pvar_AssessmentTemplateid||'''', true, false, '')));
                    
                    UPDATE AssessmentTemplate SET
                    templatename=pvar_templatename
			,taskname=pvar_taskname
			,isdefaulttemplate=pvar_isdefaulttemplate
                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE AssessmentTemplateid=pvar_AssessmentTemplateid;
                    
                    INSERT INTO history
VALUES('AssessmentTemplate_templatequestions', NOW(),
(SELECT query_to_xml('SELECT * FROM AssessmentTemplate_templatequestions WHERE AssessmentTemplate_templatequestions.AssessmentTemplateid= '''||pvar_AssessmentTemplateid||'''', true, false, '')));
								DELETE FROM  AssessmentTemplate_templatequestions WHERE AssessmentTemplateid=pvar_AssessmentTemplateid;
								
								
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
									
INSERT INTO history
VALUES('AssessmentTemplate_templateapplicability', NOW(),
(SELECT query_to_xml('SELECT * FROM AssessmentTemplate_templateapplicability WHERE AssessmentTemplate_templateapplicability.AssessmentTemplateid= '''||pvar_AssessmentTemplateid||'''', true, false, '')));
								DELETE FROM  AssessmentTemplate_templateapplicability WHERE AssessmentTemplateid=pvar_AssessmentTemplateid;
								
								
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
																,'Update_Assessment_Template'
																,'Authorization Failed Update_Assessment_Template'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';
																
																END IF;
			  	
			  END
              
$BODY$;
ALTER FUNCTION public."Update_Assessment_Template"(uuid, uuid, character varying, character varying, boolean, json, json, uuid)
    OWNER TO md_nalamvazha;
GRANT EXECUTE ON FUNCTION public."Update_Assessment_Template"(uuid, uuid, character varying, character varying, boolean, json, json, uuid) TO PUBLIC;
GRANT EXECUTE ON FUNCTION public."Update_Assessment_Template"(uuid, uuid, character varying, character varying, boolean, json, json, uuid) TO md_nalamvazha;
