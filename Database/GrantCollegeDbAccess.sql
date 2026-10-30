/* Запустите в SSMS с правами администратора SQL Server. */

USE [master];
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'WEIN\weinn')
    CREATE LOGIN [WEIN\weinn] FROM WINDOWS;
GO

USE [CollegeDB];
GO

DECLARE @LoginName sysname = N'WEIN\weinn';
DECLARE @DatabaseUser sysname;
DECLARE @Sql nvarchar(max);

/* Находим пользователя, который уже связан с этим login. */
SELECT TOP (1) @DatabaseUser = database_principals.name
FROM sys.database_principals AS database_principals
JOIN sys.server_principals AS server_principals
    ON database_principals.sid = server_principals.sid
WHERE server_principals.name = @LoginName;

IF @DatabaseUser IS NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.database_principals WHERE name = @LoginName)
    BEGIN
        ALTER USER [WEIN\weinn] WITH LOGIN = [WEIN\weinn];
        SET @DatabaseUser = @LoginName;
    END
    ELSE
    BEGIN
        CREATE USER [WEIN\weinn] FOR LOGIN [WEIN\weinn];
        SET @DatabaseUser = @LoginName;
    END
END;

IF @DatabaseUser <> N'dbo'
BEGIN
    IF IS_ROLEMEMBER(N'db_datareader', @DatabaseUser) = 0
    BEGIN
        SET @Sql = N'ALTER ROLE [db_datareader] ADD MEMBER ' + QUOTENAME(@DatabaseUser) + N';';
        EXEC sys.sp_executesql @Sql;
    END;

    IF IS_ROLEMEMBER(N'db_datawriter', @DatabaseUser) = 0
    BEGIN
        SET @Sql = N'ALTER ROLE [db_datawriter] ADD MEMBER ' + QUOTENAME(@DatabaseUser) + N';';
        EXEC sys.sp_executesql @Sql;
    END;

    IF IS_ROLEMEMBER(N'db_ddladmin', @DatabaseUser) = 0
    BEGIN
        SET @Sql = N'ALTER ROLE [db_ddladmin] ADD MEMBER ' + QUOTENAME(@DatabaseUser) + N';';
        EXEC sys.sp_executesql @Sql;
    END;
END;

SELECT @LoginName AS LoginName, @DatabaseUser AS DatabaseUser,
       N'Права приложения настроены.' AS Result;
