--http://msdn.microsoft.com/en-us/library/ms187928.aspx
--http://msdn.microsoft.com/en-us/library/ff848733.aspx
--use the following to handle date and time

drop table test1;

create table test1 (id integer, mdate datetime2);

insert into test1 (id, mdate) values (1, CURRENT_TIMESTAMP);
insert into test1 (id, mdate) values (2, GETDATE());
insert into test1 (id, mdate) values (3, {fn Now()});

select * from test1;

select id, CAST(mdate AS datetime) as 'date' from test1;

select id, CAST(mdate AS varchar) as 'date' from test1;

select id, CONVERT(varchar, mdate, 100) as 'date' from test1;