USE [jashdown] -- change it to your id

SELECT * FROM University;

SELECT * FROM SysUser;

Select * from CommSuperAdmin;

----------------------------------------
-- All of the below give the same result

SELECT code FROM University;

SELECT code 'Univ Code' FROM University;

Select University.Code FROM University;

Select U.Code FROM University U;

----------------------------------------
-- get all attributes from committee
SELECT * FROM Comm;

-- get all attributes of a campus committee
SELECT * FROM Comm, Campus WHERE Comm.CommOwn_ID = Campus.CommOwn_ID;

-- get certain attributes of a campus committee
SELECT Campus.Name 'Campus', Comm.Name 'Committee Name', Comm.EffectiveDate, Comm.Type FROM Comm, Campus WHERE Comm.CommOwn_ID = Campus.CommOwn_ID;

----------------------------------------

-- To get the extended properties of database objects
-- You may not need this

SELECT * FROM sys.extended_properties;