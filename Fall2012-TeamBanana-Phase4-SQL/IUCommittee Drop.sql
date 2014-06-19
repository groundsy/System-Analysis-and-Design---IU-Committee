-- these statements remove the table from the database
-- tables should be dropped in an order. if table a references table b then a should be dropped first.

USE [jashdown] -- change it to your id

DROP TABLE dbo.DiscItemDocument 
DROP TABLE dbo.Discussion;
DROP TABLE dbo.AnonVoting 
DROP TABLE dbo.DiscItemVoteType 
DROP TABLE dbo.VoteType 
DROP TABLE dbo.DiscItem 
DROP TABLE dbo.Meeting 
DROP TABLE dbo.CommDocument 
DROP TABLE dbo.CommCharge 
DROP TABLE dbo.CommConstitution 
DROP TABLE dbo.CommMember 
DROP TABLE dbo.Comm 
DROP TABLE dbo.CommSuperAdmin 
DROP TABLE dbo.Category 
DROP TABLE dbo.AuditLog 
DROP TABLE dbo.MemberRole 
DROP TABLE dbo.Unit 
DROP TABLE dbo.School 
DROP TABLE dbo.Campus 
DROP TABLE dbo.University 
DROP TABLE dbo.SysUser 
DROP TABLE dbo.Employer 
DROP TABLE dbo.CommOwn 