CREATE OR REPLACE FUNCTION "getById_sp_DailyTask_nextactiondetails"(
												 pvar_DailyTaskid Varchar(50)
											 )
                                             RETURNS TABLE("DailyTaskid" uuid,"DailyTask_nextactiondetailsid" uuid ,nextaction Varchar
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 DailyTask_nextactiondetails.DailyTaskid
                                             ,DailyTask_nextactiondetails.DailyTask_nextactiondetailsid   
											 ,DailyTask_nextactiondetails.nextaction
 
											 
											 FROM DailyTask_nextactiondetails
											 WHERE 
											 CAST(DailyTask_nextactiondetails.DailyTaskid AS VARCHAR)=pvar_DailyTaskid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

