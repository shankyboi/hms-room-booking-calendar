
			  CREATE OR REPLACE FUNCTION  "Add_Block"
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
 
				  ,pvar_createduser  uuid 

				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
              DECLARE lv_viewactionroles Varchar(128);
              BEGIN

				/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:04*/
		

			  
                                                                                    if pvar_Blockid is null then
                                                                                    pvar_Blockid:=gen_random_uuid();
                                                                                    end if;	
                                                                                    
			  
			  IF "Check_Authorization"(pvar_createduser, 'Block', 'create') THEN
			  pvar_returnMessage:='';
			  IF EXISTS (SELECT * from Block where upper(Block.blockcode::varchar) = upper(pvar_blockcode::varchar) and Block.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Block Code Already Exists.';

																END IF;
IF EXISTS (SELECT * from Block where upper(Block.blockname::varchar) = upper(pvar_blockname::varchar) and Block.tenantid=pvar_tenantid)
																THEN

																pvar_returnMessage := pvar_returnMessage||'Block Name Already Exists.';

																END IF;

                
			  if(pvar_returnMessage='')
			  THEN

			  

			  INSERT INTO Block(
				 blockcode
,blockname
,blockdescription
,blockimage
,blocklocationurl
,blocknearbylandmark

				 ,createduser
				 ,Blockid
				 ,tenantid
                
			  )
			  VALUES (
 				 pvar_blockcode
,pvar_blockname
,pvar_blockdescription
,pvar_blockimage
,pvar_blocklocationurl
,pvar_blocknearbylandmark

				 ,pvar_createduser
				 ,pvar_Blockid
				 ,pvar_tenantid
                   
			  );
			   
               

			  


			  
					 
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
																,'Add_Block'
																,'Authorization Failed Add_Block'
																,pvar_createduser
																);
																pvar_returnMessage := '401.1';
																
																END IF;
			  /*EXCEPTION WHEN OTHERS THEN
			 
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
						,'Store Proc Exception'
						,NOW()
						,'16'
						,'Add_Block'
						,'insert failed'
						);
                        pvar_returnMessage := 'Add_Block - Insert failed';*/
			  	
			  END
              $BODY$
              LANGUAGE plpgsql;

