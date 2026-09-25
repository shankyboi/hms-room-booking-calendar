
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_PatientVisit"
              (
			  pvar_PatientVisitid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"PatientVisitid" uuid
,visitnumber Varchar
,visitdatetime Varchar
,patientname Varchar
,visittype Varchar
,ipdnumber Varchar
,opdnumber Varchar
,consultingdoctor Varchar
,visitstatus Varchar
,notes Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:59*/
			  		 
              RETURN QUERY
			  SELECT  
				 PatientVisit.tenantid
,tenant.businessname as _tenantname
,PatientVisit.PatientVisitid
,PatientVisit.visitnumber
,CAST(COALESCE(to_char(PatientVisit.visitdatetime,'dd/MM/yyyy HH24:MI'),'') AS Varchar) as visitdatetime
,CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber AS VARCHAR) as patientname
,PatientVisit.visittype
,CAST(__IPDApplicationForm.firstname||' '||__IPDApplicationForm.lastname AS VARCHAR) as ipdnumber
,CAST(___OPDForm.patientname AS VARCHAR) as opdnumber
,CAST(____People.firstname||' '||____People.lastname AS VARCHAR) as consultingdoctor
,PatientVisit.visitstatus
,PatientVisit.notes

				 ,PatientVisit.createduser,PatientVisit.createddate,PatientVisit.modifieduser,PatientVisit.modifieddate
                 
                 
				 
			  FROM  PatientVisit 
 LEFT OUTER JOIN tenant ON PatientVisit.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON PatientVisit.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN IPDApplicationForm __IPDApplicationForm ON PatientVisit.ipdnumber=__IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm ___OPDForm ON PatientVisit.opdnumber=___OPDForm.OPDFormid
LEFT OUTER JOIN People ____People ON PatientVisit.consultingdoctor=____People.Peopleid

			  WHERE CAST(PatientVisit.PatientVisitid AS Varchar)=pvar_PatientVisitid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

