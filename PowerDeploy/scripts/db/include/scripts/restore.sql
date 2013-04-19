PRINT 'Restore backup for $(DatabaseName)'
-- we can't just use:
-- RESTORE DATABASE [$(DatabaseName)] FROM DISK = N'$(BackupFile)' WITH NOUNLOAD, REPLACE, STATS = 10
-- because there are a lot of information saved in the backup like for example the physical path. 
-- Because we want to create a backup of db a und restore it to another db b, we need to create a custom
-- restore database query...

IF EXISTS (SELECT * FROM tempdb.dbo.sysobjects WHERE ID = OBJECT_ID(N'tempdb..#dbfiles'))
BEGIN
DROP TABLE #dbfiles
END

CREATE TABLE #dbfiles(
    LogicalName NVARCHAR(128)
    ,PhysicalName NVARCHAR(260)
    ,Type CHAR(1)
    ,FileGroupName NVARCHAR(128)
    ,Size numeric(20,0)
    ,MaxSize numeric(20,0)
    ,FileId INT
    ,CreateLSN numeric(25,0)
    ,DropLSN numeric(25,0)
    ,UniqueId uniqueidentifier
    ,ReadOnlyLSN numeric(25,0)
    ,ReadWriteLSN numeric(25,0)
    ,BackupSizeInBytes INT
    ,SourceBlockSize INT
    ,FilegroupId INT
    ,LogGroupGUID uniqueidentifier
    ,DifferentialBaseLSN numeric(25)
    ,DifferentialBaseGUID uniqueidentifier
    ,IsReadOnly INT
    ,IsPresent INT
    ,TDEThumbprint varbinary(32)
)
        
INSERT INTO #dbfiles EXEC('RESTORE FILELISTONLY FROM DISK = N''$(BackupFile)''')
        
DECLARE @sql NVARCHAR(MAX)
        
DECLARE @LogicalName NVARCHAR(128)
DECLARE @Folder NVARCHAR(260)
DECLARE @Type CHAR(1)
        
-- This is to find out the most likely target location for the data/log files:
-- We will use the location where most of the already existing data/log files are stored
DECLARE dbfiles CURSOR FOR
WITH x(rank, type, folder) AS
(
    SELECT RANK() OVER(PARTITION BY type ORDER BY count(*) DESC) AS Rank, CASE type WHEN 0 THEN 'D' WHEN 1 THEN 'L' ELSE NULL END, REVERSE(SUBSTRING(REVERSE(physical_name), CHARINDEX(N'\', REVERSE(physical_name)), 1000)) AS folder
    FROM sys.master_files
    GROUP BY type, REVERSE(SUBSTRING(REVERSE(physical_name), CHARINDEX(N'\', REVERSE(physical_name)), 1000))
),
files (type, folder) AS
(
    SELECT type, max(folder)
    FROM x
    WHERE Rank = 1
    GROUP BY type
)
SELECT LogicalName, folder, dbf.Type FROM #dbfiles dbf INNER JOIN files f ON f.type = dbf.Type
ORDER BY dbf.Type -- This is to try and guarantee a valid sql string (see order below where we construct the sql)
        
SET @sql = 'RESTORE DATABASE [$(DatabaseName)] FROM DISK = N''$(BackupFile)'' WITH NOUNLOAD, REPLACE, STATS = 10, MOVE '
        
OPEN dbfiles
FETCH NEXT FROM dbfiles INTO @LogicalName, @Folder, @Type
WHILE @@FETCH_STATUS = 0
BEGIN
    IF @type = 'D'
    SET @sql = @sql + '''' + @LogicalName + ''' TO ''' + @Folder + $(DatabaseName) + '.mdf'', MOVE '
    ELSE IF @type = 'L'
    SET @sql = @sql + '''' + @LogicalName + ''' TO ''' + @Folder + $(DatabaseName) + '_log.ldf'''
FETCH NEXT FROM dbfiles INTO @LogicalName, @Folder, @Type
END
  
-- just to debug the db query      
--PRINT @sql

CLOSE dbfiles
DEALLOCATE dbfiles

DROP TABLE #dbfiles
        
EXEC(@sql)