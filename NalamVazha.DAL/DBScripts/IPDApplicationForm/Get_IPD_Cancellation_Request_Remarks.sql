CREATE OR REPLACE FUNCTION public."Get_IPD_Cancellation_Request_Remarks"(
    pvar_ipdapplicationformid uuid)
RETURNS TABLE(remarks character varying)
LANGUAGE plpgsql
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT COALESCE(ipd.reviewcomments, '')::character varying
    FROM public.IPDApplicationForm ipd
    WHERE ipd.IPDApplicationFormid = pvar_ipdapplicationformid
      AND COALESCE(ipd.isdeleted, false) = false
    LIMIT 1;
END;
$BODY$;

