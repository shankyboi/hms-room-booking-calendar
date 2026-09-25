
			  CREATE OR REPLACE FUNCTION  "Update_Room_Group"
			  (
				  pvar_RoomGroupid uuid
,pvar_tenantid uuid
,
pvar_groupname Varchar(128)
,
pvar_groupnumber Varchar(128)
,
pvar_groupdesc Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:13*/
			  IF "Check_Authorization"(pvar_modifieduser, 'RoomGroup', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from RoomGroup where upper(RoomGroup.groupname) = upper(pvar_groupname) and RoomGroup.tenantid=pvar_tenantid  and RoomGroup.RoomGroupid <> pvar_RoomGroupid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Group Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('RoomGroup', NOW(),
(SELECT query_to_xml('SELECT * FROM RoomGroup WHERE RoomGroup.RoomGroupid= '''||pvar_RoomGroupid||'''', true, false, '')));

                    
                    UPDATE RoomGroup SET
                    groupname=pvar_groupname
,groupnumber=pvar_groupnumber
,groupdesc=pvar_groupdesc

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE RoomGroupid=pvar_RoomGroupid;

                    

                    


					
							
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
																,'Update_Room_Group'
																,'Authorization Failed Update_Room_Group'
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
						,'Update_Room_Group'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Room_Group - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

