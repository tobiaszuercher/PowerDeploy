USE [master]
if exists(select 1 from sysdatabases where name = '$(DatabaseName)')
begin
  EXEC msdb.dbo.sp_delete_database_backuphistory @database_name = N'$(DatabaseName)'
  drop database [$(DatabaseName)]
  print('Database dropped.')
end else print('Database not found.')