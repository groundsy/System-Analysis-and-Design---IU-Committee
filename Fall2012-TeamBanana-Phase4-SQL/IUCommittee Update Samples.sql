-- example syntax for an update

-- UPDATE <table>
-- SET attribute = expression, attribute = expression, ...
-- WHERE predicates;


UPDATE SysUser   -- fill in the '' with what you want, T/F for the flags and a string for rank/position
SET FacultyFlag = '', FacultyRank = '', StaffFlag = 'T' ,StaffPosition = 'IT Admin' ,StudentFlag = 'F' ,IsITAdministrator = 'T'
WHERE Email = 'jashdown@iusb'; -- repalce with email of tuple to change
GO

-- to make yourself a CSA just replace email address
INSERT INTO CommSuperAdmin
(SysUser_Email,CommOwn_ID,StartDate,EndDate,CreatedBy,CreatedDate)
VALUES ('jashdown@iusb.edu',5,CURRENT_TIMESTAMP,'2013-01-01 00:00:01','bob@iu.edu',CURRENT_TIMESTAMP);