
			  CREATE OR REPLACE FUNCTION  "Add_Room_Group"
			  (
				  pvar_RoomGroupid uuid
,pvar_tenantid uuid
,
pvar_groupname Varchar(128)
,
pvar_groupnumber Varchar(128)
,
pvar_groupdesc Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:13*/
		

			  
                                                                                    if pvar_RoomGroupid is null then
                                                                                    pvar_RoomGroupid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'RoomGroup', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from RoomGroup where upper(RoomGroup.groupname::varchar) = upper(pvar_groupname::varchar) and RoomGroup.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Group Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO RoomGroup(
				 groupname
,groupnumber
,groupdesc

				 ,createduser
				 ,RoomGroupid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_groupname
,pvar_groupnumber
,pvar_groupdesc

				 ,pvar_createduser
				 ,pvar_RoomGroupid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
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
																,'Add_Room_Group'
																,'Authorization Failed Add_Room_Group'
																,pvar_createduser
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
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Room_Group'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Room_Group - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

