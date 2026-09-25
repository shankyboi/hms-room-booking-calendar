
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_GroupBooking"
              (
			  pvar_GroupBookingid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"GroupBookingid" uuid
,groupcode Varchar
,groupname Varchar
,countofmembers int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_GroupBooking_patientinfo" json,"automaton_GroupBooking_contacts" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/18/2026 05:34:25*/
			  		 
              RETURN QUERY
			  SELECT  
				 GroupBooking.tenantid
,tenant.businessname as _tenantname
,GroupBooking.GroupBookingid
,GroupBooking.groupcode
,GroupBooking.groupname
,GroupBooking.countofmembers

				 ,GroupBooking.createduser,GroupBooking.createddate,GroupBooking.modifieduser,GroupBooking.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						GroupBooking_patientinfo.name as "Name"
,GroupBooking_patientinfo.emailid as "Email ID"
,GroupBooking_patientinfo.phonenumber as "Phone Number"

							
						FROM  GroupBooking_patientinfo 

						WHERE GroupBooking.GroupBookingid =GroupBooking_patientinfo.GroupBookingid
) J)
						as automaton_GroupBooking_patientinfo
,
						(SELECT json_agg(J) FROM (SELECT   
						GroupBooking_contacts.person as "Person"
,GroupBooking_contacts.mobile as "Mobile"
,GroupBooking_contacts.email as "Email"

							
						FROM  GroupBooking_contacts 

						WHERE GroupBooking.GroupBookingid =GroupBooking_contacts.GroupBookingid
) J)
						as automaton_GroupBooking_contacts

                 
				 
			  FROM  GroupBooking 
 LEFT OUTER JOIN tenant ON GroupBooking.tenantid=tenant.tenantid

			  WHERE CAST(GroupBooking.GroupBookingid AS Varchar)=pvar_GroupBookingid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

