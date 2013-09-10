IF (DB_ID(N'${SAMPLE_DB_Database_Name}') IS NULL) 
BEGIN 
	PRINT N'Creating ${SAMPLE_DB_Database_Name}...'
	
	CREATE DATABASE [${SAMPLE_DB_Database_Name}]
END