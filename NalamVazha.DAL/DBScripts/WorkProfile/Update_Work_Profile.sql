
			  CREATE OR REPLACE FUNCTION  "Update_Work_Profile"
			  (
				  pvar_WorkProfileid uuid
,pvar_tenantid uuid
,
pvar_department  uuid
,
pvar_workprofilename Varchar(128)
,
pvar_rolename  Varchar(1024)
,
pvar_isthisaclinicalprofile  Varchar(1024)
,
pvar_workprofiledescription Varchar(256)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:29*/
			  IF "Check_Authorization"(pvar_modifieduser, 'WorkProfile', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from WorkProfile where upper(WorkProfile.workprofilename) = upper(pvar_workprofilename) and WorkProfile.tenantid=pvar_tenantid  and WorkProfile.WorkProfileid <> pvar_WorkProfileid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Work Profile Name Already Exists.';

																END IF;

               IF(pvar_isthisaclinicalprofile is not null AND pvar_isthisaclinicalprofile!='0' AND LENGTH(pvar_isthisaclinicalprofile)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_isthisaclinicalprofile, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='isthisaclinicalprofile'
                                                                and entityname='WorkProfile' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_isthisaclinicalprofile, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'isthisaclinicalprofile value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_rolename is not null AND pvar_rolename!='0' AND LENGTH(pvar_rolename)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                    FROM regexp_split_to_table(pvar_rolename, ',') AS T1
                                                                        INNER JOIN (Select DISTINCT roles.rolename from roles) AS T2 on T1.T1 = T2.rolename) AS int) <> CAST((SELECT Count(T1.T1)
                                                                    FROM regexp_split_to_table(pvar_rolename, ',')  AS T1) AS int))
                                                                    THEN
                                                                         pvar_returnMessage := pvar_returnMessage || ' rolename value is invalid';


                                                                    END IF;
                                                                    END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('WorkProfile', NOW(),
(SELECT query_to_xml('SELECT * FROM WorkProfile WHERE WorkProfile.WorkProfileid= '''||pvar_WorkProfileid||'''', true, false, '')));

                    
                    UPDATE WorkProfile SET
                    department=pvar_department
,workprofilename=pvar_workprofilename
,rolename=pvar_rolename
,isthisaclinicalprofile=pvar_isthisaclinicalprofile
,workprofiledescription=pvar_workprofiledescription

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE WorkProfileid=pvar_WorkProfileid;

                    

                    


					
							
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
																,'Update_Work_Profile'
																,'Authorization Failed Update_Work_Profile'
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
						,'Update_Work_Profile'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Work_Profile - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

