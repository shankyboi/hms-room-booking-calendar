-- FUNCTION: public.Get_All_IPD_Receivables(uuid)

-- DROP FUNCTION IF EXISTS public."Get_All_IPD_Receivables"(uuid);

CREATE OR REPLACE FUNCTION public."Get_All_IPD_Receivables"(
	pvar_ipdid uuid)
    RETURNS TABLE(receivableno text, receivabledate text, receivablefor text, remarks text, amount numeric, paidamount numeric, balance numeric, paymentstatus text) 
    LANGUAGE 'sql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
    SELECT COALESCE(r.receivableno, '') AS receivableno,
           COALESCE(to_char(r.receivabledate, 'DD/MM/YYYY'), '') AS receivabledate,
           CASE
               WHEN LOWER(TRIM(COALESCE(r.receivablefor, ''))) = 'cancellation refund'
                AND COALESCE(r.remarks, '') ILIKE '%Auto:PackageChange:BookingDepositRefund%'
                   THEN 'Package Change Refund'
               WHEN LOWER(TRIM(COALESCE(r.receivablefor, ''))) = 'others'
                AND NULLIF(TRIM(COALESCE(r.specifyothers, '')), '') IS NOT NULL
                   THEN COALESCE(r.receivablefor, 'Others') || ' - ' || TRIM(r.specifyothers)
               ELSE COALESCE(r.receivablefor, '')
           END AS receivablefor,
           COALESCE(r.remarks, '') AS remarks,
           COALESCE(r.amount, 0) AS amount,
           COALESCE(r.paidamount, 0) AS paidamount,
           GREATEST(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0), 0) AS balance,
           COALESCE(r.paymentstatus, 'Pending') AS paymentstatus
    FROM Receivable r
    WHERE r.ipdnumber = pvar_ipdid
      AND COALESCE(r.isdeleted, false) = false
         AND COALESCE(r.remarks, '') NOT ILIKE '%inactive%'

	  
    ORDER BY r.receivabledate ASC,
             r.receivableid ASC;
$BODY$;

ALTER FUNCTION public."Get_All_IPD_Receivables"(uuid)
    OWNER TO md_nalamvazha;

