
			  CREATE OR REPLACE FUNCTION  "Add_Enquiry_Type"
			  (
				  pvar_EnquiryTypeid uuid
,pvar_tenantid uuid
,
pvar_enquiryname Varchar(128)
,
pvar_enquirydesc Varchar(128)
,
pvar_isroombookingrelated  Varchar(1024)
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:50*/
		

			  
                                                                                    if pvar_EnquiryTypeid is null then
                                                                                    pvar_EnquiryTypeid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'EnquiryType', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from EnquiryType where upper(EnquiryType.enquiryname::varchar) = upper(pvar_enquiryname::varchar) and EnquiryType.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Enquiry Name Already Exists.';

																END IF;

              IF(pvar_isroombookingrelated is not null AND pvar_isroombookingrelated!='0' AND LENGTH(pvar_isroombookingrelated)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_isroombookingrelated, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='isroombookingrelated'
                                                                and entityname='EnquiryType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_isroombookingrelated, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'isroombookingrelated value is invalid';


                                                                END IF;
                                                            END IF;
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO EnquiryType(
				 enquiryname
,enquirydesc
,isroombookingrelated

				 ,createduser
				 ,EnquiryTypeid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_enquiryname
,pvar_enquirydesc
,pvar_isroombookingrelated

				 ,pvar_createduser
				 ,pvar_EnquiryTypeid
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
																,'Add_Enquiry_Type'
																,'Authorization Failed Add_Enquiry_Type'
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
						,'Add_Enquiry_Type'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Enquiry_Type - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

