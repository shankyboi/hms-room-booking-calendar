
CREATE OR REPLACE FUNCTION public."getById_sp_OPDForm_appointmentpreferences"(
	pvar_opdformid character varying)
    RETURNS TABLE("OPDFormid" uuid, "OPDForm_appointmentpreferencesid" uuid, preferreddate date, slotpreference character varying) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                                             BEGIN
											 /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 06/22/2026 09:08:49*/
											 RETURN QUERY
											 SELECT
											 OPDForm_appointmentpreferences.OPDFormid
                                             ,OPDForm_appointmentpreferences.OPDForm_appointmentpreferencesid
											 ,OPDForm_appointmentpreferences.preferreddate
,OPDForm_appointmentpreferences.slotpreference

											 FROM OPDForm_appointmentpreferences
											 WHERE
											 CAST(OPDForm_appointmentpreferences.OPDFormid AS VARCHAR)=pvar_OPDFormid
                                             ORDER BY record_order ASC;

											 END
                                             
$BODY$;
