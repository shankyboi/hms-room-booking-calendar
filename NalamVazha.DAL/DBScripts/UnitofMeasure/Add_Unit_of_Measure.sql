
			  CREATE OR REPLACE FUNCTION  "Add_Unit_of_Measure"
			  (
				  pvar_UnitofMeasureid uuid
,
pvar_unitofmeasurename Varchar(128)
,
pvar_unitofmeasuredesc Varchar(256)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:42*/
		

			  
                                                                                    if pvar_UnitofMeasureid is null then
                                                                                    pvar_UnitofMeasureid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'UnitofMeasure', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from UnitofMeasure where upper(UnitofMeasure.unitofmeasurename::varchar) = upper(pvar_unitofmeasurename::varchar))
																THEN

																pvar_returnMessage := pvar_returnMessage||'Unit of Measure Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO UnitofMeasure(
				 unitofmeasurename
,unitofmeasuredesc

				 ,createduser
				 ,UnitofMeasureid
				 
                
			  )
			  VALUES (
 				 pvar_unitofmeasurename
,pvar_unitofmeasuredesc

				 ,pvar_createduser
				 ,pvar_UnitofMeasureid
				 
                   
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
																,'Add_Unit_of_Measure'
																,'Authorization Failed Add_Unit_of_Measure'
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
						,'Add_Unit_of_Measure'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Unit_of_Measure - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

