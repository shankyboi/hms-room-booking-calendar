 
			  
			  CREATE OR REPLACE FUNCTION  "get_all_Room"
              (
			  pvar_tenantid Varchar=null
              )
			 RETURNS TABLE(
                roomcode Varchar
,block uuid
,building uuid
,floor uuid
,roomtype uuid
,bookingdeposit decimal
,roomgroup uuid
,roomnumber Varchar
,roomimage Varchar
,occupancystatus Varchar
,maintenancestatus Varchar
,housekeepingstatus Varchar
,nextdaycheckin Varchar
,nextdaycheckout Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)
,tenantid uuid

                ,"Roomid" uuid
                 
               )
               AS $BODY$
                declare lvar_tenantid varchar[];
                declare lstr_usersid varchar;
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

				 Room.roomcode
,Room.block
,Room.building
,Room.floor
,Room.roomtype
,Room.bookingdeposit
,Room.roomgroup
,Room.roomnumber
,Room.roomimage
,Room.occupancystatus
,Room.maintenancestatus
,Room.housekeepingstatus
,Room.nextdaycheckin
,Room.nextdaycheckout

				 ,Room.createduser,Room.createddate,Room.modifieduser,Room.modifieddate
				,Room.tenantid 
                ,Room.Roomid
				 
			  FROM Room
			  WHERE
               (lvar_tenantid is null or COALESCE(cast(Room.tenantid as varchar),'') = Any(lvar_tenantid))
                 
			   AND Room.isdeleted=false
			  ;
			 

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

