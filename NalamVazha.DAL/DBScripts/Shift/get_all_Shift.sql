
			  -- shifthours widened from int to decimal(18,2) to support fractional-hour shifts.
			  -- Changing a RETURNS TABLE column type requires dropping the function first --
			  -- CREATE OR REPLACE cannot alter the return type in place.
			  DROP FUNCTION IF EXISTS "get_all_Shift"(Varchar, character varying, integer, integer);

			  CREATE OR REPLACE FUNCTION  "get_all_Shift"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0

              )
			 RETURNS TABLE(
                shiftcode Varchar
,shiftname Varchar
,shiftstarttime Varchar
,shiftendtime Varchar
,shifthours decimal(18,2)
,description Varchar
,totalbreakinmins Varchar
,totalbreakinhrs decimal
,workhours Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"Shiftid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:42:17*/
               
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


				
             if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                then
                pvar_searchterm := '%' || pvar_searchterm || '%';
                else
                pvar_searchterm := null;
                end if;

			    
                RETURN QUERY
			  SELECT 

				 Shift.shiftcode
,Shift.shiftname
,Shift.shiftstarttime
,Shift.shiftendtime
,Shift.shifthours
,Shift.description
,Shift.totalbreakinmins
,Shift.totalbreakinhrs
,Shift.workhours

				 ,Shift.createduser,Shift.createddate,Shift.modifieduser,Shift.modifieddate
				,Shift.tenantid 
                ,Shift.Shiftid
				 
			  FROM Shift
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(Shift.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND Shift.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(Shift.Shiftid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Shift.shiftcode,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Shift.shiftname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Shift.description,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

