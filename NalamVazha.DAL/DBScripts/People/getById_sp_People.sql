 
			  
			  CREATE OR REPLACE  FUNCTION  "getById_sp_People"
			  (
				  pvar_Peopleid Varchar
			  )
			  RETURNS TABLE(
                practitionerid Varchar
,firstname Varchar
,lastname Varchar
,workprofile uuid
,competencylevel uuid
--,clinicaltask uuid
,designation uuid
,contactnumber Varchar
,whatsappnumber Varchar
,emailid Varchar
,gender Varchar
,dob date
,age int
,employmentstatus Varchar
,joiningdate date
,contractrenewaldate date
,photo Varchar
,nationality Varchar
,specifycountry uuid
,idtype Varchar
,idnumber Varchar
,iddocument Varchar
,paddressline1 Varchar
,paddressline2 Varchar
,pzip int
,ptown Varchar
,pcityordistrict Varchar
,pstatename Varchar
,sameaspermanentaddress Boolean
,caddressline1 Varchar
,caddressline2 Varchar
,czip int
,ctown Varchar
,ccityordistrict Varchar
,cstatename Varchar
,registrationnumber Varchar
,validtill date
,licenceupload Varchar
,issuingauthority Varchar
,bio text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid
,status varchar

                ,Peopleid uuid
                ,emergencycontact JSON,educationinfo JSON,workexperience JSON,preferredlanguageinfo JSON,clinicaltaskinfo JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
               
              RETURN QUERY
			  SELECT 
				 People.practitionerid
,People.firstname
,People.lastname
,People.workprofile
,People.competencylevel
--,People.clinicaltask
,People.designation
,People.contactnumber
,People.whatsappnumber
,People.emailid
,People.gender
,People.dob
,People.age
,People.employmentstatus
,People.joiningdate
,People.contractrenewaldate
,People.photo
,People.nationality
,People.specifycountry
,People.idtype
,People.idnumber
,People.iddocument
,People.paddressline1
,People.paddressline2
,People.pzip
,People.ptown
,People.pcityordistrict
,People.pstatename
,COALESCE(People.sameaspermanentaddress,true) as sameaspermanentaddress
,People.caddressline1
,People.caddressline2
,People.czip
,People.ctown
,People.ccityordistrict
,People.cstatename
,People.registrationnumber
,People.validtill
,People.licenceupload
,People.issuingauthority
,People.bio

				 ,People.createduser,People.createddate,People.modifieduser,People.modifieddate
				 ,People.tenantid
				 ,People.status
                 ,People.Peopleid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 People_emergencycontact.Peopleid
                                             ,People_emergencycontact.People_emergencycontactid   
											 ,People_emergencycontact.personname
,People_emergencycontact.relationship
,People_emergencycontact.phonenumber
 
											  
											 FROM People_emergencycontact
											 WHERE 
											 People_emergencycontact.Peopleid=People.Peopleid
                                             
                                             ORDER BY record_order DESC
											) J) as emergencycontact
,(SELECT json_agg(J) FROM (
											 SELECT 
											 People_educationinfo.Peopleid
                                             ,People_educationinfo.People_educationinfoid   
											 ,People_educationinfo.fieldofstudy
,People_educationinfo.degree
,People_educationinfo.educationinstitution
,People_educationinfo.certificationnumber
,People_educationinfo.yearofgraduation
,People_educationinfo.degreestatus
 
											  
											 FROM People_educationinfo
											 WHERE 
											 People_educationinfo.Peopleid=People.Peopleid
                                             
                                             ORDER BY record_order DESC
											) J) as educationinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 People_workexperience.Peopleid
                                             ,People_workexperience.People_workexperienceid   
											 ,People_workexperience.designation
,People_workexperience.institutionname
,People_workexperience.fromdate
,People_workexperience.todate
 
											  
											 FROM People_workexperience
											 WHERE 
											 People_workexperience.Peopleid=People.Peopleid
                                             
                                             ORDER BY record_order DESC
											) J) as workexperience
,(SELECT json_agg(J) FROM (
											 SELECT 
											 People_preferredlanguageinfo.Peopleid
                                             ,People_preferredlanguageinfo.People_preferredlanguageinfoid   
											 ,People_preferredlanguageinfo.languagesknown
,People_preferredlanguageinfo.proficiency
,People_preferredlanguageinfo.ability
 
											  
											 FROM People_preferredlanguageinfo
											 WHERE 
											 People_preferredlanguageinfo.Peopleid=People.Peopleid
                                             
                                             ORDER BY record_order DESC
											) J) as preferredlanguageinfo
,(SELECT json_agg(J) FROM (
											 SELECT 
											 People_clinicaltaskinfo.Peopleid
                                             ,People_clinicaltaskinfo.People_clinicaltaskinfoid   
											 ,People_clinicaltaskinfo.consultations
,People_clinicaltaskinfo.workprofile
,People_clinicaltaskinfo.tasktype
,People_clinicaltaskinfo.taskname
,People_clinicaltaskinfo.durationinminutes
,People_clinicaltaskinfo.overbookingcount
,People_clinicaltaskinfo.availableon
,People_clinicaltaskinfo.workhourstarts
,People_clinicaltaskinfo.workhourends
,People_clinicaltaskinfo.priority
,People_clinicaltaskinfo.feesamount
 
											  
											 FROM People_clinicaltaskinfo
											 WHERE 
											 People_clinicaltaskinfo.Peopleid=People.Peopleid
                                             
                                             ORDER BY record_order DESC
											) J) as clinicaltaskinfo
   
			  FROM People
			  WHERE CAST(People.Peopleid AS Varchar)=pvar_Peopleid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

