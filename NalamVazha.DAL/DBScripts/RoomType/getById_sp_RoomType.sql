 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_RoomType"
			  (
				  pvar_RoomTypeid Varchar
			  )
			  RETURNS TABLE(
                name Varchar
,prebookingdaylimit int
,minbookingdays int
,maxbookingdays int
,concessoneligibility Varchar
,suitabilityforvip Varchar
,gendersuitability Varchar
,roomtypeicon Varchar
,deposittype Varchar
,costperday decimal
,advanceperday decimal
,bookingdeposit decimal
,variableofbookingdays decimal
,attendantcostperday decimal
,attendantadvanceperday decimal
,attendantbookingdeposit decimal
,attendantvariableofbookingdays decimal
,hourlychargesapplicable Boolean
,chargeperhour decimal
,billingwaiverfordelayedstart Varchar
,waiverpercentage decimal
,roomtransfercost Varchar
,description text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,RoomTypeid uuid
                ,refundpolicy JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:17*/
               
              RETURN QUERY
			  SELECT 
				 RoomType.name
,RoomType.prebookingdaylimit
,RoomType.minbookingdays
,RoomType.maxbookingdays
,RoomType.concessoneligibility
,RoomType.suitabilityforvip
,RoomType.gendersuitability
,RoomType.roomtypeicon
,RoomType.deposittype
,RoomType.costperday
,RoomType.advanceperday
,RoomType.bookingdeposit
,RoomType.variableofbookingdays
,RoomType.attendantcostperday
,RoomType.attendantadvanceperday
,RoomType.attendantbookingdeposit
,RoomType.attendantvariableofbookingdays
,COALESCE(RoomType.hourlychargesapplicable,true) as hourlychargesapplicable
,RoomType.chargeperhour
,RoomType.billingwaiverfordelayedstart
,RoomType.waiverpercentage
,RoomType.roomtransfercost
,RoomType.description

				 ,RoomType.createduser,RoomType.createddate,RoomType.modifieduser,RoomType.modifieddate
				 ,RoomType.tenantid
                 ,RoomType.RoomTypeid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 RoomType_refundpolicy.RoomTypeid
                                             ,RoomType_refundpolicy.RoomType_refundpolicyid   
											 ,RoomType_refundpolicy.refundtype
,RoomType_refundpolicy.cancellationby
,RoomType_refundpolicy.cancellationwindowdays
,RoomType_refundpolicy.refundpercentage
 
											  
											 FROM RoomType_refundpolicy
											 WHERE 
											 RoomType_refundpolicy.RoomTypeid=RoomType.RoomTypeid
                                             
                                             ORDER BY record_order DESC
											) J) as refundpolicy
   
			  FROM RoomType
			  WHERE CAST(RoomType.RoomTypeid AS Varchar)=pvar_RoomTypeid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

