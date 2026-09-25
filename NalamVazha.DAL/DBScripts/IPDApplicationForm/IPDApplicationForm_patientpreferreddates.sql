CREATE TABLE IF NOT EXISTS IPDApplicationForm_patientpreferreddates (
    IPDApplicationForm_patientpreferreddatesid uuid PRIMARY KEY,
    IPDApplicationFormid uuid NOT NULL,
    dateofarrival date NOT NULL,
    dateofdeparture date NOT NULL,
    daysofstay integer NOT NULL,
    record_order integer,
    cma_client_row_id character varying(256),
    createduser uuid,
    createddate timestamp without time zone DEFAULT NOW(),
    modifieduser uuid,
    modifieddate timestamp without time zone,
    isdeleted boolean DEFAULT false
);

CREATE TABLE IF NOT EXISTS IPDApplicationForm_attendantpreferreddates (
    IPDApplicationForm_attendantpreferreddatesid uuid PRIMARY KEY,
    IPDApplicationFormid uuid NOT NULL,
    dateofarrivalatt date NOT NULL,
    dateofdepartureatt date NOT NULL,
    daysofstayatt integer,
    record_order integer,
    cma_client_row_id character varying(256),
    createduser uuid,
    createddate timestamp without time zone DEFAULT NOW(),
    modifieduser uuid,
    modifieddate timestamp without time zone,
    isdeleted boolean DEFAULT false
);

CREATE TABLE IF NOT EXISTS IPDApplicationForm_patientroompreference (
    IPDApplicationForm_patientroompreferenceid uuid PRIMARY KEY,
    IPDApplicationFormid uuid NOT NULL,
    roomtype uuid NOT NULL,
    record_order integer,
    cma_client_row_id character varying(256),
    createduser uuid,
    createddate timestamp without time zone DEFAULT NOW(),
    modifieduser uuid,
    modifieddate timestamp without time zone,
    isdeleted boolean DEFAULT false
);

CREATE TABLE IF NOT EXISTS IPDApplicationForm_attendantroompreference (
    IPDApplicationForm_attendantroompreferenceid uuid PRIMARY KEY,
    IPDApplicationFormid uuid NOT NULL,
    roomtypeatt uuid,
    record_order integer,
    cma_client_row_id character varying(256),
    createduser uuid,
    createddate timestamp without time zone DEFAULT NOW(),
    modifieduser uuid,
    modifieddate timestamp without time zone,
    isdeleted boolean DEFAULT false
);

CREATE INDEX IF NOT EXISTS ix_ipd_patientpreferreddates_ipdid ON IPDApplicationForm_patientpreferreddates(IPDApplicationFormid);
CREATE INDEX IF NOT EXISTS ix_ipd_attendantpreferreddates_ipdid ON IPDApplicationForm_attendantpreferreddates(IPDApplicationFormid);
CREATE INDEX IF NOT EXISTS ix_ipd_patientroompreference_ipdid ON IPDApplicationForm_patientroompreference(IPDApplicationFormid);
CREATE INDEX IF NOT EXISTS ix_ipd_attendantroompreference_ipdid ON IPDApplicationForm_attendantroompreference(IPDApplicationFormid);
