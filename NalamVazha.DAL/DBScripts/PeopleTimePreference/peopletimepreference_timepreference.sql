
CREATE TABLE IF NOT EXISTS public.peopletimepreference_timepreference
(
    peopletimepreference_timepreferenceid uuid NOT NULL,
    peopletimepreferenceid uuid,
    record_order integer,
    clinicaltask character varying(256) COLLATE pg_catalog."default" NOT NULL,
    tasktype uuid,
    taskname uuid,
    availableon character varying(1080) COLLATE pg_catalog."default" DEFAULT ''::character varying,
    taskstarttime character varying(10) COLLATE pg_catalog."default" NOT NULL,
    taskendtime character varying(10) COLLATE pg_catalog."default" NOT NULL,
    taskhours numeric(18,2) NOT NULL,
    CONSTRAINT peopletimepreference_timepreference_pkey PRIMARY KEY (peopletimepreference_timepreferenceid),
    CONSTRAINT peopletimepreference_timepreference_peopletimepreferenceid_fkey FOREIGN KEY (peopletimepreferenceid)
        REFERENCES public.peopletimepreference (peopletimepreferenceid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT peopletimepreference_timepreference_taskname_fkey FOREIGN KEY (taskname)
        REFERENCES public.task (taskid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT peopletimepreference_timepreference_tasktype_fkey FOREIGN KEY (tasktype)
        REFERENCES public.tasktype (tasktypeid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)
