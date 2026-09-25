DROP FUNCTION IF EXISTS public."get_all_Receivable"(character varying, character varying, integer, integer);

CREATE OR REPLACE FUNCTION public."get_all_Receivable"(
	pvar_tenantid character varying DEFAULT NULL::character varying,
	pvar_searchterm character varying DEFAULT ''::character varying,
	pvar_pagesize integer DEFAULT 50,
	pvar_pagenumber integer DEFAULT 0)
    RETURNS TABLE(receivableno character varying, receivabledate date, patientname uuid, patientvisit uuid, ipdnumber uuid, opdnumber uuid, receivablefor character varying,specifyothers Varchar, therapy uuid, therapycost numeric, therapykit uuid, kitprice character varying, medicine uuid, price numeric, roomtype uuid, costperday numeric, attendantcostperday numeric, room uuid, amount numeric, remarks character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone, tenantid uuid, "Receivableid" uuid) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
               BEGIN
              /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/15/2026 17:09:57*/
               
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

				 Receivable.receivableno
,Receivable.receivabledate
,Receivable.patientname
,Receivable.patientvisit
,Receivable.ipdnumber
,Receivable.opdnumber
,Receivable.receivablefor
,Receivable.specifyothers
,Receivable.therapy
,Receivable.therapycost
,Receivable.therapykit
,Receivable.kitprice
,Receivable.medicine
,Receivable.price
,Receivable.roomtype
,Receivable.costperday
,Receivable.attendantcostperday
,Receivable.room
,Receivable.amount
,Receivable.remarks

				 ,Receivable.createduser,Receivable.createddate,Receivable.modifieduser,Receivable.modifieddate
				,Receivable.tenantid 
                ,Receivable.Receivableid
				 
			  FROM Receivable
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(Receivable.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND Receivable.isdeleted=false
			  
		        AND (((pvar_searchterm is null) or COALESCE(Receivable.Receivableid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Receivable.receivableno,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Receivable.specifyothers,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(Receivable.remarks,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              
$BODY$;
