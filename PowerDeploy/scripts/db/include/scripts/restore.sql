PRINT 'Restoring backup...'
RESTORE DATABASE [$(DatabaseName)] FROM DISK = N'$(BackupFile)' WITH NOUNLOAD, REPLACE, STATS = 10