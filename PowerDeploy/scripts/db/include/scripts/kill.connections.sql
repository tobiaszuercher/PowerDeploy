declare @execSql varchar(1000), @databaseName varchar(100)
set @databaseName = '$(DatabaseName)'

set @execSql = '' 
select  @execSql = @execSql + 'kill ' + convert(char(10), spid) + ';'
from    sysprocesses
where   dbid = db_id(@databaseName)
   and
   DBID <> 0
   and
   spid <> @@spid
if @execSql <> ''
begin
  print(@execSql)
  exec(@execSql)
end else print 'No connections to kill'