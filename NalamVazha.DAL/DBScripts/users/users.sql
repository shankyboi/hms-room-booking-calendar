CREATE TABLE IF NOT EXISTS users
(
usersid uuid  PRIMARY KEY
,tenantid uuid NULL
,viewertenantids varchar  NULL
,firstname Varchar(50) NOT NULL
,lastname Varchar(50) NULL
,profilepicture Varchar(4000) NULL
,UNIQUE(username)
,username Varchar(150) NOT NULL
,userpassword Varchar(128) NOT NULL
,passwordkey Varchar(150) NULL
,UNIQUE(emailid)
,emailid Varchar(128) NOT NULL
,mobilenumber Varchar(20) NOT NULL
,userrole Varchar(1080) NOT NULL

,createduser uuid
,createddate  Timestamp(3) NOT NULL DEFAULT NOW()
,modifieduser uuid
,modifieddate Timestamp(3)
,isdeleted Boolean DEFAULT false
)


