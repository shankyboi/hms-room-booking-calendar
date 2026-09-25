
			  CREATE OR REPLACE FUNCTION  "Remove_IPD_Application_Form"
			  (
				  pvar_IPDApplicationFormid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'IPDApplicationForm', 'delete') THEN
			  
            INSERT INTO history
VALUES('IPDApplicationForm', NOW(),
(SELECT query_to_xml('SELECT * FROM IPDApplicationForm WHERE CAST(IPDApplicationForm.IPDApplicationFormid AS VARCHAR)= '''||pvar_IPDApplicationFormid||'''', true, false, '')));

			 INSERT INTO IPDApplicationForm_preferreddatesofadmission_history
SELECT * FROM IPDApplicationForm_preferreddatesofadmission 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

INSERT INTO IPDApplicationForm_medicalinfo_history
SELECT * FROM IPDApplicationForm_medicalinfo 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

INSERT INTO IPDApplicationForm_medicationinfo_history
SELECT * FROM IPDApplicationForm_medicationinfo 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

INSERT INTO IPDApplicationForm_medicalrecords_history
SELECT * FROM IPDApplicationForm_medicalrecords 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

INSERT INTO IPDApplicationForm_attendantinfo_history
SELECT * FROM IPDApplicationForm_attendantinfo 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

INSERT INTO IPDApplicationForm_roompreference_history
SELECT * FROM IPDApplicationForm_roompreference 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

INSERT INTO IPDApplicationForm_room_history
SELECT * FROM IPDApplicationForm_room 
 WHERE IPDApplicationFormid= pvar_IPDApplicationFormid::uuid;

UPDATE IPDApplicationForm_attendantpreferreddates SET isdeleted=true,modifieduser=pvar_modifieduser::uuid,modifieddate=NOW() WHERE IPDApplicationFormid=pvar_IPDApplicationFormid::uuid;
UPDATE IPDApplicationForm_attendantroompreference SET isdeleted=true,modifieduser=pvar_modifieduser::uuid,modifieddate=NOW() WHERE IPDApplicationFormid=pvar_IPDApplicationFormid::uuid;


			 UPDATE IPDApplicationForm SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(IPDApplicationForm.IPDApplicationFormid AS VARCHAR)=pvar_IPDApplicationFormid;
					 
				  pvar_returnMessage := '201.1';
					 
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
																,'Remove_IPD_Application_Form'
																,'Authorization Failed Remove_IPD_Application_Form'
																,pvar_modifieduser
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
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Remove_IPD_Application_Form'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

