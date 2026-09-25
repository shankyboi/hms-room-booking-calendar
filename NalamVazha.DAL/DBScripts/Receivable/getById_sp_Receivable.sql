DROP FUNCTION IF EXISTS public."getById_sp_Receivable"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_Receivable"(
	pvar_receivableid character varying)
    RETURNS TABLE(receivableno character varying, receivabledate date, patientname uuid, patientvisit uuid, ipdnumber uuid, opdnumber uuid, receivablefor character varying,specifyothers Varchar, therapy uuid, therapycost numeric, therapykit uuid, kitprice character varying, medicine uuid, price numeric, roomtype uuid, costperday numeric, attendantcostperday numeric, room uuid, amount numeric, remarks character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, receivableid uuid) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/15/2026 17:09:57*/
               
              RETURN QUERY
			  SELECT 
				 Receivable.receivableno
,Receivable.receivabledate
,Receivable.patientname
,Receivable.patientvisit
,Receivable.ipdnumber
,Receivable.opdnumber
,Receivable.receivablefor
,Receivable.specifyothers
,Receivable.therapy
,Receivable.therapycost
,Receivable.therapykit
,Receivable.kitprice
,Receivable.medicine
,Receivable.price
,Receivable.roomtype
,Receivable.costperday
,Receivable.attendantcostperday
,Receivable.room
,Receivable.amount
,Receivable.remarks

				 ,Receivable.createduser,Receivable.createddate,Receivable.modifieduser,Receivable.modifieddate
				 ,Receivable.tenantid
                 ,Receivable.Receivableid
                    
			  FROM Receivable
			  WHERE CAST(Receivable.Receivableid AS Varchar)=pvar_Receivableid
                       ;

					 	
			  END
              
$BODY$;

