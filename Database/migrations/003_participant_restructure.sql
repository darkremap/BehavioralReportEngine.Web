/* =====================================================================================
   MIGRATION 003 — Participant table restructuring + new Department lookup table
   Run this against your existing BehavioralReportEngine database.
   Safe to re-run: every step is guarded with existence checks.
   ===================================================================================== */

USE BehavioralReportEngine;
GO

------------------------------------------------------------------------------------------
-- 1. New lookup table: core.Department
------------------------------------------------------------------------------------------
IF OBJECT_ID('core.Department', 'U') IS NULL
BEGIN
    CREATE TABLE core.Department
    (
        DepartmentId   INT IDENTITY(1,1) NOT NULL,
        DepartmentCode NVARCHAR(50)      NOT NULL,
        NameFa         NVARCHAR(150)     NOT NULL,
        NameEn         NVARCHAR(150)     NULL,
        Description    NVARCHAR(MAX)     NULL,
        IsActive       BIT               NOT NULL CONSTRAINT DF_Department_IsActive DEFAULT (1),

        CreatedAt      DATETIME2(3)      NOT NULL CONSTRAINT DF_Department_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedBy      INT               NULL,
        UpdatedAt      DATETIME2(3)      NULL,
        UpdatedBy      INT               NULL,
        IsDeleted      BIT               NOT NULL CONSTRAINT DF_Department_IsDeleted DEFAULT (0),

        CONSTRAINT PK_Department PRIMARY KEY CLUSTERED (DepartmentId),
        CONSTRAINT UQ_Department_Code UNIQUE (DepartmentCode)
    );

    -- A few sensible starter rows — edit/remove as you like
    INSERT INTO core.Department (DepartmentCode, NameFa, NameEn) VALUES
        ('SALES',    N'واحد فروش',      'Sales'),
        ('HR',       N'منابع انسانی',   'Human Resources'),
        ('IT',       N'فناوری اطلاعات', 'IT');
END
GO

------------------------------------------------------------------------------------------
-- 2. Add the new columns to core.Participant (nullable first, so existing rows don't break)
------------------------------------------------------------------------------------------
IF COL_LENGTH('core.Participant', 'PersonnelCode') IS NULL
    ALTER TABLE core.Participant ADD PersonnelCode NVARCHAR(100) NULL;
GO
IF COL_LENGTH('core.Participant', 'FirstNameFa') IS NULL
    ALTER TABLE core.Participant ADD FirstNameFa NVARCHAR(100) NULL;
GO
IF COL_LENGTH('core.Participant', 'LastNameFa') IS NULL
    ALTER TABLE core.Participant ADD LastNameFa NVARCHAR(100) NULL;
GO
IF COL_LENGTH('core.Participant', 'FirstNameEn') IS NULL
    ALTER TABLE core.Participant ADD FirstNameEn NVARCHAR(100) NULL;
GO
IF COL_LENGTH('core.Participant', 'LastNameEn') IS NULL
    ALTER TABLE core.Participant ADD LastNameEn NVARCHAR(100) NULL;
GO
IF COL_LENGTH('core.Participant', 'DepartmentId') IS NULL
    ALTER TABLE core.Participant ADD DepartmentId INT NULL;
GO
IF COL_LENGTH('core.Participant', 'BirthDate') IS NULL
    ALTER TABLE core.Participant ADD BirthDate DATE NULL;
GO
IF COL_LENGTH('core.Participant', 'Gender') IS NULL
    ALTER TABLE core.Participant ADD Gender NVARCHAR(20) NULL;
GO
IF COL_LENGTH('core.Participant', 'JobTitle') IS NULL
    ALTER TABLE core.Participant ADD JobTitle NVARCHAR(150) NULL;
GO

------------------------------------------------------------------------------------------
-- 3. Backfill from the old columns, if they still exist
------------------------------------------------------------------------------------------
-- ExternalReference -> PersonnelCode (straight rename in meaning)
IF COL_LENGTH('core.Participant', 'ExternalReference') IS NOT NULL
BEGIN
    UPDATE core.Participant
    SET PersonnelCode = ISNULL(PersonnelCode, ExternalReference)
    WHERE PersonnelCode IS NULL;
END
GO

-- FullName -> split into FirstNameFa / LastNameFa (first word = first name,
-- everything else = last name; review/adjust manually afterwards if needed)
IF COL_LENGTH('core.Participant', 'FullName') IS NOT NULL
BEGIN
    UPDATE core.Participant
    SET FirstNameFa = ISNULL(FirstNameFa, LTRIM(RTRIM(
                          CASE WHEN CHARINDEX(' ', FullName) > 0
                               THEN LEFT(FullName, CHARINDEX(' ', FullName) - 1)
                               ELSE FullName END))),
        LastNameFa  = ISNULL(LastNameFa, LTRIM(RTRIM(
                          CASE WHEN CHARINDEX(' ', FullName) > 0
                               THEN SUBSTRING(FullName, CHARINDEX(' ', FullName) + 1, 200)
                               ELSE N'-' END)))
    WHERE FirstNameFa IS NULL OR LastNameFa IS NULL;
END
GO

-- Any remaining rows with no name at all (shouldn't normally happen) get a
-- placeholder so the NOT NULL constraint below can be applied safely.
UPDATE core.Participant SET FirstNameFa = N'(بدون نام)' WHERE FirstNameFa IS NULL;
UPDATE core.Participant SET LastNameFa  = N'-'          WHERE LastNameFa  IS NULL;
GO

------------------------------------------------------------------------------------------
-- 4. Enforce NOT NULL on FirstNameFa / LastNameFa now that every row has a value
------------------------------------------------------------------------------------------
ALTER TABLE core.Participant ALTER COLUMN FirstNameFa NVARCHAR(100) NOT NULL;
GO
ALTER TABLE core.Participant ALTER COLUMN LastNameFa NVARCHAR(100) NOT NULL;
GO

------------------------------------------------------------------------------------------
-- 5. Drop the old columns
------------------------------------------------------------------------------------------
IF COL_LENGTH('core.Participant', 'FullName') IS NOT NULL
    ALTER TABLE core.Participant DROP COLUMN FullName;
GO
IF COL_LENGTH('core.Participant', 'ExternalReference') IS NOT NULL
    ALTER TABLE core.Participant DROP COLUMN ExternalReference;
GO

------------------------------------------------------------------------------------------
-- 6. Foreign key: Participant.DepartmentId -> core.Department
------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Participant_Department')
    ALTER TABLE core.Participant
        ADD CONSTRAINT FK_Participant_Department FOREIGN KEY (DepartmentId) REFERENCES core.Department (DepartmentId);
GO

/* =====================================================================================
   END OF MIGRATION 003
   ===================================================================================== */
