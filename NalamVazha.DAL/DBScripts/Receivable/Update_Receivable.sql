
CREATE OR REPLACE FUNCTION public."Update_Receivable"(
	pvar_receivableid uuid,
	pvar_tenantid uuid,
	pvar_receivableno character varying,
	pvar_receivabledate date,
	pvar_patientname uuid,
	pvar_patientvisit uuid,
	pvar_ipdnumber uuid,
	pvar_opdnumber uuid,
	pvar_receivablefor character varying,
	pvar_specifyothers Varchar(128),
	pvar_therapy uuid,
	pvar_therapycost numeric,
	pvar_therapykit uuid,
	pvar_kitprice character varying,
	pvar_medicine uuid,
	pvar_price numeric,
	pvar_package uuid,
	pvar_room uuid,
	pvar_amount numeric,
	pvar_remarks character varying,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/15/2026 17:09:57*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Receivable', 'edit') THEN

			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Receivable where upper(Receivable.receivableno) = upper(pvar_receivableno) and Receivable.tenantid=pvar_tenantid  and Receivable.Receivableid <> pvar_Receivableid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Receivable No Already Exists.';

																END IF;

			  IF(pvar_returnMessage='')
			  THEN

                    INSERT INTO history
VALUES('Receivable', NOW(),
(SELECT query_to_xml('SELECT * FROM Receivable WHERE Receivable.Receivableid= '''||pvar_Receivableid||'''', true, false, '')));

                    UPDATE Receivable SET
                    receivableno=pvar_receivableno
,receivabledate=pvar_receivabledate
,patientname=pvar_patientname
,patientvisit=pvar_patientvisit
,ipdnumber=pvar_ipdnumber
,opdnumber=pvar_opdnumber
,receivablefor=pvar_receivablefor
,specifyothers=pvar_specifyothers
,therapy=pvar_therapy
,therapycost=pvar_therapycost
,therapykit=pvar_therapykit
,kitprice=pvar_kitprice
,medicine=pvar_medicine
,price=pvar_price
,package=pvar_package
,room=pvar_room
,amount=pvar_amount
,remarks=pvar_remarks

                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Receivableid=pvar_Receivableid;

					pvar_returnMessage :='201.1';

			  END IF;

																ELSE

																INSERT INTO system_logging
																(
																Log_code
																,system_logging_guid
																,log_application
																,log_date
																,log_level
																,log_logger
																,log_message
																,log_user_name
																)
																VALUES
																('401.1'
																,gen_random_uuid()
																,'Store Proc Authorization Check'
																,NOW()
																,'Critical'
																,'Update_Receivable'
																,'Authorization Failed Update_Receivable'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';

																END IF;

			  END
			  
$BODY$;

