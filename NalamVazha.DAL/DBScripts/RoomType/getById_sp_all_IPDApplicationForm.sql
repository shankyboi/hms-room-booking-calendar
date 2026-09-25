
CREATE OR REPLACE FUNCTION public."getById_sp_all_IPDApplicationForm"(
	pvar_ipdapplicationformid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "IPDApplicationFormid" uuid, bookingreferencenumber character varying, patientname character varying, firstname character varying, lastname character varying, gender character varying, mobilenumber character varying, whatsappnumber character varying, nationality character varying, countryoforigin character varying, generalcondition character varying, bookingstatus character varying, phase character varying, bookingstatusdate timestamp without time zone, groupbooking character varying, areyouthegroupleader character varying, numberofmember integer, groupleadersbookingreferencenumber character varying, paddressline1 character varying, paddressline2 character varying, ppincode integer, ptown character varying, pcityordistrict character varying, pstatename character varying, sameaspermanentaddress boolean, caddressline1 character varying, caddressline2 character varying, cpincode integer, ctown character varying, ccityordistrict character varying, cstatename character varying, flexiblewithdates character varying, flexiblewithroomtype character varying, joinwaitinglist character varying, passportnumber character varying, passportissuingcountry character varying, passportexpirydate character varying, uploadpassportcopy character varying, visatype character varying, visanumber character varying, visaissuedcountry character varying, visaissuedate character varying, visaexpirydate character varying, uploadvisacopy character varying, doyourequireahospitalprovidedattendant character varying, preferredduration character varying, admissionreason text, consentform character varying, consentfile character varying, agreefortermsandconditions boolean, signature text, packagename character varying, isbookingdepositmandatory boolean, verifiedstatus character varying, estimatedarrival character varying, travelarrangement character varying, typeoftravelrequired character varying, pickupfrom character varying, requiredparkingspace character varying, wheelchairassistance character varying, requireddinner character varying, specialrequest character varying, bookingtype character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, patient_emailaddress character varying, dateofarrival character varying, dateofdeparture character varying, daysofstay integer, accommodationtype character varying, "automaton_IPDApplicationForm_preferreddatesofadmission" json, "automaton_IPDApplicationForm_medicalinfo" json, "automaton_IPDApplicationForm_medicationinfo" json, "automaton_IPDApplicationForm_medicalrecords" json, "automaton_IPDApplicationForm_attendantinfo" json, "automaton_IPDApplicationForm_roompreference" json, "automaton_IPDApplicationForm_room" json, "automaton_IPDApplicationForm_preferreddatesofadmission_history" json, "automaton_IPDApplicationForm_medicalinfo_history" json, "automaton_IPDApplicationForm_medicationinfo_history" json, "automaton_IPDApplicationForm_medicalrecords_history" json, "automaton_IPDApplicationForm_attendantinfo_history" json, "automaton_IPDApplicationForm_roompreference_history" json, "automaton_IPDApplicationForm_room_history" json, automaton_review_logs json, automaton_review_logs_history json, authorized_users text, authorized_users_mobile text, verifieddate timestamp without time zone, reviewcomments character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:01*/
			  		 
            RETURN QUERY
            SELECT  
            IPDApplicationForm.tenantid
,tenant.businessname as _tenantname
,IPDApplicationForm.IPDApplicationFormid
,IPDApplicationForm.bookingreferencenumber
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''))), '') AS VARCHAR) as patientname
,IPDApplicationForm.firstname
,IPDApplicationForm.lastname
,IPDApplicationForm.gender
,IPDApplicationForm.mobilenumber
,IPDApplicationForm.whatsappnumber
,IPDApplicationForm.nationality
,CAST(__Country.countryname AS VARCHAR) as countryoforigin
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
,IPDApplicationForm.sameaspermanentaddress
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
,CAST(___Country.countryname AS VARCHAR) as passportissuingcountry
,CAST(COALESCE(to_char(IPDApplicationForm.passportexpirydate,'dd/MM/yyyy'),'') AS Varchar) as passportexpirydate
,IPDApplicationForm.uploadpassportcopy
,IPDApplicationForm.visatype
,IPDApplicationForm.visanumber
,CAST(____Country.countryname AS VARCHAR) as visaissuedcountry
,CAST(COALESCE(to_char(IPDApplicationForm.visaissuedate,'dd/MM/yyyy'),'') AS Varchar) as visaissuedate
,CAST(COALESCE(to_char(IPDApplicationForm.visaexpirydate,'dd/MM/yyyy'),'') AS Varchar) as visaexpirydate
,IPDApplicationForm.uploadvisacopy
,IPDApplicationForm.doyourequireahospitalprovidedattendant
,IPDApplicationForm.preferredduration
,IPDApplicationForm.admissionreason
,CAST(_____PatientConsent.consenttype||' '||_____PatientConsent.consentlanguage AS VARCHAR) as consentform
,IPDApplicationForm.consentfile
,IPDApplicationForm.agreefortermsandconditions
,IPDApplicationForm.signature
,CAST(______TreatmentPackage.packagename AS VARCHAR) as packagename
,IPDApplicationForm.isbookingdepositmandatory
,IPDApplicationForm.verifiedstatus
,CAST(COALESCE(to_char(IPDApplicationForm.estimatedarrival,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as estimatedarrival
,IPDApplicationForm.travelarrangement
,IPDApplicationForm.typeoftravelrequired
,IPDApplicationForm.pickupfrom
,IPDApplicationForm.requiredparkingspace
,IPDApplicationForm.wheelchairassistance
,IPDApplicationForm.requireddinner
,IPDApplicationForm.specialrequest
,IPDApplicationForm.bookingtype
            ,IPDApplicationForm.createduser,IPDApplicationForm.createddate,IPDApplicationForm.modifieduser,IPDApplicationForm.modifieddate
            ,COALESCE(_PatientProfile.emailaddress,'') as patient_emailaddress
            ,(SELECT CAST(COALESCE(to_char(pd.dateofarrival,'dd/MM/yyyy'),'') AS Varchar) FROM IPDApplicationForm_preferreddatesofadmission pd WHERE pd.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid AND COALESCE(pd.isdeleted,false)=false ORDER BY pd.record_order LIMIT 1) as dateofarrival
            ,(SELECT CAST(COALESCE(to_char(pd.dateofdeparture,'dd/MM/yyyy'),'') AS Varchar) FROM IPDApplicationForm_preferreddatesofadmission pd WHERE pd.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid AND COALESCE(pd.isdeleted,false)=false ORDER BY pd.record_order LIMIT 1) as dateofdeparture
            ,(SELECT pd.daysofstay FROM IPDApplicationForm_preferreddatesofadmission pd WHERE pd.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid AND COALESCE(pd.isdeleted,false)=false ORDER BY pd.record_order LIMIT 1) as daysofstay
            ,(SELECT CAST(_RT.name AS Varchar) FROM IPDApplicationForm_roompreference irp INNER JOIN RoomType _RT ON irp.roomtype=_RT.RoomTypeid WHERE irp.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid AND COALESCE(irp.isdeleted,false)=false ORDER BY irp.record_order LIMIT 1) as accommodationtype
            ,
						(SELECT json_agg(J) FROM (SELECT   
						to_char(IPDApplicationForm_preferreddatesofadmission.dateofarrival,'dd/MM/yyyy') as "Date of Arrival"
,to_char(IPDApplicationForm_preferreddatesofadmission.dateofdeparture,'dd/MM/yyyy') as "Date of Departure"
,IPDApplicationForm_preferreddatesofadmission.daysofstay as "Days of Stay"

							
						FROM  IPDApplicationForm_preferreddatesofadmission 

						WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_preferreddatesofadmission.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_preferreddatesofadmission.isdeleted,false) = false 
) J)
						as automaton_IPDApplicationForm_preferreddatesofadmission
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition Name"
,IPDApplicationForm_medicalinfo.duration as "Duration"
,IPDApplicationForm_medicalinfo.unit as "Unit"
,IPDApplicationForm_medicalinfo.severitylevel as "Severity Level"

							
						FROM  IPDApplicationForm_medicalinfo 
INNER JOIN MedicalCondition _MedicalCondition ON IPDApplicationForm_medicalinfo.medicalconditionname=_MedicalCondition.MedicalConditionid

						WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalinfo.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_medicalinfo.isdeleted,false) = false 
) J)
						as automaton_IPDApplicationForm_medicalinfo
,
						(SELECT json_agg(J) FROM (SELECT   
						IPDApplicationForm_medicationinfo.medicinename as "Medicine Name"
,IPDApplicationForm_medicationinfo.frequencyinaday as "Frequency in a day"
,IPDApplicationForm_medicationinfo.medicationduration as "Medication Duration"
,IPDApplicationForm_medicationinfo.quantity as "Quantity"

							
						FROM  IPDApplicationForm_medicationinfo 

						WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicationinfo.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_medicationinfo.isdeleted,false) = false 
) J)
						as automaton_IPDApplicationForm_medicationinfo
,
						(SELECT json_agg(J) FROM (SELECT   
						IPDApplicationForm_medicalrecords.medicalrecordname as "Medical Record Name"
,IPDApplicationForm_medicalrecords.medicalrecordfile as "Medical Record File"

							
						FROM  IPDApplicationForm_medicalrecords 

						WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalrecords.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_medicalrecords.isdeleted,false) = false 
) J)
						as automaton_IPDApplicationForm_medicalrecords
,
						(SELECT json_agg(J) FROM (SELECT   
						IPDApplicationForm_attendantinfo.attendantname as "Attendant Name"
,IPDApplicationForm_attendantinfo.age as "Age"
,IPDApplicationForm_attendantinfo.gender as "Gender"
,IPDApplicationForm_attendantinfo.phonenumber as "Phone Number"

							
						FROM  IPDApplicationForm_attendantinfo 

						WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_attendantinfo.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_attendantinfo.isdeleted,false) = false 
) J)
						as automaton_IPDApplicationForm_attendantinfo
,
						(
    SELECT json_agg(J ORDER BY J.record_order)
    FROM (
        SELECT
            IPDApplicationForm_roompreference.record_order,
            CAST(IPDApplicationForm_roompreference.roomtype AS VARCHAR) AS "RoomTypeid",
            CAST(_RoomType.name AS VARCHAR) AS "Room Type"
        FROM IPDApplicationForm_roompreference
        INNER JOIN RoomType _RoomType
            ON IPDApplicationForm_roompreference.roomtype = _RoomType.RoomTypeid
        WHERE IPDApplicationForm.IPDApplicationFormid = IPDApplicationForm_roompreference.IPDApplicationFormid
          AND COALESCE(IPDApplicationForm_roompreference.isdeleted, false) = false
        ORDER BY IPDApplicationForm_roompreference.record_order ASC
    ) J
) AS automaton_IPDApplicationForm_roompreference
,
						(SELECT json_agg(J) FROM (SELECT
						IPDApplicationForm_room.allottedto as "Allotted to"
,CAST(_Room.roomnumber AS VARCHAR) as "Room Number"
,CAST(_RoomType.name AS VARCHAR) as "Room Type"
,to_char(IPDApplicationForm_room.fromdate, 'dd/MM/yyyy') as "From Date"
,to_char(IPDApplicationForm_room.todate, 'dd/MM/yyyy') as "To Date"

						FROM  IPDApplicationForm_room
LEFT OUTER JOIN Room _Room ON IPDApplicationForm_room.roomnumber=_Room.Roomid
LEFT OUTER JOIN RoomType _RoomType ON _Room.roomtype=_RoomType.RoomTypeid

						WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_room.IPDApplicationFormid
AND COALESCE(IPDApplicationForm_room.isdeleted,false) = false
) J)
						as automaton_IPDApplicationForm_room
		
            ,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                to_char(IPDApplicationForm_preferreddatesofadmission.dateofarrival, 'dd/MM/yyyy') as "Date of Arrival"
,to_char(IPDApplicationForm_preferreddatesofadmission.dateofdeparture, 'dd/MM/yyyy') as "Date of Departure"
,IPDApplicationForm_preferreddatesofadmission.daysofstay as "Days of Stay"

                ,coalesce(IPDApplicationForm_preferreddatesofadmission.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_preferreddatesofadmission.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_preferreddatesofadmission.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_preferreddatesofadmission.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_preferreddatesofadmission 
INNER JOIN users ON users.usersid=IPDApplicationForm_preferreddatesofadmission.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_preferreddatesofadmission.IPDApplicationFormid

                UNION
                SELECT   
                to_char(IPDApplicationForm_preferreddatesofadmission_history.dateofarrival, 'dd/MM/yyyy') as "Date of Arrival"
,to_char(IPDApplicationForm_preferreddatesofadmission_history.dateofdeparture, 'dd/MM/yyyy') as "Date of Departure"
,IPDApplicationForm_preferreddatesofadmission_history.daysofstay as "Days of Stay"

                 ,coalesce(IPDApplicationForm_preferreddatesofadmission_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_preferreddatesofadmission_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_preferreddatesofadmission_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_preferreddatesofadmission_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_preferreddatesofadmission_history 
INNER JOIN users ON users.usersid=IPDApplicationForm_preferreddatesofadmission_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_preferreddatesofadmission_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_preferreddatesofadmission_audit
,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition Name"
,IPDApplicationForm_medicalinfo.duration as "Duration"
,IPDApplicationForm_medicalinfo.unit as "Unit"
,IPDApplicationForm_medicalinfo.severitylevel as "Severity Level"

                ,coalesce(IPDApplicationForm_medicalinfo.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_medicalinfo.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalinfo.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalinfo.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_medicalinfo 
INNER JOIN MedicalCondition _MedicalCondition ON IPDApplicationForm_medicalinfo.medicalconditionname=_MedicalCondition.MedicalConditionid
INNER JOIN users ON users.usersid=IPDApplicationForm_medicalinfo.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalinfo.IPDApplicationFormid

                UNION
                SELECT   
                CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition Name"
,IPDApplicationForm_medicalinfo_history.duration as "Duration"
,IPDApplicationForm_medicalinfo_history.unit as "Unit"
,IPDApplicationForm_medicalinfo_history.severitylevel as "Severity Level"

                 ,coalesce(IPDApplicationForm_medicalinfo_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_medicalinfo_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalinfo_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalinfo_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_medicalinfo_history 
INNER JOIN MedicalCondition _MedicalCondition ON IPDApplicationForm_medicalinfo_history.medicalconditionname=_MedicalCondition.MedicalConditionid
INNER JOIN users ON users.usersid=IPDApplicationForm_medicalinfo_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalinfo_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_medicalinfo_audit
,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                IPDApplicationForm_medicationinfo.medicinename as "Medicine Name"
,IPDApplicationForm_medicationinfo.frequencyinaday as "Frequency in a day"
,IPDApplicationForm_medicationinfo.medicationduration as "Medication Duration"
,IPDApplicationForm_medicationinfo.quantity as "Quantity"

                ,coalesce(IPDApplicationForm_medicationinfo.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_medicationinfo.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicationinfo.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicationinfo.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_medicationinfo 
INNER JOIN users ON users.usersid=IPDApplicationForm_medicationinfo.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicationinfo.IPDApplicationFormid

                UNION
                SELECT   
                IPDApplicationForm_medicationinfo_history.medicinename as "Medicine Name"
,IPDApplicationForm_medicationinfo_history.frequencyinaday as "Frequency in a day"
,IPDApplicationForm_medicationinfo_history.medicationduration as "Medication Duration"
,IPDApplicationForm_medicationinfo_history.quantity as "Quantity"

                 ,coalesce(IPDApplicationForm_medicationinfo_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_medicationinfo_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicationinfo_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicationinfo_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_medicationinfo_history 
INNER JOIN users ON users.usersid=IPDApplicationForm_medicationinfo_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicationinfo_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_medicationinfo_audit
,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                IPDApplicationForm_medicalrecords.medicalrecordname as "Medical Record Name"
,IPDApplicationForm_medicalrecords.medicalrecordfile as "Medical Record File"

                ,coalesce(IPDApplicationForm_medicalrecords.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_medicalrecords.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalrecords.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalrecords.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_medicalrecords 
INNER JOIN users ON users.usersid=IPDApplicationForm_medicalrecords.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalrecords.IPDApplicationFormid

                UNION
                SELECT   
                IPDApplicationForm_medicalrecords_history.medicalrecordname as "Medical Record Name"
,IPDApplicationForm_medicalrecords_history.medicalrecordfile as "Medical Record File"

                 ,coalesce(IPDApplicationForm_medicalrecords_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_medicalrecords_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalrecords_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_medicalrecords_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_medicalrecords_history 
INNER JOIN users ON users.usersid=IPDApplicationForm_medicalrecords_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_medicalrecords_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_medicalrecords_audit
,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                IPDApplicationForm_attendantinfo.attendantname as "Attendant Name"
,IPDApplicationForm_attendantinfo.age as "Age"
,IPDApplicationForm_attendantinfo.gender as "Gender"
,IPDApplicationForm_attendantinfo.phonenumber as "Phone Number"

                ,coalesce(IPDApplicationForm_attendantinfo.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_attendantinfo.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_attendantinfo.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_attendantinfo.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_attendantinfo 
INNER JOIN users ON users.usersid=IPDApplicationForm_attendantinfo.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_attendantinfo.IPDApplicationFormid

                UNION
                SELECT   
                IPDApplicationForm_attendantinfo_history.attendantname as "Attendant Name"
,IPDApplicationForm_attendantinfo_history.age as "Age"
,IPDApplicationForm_attendantinfo_history.gender as "Gender"
,IPDApplicationForm_attendantinfo_history.phonenumber as "Phone Number"

                 ,coalesce(IPDApplicationForm_attendantinfo_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_attendantinfo_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_attendantinfo_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_attendantinfo_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_attendantinfo_history 
INNER JOIN users ON users.usersid=IPDApplicationForm_attendantinfo_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_attendantinfo_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_attendantinfo_audit
,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                CAST(_RoomType.name AS VARCHAR) as "Room Type"

                ,coalesce(IPDApplicationForm_roompreference.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_roompreference.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_roompreference.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_roompreference.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_roompreference 
INNER JOIN RoomType _RoomType ON IPDApplicationForm_roompreference.roomtype=_RoomType.RoomTypeid
INNER JOIN users ON users.usersid=IPDApplicationForm_roompreference.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_roompreference.IPDApplicationFormid

                UNION
                SELECT   
                CAST(_RoomType.name AS VARCHAR) as "Room Type"

                 ,coalesce(IPDApplicationForm_roompreference_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_roompreference_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_roompreference_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_roompreference_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_roompreference_history 
INNER JOIN RoomType _RoomType ON IPDApplicationForm_roompreference_history.roomtype=_RoomType.RoomTypeid
INNER JOIN users ON users.usersid=IPDApplicationForm_roompreference_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_roompreference_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_roompreference_audit
,
                (
                SELECT json_agg(J) FROM (
                
                SELECT   
                IPDApplicationForm_room.allottedto as "Allotted to"
,CAST(_Room.roomnumber AS VARCHAR) as "Room Number"
,CAST(_RoomType.name AS VARCHAR) as "Room Type"
,to_char(IPDApplicationForm_room.fromdate, 'dd/MM/yyyy') as "From Date"
,to_char(IPDApplicationForm_room.todate, 'dd/MM/yyyy') as "To Date"

                ,coalesce(IPDApplicationForm_room.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_room.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_room.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_room.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_room 
LEFT OUTER JOIN Room _Room ON IPDApplicationForm_room.roomnumber=_Room.Roomid
LEFT OUTER JOIN RoomType _RoomType ON _Room.roomtype=_RoomType.RoomTypeid
INNER JOIN users ON users.usersid=IPDApplicationForm_room.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_room.IPDApplicationFormid

                UNION
                SELECT   
                IPDApplicationForm_room_history.allottedto as "Allotted to"
,CAST(_Room.roomnumber AS VARCHAR) as "Room Number"
,CAST(_RoomType.name AS VARCHAR) as "Room Type"
,to_char(IPDApplicationForm_room_history.fromdate, 'dd/MM/yyyy') as "From Date"
,to_char(IPDApplicationForm_room_history.todate, 'dd/MM/yyyy') as "To Date"

                 ,coalesce(IPDApplicationForm_room_history.action,'Added') as action
                 ,users.firstname as actionby
                 ,IPDApplicationForm_room_history.action_date as actiondatewithtime	
                ,CAST(COALESCE(to_char(IPDApplicationForm_room_history.action_date,'dd/MM/yyyy'),'') AS Varchar) as actiondate
                ,CAST(COALESCE(to_char(IPDApplicationForm_room_history.action_date,'HH24:MI'),'') AS Varchar) as actiontime	
                ,users.profilepicture
    
                FROM  IPDApplicationForm_room_history 
LEFT OUTER JOIN Room _Room ON IPDApplicationForm_room_history.roomnumber=_Room.Roomid
LEFT OUTER JOIN RoomType _RoomType ON _Room.roomtype=_RoomType.RoomTypeid
INNER JOIN users ON users.usersid=IPDApplicationForm_room_history.action_by

                WHERE IPDApplicationForm.IPDApplicationFormid =IPDApplicationForm_room_history.IPDApplicationFormid

                
                ) J)
			    as automaton_IPDApplicationForm_room_audit

 ,(SELECT json_agg(J) FROM (
                    SELECT  
                    CAST(COALESCE(to_char(reviewlogsIPDApplicationForm.createddate AT TIME ZONE 'UTC' AT TIME ZONE 'Asia/Kolkata','dd/MM/yyyy HH24:MI'),'') AS Varchar) as "Reviewed On"                    
                    ,CAST(COALESCE(NULLIF(btrim(concat_ws(' ', NULLIF(users.firstname, ''), NULLIF(users.lastname, ''))), ''), users.firstname) AS Varchar) as "Reviewed By"
                    ,COALESCE(reviewlogsIPDApplicationForm.logtype, 'Review') as "Log Type"
                    ,COALESCE(reviewlogsIPDApplicationForm.phase, '-') as "Phase"
                    ,COALESCE(reviewlogsIPDApplicationForm.previousbookingstatus, '-') as "Previous Status"
                    ,COALESCE(reviewlogsIPDApplicationForm.bookingstatus, reviewlogsIPDApplicationForm.verifiedstatus, '-') "Status"
                    ,COALESCE(reviewlogsIPDApplicationForm.previousphase, '-') as "Previous Phase"
                    ,COALESCE(reviewlogsIPDApplicationForm.reviewcomments,'-')  "Comments"
                    FROM reviewlogsIPDApplicationForm
                    INNER JOIN users
                    ON reviewlogsIPDApplicationForm.createduser=users.usersid
                    WHERE reviewlogsIPDApplicationForm.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
                    ORDER BY reviewlogsIPDApplicationForm.createddate ASC
                    ) J)
                    as automaton_review_logs
                    ,(SELECT json_agg(J) FROM (
                    SELECT  
                    COALESCE(INIPDApplicationForm.bookingreferencenumber,'-') as "Booking Reference Number"
                    ,CAST(COALESCE(to_char(reviewlogsIPDApplicationForm.createddate AT TIME ZONE 'UTC' AT TIME ZONE 'Asia/Kolkata','dd/MM/yyyy HH24:MI'),'') AS Varchar) as "Reviewed On"                    
                    ,CAST(COALESCE(NULLIF(btrim(concat_ws(' ', NULLIF(users.firstname, ''), NULLIF(users.lastname, ''))), ''), users.firstname) AS Varchar) as "Reviewed By"
                    ,COALESCE(reviewlogsIPDApplicationForm.logtype, 'Review') as "Log Type"
                    ,COALESCE(reviewlogsIPDApplicationForm.phase, '-') as "Phase"
                    ,COALESCE(reviewlogsIPDApplicationForm.previousbookingstatus, '-') as "Previous Status"
                    ,COALESCE(reviewlogsIPDApplicationForm.bookingstatus, reviewlogsIPDApplicationForm.verifiedstatus, '-') "Status"
                    ,COALESCE(reviewlogsIPDApplicationForm.previousphase, '-') as "Previous Phase"
                    ,COALESCE(reviewlogsIPDApplicationForm.reviewcomments,'-')  "Comments"
                    ,reviewlogsIPDApplicationForm.IPDApplicationFormid as "IPDApplicationFormid"
                    FROM reviewlogsIPDApplicationForm
                    INNER JOIN users
                    ON reviewlogsIPDApplicationForm.createduser=users.usersid

                    INNER JOIN IPDApplicationForm INIPDApplicationForm ON reviewlogsIPDApplicationForm.IPDApplicationFormid=INIPDApplicationForm.IPDApplicationFormid
                    WHERE reviewlogsIPDApplicationForm.IPDApplicationFormid=IPDApplicationForm.IPDApplicationFormid
 
                    ORDER BY reviewlogsIPDApplicationForm.createddate ASC
                    ) J)
                    as automaton_review_logs_history
			
            ,(SELECT STRING_AGG(u.emailid, ', ') AS authorized_users
            FROM users u
            JOIN RoleAuthorization ra ON (
            u.userrole = ANY(string_to_array(ra.viewactionroles, ',')) -- Split the authorizedroles column by commas
            )
            WHERE ra.actionname = 'CheckerView' and controllername='IPDApplicationForm') as authorized_users 
            ,(SELECT STRING_AGG(u.mobilenumber, ', ') AS authorized_users_mobile
            FROM users u
            JOIN RoleAuthorization ra ON (
            u.userrole = ANY(string_to_array(ra.viewactionroles, ',')) -- Split the authorizedroles column by commas
            AND u.isdeleted=false
            )
            WHERE ra.actionname = 'CheckerView' and controllername='IPDApplicationForm') as authorized_users_mobile 
            ,IPDApplicationForm.verifieddate
            ,IPDApplicationForm.reviewcomments
            FROM  IPDApplicationForm 
 LEFT OUTER JOIN tenant ON IPDApplicationForm.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON IPDApplicationForm.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN Country __Country ON IPDApplicationForm.countryoforigin=__Country.Countryid
LEFT OUTER JOIN Country ___Country ON IPDApplicationForm.passportissuingcountry=___Country.Countryid
LEFT OUTER JOIN Country ____Country ON IPDApplicationForm.visaissuedcountry=____Country.Countryid
INNER JOIN PatientConsent _____PatientConsent ON IPDApplicationForm.consentform=_____PatientConsent.PatientConsentid
LEFT OUTER JOIN TreatmentPackage ______TreatmentPackage ON IPDApplicationForm.packagename=______TreatmentPackage.TreatmentPackageid

            WHERE CAST(IPDApplicationForm.IPDApplicationFormid AS Varchar)=pvar_IPDApplicationFormid ;
			  
					 	
              END
              
$BODY$;


