-- FUNCTION: public.lookup_BillingPayment_collectedby(character varying)

-- DROP FUNCTION IF EXISTS public."lookup_BillingPayment_collectedby"(character varying);

CREATE OR REPLACE FUNCTION public."lookup_BillingPayment_collectedby"(
	pvar_tenantid character varying DEFAULT NULL::character varying)
    RETURNS TABLE(usersid character varying, firstname character varying, lastname character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
declare lvar_tenantid varchar[];declare lstr_usersid varchar;
BEGIN
/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:29*/

SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;

if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')
then
    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
    FROM users where users.usersid::varchar=lstr_usersid;
    if(lvar_tenantid is NULL)
    then
        SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;

    end if;
else
  lvar_tenantid=ARRAY[pvar_tenantid];
end if;
lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

RETURN QUERY
SELECT
CAST(users.usersid AS Varchar) as usersid,CAST(users.firstname AS Varchar) as firstname,CAST(users.lastname AS Varchar) as lastname
FROM users
 WHERE COALESCE(cast(users.tenantid as varchar),'') = Any(lvar_tenantid) AND users.isdeleted=false
 -- "Collected By" should only ever be a Front Desk staff member, not any user in the
 -- tenant (this previously listed every user, including patient/test accounts).
 AND LOWER(COALESCE(users.userrole,'')) = 'frontdesk admin'

 ORDER BY users.lastname ASC
;

END
$BODY$;

ALTER FUNCTION public."lookup_BillingPayment_collectedby"(character varying)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."lookup_BillingPayment_collectedby"(character varying) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."lookup_BillingPayment_collectedby"(character varying) TO develop_ukan;

GRANT EXECUTE ON FUNCTION public."lookup_BillingPayment_collectedby"(character varying) TO md_nalamvazha;

