CREATE TABLE IF NOT EXISTS PaymentConfig_keyinfo
(
PaymentConfig_keyinfoid uuid PRIMARY KEY
,PaymentConfigid uuid REFERENCES PaymentConfig(PaymentConfigid)
,record_order int
,keytype Varchar(1080) NOT NULL
,keyid Varchar(128) NOT NULL
,keysecret Varchar(128) NOT NULL
,returnurl Varchar(256) NOT NULL
,notifyurl Varchar(256) NOT NULL
,merchantid Varchar(128) NOT NULL
,status Varchar(1080) NOT NULL
);


