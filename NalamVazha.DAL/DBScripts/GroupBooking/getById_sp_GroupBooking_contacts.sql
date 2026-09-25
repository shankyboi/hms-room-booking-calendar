CREATE OR REPLACE FUNCTION "getById_sp_GroupBooking_contacts"(
												 pvar_GroupBookingid Varchar(50)
											 )
                                             RETURNS TABLE("GroupBookingid" uuid,"GroupBooking_contactsid" uuid ,person Varchar
,mobile Varchar
,email Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 GroupBooking_contacts.GroupBookingid
                                             ,GroupBooking_contacts.GroupBooking_contactsid   
											 ,GroupBooking_contacts.person
,GroupBooking_contacts.mobile
,GroupBooking_contacts.email
 
											 
											 FROM GroupBooking_contacts
											 WHERE 
											 CAST(GroupBooking_contacts.GroupBookingid AS VARCHAR)=pvar_GroupBookingid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

