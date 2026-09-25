
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_TreatmentPackage"
              (
			  pvar_TreatmentPackageid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"TreatmentPackageid" uuid
,packagename Varchar
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

                ,"automaton_TreatmentPackage_roomtypes" json,"automaton_TreatmentPackage_therapy" json,"automaton_TreatmentPackage_therapykits" json,"automaton_TreatmentPackage_therapyitems" json,"automaton_TreatmentPackage_medicines" json,"automaton_TreatmentPackage_refundpolicy" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
			  		 
              RETURN QUERY
			  SELECT  
				 TreatmentPackage.tenantid
,tenant.businessname as _tenantname
,TreatmentPackage.TreatmentPackageid
,TreatmentPackage.packagename
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
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_RoomType.name AS VARCHAR) as "Room Type"
,TreatmentPackage_roomtypes.costperday as "Cost Per Day"
,TreatmentPackage_roomtypes.percentagecovered as "Percentage Covered"
,TreatmentPackage_roomtypes.roomcost as "Room Cost"

							
						FROM  TreatmentPackage_roomtypes 
INNER JOIN RoomType _RoomType ON TreatmentPackage_roomtypes.roomtype=_RoomType.RoomTypeid

						WHERE TreatmentPackage.TreatmentPackageid =TreatmentPackage_roomtypes.TreatmentPackageid
) J)
						as automaton_TreatmentPackage_roomtypes
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_Therapies.therapyname AS VARCHAR) as "Therapy Name"
,TreatmentPackage_therapy.therapycost as "Therapy Cost"
,TreatmentPackage_therapy.numberoftimes as "Number of Times"
,TreatmentPackage_therapy.therapyprice as "Therapy Price"

							
						FROM  TreatmentPackage_therapy 
INNER JOIN Therapies _Therapies ON TreatmentPackage_therapy.therapyname=_Therapies.Therapiesid

						WHERE TreatmentPackage.TreatmentPackageid =TreatmentPackage_therapy.TreatmentPackageid
) J)
						as automaton_TreatmentPackage_therapy
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_TherapyKit.therapykitname AS VARCHAR) as "Therapy Kit Name"
,TreatmentPackage_therapykits.kitprice as "Kit Price"
,TreatmentPackage_therapykits.numberofkits as "Number of Kits"
,TreatmentPackage_therapykits.therapykitcost as "Therapy Kit Cost"

							
						FROM  TreatmentPackage_therapykits 
INNER JOIN TherapyKit _TherapyKit ON TreatmentPackage_therapykits.therapykitname=_TherapyKit.TherapyKitid

						WHERE TreatmentPackage.TreatmentPackageid =TreatmentPackage_therapykits.TreatmentPackageid
) J)
						as automaton_TreatmentPackage_therapykits
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_TherapyItem.therapyitemname AS VARCHAR) as "Therapy Item"
,TreatmentPackage_therapyitems.price as "Price"
,TreatmentPackage_therapyitems.therapyitemcount as "Therapy Item Count"
,TreatmentPackage_therapyitems.therapyitemcost as "Therapy Item Cost"

							
						FROM  TreatmentPackage_therapyitems 
INNER JOIN TherapyItem _TherapyItem ON TreatmentPackage_therapyitems.therapyitem=_TherapyItem.TherapyItemid

						WHERE TreatmentPackage.TreatmentPackageid =TreatmentPackage_therapyitems.TreatmentPackageid
) J)
						as automaton_TreatmentPackage_therapyitems
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_Medicine.medicinename AS VARCHAR) as "Medicine Name"
,TreatmentPackage_medicines.price as "Price"
,TreatmentPackage_medicines.medicinecount as "Medicine Count"
,TreatmentPackage_medicines.medicinecost as "Medicine Cost"

							
						FROM  TreatmentPackage_medicines 
INNER JOIN Medicine _Medicine ON TreatmentPackage_medicines.medicinename=_Medicine.Medicineid

						WHERE TreatmentPackage.TreatmentPackageid =TreatmentPackage_medicines.TreatmentPackageid
) J)
						as automaton_TreatmentPackage_medicines
,
						(SELECT json_agg(J) FROM (SELECT   
						TreatmentPackage_refundpolicy.refundtype as "Refund Type"
,TreatmentPackage_refundpolicy.cancellationby as "Cancellation By"
,TreatmentPackage_refundpolicy.cancellationwindowdays as "Cancellation Window (Days)"
,TreatmentPackage_refundpolicy.refundpercentage as "Refund Percentage"

							
						FROM  TreatmentPackage_refundpolicy 

						WHERE TreatmentPackage.TreatmentPackageid =TreatmentPackage_refundpolicy.TreatmentPackageid
) J)
						as automaton_TreatmentPackage_refundpolicy

                 
				 
			  FROM  TreatmentPackage 
 LEFT OUTER JOIN tenant ON TreatmentPackage.tenantid=tenant.tenantid

			  WHERE CAST(TreatmentPackage.TreatmentPackageid AS Varchar)=pvar_TreatmentPackageid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

