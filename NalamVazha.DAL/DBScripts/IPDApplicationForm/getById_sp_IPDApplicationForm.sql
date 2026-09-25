-- FUNCTION: public.getById_sp_IPDApplicationForm(character varying)

-- The return table now includes the four preference collections. PostgreSQL
-- requires the previous signature to be removed before changing OUT columns.
DROP FUNCTION IF EXISTS public."getById_sp_IPDApplicationForm"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_IPDApplicationForm"(
	pvar_ipdapplicationformid character varying)
    RETURNS TABLE(bookingreferencenumber character varying, patientname uuid, firstname character varying, lastname character varying, gender character varying, mobilenumber character varying, whatsappnumber character varying, nationality character varying, countryoforigin uuid, generalcondition character varying, bookingstatus character varying, phase character varying, bookingstatusdate timestamp without time zone, groupbooking character varying, areyouthegroupleader character varying, numberofmember integer, groupleadersbookingreferencenumber character varying, paddressline1 character varying, paddressline2 character varying, ppincode integer, ptown character varying, pcityordistrict character varying, pstatename character varying, sameaspermanentaddress boolean, caddressline1 character varying, caddressline2 character varying, cpincode integer, ctown character varying, ccityordistrict character varying, cstatename character varying, flexiblewithdates character varying, flexiblewithroomtype character varying, joinwaitinglist character varying, passportnumber character varying, passportissuingcountry uuid, passportexpirydate date, uploadpassportcopy character varying, visatype character varying, visanumber character varying, visaissuedcountry uuid, visaissuedate date, visaexpirydate date, uploadvisacopy character varying, doyourequireahospitalprovidedattendant character varying, preferredduration character varying, admissionreason text, consentform uuid, consentfile character varying, agreefortermsandconditions boolean, signature text, packagename uuid, isbookingdepositmandatory boolean, verifiedstatus character varying, estimatedarrival timestamp without time zone, travelarrangement character varying, typeoftravelrequired character varying, pickupfrom character varying, requiredparkingspace character varying, wheelchairassistance character varying, requireddinner character varying, specialrequest character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, ipdapplicationformid uuid, preferreddatesofadmission json, medicalinfo json, medicationinfo json, medicalrecords json, attendantinfo json, roompreference json, room json, patientpreferreddates json, attendantpreferreddates json, patientroompreference json, attendantroompreference json, bookingtype character varying, groupcode character varying)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 05:57:33*/
               
              RETURN QUERY
			  SELECT 
				 IPDApplicationForm.bookingreferencenumber
,IPDApplicationForm.patientname
,IPDApplicationForm.firstname
,IPDApplicationForm.lastname
,IPDApplicationForm.gender
,IPDApplicationForm.mobilenumber
,IPDApplicationForm.whatsappnumber
,IPDApplicationForm.nationality
,IPDApplicationForm.countryoforigin
,IPDApplicationForm.generalcondition
,IPDApplicationForm.bookingstatus
,IPDApplicationForm.phase
,IPDApplicationForm.bookingstatusdate
,IPDApplicationForm.groupbooking
,IPDApplicationForm.areyouthegroupleader
,IPDApplicationForm.numberofmember
,IPDApplicationForm.groupleadersbookingreferencenumber
,IPDApplicationForm.paddressline1
,IPDApplicationForm.paddressline2
,IPDApplicationForm.ppincode
,IPDApplicationForm.ptown
,IPDApplicationForm.pcityordistrict
,IPDApplicationForm.pstatename
,COALESCE(IPDApplicationForm.sameaspermanentaddress,true) as sameaspermanentaddress
,IPDApplicationForm.caddressline1
,IPDApplicationForm.caddressline2
,IPDApplicationForm.cpincode
,IPDApplicationForm.ctown
,IPDApplicationForm.ccityordistrict
,IPDApplicationForm.cstatename
,IPDApplicationForm.flexiblewithdates
,IPDApplicationForm.flexiblewithroomtype
,IPDApplicationForm.joinwaitinglist
,IPDApplicationForm.passportnumber
,IPDApplicationForm.passportissuingcountry
,IPDApplicationForm.passportexpirydate
,IPDApplicationForm.uploadpassportcopy
,IPDApplicationForm.visatype
,IPDApplicationForm.visanumber
,IPDApplicationForm.visaissuedcountry
,IPDApplicationForm.visaissuedate
,IPDApplicationForm.visaexpirydate
,IPDApplicationForm.uploadvisacopy
,IPDApplicationForm.doyourequireahospitalprovidedattendant
,IPDApplicationForm.preferredduration
,IPDApplicationForm.admissionreason
,IPDApplicationForm.consentform
,IPDApplicationForm.consentfile
,COALESCE(IPDApplicationForm.agreefortermsandconditions,true) as agreefortermsandconditions
,IPDApplicationForm.signature
,IPDApplicationForm.packagename
,COALESCE(IPDApplicationForm.isbookingdepositmandatory,true) as isbookingdepositmandatory
,IPDApplicationForm.verifiedstatus
,IPDApplicationForm.estimatedarrival
,IPDApplicationForm.travelarrangement
,IPDApplicationForm.typeoftravelrequired
,IPDApplicationForm.pickupfrom
,IPDApplicationForm.requiredparkingspace
,IPDApplicationForm.wheelchairassistance
,IPDApplicationForm.requireddinner
,IPDApplicationForm.specialrequest

				 ,IPDApplicationForm.createduser,IPDApplicationForm.createddate,IPDApplicationForm.modifieduser,IPDApplicationForm.modifieddate
				 ,IPDApplicationForm.tenantid
                 ,IPDApplicationForm.IPDApplicationFormid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 IPDApplicationForm_preferreddatesofadmission.IPDApplicationFormid
                                             ,IPDApplicationForm_preferreddatesofadmission.IPDApplicationForm_preferreddatesofadmissionid   
											 ,IPDApplicationForm_preferreddatesofadmission.dateofarrival
,IPDApplicationForm_preferreddatesofadmission.dateofdeparture
,IPDApplicationForm_preferreddatesofadmission.daysofstay
 
											  
											 FROM IPDApplicationForm_preferreddatesofadmission
											 WHERE 
											 IPDApplicationForm_preferreddatesofadmission.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_preferreddatesofadmission.isdeleted,false) = false 
                                             ORDER BY record_order DESC
											) J) as preferreddatesofadmission
,(SELECT json_agg(J) FROM (
											 SELECT 
											 IPDApplicationForm_medicalinfo.IPDApplicationFormid
                                             ,IPDApplicationForm_medicalinfo.IPDApplicationForm_medicalinfoid   
											 ,IPDApplicationForm_medicalinfo.medicalconditionname
,IPDApplicationForm_medicalinfo.duration
,IPDApplicationForm_medicalinfo.unit
,IPDApplicationForm_medicalinfo.severitylevel
 
											  
											 FROM IPDApplicationForm_medicalinfo
											 WHERE 
											 IPDApplicationForm_medicalinfo.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_medicalinfo.isdeleted,false) = false 
                                             ORDER BY record_order DESC
											) J) as medicalinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 IPDApplicationForm_medicationinfo.IPDApplicationFormid
                                             ,IPDApplicationForm_medicationinfo.IPDApplicationForm_medicationinfoid   
											 ,IPDApplicationForm_medicationinfo.medicinename
,IPDApplicationForm_medicationinfo.frequencyinaday
 
											  
											 FROM IPDApplicationForm_medicationinfo
											 WHERE 
											 IPDApplicationForm_medicationinfo.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_medicationinfo.isdeleted,false) = false 
                                             ORDER BY record_order DESC
											) J) as medicationinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 IPDApplicationForm_medicalrecords.IPDApplicationFormid
                                             ,IPDApplicationForm_medicalrecords.IPDApplicationForm_medicalrecordsid   
											 ,IPDApplicationForm_medicalrecords.medicalrecordname
,IPDApplicationForm_medicalrecords.medicalrecordfile
 
											  
											 FROM IPDApplicationForm_medicalrecords
											 WHERE 
											 IPDApplicationForm_medicalrecords.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_medicalrecords.isdeleted,false) = false 
                                             ORDER BY record_order DESC
											) J) as medicalrecords
,(SELECT json_agg(J) FROM (
											 SELECT 
											 IPDApplicationForm_attendantinfo.IPDApplicationFormid
                                             ,IPDApplicationForm_attendantinfo.IPDApplicationForm_attendantinfoid   
											 ,IPDApplicationForm_attendantinfo.attendantname
,IPDApplicationForm_attendantinfo.age
,IPDApplicationForm_attendantinfo.gender
,IPDApplicationForm_attendantinfo.phonenumber
 
											  
											 FROM IPDApplicationForm_attendantinfo
											 WHERE 
											 IPDApplicationForm_attendantinfo.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_attendantinfo.isdeleted,false) = false 
                                             ORDER BY record_order DESC
											) J) as attendantinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 IPDApplicationForm_roompreference.IPDApplicationFormid
                                             ,IPDApplicationForm_roompreference.IPDApplicationForm_roompreferenceid   
											 ,IPDApplicationForm_roompreference.roomtype
											 ,IPDApplicationForm_roompreference.cma_client_row_id
 ,IPDApplicationForm_roompreference.record_order
											  
											 FROM IPDApplicationForm_roompreference
											 WHERE 
											 IPDApplicationForm_roompreference.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_roompreference.isdeleted,false) = false 
                                             ORDER BY record_order ASC
											) J) as roompreference
,(SELECT json_agg(J) FROM (
											 SELECT
											 IPDApplicationForm_room.IPDApplicationFormid
                                             ,IPDApplicationForm_room.IPDApplicationForm_roomid
											 ,IPDApplicationForm_room.allottedto
,IPDApplicationForm_room.roomnumber
,IPDApplicationForm_room.fromdate
,IPDApplicationForm_room.todate
,Room.nextdaycheckin

											 FROM IPDApplicationForm_room
											 LEFT JOIN Room ON Room.Roomid = IPDApplicationForm_room.roomnumber
											 WHERE
											 IPDApplicationForm_room.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                                             AND COALESCE(IPDApplicationForm_room.isdeleted,false) = false
                                             ORDER BY record_order DESC
											) J) as room
,(SELECT json_agg(p ORDER BY p.record_order) FROM IPDApplicationForm_attendantpreferreddates p WHERE p.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid AND COALESCE(p.isdeleted,false)=false) AS attendantpreferreddates
,(SELECT json_agg(p ORDER BY p.record_order) FROM IPDApplicationForm_attendantroompreference p WHERE p.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid AND COALESCE(p.isdeleted,false)=false) AS attendantroompreference
,IPDApplicationForm.bookingtype
,IPDApplicationForm.groupcode 
   
			  FROM IPDApplicationForm
			  WHERE CAST(IPDApplicationForm.IPDApplicationFormid AS Varchar)=pvar_IPDApplicationFormid
                       ;

					 	
			  END
              
$BODY$;

GRANT EXECUTE ON FUNCTION public."getById_sp_IPDApplicationForm"(character varying) TO PUBLIC;

