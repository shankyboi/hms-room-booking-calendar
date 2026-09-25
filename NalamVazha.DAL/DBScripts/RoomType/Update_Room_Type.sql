
			  CREATE OR REPLACE FUNCTION  "Update_Room_Type"
			  (
				  pvar_RoomTypeid uuid
,pvar_tenantid uuid
,
pvar_name Varchar(128)
,
pvar_prebookingdaylimit int
,
pvar_minbookingdays int
,
pvar_maxbookingdays int
,
pvar_concessoneligibility  Varchar(1024)
,
pvar_suitabilityforvip  Varchar(1024)
,
pvar_gendersuitability Varchar(256)
,
pvar_roomtypeicon Varchar(256)
,
pvar_deposittype   Varchar(1024)
,
pvar_costperday decimal(18,2)
,
pvar_advanceperday decimal(18,2)
,
pvar_bookingdeposit decimal(18,2)
,
pvar_variableofbookingdays decimal(18,2)
,
pvar_attendantcostperday decimal(18,2)
,
pvar_attendantadvanceperday decimal(18,2)
,
pvar_attendantbookingdeposit decimal(18,2)
,
pvar_attendantvariableofbookingdays decimal(18,2)
,
pvar_hourlychargesapplicable Boolean
,
pvar_chargeperhour decimal(18,2)
,
pvar_billingwaiverfordelayedstart  Varchar(1024)
,
pvar_waiverpercentage decimal(18,2)
,
pvar_roomtransfercost   Varchar(1024)
,
pvar_description text
,pvar_refundpolicy json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:17*/
			  IF "Check_Authorization"(pvar_modifieduser, 'RoomType', 'edit') THEN


			  pvar_returnMessage:='';

			  
               IF(pvar_billingwaiverfordelayedstart is not null AND pvar_billingwaiverfordelayedstart!='0' AND LENGTH(pvar_billingwaiverfordelayedstart)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_billingwaiverfordelayedstart, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='billingwaiverfordelayedstart'
                                                                and entityname='RoomType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_billingwaiverfordelayedstart, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'billingwaiverfordelayedstart value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_concessoneligibility is not null AND pvar_concessoneligibility!='0' AND LENGTH(pvar_concessoneligibility)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_concessoneligibility, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='concessoneligibility'
                                                                and entityname='RoomType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_concessoneligibility, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'concessoneligibility value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_deposittype is not null AND pvar_deposittype!='0' AND LENGTH(pvar_deposittype)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_deposittype, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='deposittype'
                                                                and entityname='RoomType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_deposittype, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'deposittype value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_gendersuitability is not null AND pvar_gendersuitability!='0' AND LENGTH(pvar_gendersuitability)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_gendersuitability, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='gendersuitability'
                                                                and entityname='RoomType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_gendersuitability, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'gendersuitability value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_roomtransfercost is not null AND pvar_roomtransfercost!='0' AND LENGTH(pvar_roomtransfercost)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_roomtransfercost, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='roomtransfercost'
                                                                and entityname='RoomType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_roomtransfercost, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'roomtransfercost value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_suitabilityforvip is not null AND pvar_suitabilityforvip!='0' AND LENGTH(pvar_suitabilityforvip)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_suitabilityforvip, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='suitabilityforvip'
                                                                and entityname='RoomType' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_suitabilityforvip, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'suitabilityforvip value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('RoomType', NOW(),
(SELECT query_to_xml('SELECT * FROM RoomType WHERE RoomType.RoomTypeid= '''||pvar_RoomTypeid||'''', true, false, '')));

                    
                    UPDATE RoomType SET
                    name=pvar_name
,prebookingdaylimit=pvar_prebookingdaylimit
,minbookingdays=pvar_minbookingdays
,maxbookingdays=pvar_maxbookingdays
,concessoneligibility=pvar_concessoneligibility
,suitabilityforvip=pvar_suitabilityforvip
,gendersuitability=coalesce(pvar_gendersuitability,'')
,roomtypeicon=pvar_roomtypeicon
,deposittype=pvar_deposittype
,costperday=pvar_costperday
,advanceperday=pvar_advanceperday
,bookingdeposit=pvar_bookingdeposit
,variableofbookingdays=pvar_variableofbookingdays
,attendantcostperday=pvar_attendantcostperday
,attendantadvanceperday=pvar_attendantadvanceperday
,attendantbookingdeposit=pvar_attendantbookingdeposit
,attendantvariableofbookingdays=pvar_attendantvariableofbookingdays
,hourlychargesapplicable=pvar_hourlychargesapplicable
,chargeperhour=pvar_chargeperhour
,billingwaiverfordelayedstart=pvar_billingwaiverfordelayedstart
,waiverpercentage=pvar_waiverpercentage
,roomtransfercost=pvar_roomtransfercost
,description=pvar_description

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE RoomTypeid=pvar_RoomTypeid;

                    

                    INSERT INTO history
VALUES('RoomType_refundpolicy', NOW(),
(SELECT query_to_xml('SELECT * FROM RoomType_refundpolicy WHERE RoomType_refundpolicy.RoomTypeid= '''||pvar_RoomTypeid||'''', true, false, '')));

								DELETE FROM  RoomType_refundpolicy WHERE RoomTypeid=pvar_RoomTypeid;
								
								
								INSERT INTO RoomType_refundpolicy (
									RoomTypeid
									,RoomType_refundpolicyid 
                                    ,record_order  
									,refundtype
,cancellationby
,cancellationwindowdays
,refundpercentage

									
									)
									SELECT 
									pvar_RoomTypeid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,j->>'refundtype' as refundtype
,j->>'cancellationby' as cancellationby
,CAST(j->>'cancellationwindowdays' AS int) as cancellationwindowdays
,CAST(j->>'refundpercentage' AS decimal(18,2)) as refundpercentage

									
                                    FROM json_array_elements(pvar_refundpolicy) as j;
									



					
							
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
																,'Update_Room_Type'
																,'Authorization Failed Update_Room_Type'
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
						,'Update_Room_Type'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Room_Type - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

