
			  CREATE OR REPLACE FUNCTION  "Add_Building"
			  (
				  pvar_Buildingid uuid
,pvar_tenantid uuid
,
pvar_block  uuid
,
pvar_buildingcode Varchar(128)
,
pvar_buildingname Varchar(128)
,
pvar_buildingdescription text
,
pvar_buildingimage Varchar(256)
,
pvar_buildingnearbylandmark Varchar(128)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:07*/
		

			  
                                                                                    if pvar_Buildingid is null then
                                                                                    pvar_Buildingid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Building', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Building where upper(Building.buildingcode::varchar) = upper(pvar_buildingcode::varchar) and Building.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Building Code Already Exists.';

																END IF;
IF EXISTS (SELECT * from Building where upper(Building.buildingname::varchar) = upper(pvar_buildingname::varchar) and Building.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Building Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Building(
				 block
,buildingcode
,buildingname
,buildingdescription
,buildingimage
,buildingnearbylandmark

				 ,createduser
				 ,Buildingid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_block
,pvar_buildingcode
,pvar_buildingname
,pvar_buildingdescription
,pvar_buildingimage
,pvar_buildingnearbylandmark

				 ,pvar_createduser
				 ,pvar_Buildingid
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
																,'Add_Building'
																,'Authorization Failed Add_Building'
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
						,'Add_Building'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Building - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

