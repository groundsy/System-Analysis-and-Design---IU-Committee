-- sample insert statements
-- create your own insert statements for other tables

-- SQL Reference: 
-- http://msdn.microsoft.com/en-us/library/bb510741%28v=sql.105%29.aspx

USE [jashdown] -- change it to your id


-- Insert a system user. Employer ID is null as there is nothing yet.
INSERT INTO SysUser
(Email,FirstName,LastName,Phone,OfficeInfo,FacultyFlag,FacultyRank,StaffFlag,StaffPosition,StudentFlag,IsITAdministrator,Employer_ID,CreatedBy,CreatedDate)
VALUES
('bob@iu.edu','Bob','D','(317) 222-2222','NS','F','','F','','T','T',null,'bob@iu.edu',CURRENT_TIMESTAMP);

-- Insert the next value into CommOwn, as we are going to create an University
INSERT CommOwn DEFAULT VALUES; -- No need for values as the only column available is IDENTITY

INSERT INTO University 
(Code,Name,Address,Phone,CommOwn_ID,CreatedBy,CreatedDate)
VALUES ('IU', 'Indiana University', '107 S. Indiana Ave. Bloomington, IN 47405-7000', '(812) 855-4848',1,'bob@iu.edu', CURRENT_TIMESTAMP);

-- Insert the next value into CommOwn, as we are going to create a Campus
INSERT CommOwn DEFAULT VALUES; -- No need for values as the only column available is IDENTITY
-- Insert the next value into Employer, as we are going to create a Campus
INSERT Employer DEFAULT VALUES; -- No need for values as the only column available is IDENTITY

INSERT INTO Campus
(University_Code,Code,Name,Address,Phone,CommOwn_ID,Employer_ID,CreatedBy,CreatedDate)
VALUES ('IU','SB','Indiana University South Bend', '1700 Mishawaka Ave. P.O. Box 7111 South Bend, IN 46634-7111', '(574) 237-4872',2,1,'bob@iu.edu',CURRENT_TIMESTAMP);

-- Insert the next value into CommOwn, as we are going to create a Campus
INSERT CommOwn DEFAULT VALUES; -- No need for values as the only column available is IDENTITY
-- Insert the next value into Employer, as we are going to create a Campus
INSERT Employer DEFAULT VALUES; -- No need for values as the only column available is IDENTITY

INSERT INTO Campus
(University_Code,Code,Name,Address,Phone,CommOwn_ID,Employer_ID,CreatedBy,CreatedDate)
VALUES ('IU','BL','Indiana University Bloomington', '107 S. Indiana Ave. Bloomington, IN 47405-7000', '(812) 855-4848',3,2,'bob@iu.edu',CURRENT_TIMESTAMP);

-- Insert the next value into CommOwn, as we are going to create a School
INSERT CommOwn DEFAULT VALUES; -- No need for values as the only column available is IDENTITY
-- Insert the next value into Employer, as we are going to create a School
INSERT Employer DEFAULT VALUES; -- No need for values as the only column available is IDENTITY

INSERT INTO School
(Campus_University_Code,Campus_Code,Code,Name,Address,Phone,CommOwn_ID,Employer_ID,CreatedBy,CreatedDate)
VALUES ('IU','SB','CLAS', 'Liberal Arts and Science', 'Wiekamp Hall 3300', '(574) 520-4290', 4,3, 'bob@iu.edu',CURRENT_TIMESTAMP);

-- Insert the next value into CommOwn, as we are going to create a Unit
INSERT CommOwn DEFAULT VALUES; -- No need for values as the only column available is IDENTITY
-- Insert the next value into Employer, as we are going to create a Unit
INSERT Employer DEFAULT VALUES; -- No need for values as the only column available is IDENTITY

INSERT INTO Unit
(Campus_University_Code,Campus_Code,Code,Name,Address,Phone,CommOwn_ID,Employer_ID,School_Campus_University_Code,School_Campus_Code,School_Code,CreatedBy,CreatedDate)
VALUES ('IU','SB','CS', 'Computer Science Deptartment', 'NorthSide Hall', '574-520-5521', 5,4,'IU','SB','CLAS' ,'bob@iu.edu',CURRENT_TIMESTAMP);


-- Create a committee. The ID field is auto generated. Some fields are not populated now.
INSERT INTO Comm
(CommOwn_ID,Name,EffectiveDate,MinMembers,MaxMembers,MembershipYears,Type,IsListedPublicly,CreatedBy,CreatedDate)
VALUES
(2,'Information Technology (IT)','2012/10/25',17,17,2,'Standing','Y','bob@iu.edu',CURRENT_TIMESTAMP);

-- Update Statement
-- http://msdn.microsoft.com/en-us/library/ms177523%28v=sql.105%29.aspx
-- Update the Comm table attributes
UPDATE Comm
SET IsArchived = 'Y', ArchivedBy = 'bob@iu.edu', ArchivedDate = CURRENT_TIMESTAMP
GO


-- Added Team Banana as system users
INSERT INTO SysUser
(Email,FirstName,LastName,Phone,OfficeInfo,FacultyFlag,FacultyRank,StaffFlag,StaffPosition,StudentFlag,IsITAdministrator,Employer_ID,CreatedBy,CreatedDate)
VALUES
('jashdown@iusb.edu','Justin','Ashdown','(574) 850-9253','NS','F','','F','','T','F',1,'bob@iu.edu',CURRENT_TIMESTAMP);

INSERT INTO SysUser
(Email,FirstName,LastName,Phone,OfficeInfo,FacultyFlag,FacultyRank,StaffFlag,StaffPosition,StudentFlag,IsITAdministrator,Employer_ID,CreatedBy,CreatedDate)
VALUES
('jarshort@iusb.edu','Jared','Short','(555) 123-4567','NS','F','','F','','T','F',1,'bob@iu.edu',CURRENT_TIMESTAMP);

INSERT INTO SysUser
(Email,FirstName,LastName,Phone,OfficeInfo,FacultyFlag,FacultyRank,StaffFlag,StaffPosition,StudentFlag,IsITAdministrator,Employer_ID,CreatedBy,CreatedDate)
VALUES
('hauboldj@iusb.edu','Joel','Haubold','(888) 222-2222','NS','F','','F','','T','F',1,'bob@iu.edu',CURRENT_TIMESTAMP);

INSERT INTO SysUser
(Email,FirstName,LastName,Phone,OfficeInfo,FacultyFlag,FacultyRank,StaffFlag,StaffPosition,StudentFlag,IsITAdministrator,Employer_ID,CreatedBy,CreatedDate)
VALUES
('esground@iusb.edu','Eric','Ground','(555) 987-6543','NS','F','','F','','T','F',1,'bob@iu.edu',CURRENT_TIMESTAMP);

INSERT INTO SysUser
(Email,FirstName,LastName,Phone,OfficeInfo,FacultyFlag,FacultyRank,StaffFlag,StaffPosition,StudentFlag,IsITAdministrator,Employer_ID,CreatedBy,CreatedDate)
VALUES
('dqtruong@iusb.edu','Dung','Truong','(123) 456-7890','NS','F','','F','','T','F',1,'bob@iu.edu',CURRENT_TIMESTAMP);