
			  CREATE OR REPLACE FUNCTION  "Remove_Room_Group"
			  (
				  pvar_RoomGroupid Varchar(50)
				  ,pvar_modifieduser  Varchar(50)
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:13*/
			  IF "Check_Authorization"(pvar_modifieduser::uuid, 'RoomGroup', 'delete') THEN
			  
            INSERT INTO history
VALUES('RoomGroup', NOW(),
(SELECT query_to_xml('SELECT * FROM RoomGroup WHERE CAST(RoomGroup.RoomGroupid AS VARCHAR)= '''||pvar_RoomGroupid||'''', true, false, '')));

			 
			 UPDATE RoomGroup SET
													isdeleted=true ,modifieduser=CAST(pvar_modifieduser AS UUID),modifieddate=NOW()
													WHERE CAST(RoomGroup.RoomGroupid AS VARCHAR)=pvar_RoomGroupid;
					 
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
																,'Remove_Room_Group'
																,'Authorization Failed Remove_Room_Group'
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
						,'Remove_Room_Group'
						,'user delete failed'
						);				 
					    pvar_returnMessage := 'user delete failed';*/
				  
			  
 			  END
              $BODY$
              LANGUAGE plpgsql;

