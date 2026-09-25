DROP FUNCTION IF EXISTS public."getById_sp_all_ShiftPlanning"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_all_ShiftPlanning"(
	pvar_shiftplanningid character varying)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, "ShiftPlanningid" uuid, shiftname character varying,shiftstarttime Varchar
,shiftendtime Varchar, validfrom character varying, validto character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, "automaton_ShiftPlanning_people" json, "automaton_ShiftPlanning_bulkuploaderror" json, "automaton_ShiftPlanning_bulkuploaderrorcount" integer) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20*/
			  		 
              RETURN QUERY
			  SELECT  
				 ShiftPlanning.tenantid
,tenant.businessname as _tenantname
,ShiftPlanning.ShiftPlanningid
,CAST(_Shift.shiftname AS VARCHAR) as shiftname
,ShiftPlanning.shiftstarttime
,ShiftPlanning.shiftendtime
,CAST(COALESCE(to_char(ShiftPlanning.validfrom,'dd/MM/yyyy'),'') AS Varchar) as validfrom
,CAST(COALESCE(to_char(ShiftPlanning.validto,'dd/MM/yyyy'),'') AS Varchar) as validto
				 ,ShiftPlanning.createduser,ShiftPlanning.createddate,ShiftPlanning.modifieduser,ShiftPlanning.modifieddate
                 ,
						(SELECT json_agg(J) FROM (SELECT   
						CAST(__WorkProfile.workprofilename AS VARCHAR) as "Work Profile"
						,CAST(CASE WHEN _People.lastname IS NULL OR _People.lastname = '' THEN _People.firstname ELSE _People.firstname || ' ' || _People.lastname END AS VARCHAR) as "Person Name"
,ShiftPlanning_people.coveragetype as "Coverage Type "
							
						FROM  ShiftPlanning_people 
INNER JOIN People _People ON ShiftPlanning_people.personname=_People.Peopleid
LEFT OUTER JOIN WorkProfile __WorkProfile ON ShiftPlanning_people.workprofile=__WorkProfile.WorkProfileid
						WHERE ShiftPlanning.ShiftPlanningid =ShiftPlanning_people.ShiftPlanningid
) J)
						as automaton_ShiftPlanning_people
                 
				 ,
(SELECT json_agg(J) FROM (
SELECT unalteredjson->>'workprofile' AS "workprofile",
     unalteredjson->>'personname' AS "personname",
unalteredjson->>'coveragetype' AS "coveragetype",
errordescription
    FROM ShiftPlanning_people_bulkupload 
    WHERE pvar_ShiftPlanningid ::uuid = ShiftPlanning_people_bulkupload.ShiftPlanningid 
    AND COALESCE(ShiftPlanning_people_bulkupload.isdeleted, false) = false
    AND COALESCE(errordescription, null) IS NOT NULL
) J) AS automaton_ShiftPlanning_bulkuploaderror,
CAST((SELECT COUNT(*) FROM ShiftPlanning_people_bulkupload 
 WHERE pvar_ShiftPlanningid ::uuid = ShiftPlanning_people_bulkupload.ShiftPlanningid 
 AND COALESCE(ShiftPlanning_people_bulkupload.isdeleted, false) = false
 AND COALESCE(errordescription, null) IS NOT NULL) AS integer) AS automaton_ShiftPlanning_bulkuploaderrorcount
			  FROM  ShiftPlanning 
 LEFT OUTER JOIN tenant ON ShiftPlanning.tenantid=tenant.tenantid
INNER JOIN Shift _Shift ON ShiftPlanning.shiftname=_Shift.Shiftid
			  WHERE CAST(ShiftPlanning.ShiftPlanningid AS Varchar)=pvar_ShiftPlanningid;
			  
			  END
              
$BODY$;

