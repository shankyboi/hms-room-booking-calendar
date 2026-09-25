
			  CREATE OR REPLACE FUNCTION  "Remove_Treatment_Package"
			  (
				  pvar_TreatmentPackageid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'TreatmentPackage', 'delete') THEN
			  
            INSERT INTO history
VALUES('TreatmentPackage', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage WHERE CAST(TreatmentPackage.TreatmentPackageid AS VARCHAR)= '''||pvar_TreatmentPackageid||'''', true, false, '')));

			 INSERT INTO history
VALUES('TreatmentPackage_roomtypes', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_roomtypes WHERE TreatmentPackage_roomtypes.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

INSERT INTO history
VALUES('TreatmentPackage_therapy', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_therapy WHERE TreatmentPackage_therapy.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

INSERT INTO history
VALUES('TreatmentPackage_therapykits', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_therapykits WHERE TreatmentPackage_therapykits.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

INSERT INTO history
VALUES('TreatmentPackage_therapyitems', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_therapyitems WHERE TreatmentPackage_therapyitems.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

INSERT INTO history
VALUES('TreatmentPackage_medicines', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_medicines WHERE TreatmentPackage_medicines.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

INSERT INTO history
VALUES('TreatmentPackage_refundpolicy', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_refundpolicy WHERE TreatmentPackage_refundpolicy.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));


			 UPDATE TreatmentPackage SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(TreatmentPackage.TreatmentPackageid AS VARCHAR)=pvar_TreatmentPackageid;
					 
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
																,'Remove_Treatment_Package'
																,'Authorization Failed Remove_Treatment_Package'
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
						,'Remove_Treatment_Package'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

