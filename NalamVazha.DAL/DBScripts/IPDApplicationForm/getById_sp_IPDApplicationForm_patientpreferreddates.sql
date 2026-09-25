CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_patientpreferreddates"(pvar_ipdapplicationformid character varying)
RETURNS TABLE("IPDApplicationForm_patientpreferreddatesid" uuid, "IPDApplicationFormid" uuid, dateofarrival date, dateofdeparture date, daysofstay integer, record_order integer, cma_client_row_id character varying)
LANGUAGE sql AS $$
    SELECT p.IPDApplicationForm_patientpreferreddatesid, p.IPDApplicationFormid, p.dateofarrival, p.dateofdeparture, p.daysofstay, p.record_order, p.cma_client_row_id
    FROM IPDApplicationForm_patientpreferreddates p
    WHERE p.IPDApplicationFormid::character varying = pvar_ipdapplicationformid AND COALESCE(p.isdeleted, false) = false
    ORDER BY p.record_order, p.createddate;
$$;
