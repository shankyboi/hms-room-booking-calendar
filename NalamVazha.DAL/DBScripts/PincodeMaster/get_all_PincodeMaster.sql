 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_PincodeMaster"
              (
			    pvar_tenantid Varchar=null,
                pvar_searchterm character varying='',
                pvar_pagesize integer=50,
                pvar_pagenumber integer=0
              )
			  RETURNS TABLE(
                circlename Varchar
,regionname Varchar
,divisionname Varchar
,officename Varchar
,pincode Varchar
,officetype Varchar
,delivery Varchar
,district Varchar
,statename Varchar
,latitude Varchar
,longitude Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                ,"PincodeMasterid" uuid
               )
               AS $BODY$
               BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/16/2026 09:42:32*/

                if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                then
                pvar_searchterm := '%' || pvar_searchterm || '%';
                else
                pvar_searchterm := null;
                end if;

			 
                RETURN QUERY
                SELECT 
                PincodeMaster.circlename
,PincodeMaster.regionname
,PincodeMaster.divisionname
,PincodeMaster.officename
,PincodeMaster.pincode
,PincodeMaster.officetype
,PincodeMaster.delivery
,PincodeMaster.district
,PincodeMaster.statename
,PincodeMaster.latitude
,PincodeMaster.longitude

                ,PincodeMaster.createduser,PincodeMaster.createddate,PincodeMaster.modifieduser,PincodeMaster.modifieddate
                ,PincodeMaster.PincodeMasterid
                FROM PincodeMaster
			    
                 WHERE PincodeMaster.isdeleted=false
                
                 AND (((pvar_searchterm is null) or COALESCE(PincodeMaster.PincodeMasterid::varchar,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.circlename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.regionname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.divisionname,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.officename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.pincode,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.officetype,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.delivery,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.district,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.statename,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.latitude,'') ilike pvar_searchterm) OR ((pvar_searchterm is null) or COALESCE(PincodeMaster.longitude,'') ilike pvar_searchterm)) 
                limit pvar_pagesize offset pvar_pagenumber * pvar_pagesize;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

