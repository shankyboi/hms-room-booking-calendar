CREATE TABLE IF NOT EXISTS ShiftPlanning_people_upload
(
    ShiftPlanning_people_uploadid uuid,
   ShiftPlanning_people_uploadfile character varying(256) COLLATE pg_catalog."default",
    createddate timestamp(3) without time zone NOT NULL DEFAULT now(),
  createdby uuid
)

