 DROP FUNCTION IF EXISTS public."getById_sp_ShiftPlanning"(character varying);

CREATE OR REPLACE FUNCTION public."getById_sp_ShiftPlanning"(
	pvar_shiftplanningid character varying)
    RETURNS TABLE(shiftname uuid,shiftstarttime Varchar,shiftendtime Varchar, validfrom date, validto date, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, shiftplanningid uuid, people json) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:20*/
               
              RETURN QUERY
			  SELECT 
				 ShiftPlanning.shiftname
				 ,ShiftPlanning.shiftstarttime
,ShiftPlanning.shiftendtime
,ShiftPlanning.validfrom
,ShiftPlanning.validto

				 ,ShiftPlanning.createduser,ShiftPlanning.createddate,ShiftPlanning.modifieduser,ShiftPlanning.modifieddate
				 ,ShiftPlanning.tenantid
                 ,ShiftPlanning.ShiftPlanningid
                 ,(SELECT json_agg(J) FROM (
											 SELECT 
											 ShiftPlanning_people.ShiftPlanningid
                                             ,ShiftPlanning_people.ShiftPlanning_peopleid   
											 ,ShiftPlanning_people.personname
,ShiftPlanning_people.workprofile
,ShiftPlanning_people.coveragetype
 
											  
											 FROM ShiftPlanning_people
											 WHERE 
											 ShiftPlanning_people.ShiftPlanningid=ShiftPlanning.ShiftPlanningid
                                             
                                             ORDER BY record_order DESC
											) J) as people
   
			  FROM ShiftPlanning
			  WHERE CAST(ShiftPlanning.ShiftPlanningid AS Varchar)=pvar_ShiftPlanningid
                       ;

					 	
			  END
              
$BODY$;

