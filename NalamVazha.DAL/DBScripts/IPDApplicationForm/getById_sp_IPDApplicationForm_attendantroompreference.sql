CREATE OR REPLACE FUNCTION "getById_sp_IPDApplicationForm_attendantroompreference"(pvar_ipdapplicationformid character varying)
RETURNS TABLE("IPDApplicationForm_attendantroompreferenceid" uuid, "IPDApplicationFormid" uuid, roomtypeatt uuid, roomtypename character varying, record_order integer, cma_client_row_id character varying)
LANGUAGE sql AS $$
    SELECT p.IPDApplicationForm_attendantroompreferenceid, p.IPDApplicationFormid, p.roomtypeatt, r.name, p.record_order, p.cma_client_row_id
    FROM IPDApplicationForm_attendantroompreference p
    LEFT JOIN RoomType r ON r.RoomTypeid = p.roomtypeatt
    WHERE p.IPDApplicationFormid::character varying = pvar_ipdapplicationformid AND COALESCE(p.isdeleted, false) = false
    ORDER BY p.record_order, p.createddate;
$$;
