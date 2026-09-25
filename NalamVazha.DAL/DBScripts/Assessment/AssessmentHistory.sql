CREATE TABLE IF NOT EXISTS "AssessmentHistory"
(
    assessmenthistoryid      uuid PRIMARY KEY,
    assessmentid             uuid NOT NULL,
    tenantid                 uuid NOT NULL,

    patientname              uuid NOT NULL,
    patientvisit             uuid NULL,
    ipdform                  uuid NULL,
    opdform                  uuid NULL,

    versionnumber            integer NOT NULL,
    actiontype               varchar(30) NOT NULL,
    workflowstage            varchar(50) NOT NULL,

    assessmentdate           timestamp NOT NULL,
    questionnairetemplate    uuid NOT NULL,
    doctorname               uuid NULL,
    assessedby               varchar(200) NULL,
    assessmentnotes          text NULL,
    eligibleforfinaladmission varchar(20) NULL,
    taskname                 varchar(200) NULL,

    assessment_snapshot      jsonb NOT NULL,

    actionby                 uuid NOT NULL,
    actionbyrole             varchar(100) NULL,
    actiondate               timestamp NOT NULL DEFAULT now(),

    reviewnotes              text NULL,
    previoushistoryid        uuid NULL,

    CONSTRAINT uq_assessment_history_version
        UNIQUE (assessmentid, versionnumber)
);

CREATE INDEX IF NOT EXISTS ix_assessmenthistory_assessment
    ON "AssessmentHistory" (assessmentid, versionnumber DESC);

CREATE INDEX IF NOT EXISTS ix_assessmenthistory_ipd
    ON "AssessmentHistory" (ipdform, actiondate DESC);

CREATE INDEX IF NOT EXISTS ix_assessmenthistory_opd
    ON "AssessmentHistory" (opdform, actiondate DESC);

CREATE INDEX IF NOT EXISTS ix_assessmenthistory_patient
    ON "AssessmentHistory" (patientname, actiondate DESC);
