CREATE OR REPLACE FUNCTION public."lookup_Assessment_doctorname"(
    pvar_tenantid character varying DEFAULT NULL::character varying,
    pvar_searchterm character varying DEFAULT ''::character varying,
    pvar_pagesize integer DEFAULT 50,
    pvar_pagenumber integer DEFAULT 0)
RETURNS TABLE(
    "Peopleid" character varying,
    firstname character varying,
    lastname character varying)
LANGUAGE plpgsql
AS $BODY$
DECLARE
    lvar_tenantid varchar[];
    lstr_usersid varchar;
BEGIN
    SELECT split_part(pvar_tenantid, '|', 1), split_part(pvar_tenantid, '|', 2)
      INTO lstr_usersid, pvar_tenantid;

    IF pvar_tenantid IS NULL OR pvar_tenantid = ''
       OR pvar_tenantid = '00000000-0000-0000-0000-000000000000' THEN
        SELECT string_to_array(viewertenantids, ',')
          INTO lvar_tenantid
          FROM users
         WHERE usersid::varchar = lstr_usersid;

        IF lvar_tenantid IS NULL THEN
            SELECT array_agg(tenantid) INTO lvar_tenantid FROM tenant;
        END IF;
    ELSE
        lvar_tenantid := ARRAY[pvar_tenantid];
    END IF;

    lvar_tenantid := lvar_tenantid
        || ARRAY[''::varchar, '00000000-0000-0000-0000-000000000000'::varchar];

    pvar_searchterm := CASE
        WHEN length(COALESCE(pvar_searchterm, '')) > 0 THEN '%' || pvar_searchterm || '%'
        ELSE NULL
    END;

    RETURN QUERY
    SELECT p.peopleid::varchar,
           p.firstname::varchar,
           p.lastname::varchar
      FROM people p
      INNER JOIN workprofile wp ON wp.workprofileid = p.workprofile
     WHERE COALESCE(p.tenantid::varchar, '') = ANY(lvar_tenantid)
       AND COALESCE(p.isdeleted, false) = false
       AND COALESCE(wp.isdeleted, false) = false
       AND lower(btrim(COALESCE(wp.rolename, ''))) = 'doctor'
       AND (pvar_searchterm IS NULL
            OR COALESCE(p.firstname, '') ILIKE pvar_searchterm
            OR COALESCE(p.lastname, '') ILIKE pvar_searchterm
            OR p.peopleid::varchar ILIKE pvar_searchterm)
     ORDER BY p.firstname ASC, p.lastname ASC
     LIMIT pvar_pagesize
    OFFSET pvar_pagenumber * pvar_pagesize;
END
$BODY$;
