 
			  
			  CREATE OR REPLACE FUNCTION  "getById_sp_PincodeMaster"
			  (
				  pvar_PincodeMasterid Varchar
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

                ,PincodeMasterid uuid
                
            )
            AS $BODY$
            BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/16/2026 09:42:32*/
               
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
			  WHERE CAST(PincodeMaster.PincodeMasterid AS Varchar)=pvar_PincodeMasterid
                       ;

					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

