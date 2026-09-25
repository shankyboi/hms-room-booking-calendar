
CREATE TABLE IF NOT EXISTS public.peopletimepreference
(
    peopletimepreferenceid uuid NOT NULL,
    tenantid uuid,
    viewertenantids jsonb,
    shiftname uuid NOT NULL,
    people uuid NOT NULL,
    createduser uuid NOT NULL,
    createddate timestamp(3) without time zone NOT NULL DEFAULT now(),
    modifieduser uuid,
    modifieddate timestamp(3) without time zone,
    isdeleted boolean DEFAULT false,
    workprofile uuid NOT NULL,
    CONSTRAINT peopletimepreference_pkey PRIMARY KEY (peopletimepreferenceid),
    CONSTRAINT peopletimepreference_people_fkey FOREIGN KEY (people)
        REFERENCES public.people (peopleid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT peopletimepreference_shiftname_fkey FOREIGN KEY (shiftname)
        REFERENCES public.shift (shiftid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT peopletimepreference_workprofile_fkey FOREIGN KEY (workprofile)
        REFERENCES public.workprofile (workprofileid) MATCH SIMPLE
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
)

