
								CREATE OR REPLACE FUNCTION "lookup_change_roomtypes_TreatmentPackage_roomtype"
								(
                                pvar_RoomTypeid Varchar(50)=null
								)
                                RETURNS TABLE("RoomTypeid" Varchar
,name Varchar
,costperday Varchar
) 
						 		AS $BODY$
								BEGIN
								/*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:41:49*/
										
                                RETURN QUERY
								SELECT  
									CAST(RoomType.RoomTypeid AS Varchar) as RoomTypeid
,CAST(RoomType.name AS Varchar) as name
,CAST (RoomType.costperday AS VARCHAR) as costperday

								FROM RoomType
							    WHERE (CAST(RoomType.RoomTypeid AS VARCHAR) = pvar_RoomTypeid)
;
								
											
								
								END
                                $BODY$
                                LANGUAGE plpgsql;

