-- Replace legacy product branding in existing email templates with the
-- tenant placeholder resolved by MailSender at send time.
UPDATE AlertTemplates
SET
    alertsubject = regexp_replace(
        regexp_replace(COALESCE(alertsubject, ''), '\mNalam(Vazha)?([[:space:]]+Health)?\M', '{tenantname}', 'gi'),
        '\mRegards\M',
        '{tenantname}',
        'gi'
    ),
    alertcontent = regexp_replace(
        regexp_replace(
            regexp_replace(
                COALESCE(alertcontent, ''),
                'Regards[[:space:]]*,?([[:space:]]*<br[[:space:]]*/?>|[[:space:]]*\r?\n|[[:space:]]*)[[:space:]]*Nalam(Vazha)?([[:space:]]+Health)?([[:space:]]+Team)?',
                '{tenantname}',
                'gi'
            ),
            '\mNalam(Vazha)?([[:space:]]+Health)?\M',
            '{tenantname}',
            'gi'
        ),
        '\mRegards\M',
        '{tenantname}',
        'gi'
    ),
    modifieddate = NOW()
WHERE isdeleted = false
  AND (
      COALESCE(alertsubject, '') ~* '(\mNalam(Vazha)?\M|\mRegards\M)'
      OR COALESCE(alertcontent, '') ~* '(\mNalam(Vazha)?\M|\mRegards\M)'
  );

-- Normalize rows already converted by an earlier execution of this migration.
UPDATE AlertTemplates
SET
    alertsubject = regexp_replace(
        COALESCE(alertsubject, ''),
        '\{tenantname\}[[:space:]]*,?([[:space:]]*<br[[:space:]]*/?>|[[:space:]]*\r?\n|[[:space:]]*)[[:space:]]*\{tenantname\}([[:space:]]+Team)?',
        '{tenantname}',
        'gi'
    ),
    alertcontent = regexp_replace(
        COALESCE(alertcontent, ''),
        '\{tenantname\}[[:space:]]*,?([[:space:]]*<br[[:space:]]*/?>|[[:space:]]*\r?\n|[[:space:]]*)[[:space:]]*\{tenantname\}([[:space:]]+Team)?',
        '{tenantname}',
        'gi'
    ),
    modifieddate = NOW()
WHERE isdeleted = false
  AND (
      COALESCE(alertsubject, '') ILIKE '%{tenantname}%{tenantname}%'
      OR COALESCE(alertcontent, '') ILIKE '%{tenantname}%{tenantname}%'
  );
