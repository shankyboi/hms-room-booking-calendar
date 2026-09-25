 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_EnquiryForm"
			  (
				  pvar_EnquiryFormid Varchar
			  )
			  RETURNS TABLE(
                enquirynumber Varchar
,enquirydate date
,enquirytype uuid
,isroombookingrelated Varchar
,patientname uuid
,firstname Varchar
,lastname Varchar
,gender Varchar
,age Bigint
,phonenumber Varchar
,emailaddress Varchar
,preferredcontactmethod Varchar
,enquiryreason text
,enquiredvia Varchar
,preferredroomtype uuid
,preferreddateofarrival date
,preferreddateofdeparture date
,joinwaitinglist Boolean
,enquirystatus Varchar
,verifiedstatus Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,EnquiryFormid uuid
                ,medicalinfo JSON
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/
               
              RETURN QUERY
			  SELECT 
				 EnquiryForm.enquirynumber
,EnquiryForm.enquirydate
,EnquiryForm.enquirytype
,EnquiryForm.isroombookingrelated
,EnquiryForm.patientname
,EnquiryForm.firstname
,EnquiryForm.lastname
,EnquiryForm.gender
,EnquiryForm.age
,EnquiryForm.phonenumber
,EnquiryForm.emailaddress
,EnquiryForm.preferredcontactmethod
,EnquiryForm.enquiryreason
,EnquiryForm.enquiredvia
,EnquiryForm.preferredroomtype
,EnquiryForm.preferreddateofarrival
,EnquiryForm.preferreddateofdeparture
,COALESCE(EnquiryForm.joinwaitinglist,true) as joinwaitinglist
,EnquiryForm.enquirystatus
,EnquiryForm.verifiedstatus

				 ,EnquiryForm.createduser,EnquiryForm.createddate,EnquiryForm.modifieduser,EnquiryForm.modifieddate
				 ,EnquiryForm.tenantid
                 ,EnquiryForm.EnquiryFormid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 EnquiryForm_medicalinfo.EnquiryFormid
                                             ,EnquiryForm_medicalinfo.EnquiryForm_medicalinfoid   
											 ,EnquiryForm_medicalinfo.medicalcondition
,EnquiryForm_medicalinfo.conditionname
,EnquiryForm_medicalinfo.duration
,EnquiryForm_medicalinfo.severity
 
											  
											 FROM EnquiryForm_medicalinfo
											 WHERE 
											 EnquiryForm_medicalinfo.EnquiryFormid=EnquiryForm.EnquiryFormid
                                             
                                             ORDER BY record_order DESC
											) J) as medicalinfo
   
			  FROM EnquiryForm
			  WHERE CAST(EnquiryForm.EnquiryFormid AS Varchar)=pvar_EnquiryFormid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

