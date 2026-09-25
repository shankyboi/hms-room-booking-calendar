 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Discharge"
			  (
				  pvar_Dischargeid Varchar
			  )
			  RETURNS TABLE(
                ipdnumber uuid
,patient uuid
,room uuid
,discharge uuid
,daysofstay int
,pendingamount uuid
,paymentstatus uuid
,refundamount uuid
,refundstatus uuid
,feedbackstatus Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Dischargeid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:43:40*/
               
              RETURN QUERY
			  SELECT 
				 Discharge.ipdnumber
,Discharge.patient
,Discharge.room
,Discharge.discharge
,Discharge.daysofstay
,Discharge.pendingamount
,Discharge.paymentstatus
,Discharge.refundamount
,Discharge.refundstatus
,Discharge.feedbackstatus

				 ,Discharge.createduser,Discharge.createddate,Discharge.modifieduser,Discharge.modifieddate
				 ,Discharge.tenantid
                 ,Discharge.Dischargeid
                    
			  FROM Discharge
			  WHERE CAST(Discharge.Dischargeid AS Varchar)=pvar_Dischargeid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

