
			  -- shifthours widened from int to decimal(18,2) to match Shift.shifthours.
			  -- Changing a RETURNS TABLE column type requires dropping the function first --
			  -- CREATE OR REPLACE cannot alter the return type in place.
			  DROP FUNCTION IF EXISTS "get_all_StaffAttendance"(Varchar, character varying, integer, integer);

			  CREATE OR REPLACE FUNCTION  "get_all_StaffAttendance"
              (
			  pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0

              )
			 RETURNS TABLE(
                shiftdate date
,shift uuid
,shiftstarttime Varchar
,shiftendtime Varchar
,shifthours decimal(18,2)
,workprofile uuid
,peoplename uuid
,punchdateandtime Timestamp(3)
,earlyinmin decimal
,earlyoutmin decimal
,latemin decimal
,workhours int
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"StaffAttendanceid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:24*/
               
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

				 StaffAttendance.shiftdate
,StaffAttendance.shift
,StaffAttendance.shiftstarttime
,StaffAttendance.shiftendtime
,StaffAttendance.shifthours
,StaffAttendance.workprofile
,StaffAttendance.peoplename
,StaffAttendance.punchdateandtime
,StaffAttendance.earlyinmin
,StaffAttendance.earlyoutmin
,StaffAttendance.latemin
,StaffAttendance.workhours

				 ,StaffAttendance.createduser,StaffAttendance.createddate,StaffAttendance.modifieduser,StaffAttendance.modifieddate
				,StaffAttendance.tenantid 
                ,StaffAttendance.StaffAttendanceid
				 
			  FROM StaffAttendance
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(StaffAttendance.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND StaffAttendance.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(StaffAttendance.StaffAttendanceid::varchar,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

