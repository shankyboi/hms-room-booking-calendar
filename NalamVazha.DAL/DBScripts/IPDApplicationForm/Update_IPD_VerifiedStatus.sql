CREATE OR REPLACE FUNCTION public."Update_IPD_VerifiedStatus"(
    pvar_ipdapplicationformid uuid,
    pvar_verifiedstatus character varying,
    pvar_modifieduser uuid,
    OUT "returnMessage" character varying)
RETURNS character varying
LANGUAGE plpgsql
AS $BODY$
BEGIN
    UPDATE IPDApplicationForm
    SET verifiedstatus = pvar_verifiedstatus,
        modifieduser = pvar_modifieduser,
        modifieddate = NOW()
    WHERE IPDApplicationFormid = pvar_ipdapplicationformid;

    IF NOT FOUND THEN
        "returnMessage" := 'IPD application not found';
        RETURN;
    END IF;

    "returnMessage" := '201.1';
EXCEPTION WHEN OTHERS THEN
    "returnMessage" := SQLERRM;
END;
$BODY$;
