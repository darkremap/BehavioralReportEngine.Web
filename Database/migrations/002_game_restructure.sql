/* =====================================================================================
   MIGRATION 002 — Game table restructuring + new GameType lookup table
   Run this against your existing BehavioralReportEngine database.
   Safe to re-run: every step is guarded with existence checks.
   ===================================================================================== */

USE BehavioralReportEngine;
GO

------------------------------------------------------------------------------------------
-- 1. New lookup table: game.GameType
------------------------------------------------------------------------------------------
IF OBJECT_ID('game.GameType', 'U') IS NULL
BEGIN
    CREATE TABLE game.GameType
    (
        GameTypeId  INT IDENTITY(1,1) NOT NULL,
        TypeCode    NVARCHAR(50)      NOT NULL,
        NameFa      NVARCHAR(150)     NOT NULL,
        NameEn      NVARCHAR(150)     NOT NULL,
        Description NVARCHAR(MAX)     NULL,
        IsActive    BIT               NOT NULL CONSTRAINT DF_GameType_IsActive DEFAULT (1),

        CreatedAt   DATETIME2(3)      NOT NULL CONSTRAINT DF_GameType_CreatedAt DEFAULT (SYSUTCDATETIME()),
        CreatedBy   INT               NULL,
        UpdatedAt   DATETIME2(3)      NULL,
        UpdatedBy   INT               NULL,
        IsDeleted   BIT               NOT NULL CONSTRAINT DF_GameType_IsDeleted DEFAULT (0),

        CONSTRAINT PK_GameType PRIMARY KEY CLUSTERED (GameTypeId),
        CONSTRAINT UQ_GameType_Code UNIQUE (TypeCode)
    );

    -- A few sensible starter rows — edit/remove as you like
    INSERT INTO game.GameType (TypeCode, NameFa, NameEn) VALUES
        ('SIMULATION',   N'شبیه‌سازی',      'Simulation'),
        ('ROLE_PLAY',    N'ایفای نقش',      'Role Play'),
        ('CASE_STUDY',   N'مطالعه موردی',   'Case Study');
END
GO

------------------------------------------------------------------------------------------
-- 2. Add the new columns to game.Game (nullable first, so existing rows don't break)
------------------------------------------------------------------------------------------
IF COL_LENGTH('game.Game', 'NameFa') IS NULL
    ALTER TABLE game.Game ADD NameFa NVARCHAR(200) NULL;
GO
IF COL_LENGTH('game.Game', 'NameEn') IS NULL
    ALTER TABLE game.Game ADD NameEn NVARCHAR(200) NULL;
GO
IF COL_LENGTH('game.Game', 'Summary') IS NULL
    ALTER TABLE game.Game ADD Summary NVARCHAR(500) NULL;
GO
IF COL_LENGTH('game.Game', 'TitleFa') IS NULL
    ALTER TABLE game.Game ADD TitleFa NVARCHAR(200) NULL;
GO
IF COL_LENGTH('game.Game', 'TitleEn') IS NULL
    ALTER TABLE game.Game ADD TitleEn NVARCHAR(200) NULL;
GO
IF COL_LENGTH('game.Game', 'DescriptionFa') IS NULL
    ALTER TABLE game.Game ADD DescriptionFa NVARCHAR(MAX) NULL;
GO
IF COL_LENGTH('game.Game', 'DescriptionEn') IS NULL
    ALTER TABLE game.Game ADD DescriptionEn NVARCHAR(MAX) NULL;
GO
IF COL_LENGTH('game.Game', 'AuthorUserId') IS NULL
    ALTER TABLE game.Game ADD AuthorUserId INT NULL;
GO
IF COL_LENGTH('game.Game', 'GameTypeId') IS NULL
    ALTER TABLE game.Game ADD GameTypeId INT NULL;
GO
IF COL_LENGTH('game.Game', 'DurationMinutes') IS NULL
    ALTER TABLE game.Game ADD DurationMinutes INT NULL;
GO

------------------------------------------------------------------------------------------
-- 3. Backfill NameFa / NameEn from the old single-language columns, if they still exist
------------------------------------------------------------------------------------------
IF COL_LENGTH('game.Game', 'Name') IS NOT NULL
BEGIN
    UPDATE game.Game
    SET NameFa = ISNULL(NameFa, [Name]),
        NameEn = ISNULL(NameEn, [Name])
    WHERE NameFa IS NULL OR NameEn IS NULL;
END
GO
IF COL_LENGTH('game.Game', 'Description') IS NOT NULL
BEGIN
    UPDATE game.Game
    SET DescriptionFa = ISNULL(DescriptionFa, [Description])
    WHERE DescriptionFa IS NULL;
END
GO
-- Any remaining rows with no name at all (shouldn't normally happen) get a placeholder
-- so the NOT NULL constraint below can be applied safely.
UPDATE game.Game SET NameFa = N'(بدون نام)' WHERE NameFa IS NULL;
UPDATE game.Game SET NameEn = '(untitled)' WHERE NameEn IS NULL;
GO

------------------------------------------------------------------------------------------
-- 4. Enforce NOT NULL on NameFa / NameEn now that every row has a value
------------------------------------------------------------------------------------------
ALTER TABLE game.Game ALTER COLUMN NameFa NVARCHAR(200) NOT NULL;
GO
ALTER TABLE game.Game ALTER COLUMN NameEn NVARCHAR(200) NOT NULL;
GO

------------------------------------------------------------------------------------------
-- 5. Drop the old single-language columns
------------------------------------------------------------------------------------------
IF COL_LENGTH('game.Game', 'Name') IS NOT NULL
    ALTER TABLE game.Game DROP COLUMN [Name];
GO
IF COL_LENGTH('game.Game', 'Description') IS NOT NULL
    ALTER TABLE game.Game DROP COLUMN [Description];
GO

------------------------------------------------------------------------------------------
-- 6. Foreign keys: Game.AuthorUserId -> core.AppUser, Game.GameTypeId -> game.GameType
------------------------------------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Game_AuthorUser')
    ALTER TABLE game.Game
        ADD CONSTRAINT FK_Game_AuthorUser FOREIGN KEY (AuthorUserId) REFERENCES core.AppUser (UserId);
GO
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Game_GameType')
    ALTER TABLE game.Game
        ADD CONSTRAINT FK_Game_GameType FOREIGN KEY (GameTypeId) REFERENCES game.GameType (GameTypeId);
GO

/* =====================================================================================
   END OF MIGRATION 002
   ===================================================================================== */
