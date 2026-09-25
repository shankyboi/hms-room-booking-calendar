CREATE OR REPLACE FUNCTION "getById_sp_RoomType_refundpolicy"(
												 pvar_RoomTypeid Varchar(50)
											 )
                                             RETURNS TABLE("RoomTypeid" uuid,"RoomType_refundpolicyid" uuid ,refundtype Varchar
,cancellationby Varchar
,cancellationwindowdays int
,refundpercentage decimal
)
											 AS $BODY$
											 BEGIN
											 
											 RETURN QUERY
											 SELECT 
											 RoomType_refundpolicy.RoomTypeid
                                             ,RoomType_refundpolicy.RoomType_refundpolicyid   
											 ,RoomType_refundpolicy.refundtype
,RoomType_refundpolicy.cancellationby
,RoomType_refundpolicy.cancellationwindowdays
,RoomType_refundpolicy.refundpercentage
 
											 
											 FROM RoomType_refundpolicy
											 WHERE 
											 CAST(RoomType_refundpolicy.RoomTypeid AS VARCHAR)=pvar_RoomTypeid
                                               
                                             ORDER BY record_order DESC;
											
											 END
                                             $BODY$
                                             LANGUAGE plpgsql;

