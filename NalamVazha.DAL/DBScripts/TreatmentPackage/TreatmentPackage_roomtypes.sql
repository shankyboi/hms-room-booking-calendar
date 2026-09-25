CREATE TABLE IF NOT EXISTS TreatmentPackage_roomtypes
(
TreatmentPackage_roomtypesid uuid PRIMARY KEY
,TreatmentPackageid uuid REFERENCES TreatmentPackage(TreatmentPackageid)
,record_order int
,roomtype uuid REFERENCES RoomType(RoomTypeid) NOT NULL
,costperday decimal(18,2) NULL
,percentagecovered decimal(18,2) NOT NULL
,roomcost Varchar(256) NOT NULL
);
CREATE TABLE IF NOT EXISTS TreatmentPackage_therapy
(
TreatmentPackage_therapyid uuid PRIMARY KEY
,TreatmentPackageid uuid REFERENCES TreatmentPackage(TreatmentPackageid)
,record_order int
,therapyname uuid REFERENCES Therapies(Therapiesid) NOT NULL
,therapycost decimal(18,2) NULL
,numberoftimes int NOT NULL
,therapyprice Varchar(256) NOT NULL
);
CREATE TABLE IF NOT EXISTS TreatmentPackage_therapykits
(
TreatmentPackage_therapykitsid uuid PRIMARY KEY
,TreatmentPackageid uuid REFERENCES TreatmentPackage(TreatmentPackageid)
,record_order int
,therapykitname uuid REFERENCES TherapyKit(TherapyKitid) NOT NULL
,kitprice Varchar(256) NULL
,numberofkits int NOT NULL
,therapykitcost Varchar(256) NOT NULL
);
CREATE TABLE IF NOT EXISTS TreatmentPackage_therapyitems
(
TreatmentPackage_therapyitemsid uuid PRIMARY KEY
,TreatmentPackageid uuid REFERENCES TreatmentPackage(TreatmentPackageid)
,record_order int
,therapyitem uuid REFERENCES TherapyItem(TherapyItemid) NOT NULL
,price decimal(18,2) NULL
,therapyitemcount int NOT NULL
,therapyitemcost Varchar(256) NOT NULL
);
CREATE TABLE IF NOT EXISTS TreatmentPackage_medicines
(
TreatmentPackage_medicinesid uuid PRIMARY KEY
,TreatmentPackageid uuid REFERENCES TreatmentPackage(TreatmentPackageid)
,record_order int
,medicinename uuid REFERENCES Medicine(Medicineid) NOT NULL
,price decimal(18,2) NULL
,medicinecount int NOT NULL
,medicinecost Varchar(256) NOT NULL
);
CREATE TABLE IF NOT EXISTS TreatmentPackage_refundpolicy
(
TreatmentPackage_refundpolicyid uuid PRIMARY KEY
,TreatmentPackageid uuid REFERENCES TreatmentPackage(TreatmentPackageid)
,record_order int
,refundtype Varchar(1080) NOT NULL
,cancellationby Varchar(1080) NOT NULL
,cancellationwindowdays int NOT NULL
,refundpercentage decimal(18,2) NOT NULL
);


