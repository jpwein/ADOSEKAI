IF DB_ID(N'CollegeDB') IS NULL
    CREATE DATABASE CollegeDB;
GO

USE CollegeDB;
GO

IF OBJECT_ID(N'dbo.Groups', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Groups
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Groups PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        Direction nvarchar(200) NOT NULL CONSTRAINT DF_Groups_Direction DEFAULT N''
    );
END;
GO

IF OBJECT_ID(N'dbo.Students', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Students
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Students PRIMARY KEY,
        FirstName nvarchar(100) NOT NULL,
        LastName nvarchar(100) NOT NULL,
        Age int NOT NULL,
        Course int NOT NULL CONSTRAINT DF_Students_Course DEFAULT 1,
        GroupId int NOT NULL,
        CONSTRAINT FK_Students_Groups FOREIGN KEY (GroupId) REFERENCES dbo.Groups(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.Subjects', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Subjects
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Subjects PRIMARY KEY,
        Name nvarchar(150) NOT NULL,
        Credits int NOT NULL
    );
END;
GO

IF OBJECT_ID(N'dbo.Teachers', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Teachers
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Teachers PRIMARY KEY,
        FirstName nvarchar(100) NOT NULL,
        LastName nvarchar(100) NOT NULL,
        Email nvarchar(200) NOT NULL,
        SubjectId int NOT NULL,
        CONSTRAINT FK_Teachers_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(Id)
    );
END;
GO

IF OBJECT_ID(N'dbo.Grades', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.Grades
    (
        Id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_Grades PRIMARY KEY,
        StudentId int NOT NULL,
        SubjectId int NOT NULL,
        TeacherId int NOT NULL,
        Value decimal(5,2) NOT NULL,
        Date datetime2 NOT NULL CONSTRAINT DF_Grades_Date DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Grades_Students FOREIGN KEY (StudentId) REFERENCES dbo.Students(Id),
        CONSTRAINT FK_Grades_Subjects FOREIGN KEY (SubjectId) REFERENCES dbo.Subjects(Id),
        CONSTRAINT FK_Grades_Teachers FOREIGN KEY (TeacherId) REFERENCES dbo.Teachers(Id)
    );
END;
GO

IF NOT EXISTS (SELECT 1 FROM dbo.Groups)
    INSERT INTO dbo.Groups (Name, Direction) VALUES
        (N'ПИ-21', N'Программная инженерия'),
        (N'ИВТ-22', N'Информационные технологии'),
        (N'ПР-23', N'Прикладная информатика');

IF NOT EXISTS (SELECT 1 FROM dbo.Subjects)
    INSERT INTO dbo.Subjects (Name, Credits) VALUES
        (N'ADO.NET', 4), (N'Базы данных', 5), (N'C# и .NET', 4), (N'Веб-разработка', 3);

IF NOT EXISTS (SELECT 1 FROM dbo.Teachers)
    INSERT INTO dbo.Teachers (FirstName, LastName, Email, SubjectId)
    SELECT N'П.Р.', N'Латвия', N'pr.latvia@example.local', Id FROM dbo.Subjects WHERE Name = N'ADO.NET';
    INSERT INTO dbo.Teachers (FirstName, LastName, Email, SubjectId)
    SELECT N'Р.П.', N'Пила', N'rp.pila@example.local', Id FROM dbo.Subjects WHERE Name = N'Базы данных';
    INSERT INTO dbo.Teachers (FirstName, LastName, Email, SubjectId)
    SELECT N'А.В.', N'Север', N'av.sever@example.local', Id FROM dbo.Subjects WHERE Name = N'C# и .NET';

IF NOT EXISTS (SELECT 1 FROM dbo.Students)
BEGIN
    INSERT INTO dbo.Students (FirstName, LastName, Age, Course, GroupId)
    SELECT N'П.Р.', N'Латвия', 20, 2, Id FROM dbo.Groups WHERE Name = N'ПИ-21';
    INSERT INTO dbo.Students (FirstName, LastName, Age, Course, GroupId)
    SELECT N'Р.П.', N'Пила', 21, 2, Id FROM dbo.Groups WHERE Name = N'ПИ-21';
    INSERT INTO dbo.Students (FirstName, LastName, Age, Course, GroupId)
    SELECT N'И.С.', N'Маяк', 19, 1, Id FROM dbo.Groups WHERE Name = N'ИВТ-22';
    INSERT INTO dbo.Students (FirstName, LastName, Age, Course, GroupId)
    SELECT N'К.Д.', N'Река', 22, 4, Id FROM dbo.Groups WHERE Name = N'ИВТ-22';
    INSERT INTO dbo.Students (FirstName, LastName, Age, Course, GroupId)
    SELECT N'В.А.', N'Сокол', 20, 2, Id FROM dbo.Groups WHERE Name = N'ПР-23';
    INSERT INTO dbo.Students (FirstName, LastName, Age, Course, GroupId)
    SELECT N'Н.М.', N'Туман', 21, 3, Id FROM dbo.Groups WHERE Name = N'ПР-23';
END;
GO
