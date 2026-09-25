CREATE OR REPLACE FUNCTION public."Save_IPD_Cancellation_Request_Remarks"(
    pvar_ipdapplicationformid uuid,
    pvar_remarks character varying,
    pvar_modifieduser uuid)
RETURNS character varying
LANGUAGE plpgsql
AS $BODY$
BEGIN
    UPDATE public.IPDApplicationForm
    SET reviewcomments = pvar_remarks,
        modifieduser = pvar_modifieduser,
        modifieddate = NOW()
    WHERE IPDApplicationFormid = pvar_ipdapplicationformid
      AND COALESCE(isdeleted, false) = false;

    IF FOUND THEN
        RETURN '201.1';
    END IF;

    RETURN '404';
END;
$BODY$;

