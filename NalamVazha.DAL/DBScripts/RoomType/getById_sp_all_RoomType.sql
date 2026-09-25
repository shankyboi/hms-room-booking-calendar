
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_RoomType"
              (
			  pvar_RoomTypeid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"RoomTypeid" uuid
,name Varchar
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
,hourlychargesapplicable Varchar
,chargeperhour decimal
,billingwaiverfordelayedstart Varchar
,waiverpercentage decimal
,roomtransfercost Varchar
,description text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_RoomType_refundpolicy" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:17*/
			  		 
              RETURN QUERY
			  SELECT  
				 RoomType.tenantid
,tenant.businessname as _tenantname
,RoomType.RoomTypeid
,RoomType.name
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
,CAST(case when RoomType.hourlychargesapplicable=true then 'Yes' else 'No' End AS Varchar) as hourlychargesapplicable
,RoomType.chargeperhour
,RoomType.billingwaiverfordelayedstart
,RoomType.waiverpercentage
,RoomType.roomtransfercost
,RoomType.description

				 ,RoomType.createduser,RoomType.createddate,RoomType.modifieduser,RoomType.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						RoomType_refundpolicy.refundtype as "Refund Type"
,RoomType_refundpolicy.cancellationby as "Cancellation By"
,RoomType_refundpolicy.cancellationwindowdays as "Cancellation Window (Days)"
,RoomType_refundpolicy.refundpercentage as "Refund Percentage"

							
						FROM  RoomType_refundpolicy 

						WHERE RoomType.RoomTypeid =RoomType_refundpolicy.RoomTypeid
) J)
						as automaton_RoomType_refundpolicy

                 
				 
			  FROM  RoomType 
 LEFT OUTER JOIN tenant ON RoomType.tenantid=tenant.tenantid

			  WHERE CAST(RoomType.RoomTypeid AS Varchar)=pvar_RoomTypeid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

