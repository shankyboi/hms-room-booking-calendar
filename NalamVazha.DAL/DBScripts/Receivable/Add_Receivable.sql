
CREATE OR REPLACE FUNCTION public."Add_Receivable"(
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
	pvar_createduser uuid,
	pvar_paidamount numeric,
	pvar_paymentstatus character varying,
	pvar_billingpaymentid uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);lvar_curday_receivableno Varchar(10);lvar_val_receivableno int;
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/15/2026 17:09:57*/
		

			  
                                                                                    if pvar_Receivableid is null then
                                                                                    pvar_Receivableid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
 

pvar_receivableno := generate_formatted_numbers(
    pvar_tenantid,
    'YYYY-MM-999',    
    'receivable',  
    'receivableno'
);

			  IF "Check_Authorization"(pvar_createduser, 'Receivable', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Receivable where upper(Receivable.receivableno::varchar) = upper(pvar_receivableno::varchar) and Receivable.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Receivable No Already Exists.';

																END IF;

             /* IF(pvar_receivablefor is not null AND pvar_receivablefor!='0' AND LENGTH(pvar_receivablefor)>0)
                                                            THEN                        
                                                                 if(CAST((SELECT Count(T1.T1) 
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',') AS T1
                                                                    INNER JOIN regexp_split_to_table((Select  fielddesc 
                                                                from lookups  where fieldname='receivablefor'
                                                                and entityname='Receivable' LIMIT 1), ',') AS T2 on T1.T1 = T2.T2) AS int) <> CAST((SELECT Count(T1.T1)
                                                                FROM regexp_split_to_table(pvar_receivablefor, ',')  AS T1) AS int))
                                                                THEN
                                                                        pvar_returnMessage := pvar_returnMessage || 'receivablefor value is invalid';

                                                                END IF;
                                                            END IF;*/
  
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Receivable(
				 receivableno
,receivabledate
,patientname
,patientvisit
,ipdnumber
,opdnumber
,receivablefor
,specifyothers
,therapy
,therapycost
,therapykit
,kitprice
,medicine
,price
--,roomtype
--,costperday
--,attendantcostperday
,room
,amount
,remarks

				 ,createduser
				 ,Receivableid
				 ,tenantid
			 ,paidamount 
	,paymentstatus
	,billingpaymentid
          ,package      
			  )
			  VALUES (
 				 pvar_receivableno
,pvar_receivabledate
,pvar_patientname
,pvar_patientvisit
,pvar_ipdnumber
,pvar_opdnumber
,pvar_receivablefor
,pvar_specifyothers
,pvar_therapy
,pvar_therapycost
,pvar_therapykit
,pvar_kitprice
,pvar_medicine
,pvar_price
--,pvar_roomtype
--,pvar_costperday
--,pvar_attendantcostperday
,pvar_room
,pvar_amount
,pvar_remarks

				 ,pvar_createduser
				 ,pvar_Receivableid
				 ,pvar_tenantid
				 ,pvar_paidamount 
	,pvar_paymentstatus
	,pvar_billingpaymentid
	,pvar_package
                   
			  );
			   
               

			  

			  
					 
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
																,'Add_Receivable'
																,'Authorization Failed Add_Receivable'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
						INSERT INTO system_logging
						(
						Log_code
						,system_logging_guid
						,log_application
						,log_date
						,log_level
						,log_logger
						,log_message
						)
						VALUES
						('16'
						,gen_random_uuid()
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Receivable'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Receivable - Insert failed';*/
			  	
			  END
              
$BODY$;

