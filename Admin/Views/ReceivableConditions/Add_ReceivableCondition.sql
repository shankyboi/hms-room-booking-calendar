
			  CREATE OR REPLACE FUNCTION  "Add_ReceivableCondition"
			  (
				  pvar_ReceivableConditionsid uuid
,pvar_tenantid uuid
,
pvar_receivablefor  Varchar(1024)
,
pvar_amount decimal(18,2)
,
pvar_ismandatory Boolean
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/23/2026 09:15:10*/
		

			  
                                                                                    if pvar_ReceivableConditionsid is null then
                                                                                    pvar_ReceivableConditionsid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'ReceivableConditions', 'create') THEN
			  pvar_returnMessage:='';
			  
              IF(pvar_receivablefor is not null AND pvar_receivablefor!='0' AND LENGTH(pvar_receivablefor)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='receivablefor'
                                                                and entityname='ReceivableConditions' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'receivablefor value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO ReceivableConditions(
				 receivablefor
,amount
,ismandatory

				 ,createduser
				 ,ReceivableConditionsid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_receivablefor
,pvar_amount
,pvar_ismandatory

				 ,pvar_createduser
				 ,pvar_ReceivableConditionsid
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
																,'Add_ReceivableCondition'
																,'Authorization Failed Add_ReceivableCondition'
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
						,'Add_ReceivableCondition'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_ReceivableCondition - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

