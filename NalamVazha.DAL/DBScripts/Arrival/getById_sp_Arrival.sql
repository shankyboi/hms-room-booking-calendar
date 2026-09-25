 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Arrival"
			  (
				  pvar_Arrivalid Varchar
			  )
			  RETURNS TABLE(
                ipdnumber uuid
,patient uuid
,room uuid
,estimatedarrival uuid
,bookingstatus uuid
,travelarrangement uuid
,pickupfrom uuid
,wheelchairassistance uuid
,requireddinner uuid
,specialrequest uuid
,paymentstatus uuid
,pendingamount uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Arrivalid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 08:07:23*/
               
              RETURN QUERY
			  SELECT 
				 Arrival.ipdnumber
,Arrival.patient
,Arrival.room
,Arrival.estimatedarrival
,Arrival.bookingstatus
,Arrival.travelarrangement
,Arrival.pickupfrom
,Arrival.wheelchairassistance
,Arrival.requireddinner
,Arrival.specialrequest
,Arrival.paymentstatus
,Arrival.pendingamount

				 ,Arrival.createduser,Arrival.createddate,Arrival.modifieduser,Arrival.modifieddate
				 ,Arrival.tenantid
                 ,Arrival.Arrivalid
                    
			  FROM Arrival
			  WHERE CAST(Arrival.Arrivalid AS Varchar)=pvar_Arrivalid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

