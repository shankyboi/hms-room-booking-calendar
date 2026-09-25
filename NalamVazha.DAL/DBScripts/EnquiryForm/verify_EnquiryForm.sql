 
			  
			  CREATE OR REPLACE FUNCTION  "verify_EnquiryForm"
			  (
				  pvar_enquiryformid Varchar
				  ,pvar_verifiedby Varchar(50)
				  ,pvar_verifiedstatus Varchar(128)
				  ,pvar_reviewcomments Varchar(128)
				  
				  ,OUT pvar_returnMessage Varchar(4000)
			  )
			  RETURNS Varchar(4000) 
              AS $BODY$  
                DECLARE lvar_enquiryformid_array UUID[];
                lvar_enquiryformid UUID;  
                 lvar_invalid_status_count INTEGER;
                  lvar_total_enquiryform_count INTEGER;
              BEGIN
	        /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:43:41*/

              lvar_enquiryformid_array := STRING_TO_ARRAY(pvar_enquiryformid, ',');

              lvar_total_enquiryform_count := array_length(lvar_enquiryformid_array, 1);
               -- Check for invalid status orders
               SELECT COUNT(*) INTO lvar_invalid_status_count
               FROM EnquiryForm 
               WHERE enquiryformid = ANY(lvar_enquiryformid_array)
               AND verifiedstatus <> 'Ready For Review';

               IF lvar_invalid_status_count > 0 THEN
        CASE 
            WHEN lvar_total_enquiryform_count = 1 THEN 
                pvar_returnMessage := 'The given record is already reviewed. Please try again.';
            WHEN lvar_invalid_status_count = lvar_total_enquiryform_count THEN
                pvar_returnMessage := 'All given records are already reviewed. Please try again.';
            ELSE 
                pvar_returnMessage := 'Some of the given records are already reviewed. Please try again.';
        END CASE;
        RETURN;
    END IF;


            FOREACH lvar_enquiryformid IN ARRAY lvar_enquiryformid_array
            LOOP

			  UPDATE EnquiryForm
			  SET verifiedby=CAST(pvar_verifiedby AS UUID)
			  ,verifiedstatus=pvar_verifiedstatus
			  ,verifieddate=NOW()
			  ,reviewcomments=pvar_reviewcomments
			  
			  WHERE  enquiryformid=lvar_enquiryformid;


             INSERT INTO reviewlogsEnquiryForm(enquiryformid,  verifiedstatus, reviewcomments, createduser)
	         VALUES (lvar_enquiryformid, pvar_verifiedstatus, pvar_reviewcomments,CAST(pvar_verifiedby AS UUID));
            
            END LOOP;

			   
				pvar_returnMessage:='201.1';
					 	
			  END
              $BODY$
              LANGUAGE plpgsql;

