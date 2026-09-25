CREATE OR REPLACE FUNCTION public."prefill_Assessment_assessmentquestions"
(
    pvar_questionnairetemplate uuid
)
RETURNS TABLE
(
    questions character varying,
    optiontext character varying,
    optionvalue character varying,
    defaultvalue character varying,
    questioncategory uuid,
    questionsub uuid,
    question uuid,
    answertype character varying,
    scalerangemin integer,
    scalerangemax integer,
    isrequired boolean,
    scorevalue integer,
    record_order integer
)
LANGUAGE plpgsql
STABLE
AS $BODY$
BEGIN
    RETURN QUERY
    SELECT
        tq.assessmenttemplate_templatequestionsid::character varying AS questions,
        tq.optiontext::character varying,
        tq.optionvalue::character varying,
        tq.defaultvalue::character varying,
        tq.questioncategory,
        tq.questionsub,
        tq.question,
        COALESCE(
            NULLIF(BTRIM(tq.answertype::text), ''),
            NULLIF(BTRIM(aq.answertype::text), '')
        )::character varying AS answertype,
        tq.scalerangemin,
        tq.scalerangemax,
        tq.isrequired,
        tq.scorevalue,
        tq.record_order
    FROM assessmenttemplate_templatequestions tq
    INNER JOIN assessmenttemplate template
        ON template.assessmenttemplateid = tq.assessmenttemplateid
    LEFT JOIN assessmentquestion aq
        ON aq.assessmentquestionid = tq.question
    WHERE tq.assessmenttemplateid = pvar_questionnairetemplate
      AND COALESCE(template.isdeleted, false) = false
    ORDER BY
        COALESCE(tq.record_order, 0),
        tq.assessmenttemplate_templatequestionsid;
END;
$BODY$;

