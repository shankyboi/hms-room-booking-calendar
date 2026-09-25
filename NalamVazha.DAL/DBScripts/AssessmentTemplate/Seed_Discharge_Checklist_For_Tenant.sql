
CREATE OR REPLACE FUNCTION public."Seed_Discharge_Checklist_For_Tenant"(
    pvar_tenantid uuid,
    pvar_createduser uuid,
    OUT pvar_returnmessage character varying)
RETURNS character varying
LANGUAGE plpgsql
AS $BODY$
DECLARE
    v_category_id uuid;
    v_template_id uuid;
    v_now timestamp := NOW();
    v_ref_date text := TO_CHAR(NOW(), 'YYYYMMDD');
    v_ref_start bigint;
    v_question_ids uuid[];
BEGIN
    PERFORM pg_advisory_xact_lock(
        hashtext('Seed_Discharge_Checklist_For_Tenant'),
        hashtext(pvar_tenantid::text)
    );

    -- One top-level assessment category for this standalone checklist.
    SELECT QuestionCategoryid
      INTO v_category_id
      FROM QuestionCategory
     WHERE Tenantid = pvar_tenantid
       AND LOWER(TRIM(QuestionCategoryName)) = 'discharge checklist'
       AND IsDeleted = FALSE
     ORDER BY CreatedDate NULLS LAST
     LIMIT 1;

    IF v_category_id IS NULL THEN
        v_category_id := gen_random_uuid();
        INSERT INTO QuestionCategory
        (
            QuestionCategoryid, Tenantid, ViewerTenantIds,
            QuestionCategoryName, QuestionCategoryDesc,
            CreatedUser, CreatedDate, ModifiedUser, ModifiedDate, IsDeleted
        )
        VALUES
        (
            v_category_id, pvar_tenantid, NULL,
            'Discharge Checklist',
            'Items, payments, notes and confirmations required before an indoor patient is discharged.',
            pvar_createduser, v_now, NULL, NULL, FALSE
        );
    END IF;

    -- Create/reuse the document's operational sections as subcategories.
    CREATE TEMP TABLE tmp_discharge_subcategories
    (
        record_order integer,
        subcategory_name varchar(128),
        subcategory_id uuid
    ) ON COMMIT DROP;

    INSERT INTO tmp_discharge_subcategories(record_order, subcategory_name)
    VALUES
        (1, 'Treatment Items'),
        (2, 'Library Items'),
        (3, 'Accounts Clearance'),
        (4, 'Reception Items and Clearance');

    UPDATE tmp_discharge_subcategories s
       SET subcategory_id = q.QuestionSubCategoryid
      FROM QuestionSubCategory q
     WHERE q.Tenantid = pvar_tenantid
       AND q.QuestionCategoryname = v_category_id
       AND LOWER(TRIM(q.QuestionSubCategoryName)) = LOWER(TRIM(s.subcategory_name))
       AND q.IsDeleted = FALSE;

    UPDATE tmp_discharge_subcategories
       SET subcategory_id = gen_random_uuid()
     WHERE subcategory_id IS NULL;

    INSERT INTO QuestionSubCategory
    (
        QuestionSubCategoryid, Tenantid, ViewerTenantIds,
        QuestionCategoryname, QuestionSubCategoryName, QuestionSubCategoryDesc,
        CreatedUser, CreatedDate, ModifiedUser, ModifiedDate, IsDeleted
    )
    SELECT
        s.subcategory_id, pvar_tenantid, NULL,
        v_category_id, s.subcategory_name, NULL,
        pvar_createduser, v_now, NULL, NULL, FALSE
    FROM tmp_discharge_subcategories s
    WHERE NOT EXISTS
    (
        SELECT 1
          FROM QuestionSubCategory q
         WHERE q.QuestionSubCategoryid = s.subcategory_id
    );

    CREATE TEMP TABLE tmp_discharge_questions
    (
        record_order integer,
        subcategory_name varchar(128),
        question_text varchar(128),
        answer_type varchar(32),
        option_text varchar,
        option_value varchar,
        is_required boolean,
        question_id uuid
    ) ON COMMIT DROP;

    INSERT INTO tmp_discharge_questions
    (record_order, subcategory_name, question_text, answer_type, option_text, option_value, is_required)
    VALUES
        ( 1, 'Treatment Items', 'Lapet returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        ( 2, 'Treatment Items', 'Hot water bag returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        ( 3, 'Treatment Items', 'Ice pack returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),

        ( 4, 'Library Items', 'Books returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        ( 5, 'Library Items', 'Sports equipment returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),

        ( 6, 'Accounts Clearance', 'All payments done?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),

        ( 7, 'Reception Items and Clearance', 'Room keys returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        ( 8, 'Reception Items and Clearance', 'Cupboard keys returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        ( 9, 'Reception Items and Clearance', 'AC remote control returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        (10, 'Reception Items and Clearance', 'TV remote control returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        (11, 'Reception Items and Clearance', 'Tata Sky remote control returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        (12, 'Reception Items and Clearance', 'Items collected from reception returned?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        (13, 'Reception Items and Clearance', 'All payments done at reception?', 'Boolean', 'Yes,No', 'Yes,No', TRUE),
        (14, 'Reception Items and Clearance', 'ID proof returned and received?', 'Boolean', 'Yes,No', 'Yes,No', TRUE);

    UPDATE tmp_discharge_questions d
       SET question_id = aq.AssessmentQuestionid
      FROM AssessmentQuestion aq
      JOIN tmp_discharge_subcategories s
        ON s.subcategory_id = aq.questionsubcategory
     WHERE aq.Tenantid = pvar_tenantid
       AND aq.questioncategory = v_category_id
       AND s.subcategory_name = d.subcategory_name
       AND LOWER(TRIM(aq.QuestionText)) = LOWER(TRIM(d.question_text))
       AND aq.IsDeleted = FALSE;

    SELECT COALESCE(MAX(SPLIT_PART(QuestionnaireReferenceNumber, '-', 2)::bigint), 0)
      INTO v_ref_start
      FROM AssessmentQuestion
     WHERE Tenantid = pvar_tenantid
       AND QuestionnaireReferenceNumber ~ ('^' || v_ref_date || '-[0-9]+$');

    WITH missing AS
    (
        SELECT d.*, s.subcategory_id,
               ROW_NUMBER() OVER (ORDER BY d.record_order) AS new_number
          FROM tmp_discharge_questions d
          JOIN tmp_discharge_subcategories s USING (subcategory_name)
         WHERE d.question_id IS NULL
    ), inserted AS
    (
        INSERT INTO AssessmentQuestion
        (
            AssessmentQuestionid, Tenantid, ViewerTenantIds,
            QuestionnaireReferenceNumber, questioncategory, questionsubcategory,
            QuestionText, AnswerType, OptionText, OptionValue, DefaultValue,
            ScaleRangeMin, ScaleRangeMax, IsRequired, ScoreValue,
            CreatedUser, CreatedDate, ModifiedUser, ModifiedDate, IsDeleted
        )
        SELECT
            gen_random_uuid(), pvar_tenantid, NULL,
            v_ref_date || '-' || LPAD((v_ref_start + m.new_number)::text, 5, '0'),
            v_category_id, m.subcategory_id,
            m.question_text, m.answer_type, m.option_text, m.option_value, NULL,
            NULL, NULL, m.is_required, NULL,
            pvar_createduser, v_now, NULL, NULL, FALSE
        FROM missing m
        RETURNING AssessmentQuestionid, questionsubcategory, QuestionText
    )
    UPDATE tmp_discharge_questions d
       SET question_id = i.AssessmentQuestionid
      FROM inserted i
      JOIN tmp_discharge_subcategories s
        ON s.subcategory_id = i.questionsubcategory
     WHERE s.subcategory_name = d.subcategory_name
       AND i.QuestionText = d.question_text;

    SELECT ARRAY_AGG(question_id ORDER BY record_order)
      INTO v_question_ids
      FROM tmp_discharge_questions;

    SELECT AssessmentTemplateID
      INTO v_template_id
      FROM AssessmentTemplate
     WHERE TenantID = pvar_tenantid
       AND LOWER(TRIM(TemplateName)) = 'discharge checklist'
       AND IsDeleted = FALSE
     ORDER BY CreatedDate NULLS LAST
     LIMIT 1;

    IF v_template_id IS NULL THEN
        v_template_id := gen_random_uuid();
        INSERT INTO AssessmentTemplate
        (
            AssessmentTemplateID, TenantID, ViewerTenantIds, TemplateName,
            CreatedUser, CreatedDate, ModifiedUser, ModifiedDate, IsDeleted,
            TaskName, IsDefaultTemplate
        )
        VALUES
        (
            v_template_id, pvar_tenantid, NULL, 'Discharge Checklist',
            pvar_createduser, v_now, NULL, NULL, FALSE,
            'Discharge Checklist', TRUE
        );
    ELSE
        UPDATE AssessmentTemplate
           SET TaskName = 'Discharge Checklist',
               IsDefaultTemplate = TRUE,
               ModifiedUser = pvar_createduser,
               ModifiedDate = v_now
         WHERE AssessmentTemplateID = v_template_id;
    END IF;

    -- Keep this task limited to the 14 requested checklist rows when the seed
    -- is rerun against a template previously created with additional fields.
    DELETE FROM assessmenttemplate_templatequestions tq
     WHERE tq.assessmenttemplateid = v_template_id
       AND NOT (tq.question = ANY(v_question_ids));

    INSERT INTO assessmenttemplate_templatequestions
    (
        assessmenttemplate_templatequestionsid, assessmenttemplateid, record_order,
        questioncategory, questionsub, question,
        optiontext, optionvalue, defaultvalue, answertype,
        scalerangemin, scalerangemax, isrequired, scorevalue
    )
    SELECT
        gen_random_uuid(), v_template_id, d.record_order,
        aq.questioncategory, aq.questionsubcategory, aq.AssessmentQuestionid,
        aq.optiontext, aq.optionvalue, aq.defaultvalue, aq.answertype,
        aq.scalerangemin, aq.scalerangemax, aq.isrequired, aq.scorevalue
    FROM tmp_discharge_questions d
    JOIN AssessmentQuestion aq ON aq.AssessmentQuestionid = d.question_id
    WHERE NOT EXISTS
    (
        SELECT 1
          FROM assessmenttemplate_templatequestions tq
         WHERE tq.assessmenttemplateid = v_template_id
           AND tq.question = aq.AssessmentQuestionid
    );

    pvar_returnmessage := '201.1';
EXCEPTION WHEN OTHERS THEN
    pvar_returnmessage := SQLERRM;
END;
$BODY$;
