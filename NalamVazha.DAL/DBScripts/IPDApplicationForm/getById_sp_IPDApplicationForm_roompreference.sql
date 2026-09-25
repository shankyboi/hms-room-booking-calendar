CREATE OR REPLACE FUNCTION public."getById_sp_IPDApplicationForm_roompreference"(
    pvar_ipdapplicationformid character varying)
    RETURNS TABLE("IPDApplicationFormid" uuid, "IPDApplicationForm_roompreferenceid" uuid, roomtype uuid, cma_client_row_id character varying, record_order integer)
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN

    RETURN QUERY
    SELECT
        IPDApplicationForm_roompreference.IPDApplicationFormid
        ,IPDApplicationForm_roompreference.IPDApplicationForm_roompreferenceid
        ,IPDApplicationForm_roompreference.roomtype
        ,IPDApplicationForm_roompreference.cma_client_row_id
        ,IPDApplicationForm_roompreference.record_order

    FROM IPDApplicationForm_roompreference
    WHERE
        CAST(IPDApplicationForm_roompreference.IPDApplicationFormid AS VARCHAR)=pvar_ipdapplicationformid
        AND COALESCE(IPDApplicationForm_roompreference.isdeleted,false) = false
    ORDER BY record_order ASC;

END
$BODY$;
