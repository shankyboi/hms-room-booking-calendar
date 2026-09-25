
			  CREATE OR REPLACE FUNCTION  "count_of_Room_occupancystatus"
              (pvar_tenantid Varchar
,pvar_block Varchar(1024)
,pvar_building Varchar(1024)
,pvar_floor Varchar(1024)
,pvar_roomtype Varchar(1024)
,pvar_roomnumber Varchar(1024)
,pvar_occupancystatus Varchar(1024)
,pvar_maintenancestatus Varchar(1024)
,pvar_housekeepingstatus Varchar(1024)
)
			  RETURNS TABLE(count bigint,occupancystatus varchar)
			  AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:22*/
			  		
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



              
              RETURN QUERY
			  SELECT  
			  Count(*),Room.occupancystatus::varchar as occupancystatus
			  FROM Room
			  WHERE (lvar_tenantid is null or COALESCE(cast(Room.tenantid as varchar), '') = Any(lvar_tenantid)) AND Room.isdeleted=false
AND (pvar_block is null or LENGTH(CAST(pvar_block as Varchar))=0 or CAST(Room.block as VARCHAR)=pvar_block)
AND (pvar_building is null or LENGTH(CAST(pvar_building as Varchar))=0 or CAST(Room.building as VARCHAR)=pvar_building)
AND (pvar_floor is null or LENGTH(CAST(pvar_floor as Varchar))=0 or CAST(Room.floor as VARCHAR)=pvar_floor)
AND (pvar_roomtype is null or LENGTH(CAST(pvar_roomtype as Varchar))=0 or CAST(Room.roomtype as VARCHAR)=pvar_roomtype)
AND (pvar_roomnumber is null or LENGTH(CAST(pvar_roomnumber as Varchar))=0 or CAST(Room.roomnumber as VARCHAR)=pvar_roomnumber)
AND (pvar_maintenancestatus is null or LENGTH(CAST(pvar_maintenancestatus as Varchar))=0 or CAST(Room.maintenancestatus as VARCHAR)=pvar_maintenancestatus)
AND (pvar_housekeepingstatus is null or LENGTH(CAST(pvar_housekeepingstatus as Varchar))=0 or CAST(Room.housekeepingstatus as VARCHAR)=pvar_housekeepingstatus)

               GROUP BY Room.occupancystatus;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

