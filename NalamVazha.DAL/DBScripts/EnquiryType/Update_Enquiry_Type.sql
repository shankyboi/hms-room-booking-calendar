
			  CREATE OR REPLACE FUNCTION  "Update_Enquiry_Type"
			  (
				  pvar_EnquiryTypeid uuid
,pvar_tenantid uuid
,
pvar_enquiryname Varchar(128)
,
pvar_enquirydesc Varchar(128)
,
pvar_isroombookingrelated  Varchar(1024)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:50*/
			  IF "Check_Authorization"(pvar_modifieduser, 'EnquiryType', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from EnquiryType where upper(EnquiryType.enquiryname) = upper(pvar_enquiryname) and EnquiryType.tenantid=pvar_tenantid  and EnquiryType.EnquiryTypeid <> pvar_EnquiryTypeid)
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
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('EnquiryType', NOW(),
(SELECT query_to_xml('SELECT * FROM EnquiryType WHERE EnquiryType.EnquiryTypeid= '''||pvar_EnquiryTypeid||'''', true, false, '')));

                    
                    UPDATE EnquiryType SET
                    enquiryname=pvar_enquiryname
,enquirydesc=pvar_enquirydesc
,isroombookingrelated=pvar_isroombookingrelated

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE EnquiryTypeid=pvar_EnquiryTypeid;

                    

                    


					
							
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
																,'Update_Enquiry_Type'
																,'Authorization Failed Update_Enquiry_Type'
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
						,'Update_Enquiry_Type'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Enquiry_Type - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

