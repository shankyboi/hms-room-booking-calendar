-- FUNCTION: public.MailSender(character varying, character varying, character varying)

-- DROP FUNCTION IF EXISTS public."MailSender"(character varying, character varying, character varying);

CREATE OR REPLACE FUNCTION public."MailSender"(
	pvar_mailfor character varying,
	pvar_entityid character varying,
	pvar_createduser character varying)
    RETURNS TABLE(toname character varying, username character varying, userpassword character varying, receiveremail character varying, mobilenumber character varying, usersid uuid, userrole character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
BEGIN
   IF(pvar_mailfor = 'forgotpassword'  OR pvar_mailfor = 'registration' OR pvar_mailfor = 'User credientils') 
THEN
RETURN QUERY SELECT
      CAST(users.firstname || ' ' || COALESCE(users.lastname, '') AS Varchar) as toname,
      users.username,
      users.userpassword,
      COALESCE(users.emailid, '') as receiveremail,
      users.mobilenumber,
      users.usersid,
       users.userrole
	  FROM
         users 
      WHERE
         CAST (users.usersid AS character varying) = pvar_entityid;

END
IF;
END;
$BODY$;

ALTER FUNCTION public."MailSender"(character varying, character varying, character varying)
    OWNER TO md_nalamvazha;

GRANT EXECUTE ON FUNCTION public."MailSender"(character varying, character varying, character varying) TO PUBLIC;

GRANT EXECUTE ON FUNCTION public."MailSender"(character varying, character varying, character varying) TO develop_ukan;

GRANT EXECUTE ON FUNCTION public."MailSender"(character varying, character varying, character varying) TO md_nalamvazha;

