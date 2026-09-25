CREATE OR REPLACE FUNCTION "lookup_IPDApplicationForm_patientroompreference_roomtype"(pvar_tenantid varchar DEFAULT NULL)
RETURNS TABLE("RoomTypeid" varchar, name varchar)
LANGUAGE plpgsql AS $$
DECLARE
    lvar_tenantid varchar[];
    lstr_usersid varchar;
BEGIN
    SELECT SPLIT_PART(pvar_tenantid, '|', 1), SPLIT_PART(pvar_tenantid, '|', 2) INTO lstr_usersid, pvar_tenantid;
    IF pvar_tenantid IS NULL OR pvar_tenantid = '' OR pvar_tenantid = '00000000-0000-0000-0000-000000000000' THEN
        SELECT STRING_TO_ARRAY(viewertenantids, ',') INTO lvar_tenantid FROM users WHERE usersid::varchar = lstr_usersid;
        IF lvar_tenantid IS NULL THEN SELECT array_agg(tenantid::varchar) INTO lvar_tenantid FROM tenant; END IF;
    ELSE
        lvar_tenantid = ARRAY[pvar_tenantid];
    END IF;
    lvar_tenantid := lvar_tenantid || ARRAY['', '00000000-0000-0000-0000-000000000000'];
    RETURN QUERY SELECT RoomType.RoomTypeid::varchar, RoomType.name::varchar
    FROM RoomType
    WHERE COALESCE(RoomType.tenantid::varchar, '') = ANY(lvar_tenantid) AND RoomType.isdeleted = false
    ORDER BY RoomType.name;
END;
$$;
