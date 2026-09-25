CREATE TABLE IF NOT EXISTS RoomType_refundpolicy
(
RoomType_refundpolicyid uuid PRIMARY KEY
,RoomTypeid uuid REFERENCES RoomType(RoomTypeid)
,record_order int
,refundtype Varchar(1080) NOT NULL
,cancellationby Varchar(1080) NOT NULL
,cancellationwindowdays int NOT NULL
,refundpercentage decimal(18,2) NOT NULL
);


