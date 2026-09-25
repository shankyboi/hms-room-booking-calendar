 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PatientProfile"
			  (
				  pvar_PatientProfileid Varchar
			  )
			  RETURNS TABLE(
                registrationid Varchar
,firstname Varchar
,lastname Varchar
,gender Varchar
,dateofbirth date
,age int
,nationality Varchar
,countryoforigin uuid
,emailaddress Varchar
,mobilenumber Varchar
,whatsappnumber Varchar
,photo Varchar
,paddressline1 Varchar
,paddressline2 Varchar
,pzip int
,ptown Varchar
,pcityordistrict Varchar
,ppstatename Varchar
,sameaspermanentaddress Boolean
,caddressline1 Varchar
,caddressline2 Varchar
,czip int
,ctown Varchar
,ccityordistrict Varchar
,cstatename Varchar
,idprooftype Varchar
,idproofnumber Varchar
,uploadidproof Varchar
,languagesknown Varchar
,languagespreferrable Varchar
,otherlanguages Varchar
,maritalstatus Varchar
,education Varchar
,occupation uuid
,meditationpractice Varchar
,typeofpractice Varchar
,creativeactivities Varchar
,othercreativeactivities Varchar
,insurancetype Varchar
,insurancecompany Varchar
,policynumber Varchar
,policyclaimlimit decimal
,policyexpirydate date
,referralsource uuid
,referraltype Varchar
,referrername Varchar
,referrerphonenumber Varchar
,magazinename Varchar
,socialmediaplatform Varchar
,otherreferral Varchar
,blacklisted Varchar
,reasonforblacklisting uuid
,detailedremarks Varchar
,deceased Boolean
,causeofdeath Varchar
,dateandtimeofdeath Timestamp(3)
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,PatientProfileid uuid
                ,emergencycontactinfo JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57*/
               
              RETURN QUERY
			  SELECT 
				 PatientProfile.registrationid
,PatientProfile.firstname
,PatientProfile.lastname
,PatientProfile.gender
,PatientProfile.dateofbirth
,PatientProfile.age
,PatientProfile.nationality
,PatientProfile.countryoforigin
,PatientProfile.emailaddress
,PatientProfile.mobilenumber
,PatientProfile.whatsappnumber
,PatientProfile.photo
,PatientProfile.paddressline1
,PatientProfile.paddressline2
,PatientProfile.pzip
,PatientProfile.ptown
,PatientProfile.pcityordistrict
,PatientProfile.ppstatename
,COALESCE(PatientProfile.sameaspermanentaddress,true) as sameaspermanentaddress
,PatientProfile.caddressline1
,PatientProfile.caddressline2
,PatientProfile.czip
,PatientProfile.ctown
,PatientProfile.ccityordistrict
,PatientProfile.cstatename
,PatientProfile.idprooftype
,PatientProfile.idproofnumber
,PatientProfile.uploadidproof
,PatientProfile.languagesknown
,PatientProfile.languagespreferrable
,PatientProfile.otherlanguages
,PatientProfile.maritalstatus
,PatientProfile.education
,PatientProfile.occupation
,PatientProfile.meditationpractice
,PatientProfile.typeofpractice
,PatientProfile.creativeactivities
,PatientProfile.othercreativeactivities
,PatientProfile.insurancetype
,PatientProfile.insurancecompany
,PatientProfile.policynumber
,PatientProfile.policyclaimlimit
,PatientProfile.policyexpirydate
,PatientProfile.referralsource
,PatientProfile.referraltype
,PatientProfile.referrername
,PatientProfile.referrerphonenumber
,PatientProfile.magazinename
,PatientProfile.socialmediaplatform
,PatientProfile.otherreferral
,PatientProfile.blacklisted
,PatientProfile.reasonforblacklisting
,PatientProfile.detailedremarks
,COALESCE(PatientProfile.deceased,true) as deceased
,PatientProfile.causeofdeath
,PatientProfile.dateandtimeofdeath

				 ,PatientProfile.createduser,PatientProfile.createddate,PatientProfile.modifieduser,PatientProfile.modifieddate
				 ,PatientProfile.tenantid
                 ,PatientProfile.PatientProfileid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 PatientProfile_emergencycontactinfo.PatientProfileid
                                             ,PatientProfile_emergencycontactinfo.PatientProfile_emergencycontactinfoid   
											 ,PatientProfile_emergencycontactinfo.personname
,PatientProfile_emergencycontactinfo.relationship
,PatientProfile_emergencycontactinfo.phonenumber
 
											  
											 FROM PatientProfile_emergencycontactinfo
											 WHERE 
											 PatientProfile_emergencycontactinfo.PatientProfileid=PatientProfile.PatientProfileid
                                             
                                             ORDER BY record_order DESC
											) J) as emergencycontactinfo
   
			  FROM PatientProfile
			  WHERE CAST(PatientProfile.PatientProfileid AS Varchar)=pvar_PatientProfileid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

