CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_patientroompreference"(pvar_ipdapplicationformid character varying)
RETURNS TABLE("IPDApplicationForm_patientroompreferenceid" uuid, "IPDApplicationFormid" uuid, roomtype uuid, roomtypename character varying, record_order integer, cma_client_row_id character varying)
LANGUAGE sql AS $$
    SELECT p.IPDApplicationForm_patientroompreferenceid, p.IPDApplicationFormid, p.roomtype, r.name, p.record_order, p.cma_client_row_id
    FROM IPDApplicationForm_patientroompreference p
    LEFT JOIN RoomType r ON r.RoomTypeid = p.roomtype
    WHERE p.IPDApplicationFormid::character varying = pvar_ipdapplicationformid AND COALESCE(p.isdeleted, false) = false
    ORDER BY p.record_order, p.createddate;
$$;
