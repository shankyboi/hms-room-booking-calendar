CREATE OR REPLACE FUNCTION public."Verify_Concession_ApprovedAmount"(
    pvar_concessionformid character varying,
    pvar_approvedconcessionamount numeric)
RETURNS integer
LANGUAGE 'sql'
COST 100
VOLATILE PARALLEL UNSAFE
AS $BODY$
    SELECT COUNT(*)::integer
    FROM concessionform
    WHERE concessionformid = ANY(string_to_array(pvar_concessionformid, ',')::uuid[])
      AND ABS(COALESCE(pvar_approvedconcessionamount, 0)) > ABS(COALESCE(requestedconcessionamount, 0));
$BODY$;