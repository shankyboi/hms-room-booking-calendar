
CREATE OR REPLACE FUNCTION public."Get_OPD_Action_Eligibility"(
	pvar_opdformid uuid,
	pvar_userid uuid,
	pvar_userrole character varying)
    RETURNS TABLE(verifiedstatus character varying, canaddreceivable boolean, cancancel boolean) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
DECLARE
    lvar_userrole varchar;
BEGIN
    SELECT u.userrole
    INTO lvar_userrole
    FROM users u
    WHERE u.usersid = pvar_userid
      AND COALESCE(u.isdeleted, false) = false;

    RETURN QUERY
    SELECT
        o.verifiedstatus,
        (
            LOWER(TRIM(COALESCE(lvar_userrole, pvar_userrole, ''))) = 'frontdesk admin'
            AND (
                /* Before the doctor's final assessment review, receivables may
                   always be added even when the current payment is closed. */
                LOWER(TRIM(COALESCE(o.verifiedstatus, ''))) IN (
                    'approved',
                    'opd approved',
                    'assessment pending',
                    'assessment in progress',
                    'assessment intern review pending',
                    'assessment doctor review pending'
                )
                OR (
                    /* After review, keep the action only while the aggregate
                       active OPD receivable still has an outstanding balance. */
                    LOWER(TRIM(COALESCE(o.verifiedstatus, ''))) IN
                        ('assessment approved', 'opd completed')
                    AND COALESCE((
                        SELECT SUM(COALESCE(r.amount, 0) - COALESCE(r.paidamount, 0))
                        FROM receivable r
                        WHERE r.opdnumber = o.opdformid
                          AND COALESCE(r.isdeleted, false) = false
                    ), 0) > 0
                )
            )
        ) AS canaddreceivable,
        (
            LOWER(TRIM(COALESCE(lvar_userrole, pvar_userrole, ''))) IN ('frontdesk admin', 'health seeker')
            AND (
                LOWER(TRIM(COALESCE(lvar_userrole, pvar_userrole, ''))) = 'frontdesk admin'
                OR o.patientname = pvar_userid
            )
            AND LOWER(TRIM(COALESCE(o.verifiedstatus, ''))) NOT LIKE 'cancel%'
            AND LOWER(TRIM(COALESCE(o.verifiedstatus, ''))) NOT IN
                ('opd completed', 'assessment approved')
            AND NOT public."OPD_Has_Refund_Initiated"(o.opdformid)
        ) AS cancancel
    FROM OPDForm o
    WHERE o.opdformid = pvar_opdformid
      AND COALESCE(o.isdeleted, false) = false;
END;
$BODY$;


