
			  CREATE OR REPLACE FUNCTION  "Update_Treatment_Package"
			  (
				  pvar_TreatmentPackageid uuid
,pvar_tenantid uuid
,
pvar_packagename Varchar(128)
,
pvar_noofdays int
,
pvar_roomtypeamountaverage Varchar(256)
,
pvar_therapyamount Varchar(256)
,
pvar_therapykitamount Varchar(256)
,
pvar_therapyitemamount Varchar(256)
,
pvar_medicineamount Varchar(256)
,
pvar_calculatedpackagecost Varchar(256)
,
pvar_packagecost decimal(18,2)
,
pvar_packagebookingdeposit decimal(18,2)
,
pvar_packagebookingadvance decimal(18,2)
,
pvar_billingwaiverfordelayedstart  Varchar(1024)
,
pvar_waiverpercentage decimal(18,2)
,
pvar_roomtransfercost  Varchar(1024)
,
pvar_packagedescription Varchar(256)
,pvar_roomtypes json
,pvar_therapy json
,pvar_therapykits json
,pvar_therapyitems json
,pvar_medicines json
,pvar_refundpolicy json

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
			  IF "Check_Authorization"(pvar_modifieduser, 'TreatmentPackage', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from TreatmentPackage where upper(TreatmentPackage.packagename) = upper(pvar_packagename) and TreatmentPackage.tenantid=pvar_tenantid  and TreatmentPackage.TreatmentPackageid <> pvar_TreatmentPackageid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Package Name Already Exists.';

																END IF;

               IF(pvar_billingwaiverfordelayedstart is not null AND pvar_billingwaiverfordelayedstart!='0' AND LENGTH(pvar_billingwaiverfordelayedstart)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_billingwaiverfordelayedstart, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='billingwaiverfordelayedstart'
                                                                and entityname='TreatmentPackage' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_billingwaiverfordelayedstart, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'billingwaiverfordelayedstart value is invalid';


                                                                END IF;
                                                            END IF;
IF(pvar_roomtransfercost is not null AND pvar_roomtransfercost!='0' AND LENGTH(pvar_roomtransfercost)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_roomtransfercost, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='roomtransfercost'
                                                                and entityname='TreatmentPackage' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_roomtransfercost, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'roomtransfercost value is invalid';


                                                                END IF;
                                                            END IF;
 
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('TreatmentPackage', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage WHERE TreatmentPackage.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

                    
                    UPDATE TreatmentPackage SET
                    packagename=pvar_packagename
,noofdays=pvar_noofdays
,roomtypeamountaverage=pvar_roomtypeamountaverage
,therapyamount=pvar_therapyamount
,therapykitamount=pvar_therapykitamount
,therapyitemamount=pvar_therapyitemamount
,medicineamount=pvar_medicineamount
,calculatedpackagecost=pvar_calculatedpackagecost
,packagecost=pvar_packagecost
,packagebookingdeposit=pvar_packagebookingdeposit
,packagebookingadvance=pvar_packagebookingadvance
,billingwaiverfordelayedstart=pvar_billingwaiverfordelayedstart
,waiverpercentage=pvar_waiverpercentage
,roomtransfercost=pvar_roomtransfercost
,packagedescription=pvar_packagedescription

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE TreatmentPackageid=pvar_TreatmentPackageid;

                    

                    INSERT INTO history
VALUES('TreatmentPackage_roomtypes', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_roomtypes WHERE TreatmentPackage_roomtypes.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

								DELETE FROM  TreatmentPackage_roomtypes WHERE TreatmentPackageid=pvar_TreatmentPackageid;
								
								
								INSERT INTO TreatmentPackage_roomtypes (
									TreatmentPackageid
									,TreatmentPackage_roomtypesid 
                                    ,record_order  
									,roomtype
,costperday
,percentagecovered
,roomcost

									
									)
									SELECT 
									pvar_TreatmentPackageid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'roomtype' AS uuid) as roomtype
,CAST(j->>'costperday' AS decimal(18,2)) as costperday
,CAST(j->>'percentagecovered' AS decimal(18,2)) as percentagecovered
,j->>'roomcost' as roomcost

									
                                    FROM json_array_elements(pvar_roomtypes) as j;
									
INSERT INTO history
VALUES('TreatmentPackage_therapy', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_therapy WHERE TreatmentPackage_therapy.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

								DELETE FROM  TreatmentPackage_therapy WHERE TreatmentPackageid=pvar_TreatmentPackageid;
								
								
								INSERT INTO TreatmentPackage_therapy (
									TreatmentPackageid
									,TreatmentPackage_therapyid 
                                    ,record_order  
									,therapyname
,therapycost
,numberoftimes
,therapyprice

									
									)
									SELECT 
									pvar_TreatmentPackageid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'therapyname' AS uuid) as therapyname
,CAST(j->>'therapycost' AS decimal(18,2)) as therapycost
,CAST(j->>'numberoftimes' AS int) as numberoftimes
,j->>'therapyprice' as therapyprice

									
                                    FROM json_array_elements(pvar_therapy) as j;
									
INSERT INTO history
VALUES('TreatmentPackage_therapykits', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_therapykits WHERE TreatmentPackage_therapykits.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

								DELETE FROM  TreatmentPackage_therapykits WHERE TreatmentPackageid=pvar_TreatmentPackageid;
								
								
								INSERT INTO TreatmentPackage_therapykits (
									TreatmentPackageid
									,TreatmentPackage_therapykitsid 
                                    ,record_order  
									,therapykitname
,kitprice
,numberofkits
,therapykitcost

									
									)
									SELECT 
									pvar_TreatmentPackageid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'therapykitname' AS uuid) as therapykitname
,j->>'kitprice' as kitprice
,CAST(j->>'numberofkits' AS int) as numberofkits
,j->>'therapykitcost' as therapykitcost

									
                                    FROM json_array_elements(pvar_therapykits) as j;
									
INSERT INTO history
VALUES('TreatmentPackage_therapyitems', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_therapyitems WHERE TreatmentPackage_therapyitems.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

								DELETE FROM  TreatmentPackage_therapyitems WHERE TreatmentPackageid=pvar_TreatmentPackageid;
								
								
								INSERT INTO TreatmentPackage_therapyitems (
									TreatmentPackageid
									,TreatmentPackage_therapyitemsid 
                                    ,record_order  
									,therapyitem
,price
,therapyitemcount
,therapyitemcost

									
									)
									SELECT 
									pvar_TreatmentPackageid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'therapyitem' AS uuid) as therapyitem
,CAST(j->>'price' AS decimal(18,2)) as price
,CAST(j->>'therapyitemcount' AS int) as therapyitemcount
,j->>'therapyitemcost' as therapyitemcost

									
                                    FROM json_array_elements(pvar_therapyitems) as j;
									
INSERT INTO history
VALUES('TreatmentPackage_medicines', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_medicines WHERE TreatmentPackage_medicines.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

								DELETE FROM  TreatmentPackage_medicines WHERE TreatmentPackageid=pvar_TreatmentPackageid;
								
								
								INSERT INTO TreatmentPackage_medicines (
									TreatmentPackageid
									,TreatmentPackage_medicinesid 
                                    ,record_order  
									,medicinename
,price
,medicinecount
,medicinecost

									
									)
									SELECT 
									pvar_TreatmentPackageid
                                    ,gen_random_uuid()
                                    ,CAST(coalesce(j->>'record_order','0') as INT)
									,CAST(j->>'medicinename' AS uuid) as medicinename
,CAST(j->>'price' AS decimal(18,2)) as price
,CAST(j->>'medicinecount' AS int) as medicinecount
,j->>'medicinecost' as medicinecost

									
                                    FROM json_array_elements(pvar_medicines) as j;
									
INSERT INTO history
VALUES('TreatmentPackage_refundpolicy', NOW(),
(SELECT query_to_xml('SELECT * FROM TreatmentPackage_refundpolicy WHERE TreatmentPackage_refundpolicy.TreatmentPackageid= '''||pvar_TreatmentPackageid||'''', true, false, '')));

								DELETE FROM  TreatmentPackage_refundpolicy WHERE TreatmentPackageid=pvar_TreatmentPackageid;
								
								
								INSERT INTO TreatmentPackage_refundpolicy (
									TreatmentPackageid
									,TreatmentPackage_refundpolicyid 
                                    ,record_order  
									,refundtype
,cancellationby
,cancellationwindowdays
,refundpercentage

									
									)
									SELECT 
									pvar_TreatmentPackageid
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
																,'Update_Treatment_Package'
																,'Authorization Failed Update_Treatment_Package'
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
						,'Update_Treatment_Package'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Treatment_Package - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

