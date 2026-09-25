
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Appoinment"
              (
			  pvar_Appoinmentid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Appoinmentid" uuid
,patient Varchar
,origin Varchar
,bookingreferencenumber Varchar
,doctor Varchar
,appointmentdate Varchar
,task Varchar
,duration Varchar
,status Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 08/01/2026 11:54:47*/
			  		 
              RETURN QUERY
			  SELECT  
				 Appoinment.tenantid
,tenant.businessname as _tenantname
,Appoinment.Appoinmentid
,CAST(_ClinicalAppointment.patient AS VARCHAR) as patient
,CAST(__ClinicalAppointment.origin AS VARCHAR) as origin
,CAST(___ClinicalAppointment.ipdnumber||' '||___ClinicalAppointment.opdnumber AS VARCHAR) as bookingreferencenumber
,CAST(____ClinicalAppointment.practitioner AS VARCHAR) as doctor
,CAST(_____ClinicalAppointment.appointmentdate AS VARCHAR) as appointmentdate
,CAST(______ClinicalAppointment.taskname AS VARCHAR) as task
,CAST(_______ClinicalAppointment.durationfrom||' '||_______ClinicalAppointment.durationto AS VARCHAR) as duration
,CAST(________ClinicalAppointment.status AS VARCHAR) as status

				 ,Appoinment.createduser,Appoinment.createddate,Appoinment.modifieduser,Appoinment.modifieddate
                 
                 
				 
			  FROM  Appoinment 
 LEFT OUTER JOIN tenant ON Appoinment.tenantid=tenant.tenantid
LEFT OUTER JOIN ClinicalAppointment _ClinicalAppointment ON Appoinment.patient=_ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment __ClinicalAppointment ON Appoinment.origin=__ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment ___ClinicalAppointment ON Appoinment.bookingreferencenumber=___ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment ____ClinicalAppointment ON Appoinment.doctor=____ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment _____ClinicalAppointment ON Appoinment.appointmentdate=_____ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment ______ClinicalAppointment ON Appoinment.task=______ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment _______ClinicalAppointment ON Appoinment.duration=_______ClinicalAppointment.ClinicalAppointmentid
LEFT OUTER JOIN ClinicalAppointment ________ClinicalAppointment ON Appoinment.status=________ClinicalAppointment.ClinicalAppointmentid

			  WHERE CAST(Appoinment.Appoinmentid AS Varchar)=pvar_Appoinmentid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

