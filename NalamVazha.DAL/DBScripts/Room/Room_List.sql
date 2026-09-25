
CREATE OR REPLACE FUNCTION public."Room_List"(
	pvar_tenantid character varying,
	pvar_block character varying,
	pvar_building character varying,
	pvar_floor character varying,
	pvar_roomtype character varying,
	pvar_roomnumber character varying
	,pvar_nextdaycheckin Varchar(1024)
,pvar_nextdaycheckout Varchar(1024)
)
    RETURNS TABLE(tenantid uuid, _tenantname character varying, roomid uuid, roomcode character varying, block uuid, block_master character varying, building uuid, building_master character varying, floor uuid, floor_master character varying, roomtype uuid, roomtype_master character varying, costperday numeric, advanceperday numeric, bookingdeposit numeric, attendantcostperday numeric, attendantadvanceperday numeric, attendantbookingdeposit numeric, roomtransfercost character varying, roomgroup uuid, roomgroup_master character varying, roomnumber character varying, roomimage character varying, nextdaycheckin character varying, nextdaycheckout character varying, createduser uuid, createddate timestamp without time zone, modifieduser uuid, modifieddate timestamp without time zone) 
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
    ROWS 1000

AS $BODY$
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;
              
			  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/17/2026 11:26:00*/
			  		
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
				Room.tenantid
,tenant.businessname as _tenantName
,Room.Roomid
,Room.roomcode
,Room.block
,CAST(_Block.blockcode||' '||_Block.blockname AS VARCHAR) as block_master
,Room.building
,CAST(__Building.buildingcode||' '||__Building.buildingname AS VARCHAR) as building_master
,Room.floor
,CAST(___Floor.floorname AS VARCHAR) as floor_master
,Room.roomtype
,CAST(____RoomType.name AS VARCHAR) as roomtype_master
,Room.costperday
,Room.advanceperday
,Room.bookingdeposit
,Room.attendantcostperday
,Room.attendantadvanceperday
,Room.attendantbookingdeposit
,Room.roomtransfercost
,Room.roomgroup
,CAST(_____RoomGroup.groupname||' '||_____RoomGroup.groupnumber AS VARCHAR) as roomgroup_master
,Room.roomnumber
,Room.roomimage
,Room.nextdaycheckin
,Room.nextdaycheckout

				
				,Room.createduser,Room.createddate,Room.modifieduser,Room.modifieddate
				FROM  Room 
 LEFT OUTER JOIN tenant ON Room.tenantid=tenant.tenantid
INNER JOIN Block _Block ON Room.block=_Block.Blockid
INNER JOIN Building __Building ON Room.building=__Building.Buildingid
INNER JOIN Floor ___Floor ON Room.floor=___Floor.Floorid
INNER JOIN RoomType ____RoomType ON Room.roomtype=____RoomType.RoomTypeid
INNER JOIN RoomGroup _____RoomGroup ON Room.roomgroup=_____RoomGroup.RoomGroupid

				WHERE (lvar_tenantid is null or COALESCE(cast(Room.tenantid as varchar), '') = Any(lvar_tenantid)) AND Room.isdeleted=false
AND (pvar_block is null or pvar_block ='0' or LENGTH(CAST(pvar_block as Varchar))=0 or CAST(Room.block as VARCHAR)=pvar_block)
AND (pvar_building is null or pvar_building ='0' or LENGTH(CAST(pvar_building as Varchar))=0 or CAST(Room.building as VARCHAR)=pvar_building)
AND (pvar_floor is null or pvar_floor ='0' or LENGTH(CAST(pvar_floor as Varchar))=0 or CAST(Room.floor as VARCHAR)=pvar_floor)
AND (pvar_roomtype is null or pvar_roomtype ='0' or LENGTH(CAST(pvar_roomtype as Varchar))=0 or CAST(Room.roomtype as VARCHAR)=pvar_roomtype)
AND (pvar_roomnumber is null or pvar_roomnumber ='0' or LENGTH(CAST(pvar_roomnumber as Varchar))=0 or CAST(Room.roomnumber as VARCHAR)=pvar_roomnumber)
AND (pvar_nextdaycheckin is null or pvar_nextdaycheckin ='0' or LENGTH(CAST(pvar_nextdaycheckin as Varchar))=0 or CAST(Room.nextdaycheckin as VARCHAR)=pvar_nextdaycheckin)
AND (pvar_nextdaycheckout is null or pvar_nextdaycheckout ='0' or LENGTH(CAST(pvar_nextdaycheckout as Varchar))=0 or CAST(Room.nextdaycheckout as VARCHAR)=pvar_nextdaycheckout)

			ORDER BY ____RoomType.name, _____RoomGroup.groupname, _____RoomGroup.groupnumber;
			  
					 	
			  END
              
$BODY$;

