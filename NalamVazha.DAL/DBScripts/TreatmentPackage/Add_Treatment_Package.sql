
			  CREATE OR REPLACE FUNCTION  "Add_Treatment_Package"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
		

			  
                                                                                    if pvar_TreatmentPackageid is null then
                                                                                    pvar_TreatmentPackageid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'TreatmentPackage', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from TreatmentPackage where upper(TreatmentPackage.packagename::varchar) = upper(pvar_packagename::varchar) and TreatmentPackage.tenantid=pvar_tenantid)
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
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO TreatmentPackage(
				 packagename
,noofdays
,roomtypeamountaverage
,therapyamount
,therapykitamount
,therapyitemamount
,medicineamount
,calculatedpackagecost
,packagecost
,packagebookingdeposit
,packagebookingadvance
,billingwaiverfordelayedstart
,waiverpercentage
,roomtransfercost
,packagedescription

				 ,createduser
				 ,TreatmentPackageid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_packagename
,pvar_noofdays
,pvar_roomtypeamountaverage
,pvar_therapyamount
,pvar_therapykitamount
,pvar_therapyitemamount
,pvar_medicineamount
,pvar_calculatedpackagecost
,pvar_packagecost
,pvar_packagebookingdeposit
,pvar_packagebookingadvance
,pvar_billingwaiverfordelayedstart
,pvar_waiverpercentage
,pvar_roomtransfercost
,pvar_packagedescription

				 ,pvar_createduser
				 ,pvar_TreatmentPackageid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
								
								
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
																,'Add_Treatment_Package'
																,'Authorization Failed Add_Treatment_Package'
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
						,'Add_Treatment_Package'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Treatment_Package - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

