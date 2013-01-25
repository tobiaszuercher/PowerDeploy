CREATE USER [deployer] FOR LOGIN [ZRHN0871\deployer] WITH DEFAULT_SCHEMA=[dbo]

EXEC sp_addrolemember N'db_datareader', N'deployer'
EXEC sp_addrolemember N'db_datawriter', N'deployer'