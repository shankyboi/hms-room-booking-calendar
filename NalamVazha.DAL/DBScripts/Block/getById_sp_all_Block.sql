
			  CREATE OR REPLACE FUNCTION  "getById_sp_all_Block"
              (
			  pvar_Blockid Varchar
			  )
              RETURNS TABLE(
                tenantid uuid
,_tenantname Varchar
,"Blockid" uuid
,blockcode Varchar
,blockname Varchar
,blockdescription Varchar
,blockimage Varchar
,blocklocationurl Varchar
,blocknearbylandmark Varchar
,createduser uuid
,createddate Timestamp(5)
,modifieduser uuid
,modifieddate Timestamp(5)

                
                
                
				
                )

              AS $BODY$
                BEGIN
			   
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:04*/
			  		 
              RETURN QUERY
			  SELECT  
				 Block.tenantid
,tenant.businessname as _tenantname
,Block.Blockid
,Block.blockcode
,Block.blockname
,Block.blockdescription
,Block.blockimage
,Block.blocklocationurl
,Block.blocknearbylandmark

				 ,Block.createduser,Block.createddate,Block.modifieduser,Block.modifieddate
                 
                 
				 
			  FROM  Block 
 LEFT OUTER JOIN tenant ON Block.tenantid=tenant.tenantid

			  WHERE CAST(Block.Blockid AS Varchar)=pvar_Blockid ;
			  
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

