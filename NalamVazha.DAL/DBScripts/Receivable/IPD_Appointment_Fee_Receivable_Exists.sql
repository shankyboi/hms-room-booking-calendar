CREATE OR REPLACE FUNCTION public."IPD_Appointment_Fee_Receivable_Exists"(
    pvar_ipdnumber uuid,
    pvar_receivablefor character varying)
RETURNS boolean
LANGUAGE sql
STABLE
AS $function$
    SELECT EXISTS (
        SELECT 1
        FROM public.receivable
        WHERE ipdnumber = pvar_ipdnumber
          AND LOWER(COALESCE(receivablefor, '')) = LOWER(COALESCE(pvar_receivablefor, ''))
          AND COALESCE(isdeleted, false) = false
    );
$function$;
