
CREATE OR REPLACE FUNCTION public."Receivables"(
	pvar_tenantid character varying,
	pvar_receivabledate_automatonfrom character varying,
	pvar_receivabledate_automatonto character varying,
	pvar_patientname character varying,
	pvar_ipdnumber character varying,
	pvar_opdnumber character varying,
	pvar_receivablefor character varying,
	pvar_pagesize integer,
	pvar_pagenumber integer,
	pvar_searchterm character varying,
	pvar_sort_fields json)
    RETURNS json
    LANGUAGE 'plpgsql'
    COST 100
    VOLATILE PARALLEL UNSAFE
AS $BODY$
               declare local_sortcolumn_array text[] = (
	                select array_agg(col) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)
                );
                declare local_sortorder_array text[] = (
	                select array_agg(dir) from json_to_recordset(pvar_sort_fields) as x(col text, dir text)	
                );          
              declare lvar_tenantid varchar[];declare lstr_usersid varchar;  
               
          	  BEGIN
			  /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 04/21/2026 05:37:58*/
			  		
                SELECT  SPLIT_PART(pvar_tenantid, '|', 1),SPLIT_PART(pvar_tenantid, '|', 2) into lstr_usersid,pvar_tenantid;
		        
                if(pvar_tenantid is null or pvar_tenantid='' or pvar_tenantid='00000000-0000-0000-0000-000000000000')	
				then
                    SELECT STRING_TO_ARRAY(viewertenantids, ',') into lvar_tenantid
				    FROM users where users.usersid::varchar=lstr_usersid;	
                    if(lvar_tenantid is NULL)
					then 
						SELECT array_agg(tenant.tenantid) INTO lvar_tenantid FROM tenant;
               
					end if;
                else 
				  lvar_tenantid=ARRAY[pvar_tenantid];
                end if;
                lvar_tenantid := lvar_tenantid || ARRAY[''::character varying] || ARRAY['00000000-0000-0000-0000-000000000000'::character varying];

                    if(pvar_searchterm is not null and LENGTH(CAST(pvar_searchterm as Varchar)) > 0)
                    then
                    pvar_searchterm := '%' || pvar_searchterm || '%';
                    else
                    pvar_searchterm := null;
                    end if;
              
                    RETURN json_build_object(
                    'count'
                    ,(SELECT  
                    COUNT(*)
                    FROM  Receivable 
 LEFT OUTER JOIN tenant ON Receivable.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON Receivable.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN PatientVisit __PatientVisit ON Receivable.patientvisit=__PatientVisit.PatientVisitid
LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON Receivable.ipdnumber=___IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm ____OPDForm ON Receivable.opdnumber=____OPDForm.OPDFormid
LEFT OUTER JOIN Therapies _____Therapies ON Receivable.therapy=_____Therapies.Therapiesid
LEFT OUTER JOIN TherapyKit ______TherapyKit ON Receivable.therapykit=______TherapyKit.TherapyKitid
LEFT OUTER JOIN Medicine _______Medicine ON Receivable.medicine=_______Medicine.Medicineid
LEFT OUTER JOIN TreatmentPackage ________TreatmentPackage ON Receivable.package=________TreatmentPackage.TreatmentPackageid
LEFT OUTER JOIN Room _________Room ON Receivable.room=_________Room.Roomid

                    WHERE (lvar_tenantid is null or COALESCE(cast(Receivable.tenantid as varchar), '') = Any(lvar_tenantid)) AND Receivable.isdeleted=false
 AND(pvar_receivabledate_automatonfrom IS NULL OR pvar_receivabledate_automatonfrom = '' OR Receivable.receivabledate >= CAST(pvar_receivabledate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_receivabledate_automatonto IS NULL OR pvar_receivabledate_automatonto = '' OR Receivable.receivabledate <= CAST(pvar_receivabledate_automatonto AS TIMESTAMP(3)))
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(Receivable.patientname as VARCHAR)=pvar_patientname)
AND (pvar_ipdnumber is null or pvar_ipdnumber ='0' or LENGTH(CAST(pvar_ipdnumber as Varchar))=0 or CAST(Receivable.ipdnumber as VARCHAR)=pvar_ipdnumber)
AND (pvar_opdnumber is null or pvar_opdnumber ='0' or LENGTH(CAST(pvar_opdnumber as Varchar))=0 or CAST(Receivable.opdnumber as VARCHAR)=pvar_opdnumber)
AND (pvar_receivablefor is null or pvar_receivablefor ='0' or LENGTH(CAST(pvar_receivablefor as Varchar))=0 or CAST(Receivable.receivablefor as VARCHAR)=pvar_receivablefor)
 AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.receivableno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(Receivable.receivabledate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''), NULLIF(_PatientProfile.mobilenumber, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__PatientVisit.visitnumber||' '||__PatientVisit.ipdnumber||' '||__PatientVisit.opdnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___IPDApplicationForm.firstname, ''), NULLIF(___IPDApplicationForm.lastname, ''), NULLIF(___IPDApplicationForm.bookingreferencenumber, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.receivablefor AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.specifyothers AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____Therapies.therapyname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.therapycost AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(______TherapyKit.therapykitname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.kitprice AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_______Medicine.medicinename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.price AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(________TreatmentPackage.packagename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_________Room.roomnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.amount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.remarks AS VARCHAR) ilike pvar_searchterm)
))                   
                    ,'detail'
                    ,(SELECT json_agg(row_to_json(d)) FROM (
                    SELECT  
                    Receivable.tenantid
,tenant.businessname as _tenantName
,Receivable.Receivableid
,Receivable.receivableno
,CAST(COALESCE(to_char(Receivable.receivabledate,'dd/MM/yyyy'),'') AS Varchar) as receivabledate
,Receivable.patientname
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''), NULLIF(_PatientProfile.mobilenumber, ''))), '') AS VARCHAR) as patientname_master
,Receivable.patientvisit
,CAST(__PatientVisit.visitnumber||' '||__PatientVisit.ipdnumber||' '||__PatientVisit.opdnumber AS VARCHAR) as patientvisit_master
,Receivable.ipdnumber
,CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___IPDApplicationForm.firstname, ''), NULLIF(___IPDApplicationForm.lastname, ''), NULLIF(___IPDApplicationForm.bookingreferencenumber, ''))), '') AS VARCHAR) as ipdnumber_master
,Receivable.opdnumber
,CAST(____OPDForm.bookingreferencenumber AS VARCHAR) as opdnumber_master
,Receivable.receivablefor
,Receivable.specifyothers
,Receivable.therapy
,CAST(_____Therapies.therapyname AS VARCHAR) as therapy_master
,Receivable.therapycost
,Receivable.therapykit
,CAST(______TherapyKit.therapykitname AS VARCHAR) as therapykit_master
,Receivable.kitprice
,Receivable.medicine
,CAST(_______Medicine.medicinename AS VARCHAR) as medicine_master
,Receivable.price
,Receivable.package
,CAST(________TreatmentPackage.packagename AS VARCHAR) as package_master
,Receivable.room
,CAST(_________Room.roomnumber AS VARCHAR) as room_master
,Receivable.amount
,Receivable.remarks
,Receivable.paidamount
,Receivable.paymentstatus
                    
                    ,Receivable.createduser,Receivable.createddate,Receivable.modifieduser,Receivable.modifieddate
                    FROM  Receivable 
 LEFT OUTER JOIN tenant ON Receivable.tenantid=tenant.tenantid
INNER JOIN PatientProfile _PatientProfile ON Receivable.patientname=_PatientProfile.PatientProfileid
LEFT OUTER JOIN PatientVisit __PatientVisit ON Receivable.patientvisit=__PatientVisit.PatientVisitid
LEFT OUTER JOIN IPDApplicationForm ___IPDApplicationForm ON Receivable.ipdnumber=___IPDApplicationForm.IPDApplicationFormid
LEFT OUTER JOIN OPDForm ____OPDForm ON Receivable.opdnumber=____OPDForm.OPDFormid
LEFT OUTER JOIN Therapies _____Therapies ON Receivable.therapy=_____Therapies.Therapiesid
LEFT OUTER JOIN TherapyKit ______TherapyKit ON Receivable.therapykit=______TherapyKit.TherapyKitid
LEFT OUTER JOIN Medicine _______Medicine ON Receivable.medicine=_______Medicine.Medicineid
LEFT OUTER JOIN TreatmentPackage ________TreatmentPackage ON Receivable.package=________TreatmentPackage.TreatmentPackageid
LEFT OUTER JOIN Room _________Room ON Receivable.room=_________Room.Roomid

                    WHERE (lvar_tenantid is null or COALESCE(cast(Receivable.tenantid as varchar), '') = Any(lvar_tenantid)) AND Receivable.isdeleted=false
 AND(pvar_receivabledate_automatonfrom IS NULL OR pvar_receivabledate_automatonfrom = '' OR Receivable.receivabledate >= CAST(pvar_receivabledate_automatonfrom AS TIMESTAMP(3))) 
                                AND (pvar_receivabledate_automatonto IS NULL OR pvar_receivabledate_automatonto = '' OR Receivable.receivabledate <= CAST(pvar_receivabledate_automatonto AS TIMESTAMP(3)))
AND (pvar_patientname is null or pvar_patientname ='0' or LENGTH(CAST(pvar_patientname as Varchar))=0 or CAST(Receivable.patientname as VARCHAR)=pvar_patientname)
AND (pvar_ipdnumber is null or pvar_ipdnumber ='0' or LENGTH(CAST(pvar_ipdnumber as Varchar))=0 or CAST(Receivable.ipdnumber as VARCHAR)=pvar_ipdnumber)
AND (pvar_opdnumber is null or pvar_opdnumber ='0' or LENGTH(CAST(pvar_opdnumber as Varchar))=0 or CAST(Receivable.opdnumber as VARCHAR)=pvar_opdnumber)
AND (pvar_receivablefor is null or pvar_receivablefor ='0' or LENGTH(CAST(pvar_receivablefor as Varchar))=0 or CAST(Receivable.receivablefor as VARCHAR)=pvar_receivablefor)

                     AND ( ((pvar_searchterm is null) or COALESCE(tenant.businessname::varchar,'') ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.receivableno AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(COALESCE(to_char(Receivable.receivabledate,'dd/MM/yyyy'),'') AS Varchar) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''), NULLIF(_PatientProfile.mobilenumber, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(__PatientVisit.visitnumber||' '||__PatientVisit.ipdnumber||' '||__PatientVisit.opdnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(NULLIF(btrim(concat_ws(' ', NULLIF(___IPDApplicationForm.firstname, ''), NULLIF(___IPDApplicationForm.lastname, ''), NULLIF(___IPDApplicationForm.bookingreferencenumber, ''))), '') AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(____OPDForm.bookingreferencenumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.receivablefor AS VARCHAR) ilike pvar_searchterm)
  OR ((pvar_searchterm is null) or CAST(Receivable.specifyothers AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_____Therapies.therapyname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.therapycost AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(______TherapyKit.therapykitname AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.kitprice AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_______Medicine.medicinename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.price AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(________TreatmentPackage.packagename AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(_________Room.roomnumber AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.amount AS VARCHAR) ilike pvar_searchterm)
 OR ((pvar_searchterm is null) or CAST(Receivable.remarks AS VARCHAR) ilike pvar_searchterm)
) 
                    ORDER BY 
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'receivableno' THEN Receivable.receivableno::TEXT
WHEN 'receivableno' THEN Receivable.receivableno::TEXT
WHEN 'patientname' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''), NULLIF(_PatientProfile.mobilenumber, ''))), '')::TEXT
WHEN 'patientvisit' THEN __PatientVisit.visitnumber||' '||__PatientVisit.ipdnumber||' '||__PatientVisit.opdnumber::TEXT
WHEN 'ipdnumber' THEN NULLIF(btrim(concat_ws(' ', NULLIF(___IPDApplicationForm.firstname, ''), NULLIF(___IPDApplicationForm.lastname, ''), NULLIF(___IPDApplicationForm.bookingreferencenumber, ''))), '')::TEXT
WHEN 'opdnumber' THEN ____OPDForm.bookingreferencenumber::TEXT
WHEN 'receivablefor' THEN Receivable.receivablefor::TEXT
WHEN 'specifyothers' THEN Receivable.specifyothers::TEXT
WHEN 'therapy' THEN _____Therapies.therapyname::TEXT
WHEN 'therapycost' THEN Receivable.therapycost::TEXT
WHEN 'therapykit' THEN ______TherapyKit.therapykitname::TEXT
WHEN 'kitprice' THEN Receivable.kitprice::TEXT
WHEN 'medicine' THEN _______Medicine.medicinename::TEXT
WHEN 'price' THEN Receivable.price::TEXT
WHEN 'package' THEN ________TreatmentPackage.packagename::TEXT
WHEN 'room' THEN _________Room.roomnumber::TEXT
WHEN 'amount' THEN Receivable.amount::TEXT
WHEN 'remarks' THEN Receivable.remarks::TEXT
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC
,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'receivabledate' THEN Receivable.receivabledate
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'asc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'therapycost' THEN Receivable.therapycost::NUMERIC
WHEN 'price' THEN Receivable.price::NUMERIC
WHEN 'amount' THEN Receivable.amount::NUMERIC
		
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END ASC,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'receivableno' THEN Receivable.receivableno::TEXT
WHEN 'receivableno' THEN Receivable.receivableno::TEXT
WHEN 'patientname' THEN NULLIF(btrim(concat_ws(' ', NULLIF(_PatientProfile.firstname, ''), NULLIF(_PatientProfile.lastname, ''), NULLIF(_PatientProfile.mobilenumber, ''))), '')::TEXT
WHEN 'patientvisit' THEN __PatientVisit.visitnumber||' '||__PatientVisit.ipdnumber||' '||__PatientVisit.opdnumber::TEXT
WHEN 'ipdnumber' THEN NULLIF(btrim(concat_ws(' ', NULLIF(___IPDApplicationForm.firstname, ''), NULLIF(___IPDApplicationForm.lastname, ''), NULLIF(___IPDApplicationForm.bookingreferencenumber, ''))), '')::TEXT
WHEN 'opdnumber' THEN ____OPDForm.bookingreferencenumber::TEXT
WHEN 'receivablefor' THEN Receivable.receivablefor::TEXT
WHEN 'specifyothers' THEN Receivable.specifyothers::TEXT
WHEN 'therapy' THEN _____Therapies.therapyname::TEXT
WHEN 'therapycost' THEN Receivable.therapycost::TEXT
WHEN 'therapykit' THEN ______TherapyKit.therapykitname::TEXT
WHEN 'kitprice' THEN Receivable.kitprice::TEXT
WHEN 'medicine' THEN _______Medicine.medicinename::TEXT
WHEN 'price' THEN Receivable.price::TEXT
WHEN 'package' THEN ________TreatmentPackage.packagename::TEXT
WHEN 'room' THEN _________Room.roomnumber::TEXT
WHEN 'amount' THEN Receivable.amount::TEXT
WHEN 'remarks' THEN Receivable.remarks::TEXT
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'receivabledate' THEN Receivable.receivabledate
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
,
                    CASE WHEN local_sortorder_array[1] = 'desc' THEN
                    CASE local_sortcolumn_array[1]
                      WHEN 'therapycost' THEN Receivable.therapycost::NUMERIC
WHEN 'price' THEN Receivable.price::NUMERIC
WHEN 'amount' THEN Receivable.amount::NUMERIC
			
                    ELSE NULL
                    END
                    ELSE
                    NULL
                    END DESC
                    ,Receivable.receivabledate DESC
                    limit pvar_pagesize
                    offset pvar_pagenumber * pvar_pagesize			 	
			 	
                    ) d));
	
			  
					 	
			  END
              
$BODY$;

