-- FUNCTION: public.Seed_Template_For_Tenant(uuid, uuid)

-- DROP FUNCTION IF EXISTS public."Seed_Template_For_Tenant"(uuid, uuid);

CREATE OR REPLACE FUNCTION public."Seed_Template_For_Tenant"(
	pvar_tenantid uuid,
	pvar_createduser uuid,
	OUT pvar_returnmessage character varying)
    RETURNS character varying
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
DECLARE
    -- Question Category IDs
    v_cat_body_energy       uuid := gen_random_uuid();
    v_cat_sleep             uuid := gen_random_uuid();
    v_cat_gut_health        uuid := gen_random_uuid();
    v_cat_bowel_movement    uuid := gen_random_uuid();
    v_cat_fluid_balance     uuid := gen_random_uuid();
    v_cat_indicators        uuid := gen_random_uuid();
    v_cat_habits            uuid := gen_random_uuid();
    v_cat_womens_health     uuid := gen_random_uuid();
    v_cat_medical_history   uuid := gen_random_uuid();

    -- Question SubCategory IDs
    v_sub_body_energy_ind   uuid := gen_random_uuid();
    v_sub_body_energy_hab   uuid := gen_random_uuid();
    v_sub_sleep_ind         uuid := gen_random_uuid();
    v_sub_sleep_hab         uuid := gen_random_uuid();
    v_sub_gut_ind           uuid := gen_random_uuid();
    v_sub_gut_hab           uuid := gen_random_uuid();
    v_sub_bowel_ind         uuid := gen_random_uuid();
    v_sub_bowel_hab         uuid := gen_random_uuid();
    v_sub_fluid_ind         uuid := gen_random_uuid();
    v_sub_fluid_hab         uuid := gen_random_uuid();
    v_sub_fluid_habit       uuid := gen_random_uuid();
    v_sub_indicators        uuid := gen_random_uuid();
    v_sub_habits_ctx        uuid := gen_random_uuid();
    v_sub_habits_ctx2       uuid := gen_random_uuid();
    v_sub_womens_health     uuid := gen_random_uuid();
    v_sub_medical_history   uuid := gen_random_uuid();

    v_template_id           uuid := gen_random_uuid();
    v_question_ids          uuid[];
    v_template_question_count integer := 0;

    v_now                   timestamp := NOW();
    v_ref_date              text      := TO_CHAR(NOW(), 'YYYYMMDD');
    v_ref_start             bigint    := 0;
BEGIN
    -- Serialize seeding for the same tenant so concurrent executions cannot
    -- calculate and insert the same questionnaire reference numbers.
    PERFORM pg_advisory_xact_lock(
        hashtext('Seed_Template_For_Tenant'),
        hashtext(pvar_tenantid::text)
    );

    -- Continue after the highest reference already used today for this tenant.
    -- The previous implementation restarted at 00001 on every execution.
    SELECT COALESCE(MAX(SPLIT_PART(aq.QuestionnaireReferenceNumber, '-', 2)::bigint), 0)
      INTO v_ref_start
      FROM AssessmentQuestion aq
     WHERE aq.Tenantid = pvar_tenantid
       AND aq.QuestionnaireReferenceNumber ~ ('^' || v_ref_date || '-[0-9]+$');

    -- =========================================================
    -- INSERT Question Categories
    -- =========================================================
    INSERT INTO QuestionCategory
    (
        QuestionCategoryid, Tenantid, ViewerTenantIds,
        QuestionCategoryName, QuestionCategoryDesc,
        CreatedUser, CreatedDate, ModifiedUser, ModifiedDate, IsDeleted
    )
    VALUES
    (v_cat_body_energy,    pvar_tenantid, NULL, 'Body & Energy',    NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_sleep,          pvar_tenantid, NULL, 'Sleep',            NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_gut_health,     pvar_tenantid, NULL, 'Gut Health',       NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_bowel_movement, pvar_tenantid, NULL, 'Bowel Movement',   NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_fluid_balance,  pvar_tenantid, NULL, 'Fluid Balance',    NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_indicators,     pvar_tenantid, NULL, 'Indicators',       NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_habits,         pvar_tenantid, NULL, 'Habits',           NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_womens_health,  pvar_tenantid, NULL, 'Women''s Health',  NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_cat_medical_history,pvar_tenantid, NULL, 'Medical History',  NULL, pvar_createduser, v_now, NULL, NULL, FALSE);

    -- =========================================================
    -- INSERT Question SubCategories
    -- =========================================================
    INSERT INTO QuestionSubCategory
    (
        QuestionSubCategoryid, Tenantid, ViewerTenantIds,
        QuestionCategoryname, QuestionSubCategoryName, QuestionSubCategoryDesc,
        CreatedUser, CreatedDate, ModifiedUser, ModifiedDate, IsDeleted
    )
    VALUES
    (v_sub_body_energy_ind,  pvar_tenantid, NULL, v_cat_body_energy,    'Body & Energy - Indicators',           NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_body_energy_hab,  pvar_tenantid, NULL, v_cat_body_energy,    'Body & Energy - Habits & Context',     NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_sleep_ind,        pvar_tenantid, NULL, v_cat_sleep,          'Sleep - Indicators',                   NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_sleep_hab,        pvar_tenantid, NULL, v_cat_sleep,          'Sleep - Habits & Context',             NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_gut_ind,          pvar_tenantid, NULL, v_cat_gut_health,     'Gut Health & Diet - Indicators',       NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_gut_hab,          pvar_tenantid, NULL, v_cat_gut_health,     'Gut Health & Diet - Habits & Context', NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_bowel_ind,        pvar_tenantid, NULL, v_cat_bowel_movement, 'Bowel Movement - Indicators',          NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_bowel_hab,        pvar_tenantid, NULL, v_cat_bowel_movement, 'Bowel Movement - Habits & Context',    NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_fluid_ind,        pvar_tenantid, NULL, v_cat_fluid_balance,  'Fluid Balance - Indicators',           NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_fluid_hab,        pvar_tenantid, NULL, v_cat_fluid_balance,  'Fluid Balance - Habits',               NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_fluid_habit,      pvar_tenantid, NULL, v_cat_fluid_balance,  'Fluid Balance - Habit',                NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_indicators,       pvar_tenantid, NULL, v_cat_indicators,     'Indicators',                           NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_habits_ctx,       pvar_tenantid, NULL, v_cat_habits,         'Habits & Context',                     NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_habits_ctx2,      pvar_tenantid, NULL, v_cat_habits,         'Habits / Context',                     NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_womens_health,    pvar_tenantid, NULL, v_cat_womens_health,  'Menstrual & Reproductive History',     NULL, pvar_createduser, v_now, NULL, NULL, FALSE),
    (v_sub_medical_history,  pvar_tenantid, NULL, v_cat_medical_history,'Past, Family, Allergy & Treatment',    NULL, pvar_createduser, v_now, NULL, NULL, FALSE);

    -- =========================================================
    -- INSERT Assessment Questions
    -- QuestionnaireReferenceNumber auto-generated as YYYYMMDD-NNNNN
    -- continuing after the tenant's highest number for the current date
    -- =========================================================
    WITH inserted_questions AS (
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
        gen_random_uuid(),
        pvar_tenantid,
        NULL,
        v_ref_date || '-' || LPAD(
            (v_ref_start + ROW_NUMBER() OVER ())::text,
            GREATEST(5, LENGTH((v_ref_start + ROW_NUMBER() OVER ())::text)),
            '0'
        ),
        q.category_id,
        q.subcategory_id,
        q.question_text,
        q.answer_type,
        q.option_text,
        q.option_value,
        q.default_value,
      q.scale_min::integer,
    q.scale_max::integer,
        FALSE,
        NULL,
        pvar_createduser,
        v_now,
        NULL,
        NULL,
        FALSE
    FROM (VALUES

        -- Body & Energy - Indicators
        (v_cat_body_energy, v_sub_body_energy_ind, 'Weight pattern', 'SingleSelect',
         'Low,Difficult to increase,Medium,Losing fast,Steady,Overweight,Increases fast',
         'Low,Difficult to increase,Medium,Losing fast,Steady,Overweight,Increases fast',
         NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_ind, 'Sense of body feeling', 'SingleSelect',
         'Absolute lightness,Mostly light,Heaviness after meals or inactivity,Constant dull heaviness,Body-wide stiffness / ache',
         'Absolute lightness,Mostly light,Heaviness after meals or inactivity,Constant dull heaviness,Body-wide stiffness / ache',
         NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_ind, 'Do you feel exhausted during the day ?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_ind, 'What time during the day you feel exhausted ?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        -- Body & Energy - Habits & Context
        (v_cat_body_energy, v_sub_body_energy_hab, 'Physical activity includes', 'MultiSelect',
         'Walking,Exercise,Running ,Sports,Yoga,Stretching,Gardening,Outdoor work,Gym,Physiotherapy,Trekking,Swimming,Mostly sedentary',
         'Walking,Exercise,Running ,Sports,Yoga,Stretching,Gardening,Outdoor work,Gym,Physiotherapy,Trekking,Swimming,Mostly sedentary',
         NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Physical activity duration in hours per day', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Physical activity frequency', 'SingleSelect',
         'Daily,3-5 days/week,<3 days/week', 'Daily,3-5 days/week,<3 days/week', NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Physical activity regularity', 'SingleSelect',
         'Regular,Irregular', 'Regular,Irregular', NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Stimulants used for energy', 'SingleSelect',
         'Tea,Coffee,Tobacco,Alcohol,Drugs,None', 'Tea,Coffee,Tobacco,Alcohol,Drugs,None', NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Stimulants frequency per day', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Stimulants - years of use', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_body_energy, v_sub_body_energy_hab, 'Are you trying or planning to stop any of the stimulants?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        -- Sleep - Indicators
        (v_cat_sleep, v_sub_sleep_ind, 'Sleep quality (last 14 days)', 'SingleSelect',
         'Deep & continuous,Less than needed,Excessive,Interrupted (urination / nightmares),Difficulty falling asleep,Early waking and unable to sleep again,Insomnia / sleep medication needed',
         'Deep & continuous,Less than needed,Excessive,Interrupted (urination / nightmares),Difficulty falling asleep,Early waking and unable to sleep again,Insomnia / sleep medication needed',
         NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_ind, 'How do you feel when you wake up?', 'SingleSelect',
         'Wake up fresh, light, eyes clear; Wake up heavy, dull, eyes burning or sleepy',
         'Wake up fresh, light, eyes clear; Wake up heavy, dull, eyes burning or sleepy',
         NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_ind, 'Frequent dreams?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_ind, 'Remember dreams?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_ind, 'Dream type', 'MultiSelect',
         'Frightening,Sad,Depressive,Other', 'Frightening,Sad,Depressive,Other', NULL, NULL, NULL),

        -- Sleep - Habits & Context
        (v_cat_sleep, v_sub_sleep_hab, 'Usual bedtime', 'SingleSelect',
         'Before 10 PM,10-11 PM,11 PM-12 AM,After midnight,No fixed time',
         'Before 10 PM,10-11 PM,11 PM-12 AM,After midnight,No fixed time', NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_hab, 'Usual wake-up time', 'SingleSelect',
         'Before 6 AM,6-7 AM,7-8 AM,After 8 AM,Varies daily',
         'Before 6 AM,6-7 AM,7-8 AM,After 8 AM,Varies daily', NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_hab, 'Before sleep, you usually', 'MultiSelect',
         'Use screens (phone / TV / laptop),Read,Think about work,Eat late,Sit in silence / prayer / breathing,No fixed routine',
         'Use screens (phone / TV / laptop),Read,Think about work,Eat late,Sit in silence / prayer / breathing,No fixed routine',
         NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_hab, 'Screen use before sleep', 'SingleSelect',
         'Always,Often,Occasionally,Never', 'Always,Often,Occasionally,Never', NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_hab, 'Daytime sleep', 'SingleSelect',
         'None,Short nap (<30 min),Long nap (>30 min),Irregular daytime sleep',
         'None,Short nap (<30 min),Long nap (>30 min),Irregular daytime sleep', NULL, NULL, NULL),

        -- Gut Health - Indicators
        (v_cat_gut_health, v_sub_gut_ind, 'Overall Appetite nature', 'SingleSelect',
         'Good,Variable,Scanty,Very hungry,Excess appetite,Slow appetite,Loss of taste / appetite,Uncontrollable eating',
         'Good,Variable,Scanty,Very hungry,Excess appetite,Slow appetite,Loss of taste / appetite,Uncontrollable eating',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_ind, 'Hunger before meals', 'SingleSelect',
         'Clear pleasant hunger,Mild emptiness,Burning / cramps / acidic pain,Sluggish (eating by habit),Rare or no hunger',
         'Clear pleasant hunger,Mild emptiness,Burning / cramps / acidic pain,Sluggish (eating by habit),Rare or no hunger',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_ind, 'Taste cravings', 'MultiSelect',
         'Sweet,Sour,Salty,Pungent,Bitter,Astringent,None',
         'Sweet,Sour,Salty,Pungent,Bitter,Astringent,None', NULL, NULL, NULL),

        -- Gut Health - Habits & Context
        (v_cat_gut_health, v_sub_gut_hab, 'First food in the morning', 'MultiSelect',
         'Water, Milk Tea, Black tea, Herbal tea, Coffee,Milk,Green juice, Fruit juice,Hot water, Warm water, Cool water, Lemon water.,Honey water, Breakfast, Other',
         'Water, Milk Tea, Black tea, Herbal tea, Coffee,Milk,Green juice, Fruit juice,Hot water, Warm water, Cool water, Lemon water.,Honey water, Breakfast, Other',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Breakfast time', 'SingleSelect',
         'Before 7,7-8:30,8:30-10,After 10,Skipped', 'Before 7,7-8:30,8:30-10,After 10,Skipped', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Breakfast type', 'SingleSelect',
         'Home-made,Restaurant, Both', 'Home-made,Restaurant, Both', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Breakfast nature', 'SingleSelect',
         'Light,Heavy,Liquid,Solid', 'Light,Heavy,Liquid,Solid', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Usual Breakfast items', 'MultiSelect',
         'Idli, Dosa, Dhokla, Poha, Kakhra, Idiyappam, Puttu, Tea, Coffee, Oats, Fruits, vegetable Salad, Porridge, Roti, Rice, Dal, Vegetable gravy, Boiled vegetables, Egg, Milk, Other',
         'Idli, Dosa, Dhokla, Poha, Kakhra, Idiyappam, Puttu, Tea, Coffee, Oats, Fruits, vegetable Salad, Porridge, Roti, Rice, Dal, Vegetable gravy, Boiled vegetables, Egg, Milk, Other',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Lunch time', 'SingleSelect',
         'Before 12,12-1,1-2,After 2,Skipped', 'Before 12,12-1,1-2,After 2,Skipped', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Lunch type', 'SingleSelect',
         'Home-made,Restaurant, Both', 'Home-made,Restaurant, Both', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Lunch nature', 'SingleSelect',
         'Balanced,Heavy,Processed', 'Balanced,Heavy,Processed', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Usual Lunch items', 'MultiSelect',
         'Roti, Rice, Kichadi, Dal, Vegetable gravy, Boiled vegetables, Egg, Poori, Idli, Dosa, Dhokla, Poha, Kakhra, Tea, Coffee, Oats, Fruits, Vegetable Salad, Porridge, Milk, Other',
         'Roti, Rice, Kichadi, Dal, Vegetable gravy, Boiled vegetables, Egg, Poori, Idli, Dosa, Dhokla, Poha, Kakhra, Tea, Coffee, Oats, Fruits, Vegetable Salad, Porridge, Milk, Other',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Dinner time', 'SingleSelect',
         'Before 6,6-7:30,7:30-9,After 9,Skipped', 'Before 6,6-7:30,7:30-9,After 9,Skipped', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Dinner type', 'SingleSelect',
         'Home-made,Restaurant, Both', 'Home-made,Restaurant, Both', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Dinner nature', 'MultiSelect',
         'Light,Heavy,Late-night eating', 'Light,Heavy,Late-night eating', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Usual Dinner items', 'MultiSelect',
         'Noodles, Pizza, Pasta, Dhokla, Vada, Poori, Idli, Dosa, Dhokla, Poha, Kakhra, Tea, Coffee, Oats, Fruits, Vegetable Salad, Porridge, Roti, Rice, Dal, Vegetable gravy, Boiled vegetables, Egg, Milk, Other',
         'Noodles, Pizza, Pasta, Dhokla, Vada, Poori, Idli, Dosa, Dhokla, Poha, Kakhra, Tea, Coffee, Oats, Fruits, Vegetable Salad, Porridge, Roti, Rice, Dal, Vegetable gravy, Boiled vegetables, Egg, Milk, Other',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Gap between dinner & sleep', 'SingleSelect',
         'Immediately,30 mins,1 hr,2 hrs,3 hrs', 'Immediately,30 mins,1 hr,2 hrs,3 hrs', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Diet type', 'SingleSelect',
         'Vegan (plant foods only),Vegetarian (plant foods + dairy),Eggetarian (vegetarian + eggs),Pescatarian (vegetarian + fish),Non-vegetarian (includes meat)',
         'Vegan (plant foods only),Vegetarian (plant foods + dairy),Eggetarian (vegetarian + eggs),Pescatarian (vegetarian + fish),Non-vegetarian (includes meat)',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Non-veg frequency', 'SingleSelect',
         'Daily, Once a week,Twice a week, More than twice a week',
         'Daily, Once a week,Twice a week, More than twice a week', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Food preference for spicy/tangy food', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Food preference for fried food', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Food preference for fast food', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Food preference for refrigerated food', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Food preference for very cold food', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Food preference for fermented food', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Consumption of Artificial colour/preservatives/flavourings', 'SingleSelect',
         'Never, Occasional, Often, Everyday', 'Never, Occasional, Often, Everyday', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Fasting practice', 'MultiSelect',
         'No,Fruit diet,One meal/day,Water fast,Juice diet',
         'No,Fruit diet,One meal/day,Water fast,Juice diet', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Fasting Frequency: ___ days/week', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'If no fasting reason', 'MultiSelect',
         'Never tried, Scared to try, Diabetic, Cant sustain, Medically prohibited',
         'Never tried, Scared to try, Diabetic, Cant sustain, Medically prohibited', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Chewing habit', 'SingleSelect',
         'I eat fast, without chewing,I chew 2-4 times,I chew till its liquid,I chew thoroughly with awareness',
         'I eat fast, without chewing,I chew 2-4 times,I chew till its liquid,I chew thoroughly with awareness',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Eating context of most meals', 'SingleSelect',
         'Together with Family/Friends,Eat alone,watching screen,Sit cross legged on floor,Dining table,Random postures on couch',
         'Together with Family/Friends,Eat alone,watching screen,Sit cross legged on floor,Dining table,Random postures on couch',
         NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Fruits intake frequency', 'Number',
         'Frequency', 'Frequency', NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Fruits intake quantity per day in grams', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Amount of oil/ghee per head per month in Litres', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_gut_health, v_sub_gut_hab, 'Amount of sugar per head per month in kg', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        -- Bowel Movement - Indicators
        (v_cat_bowel_movement, v_sub_bowel_ind, 'Frequency', 'SingleSelect',
         'Daily 1-2 times,Once daily (mild effort),Once in 1-2 days,Once in 2-3 days,Needs laxative',
         'Daily 1-2 times,Once daily (mild effort),Once in 1-2 days,Once in 2-3 days,Needs laxative', NULL, NULL, NULL),

        (v_cat_bowel_movement, v_sub_bowel_ind, 'Timing', 'SingleSelect',
         'Morning,Evening, Twice or More than twice a day',
         'Morning,Evening, Twice or More than twice a day', NULL, NULL, NULL),

        (v_cat_bowel_movement, v_sub_bowel_ind, 'Stool type', 'MultiSelect',
         'Smooth soft formed,Slightly firm / cracked,Dry / lumpy,Hard pellets / constipation,Loose / diarrhoea,Thick / mucus / oily,Blood / worms,Foul smell',
         'Smooth soft formed,Slightly firm / cracked,Dry / lumpy,Hard pellets / constipation,Loose / diarrhoea,Thick / mucus / oily,Blood / worms,Foul smell',
         NULL, NULL, NULL),

        (v_cat_bowel_movement, v_sub_bowel_ind, 'Other signs', 'SingleSelect',
         'Floats,Sinks,Sticks to pan,Does not stick', 'Floats,Sinks,Sticks to pan,Does not stick', NULL, NULL, NULL),

        (v_cat_bowel_movement, v_sub_bowel_ind, 'Satisfied after passing stools', 'SingleSelect',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_bowel_movement, v_sub_bowel_ind, 'Gas passed', 'MultiSelect',
         'Often,Sometimes,Rarely,Stinks heavily,Stinks sometimes,Never stinks,Loud embarassing,Soft',
         'Often,Sometimes,Rarely,Stinks heavily,Stinks sometimes,Never stinks,Loud embarassing,Soft', NULL, NULL, NULL),

        (v_cat_bowel_movement, v_sub_bowel_ind, 'Bloating', 'MultiSelect',
         'Often,Sometimes,Rarely,Painful,Relieved with time,Need intervention',
         'Often,Sometimes,Rarely,Painful,Relieved with time,Need intervention', NULL, NULL, NULL),

        -- Bowel Movement - Habits & Context
        (v_cat_bowel_movement, v_sub_bowel_hab, 'Type of toilet used', 'SingleSelect',
         'Indian,Western,Bed Pan,Diaper,Angloindian',
         'Indian,Western,Bed Pan,Diaper,Angloindian', NULL, NULL, NULL),

        -- Fluid Balance - Indicators
        (v_cat_fluid_balance, v_sub_fluid_ind, 'Thirst pattern', 'SingleSelect',
         'Normal,Excess,Low', 'Normal,Excess,Low', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_ind, 'Sweat', 'MultiSelect',
         'Excess,low,moderate,itching,burning,crawling',
         'Excess,low,moderate,itching,burning,crawling', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_ind, 'Urine colour', 'SingleSelect',
         'Transparent,light yellow,dark yellow,orange,red,brown,black',
         'Transparent,light yellow,dark yellow,orange,red,brown,black', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_ind, 'Urine nature', 'MultiSelect',
         'clear,frothy,cloudy,watery,with blood,with pus,burning,itching,delayed,smelly,Incontinence',
         'clear,frothy,cloudy,watery,with blood,with pus,burning,itching,delayed,smelly,Incontinence', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_ind, 'Nasal mucus', 'SingleSelect',
         'congested,watery,dry', 'congested,watery,dry', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_ind, 'Sneezing pattern', 'MultiSelect',
         'early in the morning,in cold weather,after AC use',
         'early in the morning,in cold weather,after AC use', NULL, NULL, NULL),

        -- Fluid Balance - Habits
        (v_cat_fluid_balance, v_sub_fluid_hab, 'Type of water', 'SingleSelect',
         'Spring / rainwater,Well (boiled / filtered),Tap,RO / bottled,Low intake / doubtful quality',
         'Spring / rainwater,Well (boiled / filtered),Tap,RO / bottled,Low intake / doubtful quality', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_hab, 'Cold / aerated drinks', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_hab, 'Cold drink type', 'MultiSelect',
         'Cola,Soda,Beer,Ice cream,Other', 'Cola,Soda,Beer,Ice cream,Other', NULL, NULL, NULL),

        -- Fluid Balance - Habit (meals)
        (v_cat_fluid_balance, v_sub_fluid_habit, 'Water during meals: ______ ml/meal', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_habit, 'Other fluids during meals: ______ ml/meal', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_habit, 'Total daily water: ______ ml/day', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_fluid_balance, v_sub_fluid_habit, 'Other drinks: ______ ml/day', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        -- Indicators
        (v_cat_indicators, v_sub_indicators, 'Occupation', 'Text',
         'Occupation', 'Occupation', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Distance from office', 'Number',
         'Distance from the office in kms', 'Distance from the office in kms', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Working hours', 'Text',
         'Working hrs - From and to', 'Working hrs - From and to', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Work posture', 'SingleSelect',
         'Mostly sitting,Standing long hours,Physically laborious,Office-based desk work,Frequent travel',
         'Mostly sitting,Standing long hours,Physically laborious,Office-based desk work,Frequent travel', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Satisfaction with work', 'SingleSelect',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Reason not satisfied', 'SingleSelect',
         'Nature of work does not suit me,Lack of interest,Excess workload,Value mismatch,Emotionally draining,Financial compulsion,Other',
         'Nature of work does not suit me,Lack of interest,Excess workload,Value mismatch,Emotionally draining,Financial compulsion,Other',
         NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Predominant mental condition', 'MultiSelect',
         'Think too much,Confusion,Indecisiveness,Nervousness,Lack of confidence,Loss of concentration / focus,Restlessness,Impulsiveness,Critical of others,Intolerance of delays,Brain fog/Lethargy,Over-sleepy,Calm,Thoughts of self harm or suicide',
         'Think too much,Confusion,Indecisiveness,Nervousness,Lack of confidence,Loss of concentration / focus,Restlessness,Impulsiveness,Critical of others,Intolerance of delays,Brain fog/Lethargy,Over-sleepy,Calm,Thoughts of self harm or suicide',
         NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Predominant emotional condition', 'MultiSelect',
         'Joyous,Friendly,Co-operative,Reserved,Stressful,Solitude,Worthlessness,Guilt,Hopelessness,Anxiety,Anger,Hostility,Irritability,Frustration,Impatience,Resentment,Outburst of temper,Depression,Loneliness,Worry,Fear',
         'Joyous,Friendly,Co-operative,Reserved,Stressful,Solitude,Worthlessness,Guilt,Hopelessness,Anxiety,Anger,Hostility,Irritability,Frustration,Impatience,Resentment,Outburst of temper,Depression,Loneliness,Worry,Fear',
         NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Stress present', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Stress level (0=No Stress, 10=Intolerable)', 'Scale',
         '0-10', '0-10', NULL, '0', '10'),

        (v_cat_indicators, v_sub_indicators, 'Do you have any specific reason for stress in your life', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'If yes, give details', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Social connectedness', 'SingleSelect',
         'Mixes easily with people and neighbours,Maintains a few meaningful connections,Feels isolated or disconnected,Frequent conflicts with colleagues or others',
         'Mixes easily with people and neighbours,Maintains a few meaningful connections,Feels isolated or disconnected,Frequent conflicts with colleagues or others',
         NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Specific challenges with', 'MultiSelect',
         'wife, children, parents, collegues, neighbour,others',
         'wife, children, parents, collegues, neighbour,others', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Contraception usage', 'MultiSelect',
         'Condoms,Copper T,Pills,Surgery,Rhythm method',
         'Condoms,Copper T,Pills,Surgery,Rhythm method', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Sexual life', 'SingleSelect',
         'Satisfactory,Unsatisfactory,Painful,Not Applicable', 'Satisfactory,Unsatisfactory,Painful,Not Applicable', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Money stress', 'SingleSelect',
         'No stress or burden,Mild concern at times,Ongoing manageable stress,Constant anxiety / pressure',
         'No stress or burden,Mild concern at times,Ongoing manageable stress,Constant anxiety / pressure', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Interested in spirituality?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Interested in philosophy / sect / religious activities?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'Do you believe in God?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'What is your ambition?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_indicators, v_sub_indicators, 'What is the goal of your life?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        -- Habits - Habits & Context
        (v_cat_habits, v_sub_habits_ctx, 'Regain energy at work', 'SingleSelect',
         'Pause / rest,Consume tea/coffee,Eat food/snacks,Regular breaks during work,Overworking without breaks,Clear work boundaries,Blurred boundaries',
         'Pause / rest,Consume tea/coffee,Eat food/snacks,Regular breaks during work,Overworking without breaks,Clear work boundaries,Blurred boundaries',
         NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Environment at workplace', 'MultiSelect',
         'Co-operative,Non-Co-operative,Friendly,Stressful,Tiring,Boring,Suppressive,Dominating,Other',
         'Co-operative,Non-Co-operative,Friendly,Stressful,Tiring,Boring,Suppressive,Dominating,Other', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Mental planning & focus style', 'SingleSelect',
         'Plans day and sets priorities,Avoids planning altogether',
         'Plans day and sets priorities,Avoids planning altogether', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Emotional processing style', 'MultiSelect',
         'Distract with screens, food, drink,Express emotions openly, can be reactive,Suppress emotions, Keep emotions to self,Avoid emotional conversations,Talk to someone,Observe emotions, Meditate or Pause to respond',
         'Distract with screens, food, drink,Express emotions openly, can be reactive,Suppress emotions, Keep emotions to self,Avoid emotional conversations,Talk to someone,Observe emotions, Meditate or Pause to respond',
         NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Past psychological trauma', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Trauma resolution', 'SingleSelect',
         'Resolved,Partially resolved,Ongoing impact',
         'Resolved,Partially resolved,Ongoing impact', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Sought counselling', 'SingleSelect',
         'Sought counselling / support,Never addressed trauma',
         'Sought counselling / support,Never addressed trauma', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Marital status', 'SingleSelect',
         'Married,Unmarried,Widow,Widower,Remarriage,Extra-marital relations',
         'Married,Unmarried,Widow,Widower,Remarriage,Extra-marital relations', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Quality time with family', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Quality time with friends', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Family relationship state', 'SingleSelect',
         'Happy and supportive,Disturbed,Needs active adjustment',
         'Happy and supportive,Disturbed,Needs active adjustment', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Conflict handling by', 'SingleSelect',
         'Open discussion,Avoidance,Suppression,Emotional reaction',
         'Open discussion,Avoidance,Suppression,Emotional reaction', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Hobbies tried', 'MultiSelect',
         'Reading,Music,Singing,Games,Trekking,Movies,TV,Exercise,Yoga,Nothing',
         'Reading,Music,Singing,Games,Trekking,Movies,TV,Exercise,Yoga,Nothing', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Do you practice yoga / meditation', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Do you work towards your goal?', 'SingleSelect',
         'Goal written down,Goal often reflected upon,No clarity / avoidance, Steps taken continuously',
         'Goal written down,Goal often reflected upon,No clarity / avoidance, Steps taken continuously',
         NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Daily environment', 'SingleSelect',
         'Natural / open-air spaces,Indoor well-ventilated spaces,Crowded low-ventilation spaces,Enclosed air-conditioned spaces',
         'Natural / open-air spaces,Indoor well-ventilated spaces,Crowded low-ventilation spaces,Enclosed air-conditioned spaces',
         NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Sunlight exposure', 'SingleSelect',
         'Daily,Most days,Occasionally,Rarely / never',
         'Daily,Most days,Occasionally,Rarely / never', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Contact with nature', 'SingleSelect',
         'Direct grounding (barefoot, soil, water, trees),Regular outdoor presence,Minimal or indirect contact,No conscious nature contact',
         'Direct grounding (barefoot, soil, water, trees),Regular outdoor presence,Minimal or indirect contact,No conscious nature contact',
         NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx, 'Amount of exposure to chemicals - choose what you currently use', 'MultiSelect',
         'Plastic containers - water bottle,Microwave oven use,Deodorant,Mosquito repellants,Teflon coated Non stick pan for cooking,Aluminium cookware,Room freshner',
         'Plastic containers - water bottle,Microwave oven use,Deodorant,Mosquito repellants,Teflon coated Non stick pan for cooking,Aluminium cookware,Room freshner',
         NULL, NULL, NULL),

        -- Habits - Habits / Context
        (v_cat_habits, v_sub_habits_ctx2, 'What do you like to read / watch?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx2, 'Time spent daily / weekly in reading or any hobby', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx2, 'As a child, what did you like to do?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx2, 'Are you continuing it?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx2, 'Why did you discontinue it?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_habits, v_sub_habits_ctx2, 'If you want to restart a hobby, what would that be?', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        -- Questions from the printed personal-history form that were previously missing
        (v_cat_fluid_balance, v_sub_fluid_ind, 'Urinary output pattern', 'SingleSelect',
         'Normal,Excess,Less', 'Normal,Excess,Less', NULL, NULL, NULL),
        (v_cat_fluid_balance, v_sub_fluid_ind, 'Urinary frequency per day', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_fluid_balance, v_sub_fluid_ind, 'Do you get up at night to urinate?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),
        (v_cat_fluid_balance, v_sub_fluid_ind, 'How many times do you get up at night to urinate?', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_fluid_balance, v_sub_fluid_ind, 'Additional urinary complaints', 'MultiSelect',
         'Burning,Obstruction,Dribbling,Other', 'Burning,Obstruction,Dribbling,Other', NULL, NULL, NULL),
        (v_cat_fluid_balance, v_sub_fluid_ind, 'Other urinary complaint details', 'Text',
         NULL, NULL, NULL, NULL, NULL),

        (v_cat_sleep, v_sub_sleep_hab, 'Total sleeping hours per day', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_sleep, v_sub_sleep_hab, 'Do you sleep during the day?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),
        (v_cat_sleep, v_sub_sleep_hab, 'Hours slept during the day', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        -- Women''s menstrual and reproductive history
        (v_cat_womens_health, v_sub_womens_health, 'Age at menarche', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Menstrual cycle regularity', 'SingleSelect',
         'Regular,Irregular', 'Regular,Irregular', NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Number of days of menstrual flow', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Menstrual flow', 'SingleSelect',
         'Normal,Scanty,Heavy', 'Normal,Scanty,Heavy', NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Menstrual complaints', 'MultiSelect',
         'Foul smelling discharge,White discharge,Painful menstruation,Other', 'Foul smelling discharge,White discharge,Painful menstruation,Other', NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Other menstrual complaint details', 'Text',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Any abortion or MTP?', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'When did the abortion or MTP occur?', 'Text',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Number of abortions or MTPs', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Menopause status', 'Boolean',
         'Yes,No', 'Yes,No', NULL, NULL, NULL),
        (v_cat_womens_health, v_sub_womens_health, 'Age at menopause', 'Number',
         NULL, NULL, NULL, NULL, NULL),

        -- Additional dietary questions from the printed form
        (v_cat_gut_health, v_sub_gut_hab, 'Non-vegetarian food frequency details', 'Text',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_fluid_balance, v_sub_fluid_hab, 'Quantity of cold or aerated drinks per day in ml', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_gut_health, v_sub_gut_hab, 'Main spices used in the daily diet', 'MultiSelect',
         'Chilli,Garam masala,Readymade spices,Tea masala,Sugar,Other', 'Chilli,Garam masala,Readymade spices,Tea masala,Sugar,Other', NULL, NULL, NULL),

        -- Additional psychological, social and family questions
        (v_cat_indicators, v_sub_indicators, 'How many friends do you have?', 'Number',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_indicators, v_sub_indicators, 'Conflict details with people, office staff or colleagues', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_habits, v_sub_habits_ctx, 'Relationship with family members', 'MultiSelect',
         'Normal,Disturbed,Wife,Husband,Children,Other', 'Normal,Disturbed,Wife,Husband,Children,Other', NULL, NULL, NULL),
        (v_cat_habits, v_sub_habits_ctx, 'Other family relationship details', 'Text',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_habits, v_sub_habits_ctx, 'Psychological trauma details', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_habits, v_sub_habits_ctx, 'Yoga, meditation or relaxation technique details', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),

        -- Past, family, allergy and treatment history
        (v_cat_medical_history, v_sub_medical_history, 'Past medical history', 'MultiSelect',
         'Diabetes,Hypertension,Heart disease,Bronchial asthma,Allergy,Jaundice,Joint pain,Joint swelling,Tuberculosis,Drug reaction,Surgery,Accident,Psychiatric or psychological illness,Other',
         'Diabetes,Hypertension,Heart disease,Bronchial asthma,Allergy,Jaundice,Joint pain,Joint swelling,Tuberculosis,Drug reaction,Surgery,Accident,Psychiatric or psychological illness,Other', NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Other past medical history details', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Family medical history', 'MultiSelect',
         'Diabetes,Hypertension,Heart disease,Bronchial asthma,Allergy,Psychological problem,Other',
         'Diabetes,Hypertension,Heart disease,Bronchial asthma,Allergy,Psychological problem,Other', NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Family member affected', 'MultiSelect',
         'Mother,Father,Brother,Sister,Paternal uncle,Maternal uncle,Children,Other',
         'Mother,Father,Brother,Sister,Paternal uncle,Maternal uncle,Children,Other', NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Other family medical history details', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Known allergy to medicine, chemicals or other things', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Treatment system used', 'MultiSelect',
         'Ayurveda,Homoeopathy,Nature Cure,Yoga,Allopathy', 'Ayurveda,Homoeopathy,Nature Cure,Yoga,Allopathy', NULL, NULL, NULL),
        (v_cat_medical_history, v_sub_medical_history, 'Name of medicine currently being taken', 'MultilineText',
         NULL, NULL, NULL, NULL, NULL)

    ) AS q (
        category_id,
        subcategory_id,
        question_text,
        answer_type,
        option_text,
        option_value,
        default_value,
        scale_min,
        scale_max
    )
    RETURNING AssessmentQuestionid
    )
    SELECT COALESCE(ARRAY_AGG(AssessmentQuestionid), ARRAY[]::uuid[])
      INTO v_question_ids
      FROM inserted_questions;

INSERT INTO AssessmentTemplate
(
    AssessmentTemplateID,
    TenantID,
    ViewerTenantIds,
    TemplateName,
    CreatedUser,
    CreatedDate,
    ModifiedUser,
    ModifiedDate,
    IsDeleted,
    TaskName,
    IsDefaultTemplate
)
VALUES
(
    v_template_id,
    pvar_tenantid,
    NULL,
    'Default Template',
   pvar_createduser,
    now(),
    Null,
    Null,
    FALSE,
    'Default Template',
    FALSE
);
	
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
    ROW_NUMBER() OVER (ORDER BY aq.questionnairereferencenumber),
    aq.questioncategory,
    aq.questionsubcategory,
    aq.assessmentquestionid,
    aq.optiontext,
    aq.optionvalue,
    aq.defaultvalue,
    aq.answertype,
    aq.scalerangemin,
    aq.scalerangemax,
    aq.isrequired,
    aq.scorevalue
FROM assessmentquestion aq
WHERE aq.assessmentquestionid = ANY(v_question_ids)
  AND aq.isdeleted = FALSE;

    GET DIAGNOSTICS v_template_question_count = ROW_COUNT;

    IF v_template_question_count <> CARDINALITY(v_question_ids) THEN
        RAISE EXCEPTION
            'Template question mapping failed: expected %, inserted %',
            CARDINALITY(v_question_ids),
            v_template_question_count;
    END IF;

    pvar_returnmessage := '201.1';

EXCEPTION WHEN OTHERS THEN
    pvar_returnmessage := SQLERRM;
END
$BODY$;

ALTER FUNCTION public."Seed_Template_For_Tenant"(uuid, uuid)
    OWNER TO md_nalamvazha;

