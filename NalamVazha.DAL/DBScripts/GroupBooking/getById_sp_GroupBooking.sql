 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_GroupBooking"
			  (
				  pvar_GroupBookingid Varchar
			  )
			  RETURNS TABLE(
                groupcode Varchar
,groupname Varchar
,countofmembers int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,GroupBookingid uuid
                ,patientinfo JSON,contacts JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/18/2026 05:34:25*/
               
              RETURN QUERY
			  SELECT 
				 GroupBooking.groupcode
,GroupBooking.groupname
,GroupBooking.countofmembers

				 ,GroupBooking.createduser,GroupBooking.createddate,GroupBooking.modifieduser,GroupBooking.modifieddate
				 ,GroupBooking.tenantid
                 ,GroupBooking.GroupBookingid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 GroupBooking_patientinfo.GroupBookingid
                                             ,GroupBooking_patientinfo.GroupBooking_patientinfoid   
											 ,GroupBooking_patientinfo.name
,GroupBooking_patientinfo.emailid
,GroupBooking_patientinfo.phonenumber
 
											  
											 FROM GroupBooking_patientinfo
											 WHERE 
											 GroupBooking_patientinfo.GroupBookingid=GroupBooking.GroupBookingid
                                             
                                             ORDER BY record_order DESC
											) J) as patientinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 GroupBooking_contacts.GroupBookingid
                                             ,GroupBooking_contacts.GroupBooking_contactsid   
											 ,GroupBooking_contacts.person
,GroupBooking_contacts.mobile
,GroupBooking_contacts.email
 
											  
											 FROM GroupBooking_contacts
											 WHERE 
											 GroupBooking_contacts.GroupBookingid=GroupBooking.GroupBookingid
                                             
                                             ORDER BY record_order DESC
											) J) as contacts
   
			  FROM GroupBooking
			  WHERE CAST(GroupBooking.GroupBookingid AS Varchar)=pvar_GroupBookingid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

