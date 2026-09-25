CREATE TABLE IF NOT EXISTS ShiftPlanning_people_bulkupload
(
ShiftPlanningid uuid
,ShiftPlanning_peopleid uuid
,ShiftPlanning_people_uploadfileid uuid
,record_order int
,personname Varchar(256)
,workprofile Varchar(256)
,coveragetype Varchar(256)
,action_date timestamp(3) without time zone NOT NULL DEFAULT now()
,action_by uuid
,action character varying(50) COLLATE pg_catalog."default"
,errordescription character varying(256) COLLATE pg_catalog."default"
,unalteredjson jsonb
,isdeleted boolean
);


