 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_otplogs"
              (
			    pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
              )
			  RETURNS TABLE(
                username Varchar
,otpcode int
,expirytime Timestamp(3)
,isused Boolean
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"otplogsid" uuid
               )
               AS $BODY$
               BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:45*/

                if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                then
                pvar_searchterm := '%' || pvar_searchterm || '%';
                else
                pvar_searchterm := null;
                end if;

			 
                RETURN QUERY
                SELECT 
                otplogs.username
,otplogs.otpcode
,otplogs.expirytime
,COALESCE(otplogs.isused,true) as isused

                ,otplogs.createduser,otplogs.createddate,otplogs.modifieduser,otplogs.modifieddate
                ,otplogs.otplogsid
                FROM otplogs
			    
                 WHERE otplogs.isdeleted=false
                
                 AND (((pvar_searchterm is null) or COALESCE(otplogs.otplogsid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(otplogs.username,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

