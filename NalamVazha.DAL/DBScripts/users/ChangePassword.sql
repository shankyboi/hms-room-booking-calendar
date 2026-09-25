  
                    CREATE OR REPLACE FUNCTION "ChangePassword"
                    (
                    pvar_usersid Varchar(50)
                    ,pvar_userpassword Varchar(128)  
                    ,pvar_passwordkey Varchar(150)  
                    ,pvar_modifieduser  uuid 
                    ,OUT pvar_returnMessage Varchar(4000) 
                    )  
                    RETURNS Varchar(4000) 
                    AS $BODY$  
                    BEGIN
                    /*This code generated from tDev Powered by Mahat, Build Number :#2024-01-001(Updated on 06-01-2024 12:57PM) on 03/10/2026 09:40:22*/

 
                            UPDATE users SET
                            userpassword = pvar_userpassword
                            ,passwordkey = pvar_passwordkey
                            ,modifieduser=pvar_modifieduser
                            WHERE usersid::varchar = pvar_usersid;

                            pvar_returnMessage := '201.1';


                               /* EXCEPTION WHEN OTHERS THEN
			 
                                INSERT INTO system_logging
                                (
                                Log_code
                                ,system_logging_guid
                                ,log_application
                                ,log_date
                                ,log_level
                                ,log_logger
                                ,log_message
                                )
                                VALUES
                                ('16'
                                ,gen_random_uuid()
                                ,'Postgre Function Exception'
                                ,NOW()
                                ,'16'
                                ,'{ActionMethodName}'
                                ,'update failed'
                                );*/
			  
                    END
                    $BODY$
                    LANGUAGE plpgsql;
                    

