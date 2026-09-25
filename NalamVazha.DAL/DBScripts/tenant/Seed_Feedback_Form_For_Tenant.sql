CREATE OR REPLACE FUNCTION public."Seed_Feedback_Form_For_Tenant"
(
    pvar_tenantid uuid,
    pvar_createduser uuid,
    OUT pvar_returnmessage character varying
)
RETURNS character varying
LANGUAGE plpgsql
AS $BODY$
DECLARE
    v_template_id uuid;
    v_now timestamp := NOW();
BEGIN
    PERFORM pg_advisory_xact_lock(
        hashtext('Seed_Feedback_Form_For_Tenant'),
        hashtext(pvar_tenantid::text)
    );

    /* ============================================================
       1. CATEGORIES
       ============================================================ */

    CREATE TEMP TABLE tmp_feedback_categories
    (
        record_order integer,
        category_name varchar(128),
        category_description varchar(256),
        category_id uuid
    ) ON COMMIT DROP;

    INSERT INTO tmp_feedback_categories
    (
        record_order,
        category_name,
        category_description
    )
    VALUES
        (1, 'Stay and Facilities',
            'Feedback regarding accommodation and housekeeping.'),

        (2, 'Treatment Services',
            'Feedback regarding treatment arrangements and quality.'),

        (3, 'Kitchen and Food',
            'Feedback regarding kitchen service and food quality.'),

        (4, 'Clinical Services',
            'Feedback regarding doctor interaction and clinical services.'),

        (5, 'Wellness Services',
            'Feedback regarding counselling, hypnotherapy and yoga.'),

        (6, 'Therapy Services',
            'Feedback regarding physiotherapy, acupressure and acupuncture.'),

        (7, 'General Feedback',
            'Overall feedback and suggestions.');

    UPDATE tmp_feedback_categories c
       SET category_id = qc.QuestionCategoryid
      FROM QuestionCategory qc
     WHERE qc.Tenantid = pvar_tenantid
       AND LOWER(TRIM(qc.QuestionCategoryName))
           = LOWER(TRIM(c.category_name))
       AND qc.IsDeleted = FALSE;

    UPDATE tmp_feedback_categories
       SET category_id = gen_random_uuid()
     WHERE category_id IS NULL;

    INSERT INTO QuestionCategory
    (
        QuestionCategoryid,
        Tenantid,
        ViewerTenantIds,
        QuestionCategoryName,
        QuestionCategoryDesc,
        CreatedUser,
        CreatedDate,
        ModifiedUser,
        ModifiedDate,
        IsDeleted
    )
    SELECT
        c.category_id,
        pvar_tenantid,
        NULL,
        c.category_name,
        c.category_description,
        pvar_createduser,
        v_now,
        NULL,
        NULL,
        FALSE
    FROM tmp_feedback_categories c
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM QuestionCategory qc
        WHERE qc.QuestionCategoryid = c.category_id
    );

    /* ============================================================
       2. SUBCATEGORIES
       ============================================================ */

    CREATE TEMP TABLE tmp_feedback_subcategories
    (
        record_order integer,
        category_name varchar(128),
        subcategory_name varchar(128),
        subcategory_id uuid
    ) ON COMMIT DROP;

    INSERT INTO tmp_feedback_subcategories
    (
        record_order,
        category_name,
        subcategory_name
    )
    VALUES
        (1,  'Stay and Facilities', 'Accommodation'),
        (2,  'Stay and Facilities', 'Housekeeping'),

        (3,  'Treatment Services', 'Treatment Management'),
        (4,  'Treatment Services', 'Treatment Quality'),
        (5,  'Treatment Services', 'Enema'),
        (6,  'Treatment Services', 'Massage'),

        (7,  'Kitchen and Food', 'Kitchen Services'),
        (8,  'Kitchen and Food', 'Food Quality'),

        (9,  'Clinical Services', 'Doctor Interaction'),

        (10, 'Wellness Services', 'Counselling'),
        (11, 'Wellness Services', 'Hypnotherapy'),
        (12, 'Wellness Services', 'Yoga Classes'),

        (13, 'Therapy Services', 'Physiotherapy'),
        (14, 'Therapy Services', 'Acupressure'),
        (15, 'Therapy Services', 'Acupuncture'),

        (16, 'General Feedback', 'Overall Suggestions');

    UPDATE tmp_feedback_subcategories s
       SET subcategory_id = qsc.QuestionSubCategoryid
      FROM QuestionSubCategory qsc
      JOIN tmp_feedback_categories c
        ON c.category_id = qsc.QuestionCategoryname
     WHERE qsc.Tenantid = pvar_tenantid
       AND c.category_name = s.category_name
       AND LOWER(TRIM(qsc.QuestionSubCategoryName))
           = LOWER(TRIM(s.subcategory_name))
       AND qsc.IsDeleted = FALSE;

    UPDATE tmp_feedback_subcategories
       SET subcategory_id = gen_random_uuid()
     WHERE subcategory_id IS NULL;

    INSERT INTO QuestionSubCategory
    (
        QuestionSubCategoryid,
        Tenantid,
        ViewerTenantIds,
        QuestionCategoryname,
        QuestionSubCategoryName,
        QuestionSubCategoryDesc,
        CreatedUser,
        CreatedDate,
        ModifiedUser,
        ModifiedDate,
        IsDeleted
    )
    SELECT
        s.subcategory_id,
        pvar_tenantid,
        NULL,
        c.category_id,
        s.subcategory_name,
        NULL,
        pvar_createduser,
        v_now,
        NULL,
        NULL,
        FALSE
    FROM tmp_feedback_subcategories s
    JOIN tmp_feedback_categories c
      ON c.category_name = s.category_name
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM QuestionSubCategory qsc
        WHERE qsc.QuestionSubCategoryid = s.subcategory_id
    );

    /* ============================================================
       3. QUESTIONS

       Rating:
       AnswerType  = SingleSelect
       OptionText  = Excellent,Good,Average,Poor,Bad
       OptionValue = 5,4,3,2,1

       Suggestion:
       AnswerType = MultilineText
       ============================================================ */

    CREATE TEMP TABLE tmp_feedback_questions
    (
        record_order integer,
        reference_number varchar(128),
        category_name varchar(128),
        subcategory_name varchar(128),
        question_text varchar(128),
        answer_type varchar(32),
        option_text varchar(128),
        option_value varchar(128),
        scale_min integer,
        scale_max integer,
        is_required boolean,
        question_id uuid
    ) ON COMMIT DROP;

    INSERT INTO tmp_feedback_questions
    (
        record_order,
        reference_number,
        category_name,
        subcategory_name,
        question_text,
        answer_type,
        option_text,
        option_value,
        scale_min,
        scale_max,
        is_required
    )
    VALUES
        (1, 'FB-001-R', 'Stay and Facilities', 'Accommodation',
         'How would you rate the accommodation facilities?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (2, 'FB-001-S', 'Stay and Facilities', 'Accommodation',
         'Please provide suggestions regarding accommodation facilities.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (3, 'FB-002-R', 'Treatment Services', 'Treatment Management',
         'How would you rate the treatment management and arrangements?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (4, 'FB-002-S', 'Treatment Services', 'Treatment Management',
         'Please provide suggestions regarding treatment management.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (5, 'FB-003-R', 'Treatment Services', 'Treatment Quality',
         'How would you rate the morning treatments from 8:30 AM to 12:00 PM?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (6, 'FB-003-S', 'Treatment Services', 'Treatment Quality',
         'Please provide suggestions regarding morning treatments.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (7, 'FB-004-R', 'Treatment Services', 'Treatment Quality',
         'How would you rate the afternoon treatments from 2:00 PM to 6:00 PM?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (8, 'FB-004-S', 'Treatment Services', 'Treatment Quality',
         'Please provide suggestions regarding afternoon treatments.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (9, 'FB-005-R', 'Treatment Services', 'Enema',
         'How would you rate the quality of the Enema treatment?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (10, 'FB-005-S', 'Treatment Services', 'Enema',
         'Please provide suggestions regarding the Enema treatment.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (11, 'FB-006-R', 'Treatment Services', 'Massage',
         'How would you rate the quality of the Massage treatment?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (12, 'FB-006-S', 'Treatment Services', 'Massage',
         'Please provide suggestions regarding the Massage treatment.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (13, 'FB-007-R', 'Kitchen and Food', 'Kitchen Services',
         'How would you rate the kitchen services?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (14, 'FB-007-S', 'Kitchen and Food', 'Kitchen Services',
         'Please provide suggestions regarding kitchen services.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (15, 'FB-008-R', 'Kitchen and Food', 'Food Quality',
         'How would you rate the quality of the food served?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (16, 'FB-008-S', 'Kitchen and Food', 'Food Quality',
         'Please provide suggestions regarding food quality.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (17, 'FB-009-R', 'Stay and Facilities', 'Housekeeping',
         'How would you rate the cleanliness and housekeeping services?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (18, 'FB-009-S', 'Stay and Facilities', 'Housekeeping',
         'Please provide suggestions regarding cleanliness and housekeeping.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (19, 'FB-010-R', 'Clinical Services', 'Doctor Interaction',
         'How would you rate your interaction with the doctor?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (20, 'FB-010-S', 'Clinical Services', 'Doctor Interaction',
         'Please provide suggestions regarding your interaction with the doctor.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (21, 'FB-011-R', 'Wellness Services', 'Counselling',
         'How would you rate the counselling service?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (22, 'FB-011-S', 'Wellness Services', 'Counselling',
         'Please provide suggestions regarding counselling.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (23, 'FB-012-R', 'Wellness Services', 'Hypnotherapy',
         'How would you rate the hypnotherapy service?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (24, 'FB-012-S', 'Wellness Services', 'Hypnotherapy',
         'Please provide suggestions regarding hypnotherapy.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (25, 'FB-013-R', 'Wellness Services', 'Yoga Classes',
         'How would you rate the morning Yoga class from 6:30 AM to 8:00 AM?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (26, 'FB-013-S', 'Wellness Services', 'Yoga Classes',
         'Please provide suggestions regarding the morning Yoga class.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (27, 'FB-014-R', 'Wellness Services', 'Yoga Classes',
         'How would you rate the evening Yoga class?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (28, 'FB-014-S', 'Wellness Services', 'Yoga Classes',
         'Please provide suggestions regarding the evening Yoga class.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (29, 'FB-015-R', 'Therapy Services', 'Physiotherapy',
         'How would you rate the physiotherapy service?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (30, 'FB-015-S', 'Therapy Services', 'Physiotherapy',
         'Please provide suggestions regarding physiotherapy.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (31, 'FB-016-R', 'Therapy Services', 'Acupressure',
         'How would you rate the acupressure treatment?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (32, 'FB-016-S', 'Therapy Services', 'Acupressure',
         'Please provide suggestions regarding acupressure.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (33, 'FB-017-R', 'Therapy Services', 'Acupuncture',
         'How would you rate the acupuncture treatment?',
         'SingleSelect', 'Excellent,Good,Average,Poor,Bad', '5,4,3,2,1',
         NULL, NULL, TRUE),

        (34, 'FB-017-S', 'Therapy Services', 'Acupuncture',
         'Please provide suggestions regarding acupuncture.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE),

        (35, 'FB-018-S', 'General Feedback', 'Overall Suggestions',
         'Please provide any additional feedback or suggestions.',
         'MultilineText', NULL, NULL, NULL, NULL, FALSE);

    /* Reuse questions already seeded for this tenant. */

    UPDATE tmp_feedback_questions q
       SET question_id = aq.AssessmentQuestionid
      FROM AssessmentQuestion aq
     WHERE aq.Tenantid = pvar_tenantid
       AND LOWER(TRIM(aq.QuestionnaireReferenceNumber))
           = LOWER(TRIM(q.reference_number))
       AND aq.IsDeleted = FALSE;

    UPDATE tmp_feedback_questions
       SET question_id = gen_random_uuid()
     WHERE question_id IS NULL;

    INSERT INTO AssessmentQuestion
    (
        AssessmentQuestionid,
        Tenantid,
        ViewerTenantIds,
        QuestionnaireReferenceNumber,
        questioncategory,
        questionsubcategory,
        QuestionText,
        AnswerType,
        OptionText,
        OptionValue,
        DefaultValue,
        ScaleRangeMin,
        ScaleRangeMax,
        IsRequired,
        ScoreValue,
        CreatedUser,
        CreatedDate,
        ModifiedUser,
        ModifiedDate,
        IsDeleted
    )
    SELECT
        q.question_id,
        pvar_tenantid,
        NULL,
        q.reference_number,
        c.category_id,
        s.subcategory_id,
        q.question_text,
        q.answer_type,
        q.option_text,
        q.option_value,
        NULL,
        q.scale_min,
        q.scale_max,
        q.is_required,
        NULL,
        pvar_createduser,
        v_now,
        NULL,
        NULL,
        FALSE
    FROM tmp_feedback_questions q
    JOIN tmp_feedback_categories c
      ON c.category_name = q.category_name
    JOIN tmp_feedback_subcategories s
      ON s.category_name = q.category_name
     AND s.subcategory_name = q.subcategory_name
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM AssessmentQuestion aq
        WHERE aq.AssessmentQuestionid = q.question_id
    );

    /* ============================================================
       4. ASSESSMENT TEMPLATE
       ============================================================ */

    SELECT AssessmentTemplateID
      INTO v_template_id
      FROM AssessmentTemplate
     WHERE TenantID = pvar_tenantid
       AND LOWER(TRIM(TemplateName)) = 'feedback form'
       AND IsDeleted = FALSE
     ORDER BY CreatedDate NULLS LAST
     LIMIT 1;

    IF v_template_id IS NULL THEN
        v_template_id := gen_random_uuid();

        INSERT INTO AssessmentTemplate
        (
            AssessmentTemplateID,
            TenantID,
            ViewerTenantIds,
            TemplateName,
            TaskName,
            IsDefaultTemplate,
            CreatedUser,
            CreatedDate,
            ModifiedUser,
            ModifiedDate,
            IsDeleted
        )
        VALUES
        (
            v_template_id,
            pvar_tenantid,
            NULL,
            'Feedback Form',
            'FeedBack form',
            TRUE,
            pvar_createduser,
            v_now,
            NULL,
            NULL,
            FALSE
        );
    ELSE
        UPDATE AssessmentTemplate
           SET TaskName = 'FeedBack form',
               IsDefaultTemplate = TRUE,
               ModifiedUser = pvar_createduser,
               ModifiedDate = v_now
         WHERE AssessmentTemplateID = v_template_id;
    END IF;

    /* ============================================================
       5. CONNECT QUESTIONS TO TEMPLATE
       ============================================================ */

    INSERT INTO assessmenttemplate_templatequestions
    (
        assessmenttemplate_templatequestionsid,
        assessmenttemplateid,
        record_order,
        questioncategory,
        questionsub,
        question,
        optiontext,
        optionvalue,
        defaultvalue,
        answertype,
        scalerangemin,
        scalerangemax,
        isrequired,
        scorevalue
    )
    SELECT
        gen_random_uuid(),
        v_template_id,
        q.record_order,
        aq.questioncategory,
        aq.questionsubcategory,
        aq.AssessmentQuestionid,
        aq.OptionText,
        aq.OptionValue,
        aq.DefaultValue,
        aq.AnswerType,
        aq.ScaleRangeMin,
        aq.ScaleRangeMax,
        aq.IsRequired,
        aq.ScoreValue
    FROM tmp_feedback_questions q
    JOIN AssessmentQuestion aq
      ON aq.AssessmentQuestionid = q.question_id
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM assessmenttemplate_templatequestions tq
        WHERE tq.assessmenttemplateid = v_template_id
          AND tq.question = q.question_id
    );

    pvar_returnmessage := '201.1';

EXCEPTION
    WHEN OTHERS THEN
        pvar_returnmessage := SQLERRM;
END;
$BODY$;