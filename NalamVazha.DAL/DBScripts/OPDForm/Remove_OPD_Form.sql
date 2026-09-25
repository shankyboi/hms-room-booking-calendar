
			  CREATE OR REPLACE FUNCTION  "Remove_OPD_Form"
			  (
				  pvar_OPDFormid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000)
              AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:54*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'OPDForm', 'delete') THEN

            INSERT INTO history
VALUES('OPDForm', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm WHERE CAST(OPDForm.OPDFormid AS VARCHAR)= '''||pvar_OPDFormid||'''', true, false, '')));

			 INSERT INTO history
VALUES('OPDForm_medicalinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_medicalinfo WHERE OPDForm_medicalinfo.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

INSERT INTO history
VALUES('OPDForm_medicationinfo', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_medicationinfo WHERE OPDForm_medicationinfo.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

INSERT INTO history
VALUES('OPDForm_medicalrecords', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_medicalrecords WHERE OPDForm_medicalrecords.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));

INSERT INTO history
VALUES('OPDForm_appointmentpreferences', NOW(),
(SELECT query_to_xml('SELECT * FROM OPDForm_appointmentpreferences WHERE OPDForm_appointmentpreferences.OPDFormid= '''||pvar_OPDFormid||'''', true, false, '')));


			 UPDATE OPDForm SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(OPDForm.OPDFormid AS VARCHAR)=pvar_OPDFormid;

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
																,'Remove_OPD_Form'
																,'Authorization Failed Remove_OPD_Form'
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
						,'Remove_OPD_Form'
						,'user delete failed'
						);
					    pvar_returnMessage := 'user delete failed';*/


 			  END
              $BODY$
              LANGUAGE plpgsql;

