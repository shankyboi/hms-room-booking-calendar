-- FUNCTION: public.Update_Floor(uuid, uuid, uuid, uuid, character varying, bigint, uuid)

-- DROP FUNCTION IF EXISTS public."Update_Floor"(uuid, uuid, uuid, uuid, character varying, bigint, uuid);

CREATE OR REPLACE FUNCTION public."Update_Floor"(
	pvar_floorid uuid,
	pvar_tenantid uuid,
	pvar_block uuid,
	pvar_building uuid,
	pvar_floorname character varying,
	pvar_floornumber bigint,
	pvar_modifieduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/09/2026 07:02:00*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Floor', 'edit') THEN

			  pvar_returnMessage:='';

			 IF EXISTS (
    SELECT 1
    FROM Floor f
    WHERE UPPER(f.floorname) = UPPER(pvar_floorname)
      AND f.tenantid = pvar_tenantid
      AND f.building = pvar_building
      AND f.block = pvar_block
      --AND COALESCE(f.isdeleted, false) = false
      AND f.floorid <> pvar_floorid
)
THEN
    pvar_returnMessage := pvar_returnMessage || 'Floor Name Already Exists.';
END IF;

			IF EXISTS (
    SELECT 1
    FROM Floor f
    WHERE f.floornumber = pvar_floornumber
      AND f.tenantid = pvar_tenantid
      AND f.building = pvar_building
      AND f.block = pvar_block
      --AND COALESCE(f.isdeleted, false) = false
      AND f.floorid <> pvar_floorid
)
THEN
    pvar_returnMessage := pvar_returnMessage || 'Floor Number Already Exists.';
END IF;


			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Floor', NOW(),
(SELECT query_to_xml('SELECT * FROM Floor WHERE Floor.Floorid= '''||pvar_Floorid||'''', true, false, '')));

                    
                    UPDATE Floor SET
                    block=pvar_block
,building=pvar_building
,floorname=pvar_floorname
,floornumber=pvar_floornumber
                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Floorid=pvar_Floorid;

                    

                    

					
							
					pvar_returnMessage :='201.1';
			
			  END IF;

			  
																ELSE
																

															
																INSERT INTO system_logging
																(
																Log_code
																,system_logging_guid
																,log_application
																,log_date
																,log_level
																,log_logger
																,log_message
																,log_user_name
																)
																VALUES
																('401.1'
																,gen_random_uuid()
																,'Store Proc Authorization Check'
																,NOW()
																,'Critical'
																,'Update_Floor'
																,'Authorization Failed Update_Floor'
																,pvar_modifieduser
																);
																pvar_returnMessage = '401.1';
																
																END IF;

			  			 /* EXCEPTION WHEN OTHERS THEN
			 
						INSERT INTO system_logging
						(
						Log_code
						,system_logging_guid
						,log_application
						,log_date
						,log_level
						,log_logger
						,log_message
						)
						VALUES
						('16'
						,gen_random_uuid()
						,'Postgre Function Exception'
						,NOW()
						,'16'
						,'Update_Floor'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Floor - update failed';*/
			  	
			  END
              
$BODY$;
