
			  -- shifthours widened from int to decimal(18,2): the UI always posts a formatted
			  -- value like "8.00" (to support fractional-hour shifts), which an int param
			  -- rejects outright. Drop the old int-typed overload so this replaces it cleanly.
			  DROP FUNCTION IF EXISTS "Add_Shift"(uuid, uuid, character varying, character varying, character varying, character varying, int, character varying, character varying, decimal, character varying, json, uuid);

			  CREATE OR REPLACE FUNCTION  "Add_Shift"
			  (
				  pvar_Shiftid uuid
,pvar_tenantid uuid
,
pvar_shiftcode Varchar(256)
,
pvar_shiftname Varchar(128)
,
pvar_shiftstarttime Varchar(256)
,
pvar_shiftendtime Varchar(256)
,
pvar_shifthours decimal(18,2)
,
pvar_description Varchar(256)
,
pvar_totalbreakinmins Varchar(256)
,
pvar_totalbreakinhrs decimal(18,2)
,
pvar_workhours Varchar(256)
,pvar_breakdurationdetails json
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_shiftcode Varchar(10);lvar_val_shiftcode int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17*/
		

			  
                                                                                    if pvar_Shiftid is null then
                                                                                    pvar_Shiftid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
select cast(to_char(NOW(),'yyyy') as Varchar(4)) ||'-'|| RIGHT('00' ||cast(to_char(NOW(),'MM') as Varchar(2)),2) 
                                          INTO lvar_curday_shiftcode;
                                        select COALESCE(max(RIGHT(Shift.shiftcode,4)),'0') INTO lvar_val_shiftcode from
                                        Shift where substring(Shift.shiftcode,1,7) = lvar_curday_shiftcode and (Shift.shiftcode) NOT LIKE '%/%';
                                        lvar_val_shiftcode:=lvar_val_shiftcode + 1;
                                        pvar_shiftcode:= (lvar_curday_shiftcode||'-'|| cast(to_char(lvar_val_shiftcode,'fm0000') as Varchar(4)));

			  IF "Check_Authorization"(pvar_createduser, 'Shift', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Shift where upper(Shift.shiftcode::varchar) = upper(pvar_shiftcode::varchar) and Shift.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Shift Code Already Exists.';

																END IF;
IF EXISTS (SELECT * from Shift where upper(Shift.shiftname::varchar) = upper(pvar_shiftname::varchar) and Shift.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Shift Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Shift(
				 shiftcode
,shiftname
,shiftstarttime
,shiftendtime
,shifthours
,description
,totalbreakinmins
,totalbreakinhrs
,workhours

				 ,createduser
				 ,Shiftid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_shiftcode
,pvar_shiftname
,pvar_shiftstarttime
,pvar_shiftendtime
,pvar_shifthours
,pvar_description
,pvar_totalbreakinmins
,pvar_totalbreakinhrs
,pvar_workhours

				 ,pvar_createduser
				 ,pvar_Shiftid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
								INSERT INTO Shift_breakdurationdetails (
									Shiftid
									,Shift_breakdurationdetailsid 
                                    ,record_order  
									,breakname
,starttime
,endtime
,durationinmin

									
									)
									SELECT 
									pvar_Shiftid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'breakname' as breakname
,j->>'starttime' as starttime
,j->>'endtime' as endtime
,CAST(j->>'durationinmin' AS decimal(18,2)) as durationinmin

									
                                    FROM json_array_elements(pvar_breakdurationdetails) as j;
									

					 
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
																,'Add_Shift'
																,'Authorization Failed Add_Shift'
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
						,'Add_Shift'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Shift - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

