
CREATE OR REPLACE FUNCTION public."Create_Healthcare_Provider"(
	pvar_tenantid uuid,
	pvar_businessname character varying,
	pvar_shortcode character varying,
	pvar_natureofbusiness character varying,
	pvar_businessemail character varying,
	pvar_businessphone character varying,
	pvar_businesswebsite character varying,
	pvar_organizationlogo character varying,
	pvar_numberofemployees integer,
	pvar_enablepatientautologin boolean,
	pvar_allowdoctortoadmitpatients boolean,
	pvar_preadmissionnoticedays integer,
	pvar_addressline1 character varying,
	pvar_addressline2 character varying,
	pvar_zip character varying,
	pvar_statename character varying,
	pvar_country uuid,
	pvar_parentid uuid,
	pvar_username character varying,
	pvar_userrole character varying,
	pvar_password character varying,
	pvar_passwordkey character varying,
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
      declare lvar_tasktypeid uuid;
           lvar_processslaid uuid;
              
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:17*/
		

			  
			  
			  
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from tenant where upper(tenant.shortcode::varchar) = upper(pvar_shortcode::varchar))
																THEN

																pvar_returnMessage := pvar_returnMessage||'Short Code Already Exists.';

																END IF;

              IF(pvar_natureofbusiness is not null AND pvar_natureofbusiness!='0' AND LENGTH(pvar_natureofbusiness)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_natureofbusiness, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='natureofbusiness'
                                                                and entityname='tenant' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_natureofbusiness, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'natureofbusiness value is invalid';

                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO tenant(
				 businessname
,shortcode
,natureofbusiness
,businessemail
,businessphone
,businesswebsite
,organizationlogo
,numberofemployees
,enablepatientautologin
,allowdoctortoadmitpatients
,preadmissionnoticedays
,addressline1
,addressline2
,zip
,statename
,country
,parentid
,username
,userrole
,password

				 ,createduser
				 ,tenantid
				 
                
			  )
			  VALUES (
 				 pvar_businessname
,pvar_shortcode
,pvar_natureofbusiness
,pvar_businessemail
,pvar_businessphone
,pvar_businesswebsite
,pvar_organizationlogo
,pvar_numberofemployees
,pvar_enablepatientautologin
,pvar_allowdoctortoadmitpatients
,pvar_preadmissionnoticedays
,pvar_addressline1
,pvar_addressline2
,pvar_zip
,pvar_statename
,pvar_country
,pvar_parentid
,pvar_username
,pvar_userrole
,pvar_password

				 ,pvar_createduser
				 ,pvar_tenantid
				 
                   
			  );
			   
               

			     INSERT INTO users(
										firstname
										,profilepicture
										,username
										,userpassword
										,emailid
										,mobilenumber
										,userrole
										,passwordkey

										,createduser
										,usersid
										,tenantid
                
										)
										VALUES (
										pvar_businessname
									 	,pvar_organizationlogo
										,pvar_username
										,pvar_password
										,pvar_businessemail
										,pvar_businessphone
										,pvar_userrole
										,pvar_passwordkey

										,pvar_tenantid
										,pvar_tenantid
										,pvar_tenantid
                   
										);

 lvar_tasktypeid:=gen_random_uuid();

 

INSERT INTO tasktype(
	tasktypeid, tenantid, tasktypename, createduser)
	VALUES (lvar_tasktypeid, pvar_tenantid,'Consultation',pvar_tenantid);

 INSERT INTO task(
    taskid, tenantid, tasktype, taskname, createduser
)
VALUES 
(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'OP Follow up', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'IP Screening', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'IP New', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'IP Rounds', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'IP Discharge', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'OP New', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'Online OP New', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'Online OP Follow up', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'Default Template', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'Feedbackform', pvar_tenantid),

(gen_random_uuid(), pvar_tenantid, lvar_tasktypeid, 'Dischargechecklist', pvar_tenantid);
 
INSERT INTO enquirytype(
	enquirytypeid, tenantid, enquiryname,isroombookingrelated, createduser)
	VALUES (gen_random_uuid(), pvar_tenantid, 'IPD Appointment Booking','Yes', pvar_tenantid);

	
INSERT INTO enquirytype(
	enquirytypeid, tenantid, enquiryname,isroombookingrelated, createduser)
	VALUES (gen_random_uuid(), pvar_tenantid, 'Follow-up Enquiry','No', pvar_tenantid);

	
	INSERT INTO enquirytype(
	enquirytypeid, tenantid, enquiryname,isroombookingrelated, createduser)
	VALUES (gen_random_uuid(), pvar_tenantid, 'General','No', pvar_tenantid);
	

lvar_processslaid:=gen_random_uuid();

INSERT INTO processsla(
	processslaid, tenantid, processname, createduser)
	VALUES (lvar_processslaid, pvar_tenantid, 'IPD Booking SLA', pvar_tenantid);

INSERT INTO processsla_processsla(
	processsla_processslaid, processslaid, record_order, initialstage, targetstage, slaindays, slainhrs)
VALUES
(gen_random_uuid(), lvar_processslaid, 1, 'Pending', 'Provisional Booking', 1, 1),
(gen_random_uuid(), lvar_processslaid, 2, 'Provisional Booking', 'Provisional Confirmed', 1, 1),
(gen_random_uuid(), lvar_processslaid, 3, 'Provisional Confirmed', 'Assessment Form - In Draft', 1, 1),
(gen_random_uuid(), lvar_processslaid, 4, 'Assessment Form - In Draft', 'Assessment Form - Review Pending', 1, 1),
(gen_random_uuid(), lvar_processslaid, 4, 'Provisional Confirmed', 'Assessment Form - Review Pending', 1, 1),
(gen_random_uuid(), lvar_processslaid, 5, 'Assessment Form - Review Pending', 'Assessment Form Reviewed', 1, 1),
(gen_random_uuid(), lvar_processslaid, 6, 'Assessment Form Reviewed', 'Screening Scheduled', 1, 1),
(gen_random_uuid(), lvar_processslaid, 7, 'Screening Scheduled', 'Admission Approved', 1, 1),
(gen_random_uuid(), lvar_processslaid, 8, 'Admission Approved', 'Arrival Confirmed', 1, 1),
(gen_random_uuid(), lvar_processslaid, 9, 'Arrival Confirmed', 'Consultation Scheduled', 1, 1),
(gen_random_uuid(), lvar_processslaid, 10, 'Consultation Scheduled', 'Admission Confirmed', 1, 1),
(gen_random_uuid(), lvar_processslaid, 11, 'Admission Confirmed', 'Admitted', 1, 1);

IF(pvar_parentid is not null)
THEN
 

update users set viewertenantids=tenantid::varchar||','|| sub.tenantids FROM (SELECT STRING_AGG(tenantid::varchar, ',') AS tenantids
FROM tenant
WHERE parentid =pvar_parentid)sub
where tenantid=pvar_parentid;

END IF ;

-- Seed alert templates for the new tenant from system-wide (tenantid IS NULL) templates
PERFORM "Seed_AlertTemplates_For_Tenant"(pvar_tenantid);
PERFORM "Seed_Template_For_Tenant"(pvar_tenantid , pvar_createduser);
PERFORM "Seed_Discharge_Checklist_For_Tenant"(pvar_tenantid,pvar_createduser);
PERFORM "Seed_Feedback_Form_For_Tenant"(pvar_tenantid,pvar_createduser);
					 
			  pvar_returnMessage :='201.1';
               
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
						,'Create_Healthcare_Provider'
						,'insert failed'
						);
                        pvar_returnMessage := 'Create_Healthcare_Provider - Insert failed';*/
			  	
			  END
              
$BODY$;

