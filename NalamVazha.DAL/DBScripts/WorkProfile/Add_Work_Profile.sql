
CREATE OR REPLACE FUNCTION public."Add_Work_Profile"(
	pvar_workprofileid uuid,
	pvar_tenantid uuid,
	pvar_department uuid,
	pvar_workprofilename character varying,
	pvar_rolename character varying,
	pvar_isthisaclinicalprofile character varying,
	pvar_workprofiledescription character varying,
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:29*/
		

			  
                                                                                    if pvar_WorkProfileid is null then
                                                                                    pvar_WorkProfileid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'WorkProfile', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from WorkProfile where upper(WorkProfile.workprofilename::varchar) = upper(pvar_workprofilename::varchar) and WorkProfile.tenantid=pvar_tenantid and WorkProfile.isdeleted=false)
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
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO WorkProfile(
				 department
,workprofilename
,rolename
,isthisaclinicalprofile
,workprofiledescription

				 ,createduser
				 ,WorkProfileid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_department
,pvar_workprofilename
,pvar_rolename
,pvar_isthisaclinicalprofile
,pvar_workprofiledescription

				 ,pvar_createduser
				 ,pvar_WorkProfileid
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
																,'Add_Work_Profile'
																,'Authorization Failed Add_Work_Profile'
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
						,'Add_Work_Profile'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Work_Profile - Insert failed';*/
			  	
			  END
              
$BODY$;
