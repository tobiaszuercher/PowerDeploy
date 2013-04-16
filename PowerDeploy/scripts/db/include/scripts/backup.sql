IF db_id('$(DatabaseName)') IS NOT NULL
BEGIN
  PRINT 'Creating backup...'
  BACKUP DATABASE [$(DatabaseName)] TO DISK = '$(BackupFile)' WITH COPY_ONLY, CHECKSUM
  PRINT 'Veryfing...'
  RESTORE VERIFYONLY FROM DISK = N'$(BackupFile)' WITH NOUNLOAD, NOREWIND
END
ELSE
  PRINT 'This database does not seem to exist --> CANNOT CREATE BACKUP!'