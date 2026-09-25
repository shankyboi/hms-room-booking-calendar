
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_EnquiryForm"
              (
			  pvar_EnquiryFormid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"EnquiryFormid" uuid
,enquirynumber Varchar
,enquirydate Varchar
,enquirytype Varchar
,isroombookingrelated Varchar
,patientname Varchar
,firstname Varchar
,lastname Varchar
,gender Varchar
,age Bigint
,phonenumber Varchar
,emailaddress Varchar
,preferredcontactmethod Varchar
,enquiryreason text
,enquiredvia Varchar
,preferredroomtype Varchar
,preferreddateofarrival Varchar
,preferreddateofdeparture Varchar
,joinwaitinglist Varchar
,enquirystatus Varchar
,verifiedstatus Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"automaton_EnquiryForm_medicalinfo" json
                
				,automaton_review_logs json
                ,automaton_review_logs_history json
				
			
                ,authorized_users text
                ,authorized_users_mobile text
                 ,verifieddate Timestamp(3)
                ,reviewcomments Varchar(1024)
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
			  		 
            RETURN QUERY
            SELECT  
            EnquiryForm.tenantid
,tenant.businessname as _tenantname
,EnquiryForm.EnquiryFormid
,EnquiryForm.enquirynumber
,CAST(COALESCE(to_char(EnquiryForm.enquirydate,'dd/MM/yyyy'),'') AS Varchar) as enquirydate
,CAST(_EnquiryType.enquiryname AS VARCHAR) as enquirytype
,EnquiryForm.isroombookingrelated
,CAST(__PatientProfile.firstname||' '||__PatientProfile.lastname||' '||__PatientProfile.mobilenumber AS VARCHAR) as patientname
,EnquiryForm.firstname
,EnquiryForm.lastname
,EnquiryForm.gender
,EnquiryForm.age
,EnquiryForm.phonenumber
,EnquiryForm.emailaddress
,EnquiryForm.preferredcontactmethod
,EnquiryForm.enquiryreason
,EnquiryForm.enquiredvia
,CAST(___RoomType.name AS VARCHAR) as preferredroomtype
,CAST(COALESCE(to_char(EnquiryForm.preferreddateofarrival,'dd/MM/yyyy'),'') AS Varchar) as preferreddateofarrival
,CAST(COALESCE(to_char(EnquiryForm.preferreddateofdeparture,'dd/MM/yyyy'),'') AS Varchar) as preferreddateofdeparture
,CAST(case when EnquiryForm.joinwaitinglist=true then 'Yes' else 'No' End AS Varchar) as joinwaitinglist
,EnquiryForm.enquirystatus
,EnquiryForm.verifiedstatus

            ,EnquiryForm.createduser,EnquiryForm.createddate,EnquiryForm.modifieduser,EnquiryForm.modifieddate
            ,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(_MedicalCondition.conditionname AS VARCHAR) as "Medical Condition"
,EnquiryForm_medicalinfo.conditionname as "Condition Name"
,EnquiryForm_medicalinfo.duration as "Duration"
,EnquiryForm_medicalinfo.severity as "Severity"

							
						FROM  EnquiryForm_medicalinfo 
LEFT OUTER JOIN MedicalCondition _MedicalCondition ON EnquiryForm_medicalinfo.medicalcondition=_MedicalCondition.MedicalConditionid

						WHERE EnquiryForm.EnquiryFormid =EnquiryForm_medicalinfo.EnquiryFormid
) J)
						as automaton_EnquiryForm_medicalinfo
		
            
 ,(SELECT json_agg(J) FROM (
                    SELECT  
                    CAST(COALESCE(to_char(reviewlogsEnquiryForm.createddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as "Reviewed On"
                    ,users.firstname as "Reviewed By"
                    ,reviewlogsEnquiryForm.verifiedstatus "Status" 
                    ,COALESCE(reviewlogsEnquiryForm.reviewcomments,'-')  "Comments"
                    FROM reviewlogsEnquiryForm
                    INNER JOIN users
                    ON reviewlogsEnquiryForm.createduser=users.usersid
                    WHERE reviewlogsEnquiryForm.EnquiryFormid=EnquiryForm.EnquiryFormid
                    ORDER BY EnquiryForm.enquirynumber DESC
                    ) J)
                    as automaton_review_logs
                    ,(SELECT json_agg(J) FROM (
                    SELECT  
                    COALESCE(INEnquiryForm.enquirynumber,'-') as "Enquiry Number"
                    ,CAST(COALESCE(to_char(reviewlogsEnquiryForm.createddate,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as "Reviewed On"
                    ,users.firstname as "Reviewed By"
                    ,reviewlogsEnquiryForm.verifiedstatus "Status" 
                    ,COALESCE(reviewlogsEnquiryForm.reviewcomments,'-')  "Comments"
                    ,reviewlogsEnquiryForm.EnquiryFormid as "EnquiryFormid"
                    FROM reviewlogsEnquiryForm
                    INNER JOIN users
                    ON reviewlogsEnquiryForm.createduser=users.usersid

                    INNER JOIN EnquiryForm INEnquiryForm ON reviewlogsEnquiryForm.EnquiryFormid=INEnquiryForm.EnquiryFormid
                    WHERE  INEnquiryForm.enquirynumber
                    LIKE LEFT(EnquiryForm.enquirynumber, POSITION('/' IN EnquiryForm.enquirynumber || '/') - 1) ||'%'
 
                    ORDER BY INEnquiryForm.enquirynumber DESC
                    ) J)
                    as automaton_review_logs_history
			
            ,(SELECT STRING_AGG(u.emailid, ', ') AS authorized_users
            FROM users u
            JOIN RoleAuthorization ra ON (
            u.userrole = ANY(string_to_array(ra.viewactionroles, ',')) -- Split the authorizedroles column by commas
            )
            WHERE ra.actionname = 'CheckerView' and controllername='EnquiryForm') as authorized_users 
            ,(SELECT STRING_AGG(u.mobilenumber, ', ') AS authorized_users_mobile
            FROM users u
            JOIN RoleAuthorization ra ON (
            u.userrole = ANY(string_to_array(ra.viewactionroles, ',')) -- Split the authorizedroles column by commas
            AND u.isdeleted=false
            )
            WHERE ra.actionname = 'CheckerView' and controllername='EnquiryForm') as authorized_users_mobile 
            ,EnquiryForm.verifieddate
            ,EnquiryForm.reviewcomments
            FROM  EnquiryForm 
 LEFT OUTER JOIN tenant ON EnquiryForm.tenantid=tenant.tenantid
INNER JOIN EnquiryType _EnquiryType ON EnquiryForm.enquirytype=_EnquiryType.EnquiryTypeid
LEFT OUTER JOIN PatientProfile __PatientProfile ON EnquiryForm.patientname=__PatientProfile.PatientProfileid
LEFT OUTER JOIN RoomType ___RoomType ON EnquiryForm.preferredroomtype=___RoomType.RoomTypeid

            WHERE CAST(EnquiryForm.EnquiryFormid AS Varchar)=pvar_EnquiryFormid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

