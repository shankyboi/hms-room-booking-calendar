
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_PatientProfile"
              (
			  pvar_PatientProfileid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"PatientProfileid" uuid
,registrationid Varchar
,firstname Varchar
,lastname Varchar
,gender Varchar
,dateofbirth Varchar
,age int
,nationality Varchar
,countryoforigin Varchar
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
,occupation Varchar
,meditationpractice Varchar
,typeofpractice Varchar
,creativeactivities Varchar
,othercreativeactivities Varchar
,insurancetype Varchar
,insurancecompany Varchar
,policynumber Varchar
,policyclaimlimit decimal
,policyexpirydate Varchar
,referralsource Varchar
,referraltype Varchar
,referrername Varchar
,referrerphonenumber Varchar
,magazinename Varchar
,socialmediaplatform Varchar
,otherreferral Varchar
,blacklisted Varchar
,reasonforblacklisting Varchar
,detailedremarks Varchar
,deceased Boolean
,causeofdeath Varchar
,dateandtimeofdeath Varchar
,patientcategory uuid
,patientcategoryname Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_PatientProfile_emergencycontactinfo" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:57*/
			  		 
              RETURN QUERY
			  SELECT  
				 PatientProfile.tenantid
,tenant.businessname as _tenantname
,PatientProfile.PatientProfileid
,PatientProfile.registrationid
,PatientProfile.firstname
,PatientProfile.lastname
,PatientProfile.gender
,CAST(COALESCE(to_char(PatientProfile.dateofbirth,'dd/MM/yyyy'),'') AS Varchar) as dateofbirth
,PatientProfile.age
,PatientProfile.nationality
,CAST(_Country.countryname AS VARCHAR) as countryoforigin
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
,PatientProfile.sameaspermanentaddress
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
,CAST(__Occupation.occupationname AS VARCHAR) as occupation
,PatientProfile.meditationpractice
,PatientProfile.typeofpractice
,PatientProfile.creativeactivities
,PatientProfile.othercreativeactivities
,PatientProfile.insurancetype
,PatientProfile.insurancecompany
,PatientProfile.policynumber
,PatientProfile.policyclaimlimit
,CAST(COALESCE(to_char(PatientProfile.policyexpirydate,'dd/MM/yyyy'),'') AS Varchar) as policyexpirydate
,CAST(___ReferralSource.referralsourcename AS VARCHAR) as referralsource
,PatientProfile.referraltype
,PatientProfile.referrername
,PatientProfile.referrerphonenumber
,PatientProfile.magazinename
,PatientProfile.socialmediaplatform
,PatientProfile.otherreferral
,PatientProfile.blacklisted
,CAST(____Blacklistreason.reason AS VARCHAR) as reasonforblacklisting
,PatientProfile.detailedremarks
,PatientProfile.deceased
,PatientProfile.causeofdeath
,CAST(COALESCE(to_char(PatientProfile.dateandtimeofdeath,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as dateandtimeofdeath
,PatientProfile.PatientCategory as patientcategory
,COALESCE(_____PatientCategory.patientcategoryname,'Not Categorized') as patientcategoryname

				 ,PatientProfile.createduser,PatientProfile.createddate,PatientProfile.modifieduser,PatientProfile.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						PatientProfile_emergencycontactinfo.personname as "Person Name"
,PatientProfile_emergencycontactinfo.relationship as "Relationship"
,PatientProfile_emergencycontactinfo.phonenumber as "Phone Number"

							
						FROM  PatientProfile_emergencycontactinfo 

						WHERE PatientProfile.PatientProfileid =PatientProfile_emergencycontactinfo.PatientProfileid
) J)
						as automaton_PatientProfile_emergencycontactinfo

                 
				 
			  FROM  PatientProfile 
 LEFT OUTER JOIN tenant ON PatientProfile.tenantid=tenant.tenantid
LEFT OUTER JOIN Country _Country ON PatientProfile.countryoforigin=_Country.Countryid
LEFT OUTER JOIN Occupation __Occupation ON PatientProfile.occupation=__Occupation.Occupationid
LEFT OUTER JOIN ReferralSource ___ReferralSource ON PatientProfile.referralsource=___ReferralSource.ReferralSourceid
LEFT OUTER JOIN Blacklistreason ____Blacklistreason ON PatientProfile.reasonforblacklisting=____Blacklistreason.Blacklistreasonid
LEFT OUTER JOIN PatientCategory _____PatientCategory ON PatientProfile.PatientCategory=_____PatientCategory.PatientCategoryid

			  WHERE CAST(PatientProfile.PatientProfileid AS Varchar)=pvar_PatientProfileid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

