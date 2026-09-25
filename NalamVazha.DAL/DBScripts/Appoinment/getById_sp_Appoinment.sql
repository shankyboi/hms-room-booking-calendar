 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_Appoinment"
			  (
				  pvar_Appoinmentid Varchar
			  )
			  RETURNS TABLE(
                patient uuid
,origin uuid
,bookingreferencenumber uuid
,doctor uuid
,appointmentdate uuid
,task uuid
,duration uuid
,status uuid
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,Appoinmentid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:47*/
               
              RETURN QUERY
			  SELECT 
				 Appoinment.patient
,Appoinment.origin
,Appoinment.bookingreferencenumber
,Appoinment.doctor
,Appoinment.appointmentdate
,Appoinment.task
,Appoinment.duration
,Appoinment.status

				 ,Appoinment.createduser,Appoinment.createddate,Appoinment.modifieduser,Appoinment.modifieddate
				 ,Appoinment.tenantid
                 ,Appoinment.Appoinmentid
                    
			  FROM Appoinment
			  WHERE CAST(Appoinment.Appoinmentid AS Varchar)=pvar_Appoinmentid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

