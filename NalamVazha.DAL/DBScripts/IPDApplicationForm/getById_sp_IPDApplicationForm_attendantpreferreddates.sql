CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_attendantpreferreddates"(pvar_ipdapplicationformid character varying)
RETURNS TABLE("IPDApplicationForm_attendantpreferreddatesid" uuid, "IPDApplicationFormid" uuid, dateofarrivalatt date, dateofdepartureatt date, daysofstayatt integer, record_order integer, cma_client_row_id character varying)
LANGUAGE sql AS $$
    SELECT p.IPDApplicationForm_attendantpreferreddatesid, p.IPDApplicationFormid, p.dateofarrivalatt, p.dateofdepartureatt, p.daysofstayatt, p.record_order, p.cma_client_row_id
    FROM IPDApplicationForm_attendantpreferreddates p
    WHERE p.IPDApplicationFormid::character varying = pvar_ipdapplicationformid AND COALESCE(p.isdeleted, false) = false
    ORDER BY p.record_order, p.createddate;
$$;
