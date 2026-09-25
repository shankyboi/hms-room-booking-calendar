 DROP FUNCTION IF EXISTS public."getById_sp_all_Receivable"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_all_Receivable"(
	pvar_receivableid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "Receivableid" uuid, receivableno character varying, receivabledate character varying, patientname character varying, patientvisit character varying, ipdnumber character varying, opdnumber character varying, receivablefor character varying,specifyothers Varchar, therapy character varying, therapycost numeric, therapykit character varying, kitprice character varying, medicine character varying, price numeric, roomtype character varying, costperday numeric, attendantcostperday numeric, room character varying, amount numeric, remarks character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/15/2026 17:09:57*/
			  		 
              RETURN QUERY
			  SELECT  
				 Receivable.tenantid
,tenant.businessname as _tenantname
,Receivable.Receivableid
,Receivable.receivableno
,CAST(COALESCE(to_char(Receivable.receivabledate,'dd/MM/yyyy'),'') AS Varchar) as receivabledate
,CAST(_PatientProfile.firstname||' '||_PatientProfile.lastname||' '||_PatientProfile.mobilenumber AS VARCHAR) as patientname
,CAST(__PatientVisit.visitnumber||' '||__PatientVisit.ipdnumber||' '||__PatientVisit.opdnumber AS VARCHAR) as patientvisit
,CAST(___IPDApplicationForm.firstname||' '||___IPDApplicationForm.lastname||' '||___IPDApplicationForm.bookingreferencenumber AS VARCHAR) as ipdnumber
,CAST(____OPDForm.bookingreferencenumber AS VARCHAR) as opdnumber
,Receivable.receivablefor
,Receivable.specifyothers
,CAST(_____Therapies.therapyname AS VARCHAR) as therapy
,Receivable.therapycost
,CAST(______TherapyKit.therapykitname AS VARCHAR) as therapykit
,Receivable.kitprice
,CAST(_______Medicine.medicinename AS VARCHAR) as medicine
,Receivable.price
,CAST(________RoomType.name AS VARCHAR) as roomtype
,Receivable.costperday
,Receivable.attendantcostperday
,CAST(_________Room.roomnumber AS VARCHAR) as room
,Receivable.amount
,Receivable.remarks

				 ,Receivable.createduser,Receivable.createddate,Receivable.modifieduser,Receivable.modifieddate
                 
                 
				 
			  FROM  Receivable 
 LEFT OUTER JOIN tenant ON Receivable.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON Receivable.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN PatientVisit __PatientVisit ON Receivable.patientvisit=__PatientVisit.PatientVisitid
LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON Receivable.ipdnumber=___IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm ____OPDForm ON Receivable.opdnumber=____OPDForm.OPDFormid
LEFT OUTER JOIN Therapies _____Therapies ON Receivable.therapy=_____Therapies.Therapiesid
LEFT OUTER JOIN TherapyKit ______TherapyKit ON Receivable.therapykit=______TherapyKit.TherapyKitid
LEFT OUTER JOIN Medicine _______Medicine ON Receivable.medicine=_______Medicine.Medicineid
LEFT OUTER JOIN RoomType ________RoomType ON Receivable.roomtype=________RoomType.RoomTypeid
LEFT OUTER JOIN Room _________Room ON Receivable.room=_________Room.Roomid

			  WHERE CAST(Receivable.Receivableid AS Varchar)=pvar_Receivableid ;
			  
					 	
			  END
              
$BODY$;


