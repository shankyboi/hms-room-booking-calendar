
			  CREATE OR REPLACE FUNCTION  "Update_Block"
			  (
				  pvar_Blockid uuid
,pvar_tenantid uuid
,
pvar_blockcode Varchar(128)
,
pvar_blockname Varchar(128)
,
pvar_blockdescription Varchar(128)
,
pvar_blockimage Varchar(256)
,
pvar_blocklocationurl Varchar(256)
,
pvar_blocknearbylandmark Varchar(128)

				  ,pvar_modifieduser  uuid  

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:04*/
			  IF "Check_Authorization"(pvar_modifieduser, 'Block', 'edit') THEN


			  pvar_returnMessage:='';

			  if EXISTS (SELECT * from Block where upper(Block.blockcode) = upper(pvar_blockcode) and Block.tenantid=pvar_tenantid  and Block.Blockid <> pvar_Blockid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Block Code Already Exists.';

																END IF;
if EXISTS (SELECT * from Block where upper(Block.blockname) = upper(pvar_blockname) and Block.tenantid=pvar_tenantid  and Block.Blockid <> pvar_Blockid)
																THEN

																  pvar_returnMessage := pvar_returnMessage||'Block Name Already Exists.';

																END IF;

                
			  IF(pvar_returnMessage='')
			  THEN
               
                    INSERT INTO history
VALUES('Block', NOW(),
(SELECT query_to_xml('SELECT * FROM Block WHERE Block.Blockid= '''||pvar_Blockid||'''', true, false, '')));

                    
                    UPDATE Block SET
                    blockcode=pvar_blockcode
,blockname=pvar_blockname
,blockdescription=pvar_blockdescription
,blockimage=pvar_blockimage
,blocklocationurl=pvar_blocklocationurl
,blocknearbylandmark=pvar_blocknearbylandmark

                    
                    ,modifieduser=pvar_modifieduser,modifieddate=NOW()
                    WHERE Blockid=pvar_Blockid;

                    

                    


					
							
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
																,'Update_Block'
																,'Authorization Failed Update_Block'
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
						,'Update_Block'
						,'update failed'
						);
                        pvar_returnMessage := 'Update_Block - update failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

