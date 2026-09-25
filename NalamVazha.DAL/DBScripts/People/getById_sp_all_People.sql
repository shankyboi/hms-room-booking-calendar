
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_People"
              (
			  pvar_Peopleid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Peopleid" uuid
,practitionerid Varchar
,firstname Varchar
,lastname Varchar
,workprofile Varchar
,competencylevel Varchar
,clinicaltask Varchar
,designation Varchar
,contactnumber Varchar
,whatsappnumber Varchar
,emailid Varchar
,gender Varchar
,dob Varchar
,age int
,employmentstatus Varchar
,joiningdate Varchar
,contractrenewaldate Varchar
,photo Varchar
,nationality Varchar
,specifycountry Varchar
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
,validtill Varchar
,licenceupload Varchar
,issuingauthority Varchar
,bio text
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_People_emergencycontact" json,"automaton_People_educationinfo" json,"automaton_People_workexperience" json,"automaton_People_preferredlanguageinfo" json,"automaton_People_clinicaltaskinfo" json
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 11:34:27*/
			  		 
              RETURN QUERY
			  SELECT  
				 People.tenantid
,tenant.businessname as _tenantname
,People.Peopleid
,People.practitionerid
,People.firstname
,People.lastname
,CAST(_WorkProfile.workprofilename AS VARCHAR) as workprofile
,CAST(__Competency.competencyname AS VARCHAR) as competencylevel
,CAST(___ClinicalTask.workprofile||' '||___ClinicalTask.competency AS VARCHAR) as clinicaltask
,CAST(____Designation.designation AS VARCHAR) as designation
,People.contactnumber
,People.whatsappnumber
,People.emailid
,People.gender
,CAST(COALESCE(to_char(People.dob,'dd/MM/yyyy'),'') AS Varchar) as dob
,People.age
,People.employmentstatus
,CAST(COALESCE(to_char(People.joiningdate,'dd/MM/yyyy'),'') AS Varchar) as joiningdate
,CAST(COALESCE(to_char(People.contractrenewaldate,'dd/MM/yyyy'),'') AS Varchar) as contractrenewaldate
,People.photo
,People.nationality
,CAST(_____Country.countryname AS VARCHAR) as specifycountry
,People.idtype
,People.idnumber
,People.iddocument
,People.paddressline1
,People.paddressline2
,People.pzip
,People.ptown
,People.pcityordistrict
,People.pstatename
,People.sameaspermanentaddress
,People.caddressline1
,People.caddressline2
,People.czip
,People.ctown
,People.ccityordistrict
,People.cstatename
,People.registrationnumber
,CAST(COALESCE(to_char(People.validtill,'dd/MM/yyyy'),'') AS Varchar) as validtill
,People.licenceupload
,People.issuingauthority
,People.bio

				 ,People.createduser,People.createddate,People.modifieduser,People.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						People_emergencycontact.personname as "Person Name"
,People_emergencycontact.relationship as "Relationship"
,People_emergencycontact.phonenumber as "Phone Number"

							
						FROM  People_emergencycontact 

						WHERE People.Peopleid =People_emergencycontact.Peopleid
) J)
						as automaton_People_emergencycontact
,
						(SELECT json_agg(J) FROM (SELECT   
						People_educationinfo.fieldofstudy as "Field of Study"
,People_educationinfo.degree as "Degree"
,People_educationinfo.educationinstitution as "Education Institution"
,People_educationinfo.certificationnumber as "Certification Number"
,People_educationinfo.yearofgraduation as "Year of Graduation"
,People_educationinfo.degreestatus as "Degree Status"

							
						FROM  People_educationinfo 

						WHERE People.Peopleid =People_educationinfo.Peopleid
) J)
						as automaton_People_educationinfo
,
						(SELECT json_agg(J) FROM (SELECT   
						People_workexperience.designation as "Designation"
,People_workexperience.institutionname as "Institution Name"
,to_char(People_workexperience.fromdate, 'dd/MM/yyyy') as "From Date"
,to_char(People_workexperience.todate, 'dd/MM/yyyy') as "To Date"

							
						FROM  People_workexperience 

						WHERE People.Peopleid =People_workexperience.Peopleid
) J)
						as automaton_People_workexperience
,
						(SELECT json_agg(J) FROM (SELECT   
						People_preferredlanguageinfo.languagesknown as "Languages Known"
,People_preferredlanguageinfo.proficiency as "Proficiency"
,People_preferredlanguageinfo.ability as "Ability"

							
						FROM  People_preferredlanguageinfo 

						WHERE People.Peopleid =People_preferredlanguageinfo.Peopleid
) J)
						as automaton_People_preferredlanguageinfo
,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_WorkProfile.workprofilename AS VARCHAR) as "Work Profile"
,CAST(__TaskType.tasktypename AS VARCHAR) as "Task Type"
,CAST(___Task.taskname AS VARCHAR) as "Task Name"
,People_clinicaltaskinfo.durationinminutes as "Duration in Minutes"
,People_clinicaltaskinfo.overbookingcount as "Over Booking Count"
,People_clinicaltaskinfo.availableon as "Available on"
,People_clinicaltaskinfo.workhourstarts as "Work Hour Starts"
,People_clinicaltaskinfo.workhourends as "Work Hour Ends"
,People_clinicaltaskinfo.priority as "Priority"
,People_clinicaltaskinfo.feesamount as "Fees Amount"

							
						FROM  People_clinicaltaskinfo 
LEFT OUTER JOIN WorkProfile _WorkProfile ON People_clinicaltaskinfo.workprofile=_WorkProfile.WorkProfileid
LEFT OUTER JOIN TaskType __TaskType ON People_clinicaltaskinfo.tasktype=__TaskType.TaskTypeid
LEFT OUTER JOIN Task ___Task ON People_clinicaltaskinfo.taskname=___Task.Taskid

						WHERE People.Peopleid =People_clinicaltaskinfo.Peopleid
) J)
						as automaton_People_clinicaltaskinfo

                 
				 
			  FROM  People 
 LEFT OUTER JOIN tenant ON People.tenantid=tenant.tenantid
INNER JOIN WorkProfile _WorkProfile ON People.workprofile=_WorkProfile.WorkProfileid
INNER JOIN Competency __Competency ON People.competencylevel=__Competency.Competencyid
LEFT OUTER JOIN ClinicalTask ___ClinicalTask ON People.clinicaltask=___ClinicalTask.ClinicalTaskid
INNER JOIN Designation ____Designation ON People.designation=____Designation.Designationid
LEFT OUTER JOIN Country _____Country ON People.specifycountry=_____Country.Countryid

			  WHERE CAST(People.Peopleid AS Varchar)=pvar_Peopleid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

