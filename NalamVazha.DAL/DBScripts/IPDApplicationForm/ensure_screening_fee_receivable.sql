CREATE OR REPLACE FUNCTION public.ensure_screening_fee_receivable(
    p_ipdformid uuid,
    p_tenantid uuid,
    p_createdby uuid)
RETURNS integer
LANGUAGE plpgsql
AS $function$
BEGIN
    -- Appointment scheduling and doctor approval can both request this bill.
    -- Serialize requests for the same IPD so concurrent calls cannot insert twice.
    PERFORM pg_advisory_xact_lock(hashtext(p_ipdformid::text));

    -- A paid receivable is intentionally treated as existing. Recreating it would
    -- incorrectly make the health seeker owe the screening fee a second time.
    IF EXISTS (
        SELECT 1
        FROM public.receivable r
        WHERE r.ipdnumber = p_ipdformid
          AND LOWER(TRIM(COALESCE(r.receivablefor, ''))) = 'ipd screening fee'
          AND COALESCE(r.isdeleted, false) = false
    ) THEN
        RETURN 1;
    END IF;

    RETURN public.create_screening_fee_receivable(
        p_ipdformid,
        p_tenantid,
        p_createdby);
END;
$function$;
