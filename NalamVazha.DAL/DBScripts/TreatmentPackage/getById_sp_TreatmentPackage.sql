 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_TreatmentPackage"
			  (
				  pvar_TreatmentPackageid Varchar
			  )
			  RETURNS TABLE(
                packagename Varchar
,noofdays int
,roomtypeamountaverage Varchar
,therapyamount Varchar
,therapykitamount Varchar
,therapyitemamount Varchar
,medicineamount Varchar
,calculatedpackagecost Varchar
,packagecost decimal
,packagebookingdeposit decimal
,packagebookingadvance decimal
,billingwaiverfordelayedstart Varchar
,waiverpercentage decimal
,roomtransfercost Varchar
,packagedescription Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,TreatmentPackageid uuid
                ,roomtypes JSON,therapy JSON,therapykits JSON,therapyitems JSON,medicines JSON,refundpolicy JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
               
              RETURN QUERY
			  SELECT 
				 TreatmentPackage.packagename
,TreatmentPackage.noofdays
,TreatmentPackage.roomtypeamountaverage
,TreatmentPackage.therapyamount
,TreatmentPackage.therapykitamount
,TreatmentPackage.therapyitemamount
,TreatmentPackage.medicineamount
,TreatmentPackage.calculatedpackagecost
,TreatmentPackage.packagecost
,TreatmentPackage.packagebookingdeposit
,TreatmentPackage.packagebookingadvance
,TreatmentPackage.billingwaiverfordelayedstart
,TreatmentPackage.waiverpercentage
,TreatmentPackage.roomtransfercost
,TreatmentPackage.packagedescription

				 ,TreatmentPackage.createduser,TreatmentPackage.createddate,TreatmentPackage.modifieduser,TreatmentPackage.modifieddate
				 ,TreatmentPackage.tenantid
                 ,TreatmentPackage.TreatmentPackageid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 TreatmentPackage_roomtypes.TreatmentPackageid
                                             ,TreatmentPackage_roomtypes.TreatmentPackage_roomtypesid   
											 ,TreatmentPackage_roomtypes.roomtype
,TreatmentPackage_roomtypes.costperday
,TreatmentPackage_roomtypes.percentagecovered
,TreatmentPackage_roomtypes.roomcost
 
											  
											 FROM TreatmentPackage_roomtypes
											 WHERE 
											 TreatmentPackage_roomtypes.TreatmentPackageid=TreatmentPackage.TreatmentPackageid
                                             
                                             ORDER BY record_order DESC
											) J) as roomtypes
,(SELECT json_agg(J) FROM (
											 SELECT 
											 TreatmentPackage_therapy.TreatmentPackageid
                                             ,TreatmentPackage_therapy.TreatmentPackage_therapyid   
											 ,TreatmentPackage_therapy.therapyname
,TreatmentPackage_therapy.therapycost
,TreatmentPackage_therapy.numberoftimes
,TreatmentPackage_therapy.therapyprice
 
											  
											 FROM TreatmentPackage_therapy
											 WHERE 
											 TreatmentPackage_therapy.TreatmentPackageid=TreatmentPackage.TreatmentPackageid
                                             
                                             ORDER BY record_order DESC
											) J) as therapy
,(SELECT json_agg(J) FROM (
											 SELECT 
											 TreatmentPackage_therapykits.TreatmentPackageid
                                             ,TreatmentPackage_therapykits.TreatmentPackage_therapykitsid   
											 ,TreatmentPackage_therapykits.therapykitname
,TreatmentPackage_therapykits.kitprice
,TreatmentPackage_therapykits.numberofkits
,TreatmentPackage_therapykits.therapykitcost
 
											  
											 FROM TreatmentPackage_therapykits
											 WHERE 
											 TreatmentPackage_therapykits.TreatmentPackageid=TreatmentPackage.TreatmentPackageid
                                             
                                             ORDER BY record_order DESC
											) J) as therapykits
,(SELECT json_agg(J) FROM (
											 SELECT 
											 TreatmentPackage_therapyitems.TreatmentPackageid
                                             ,TreatmentPackage_therapyitems.TreatmentPackage_therapyitemsid   
											 ,TreatmentPackage_therapyitems.therapyitem
,TreatmentPackage_therapyitems.price
,TreatmentPackage_therapyitems.therapyitemcount
,TreatmentPackage_therapyitems.therapyitemcost
 
											  
											 FROM TreatmentPackage_therapyitems
											 WHERE 
											 TreatmentPackage_therapyitems.TreatmentPackageid=TreatmentPackage.TreatmentPackageid
                                             
                                             ORDER BY record_order DESC
											) J) as therapyitems
,(SELECT json_agg(J) FROM (
											 SELECT 
											 TreatmentPackage_medicines.TreatmentPackageid
                                             ,TreatmentPackage_medicines.TreatmentPackage_medicinesid   
											 ,TreatmentPackage_medicines.medicinename
,TreatmentPackage_medicines.price
,TreatmentPackage_medicines.medicinecount
,TreatmentPackage_medicines.medicinecost
 
											  
											 FROM TreatmentPackage_medicines
											 WHERE 
											 TreatmentPackage_medicines.TreatmentPackageid=TreatmentPackage.TreatmentPackageid
                                             
                                             ORDER BY record_order DESC
											) J) as medicines
,(SELECT json_agg(J) FROM (
											 SELECT 
											 TreatmentPackage_refundpolicy.TreatmentPackageid
                                             ,TreatmentPackage_refundpolicy.TreatmentPackage_refundpolicyid   
											 ,TreatmentPackage_refundpolicy.refundtype
,TreatmentPackage_refundpolicy.cancellationby
,TreatmentPackage_refundpolicy.cancellationwindowdays
,TreatmentPackage_refundpolicy.refundpercentage
 
											  
											 FROM TreatmentPackage_refundpolicy
											 WHERE 
											 TreatmentPackage_refundpolicy.TreatmentPackageid=TreatmentPackage.TreatmentPackageid
                                             
                                             ORDER BY record_order DESC
											) J) as refundpolicy
   
			  FROM TreatmentPackage
			  WHERE CAST(TreatmentPackage.TreatmentPackageid AS Varchar)=pvar_TreatmentPackageid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

