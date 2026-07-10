/* =====================================================================================
   BEHAVIORAL REPORT ENGINE - DATABASE SCHEMA
   Platform: Microsoft SQL Server 2019+
   Purpose : Multi-game, multi-organization behavioral analytics reporting platform
             (SELVA, HERMIOS, Crisis Orbit, Wave Riding, and future games)

   Design principles:
     - Schemas separate concerns: core (identity/org), game (definitions & versioning),
       session (what happened), template (report structure), report (generated content)
     - Nothing hardcodes "5 indicators" - indicator count is driven by rows, not columns
     - Scoring scales are data, not hardcoded 1-5 logic
     - Report templates are decoupled from report content (engine reusability)
     - Soft delete + audit trail on all business tables
     - JSON used ONLY for flexible/non-queried metadata (see notes at bottom)

   HOW TO RUN:
     - Open this file in SQL Server Management Studio (SSMS) or Azure Data Studio.
     - Execute the whole file (F5). It will create the database if it does not
       already exist, then create all schemas, tables, constraints, indexes and
       seed data inside it.
     - Safe to re-run only on a fresh database: table creation will fail on
       already-existing objects (by design, to avoid silently altering a live
       schema). To rebuild from scratch:  DROP DATABASE BehavioralReportEngine;
   ===================================================================================== */

------------------------------------------------------------------------------------------
-- 0. CREATE DATABASE (only if it does not already exist)
------------------------------------------------------------------------------------------
IF DB_ID(N'BehavioralReportEngine') IS NULL
BEGIN
    CREATE DATABASE BehavioralReportEngine;
END
GO

USE BehavioralReportEngine;
GO

------------------------------------------------------------------------------------------
-- SCHEMAS
------------------------------------------------------------------------------------------
GO
CREATE SCHEMA core AUTHORIZATION dbo;
GO
CREATE SCHEMA game AUTHORIZATION dbo;
GO
CREATE SCHEMA session AUTHORIZATION dbo;
GO
CREATE SCHEMA template AUTHORIZATION dbo;
GO
CREATE SCHEMA report AUTHORIZATION dbo;
GO

/* =====================================================================================
   SCHEMA: core  -- organizations, users, participants
   ===================================================================================== */

CREATE TABLE core.Organization
(
    OrganizationId   INT IDENTITY(1,1) NOT NULL,
    OrganizationCode NVARCHAR(50)      NOT NULL,
    Name             NVARCHAR(200)     NOT NULL,
    Industry         NVARCHAR(100)     NULL,
    Country          NVARCHAR(100)     NULL,
    LogoPath         NVARCHAR(500)     NULL,
    IsActive         BIT               NOT NULL CONSTRAINT DF_Organization_IsActive DEFAULT (1),

    CreatedAt        DATETIME2(3)      NOT NULL CONSTRAINT DF_Organization_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy        INT               NULL,
    UpdatedAt        DATETIME2(3)      NULL,
    UpdatedBy        INT               NULL,
    IsDeleted        BIT               NOT NULL CONSTRAINT DF_Organization_IsDeleted DEFAULT (0),
    DeletedAt        DATETIME2(3)      NULL,
    DeletedBy        INT               NULL,

    CONSTRAINT PK_Organization PRIMARY KEY CLUSTERED (OrganizationId),
    CONSTRAINT UQ_Organization_Code UNIQUE (OrganizationCode)
);

-- Run this on databases created before LogoPath existed:
-- ALTER TABLE core.Organization ADD LogoPath NVARCHAR(500) NULL;
GO

CREATE TABLE core.UserRole
(
    RoleId      INT IDENTITY(1,1) NOT NULL,
    RoleCode    NVARCHAR(50)      NOT NULL,
    RoleName    NVARCHAR(100)     NOT NULL,
    Description NVARCHAR(400)    NULL,

    CONSTRAINT PK_UserRole PRIMARY KEY CLUSTERED (RoleId),
    CONSTRAINT UQ_UserRole_Code UNIQUE (RoleCode)
);
GO
-- Seed baseline roles (extensible: add rows, no schema change needed)
INSERT INTO core.UserRole (RoleCode, RoleName, Description) VALUES
    ('ADMIN',       'Platform Administrator', 'Full platform administration'),
    ('ORG_ADMIN',   'Organization Administrator', 'Manages a single organization'),
    ('FACILITATOR', 'Facilitator', 'Runs game sessions'),
    ('ANALYST',     'Behavioral Analyst', 'Reviews and authors report content'),
    ('VIEWER',      'Report Viewer', 'Read-only access to reports');
GO

CREATE TABLE core.AppUser
(
    UserId         INT IDENTITY(1,1) NOT NULL,
    OrganizationId INT               NULL,          -- NULL = internal platform staff
    FullName       NVARCHAR(200)     NOT NULL,
    Email          NVARCHAR(256)     NOT NULL,
    IsActive       BIT               NOT NULL CONSTRAINT DF_AppUser_IsActive DEFAULT (1),

    CreatedAt      DATETIME2(3)      NOT NULL CONSTRAINT DF_AppUser_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy      INT               NULL,
    UpdatedAt      DATETIME2(3)      NULL,
    UpdatedBy      INT               NULL,
    IsDeleted      BIT               NOT NULL CONSTRAINT DF_AppUser_IsDeleted DEFAULT (0),
    DeletedAt      DATETIME2(3)      NULL,
    DeletedBy      INT               NULL,

    CONSTRAINT PK_AppUser PRIMARY KEY CLUSTERED (UserId),
    CONSTRAINT UQ_AppUser_Email UNIQUE (Email),
    CONSTRAINT FK_AppUser_Organization FOREIGN KEY (OrganizationId)
        REFERENCES core.Organization (OrganizationId)
);
GO

CREATE TABLE core.AppUserRole
(
    AppUserRoleId INT IDENTITY(1,1) NOT NULL,
    UserId        INT               NOT NULL,
    RoleId        INT               NOT NULL,

    CreatedAt     DATETIME2(3)      NOT NULL CONSTRAINT DF_AppUserRole_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy     INT               NULL,

    CONSTRAINT PK_AppUserRole PRIMARY KEY CLUSTERED (AppUserRoleId),
    CONSTRAINT UQ_AppUserRole UNIQUE (UserId, RoleId),
    CONSTRAINT FK_AppUserRole_User FOREIGN KEY (UserId) REFERENCES core.AppUser (UserId),
    CONSTRAINT FK_AppUserRole_Role FOREIGN KEY (RoleId) REFERENCES core.UserRole (RoleId)
);
GO

CREATE TABLE core.Participant
(
    ParticipantId      INT IDENTITY(1,1) NOT NULL,
    OrganizationId     INT               NOT NULL,
    ExternalReference  NVARCHAR(100)     NULL,       -- ERP / HRIS employee id, for future integration
    FullName           NVARCHAR(200)     NOT NULL,
    Email              NVARCHAR(256)     NULL,
    MetadataJson       NVARCHAR(MAX)     NULL,        -- flexible org-specific attrs (dept, position...) - see JSON notes
    IsActive           BIT               NOT NULL CONSTRAINT DF_Participant_IsActive DEFAULT (1),

    CreatedAt          DATETIME2(3)      NOT NULL CONSTRAINT DF_Participant_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy          INT               NULL,
    UpdatedAt          DATETIME2(3)      NULL,
    UpdatedBy          INT               NULL,
    IsDeleted          BIT               NOT NULL CONSTRAINT DF_Participant_IsDeleted DEFAULT (0),
    DeletedAt          DATETIME2(3)      NULL,
    DeletedBy          INT               NULL,

    CONSTRAINT PK_Participant PRIMARY KEY CLUSTERED (ParticipantId),
    CONSTRAINT FK_Participant_Organization FOREIGN KEY (OrganizationId)
        REFERENCES core.Organization (OrganizationId),
    CONSTRAINT FK_Participant_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES core.AppUser (UserId),
    CONSTRAINT FK_Participant_UpdatedBy FOREIGN KEY (UpdatedBy) REFERENCES core.AppUser (UserId),
    CONSTRAINT CK_Participant_MetadataJson CHECK (MetadataJson IS NULL OR ISJSON(MetadataJson) = 1)
);
GO
CREATE NONCLUSTERED INDEX IX_Participant_Organization ON core.Participant (OrganizationId) WHERE IsDeleted = 0;
GO

/* =====================================================================================
   SCHEMA: game  -- game & analytical-framework definitions (master data), versioned
   ===================================================================================== */

-- Lookup table for the kind of game/exercise (Simulation, Role Play, Case Study...).
-- Kept as its own table so new types can be added without a schema change.
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
GO

CREATE TABLE game.Game
(
    GameId          INT IDENTITY(1,1) NOT NULL,
    GameCode        NVARCHAR(50)      NOT NULL,    -- SELVA, HERMIOS, CRISIS_ORBIT, WAVE_RIDING
    NameFa          NVARCHAR(200)     NOT NULL,    -- نام فارسی
    NameEn          NVARCHAR(200)     NOT NULL,    -- نام انگلیسی
    Summary         NVARCHAR(500)     NULL,        -- خلاصه
    TitleFa         NVARCHAR(200)     NULL,        -- عنوان فارسی
    TitleEn         NVARCHAR(200)     NULL,        -- عنوان انگلیسی
    DescriptionFa   NVARCHAR(MAX)     NULL,        -- توضیحات فارسی
    DescriptionEn   NVARCHAR(MAX)     NULL,        -- توضیحات انگلیسی
    AuthorUserId    INT               NULL,        -- مولف (core.AppUser)
    GameTypeId      INT               NULL,        -- نوع بازی
    DurationMinutes INT               NULL,        -- مدت بازی
    IsActive        BIT               NOT NULL CONSTRAINT DF_Game_IsActive DEFAULT (1),

    CreatedAt   DATETIME2(3)      NOT NULL CONSTRAINT DF_Game_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy   INT               NULL,
    UpdatedAt   DATETIME2(3)      NULL,
    UpdatedBy   INT               NULL,
    IsDeleted   BIT               NOT NULL CONSTRAINT DF_Game_IsDeleted DEFAULT (0),
    DeletedAt   DATETIME2(3)      NULL,
    DeletedBy   INT               NULL,

    CONSTRAINT PK_Game PRIMARY KEY CLUSTERED (GameId),
    CONSTRAINT UQ_Game_Code UNIQUE (GameCode),
    CONSTRAINT FK_Game_AuthorUser FOREIGN KEY (AuthorUserId) REFERENCES core.AppUser (UserId),
    CONSTRAINT FK_Game_GameType FOREIGN KEY (GameTypeId) REFERENCES game.GameType (GameTypeId)
);
GO

-- Framework versioning: every indicator/pattern set belongs to a GameVersion,
-- so frameworks can evolve without breaking historical reports.
CREATE TABLE game.GameVersion
(
    GameVersionId INT IDENTITY(1,1) NOT NULL,
    GameId        INT               NOT NULL,
    VersionNumber NVARCHAR(20)      NOT NULL,   -- e.g. '1.0', '2.1'
    VersionLabel  NVARCHAR(100)     NULL,
    EffectiveFrom DATETIME2(3)      NOT NULL CONSTRAINT DF_GameVersion_EffFrom DEFAULT (SYSUTCDATETIME()),
    EffectiveTo   DATETIME2(3)      NULL,
    IsCurrent     BIT               NOT NULL CONSTRAINT DF_GameVersion_IsCurrent DEFAULT (0),
    ReleaseNotes  NVARCHAR(MAX)     NULL,

    CreatedAt     DATETIME2(3)      NOT NULL CONSTRAINT DF_GameVersion_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy     INT               NULL,
    UpdatedAt     DATETIME2(3)      NULL,
    UpdatedBy     INT               NULL,
    IsDeleted     BIT               NOT NULL CONSTRAINT DF_GameVersion_IsDeleted DEFAULT (0),
    DeletedAt     DATETIME2(3)      NULL,
    DeletedBy     INT               NULL,

    CONSTRAINT PK_GameVersion PRIMARY KEY CLUSTERED (GameVersionId),
    CONSTRAINT UQ_GameVersion UNIQUE (GameId, VersionNumber),
    CONSTRAINT FK_GameVersion_Game FOREIGN KEY (GameId) REFERENCES game.Game (GameId)
);
GO
CREATE NONCLUSTERED INDEX IX_GameVersion_Current ON game.GameVersion (GameId) WHERE IsCurrent = 1;
GO

-- Scoring scale as data: supports 1-5 today, 0-10 or Likert scales tomorrow
CREATE TABLE game.ScoringScale
(
    ScoringScaleId INT IDENTITY(1,1) NOT NULL,
    ScaleCode      NVARCHAR(50)      NOT NULL,     -- '1-5-STANDARD', '0-10-EXTENDED'
    Name           NVARCHAR(100)     NOT NULL,
    MinValue       DECIMAL(5,2)      NOT NULL,
    MaxValue       DECIMAL(5,2)      NOT NULL,
    StepValue      DECIMAL(5,2)      NOT NULL CONSTRAINT DF_ScoringScale_Step DEFAULT (1),
    Description    NVARCHAR(MAX)     NULL,

    CreatedAt      DATETIME2(3)      NOT NULL CONSTRAINT DF_ScoringScale_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy      INT               NULL,
    UpdatedAt      DATETIME2(3)      NULL,
    UpdatedBy      INT               NULL,
    IsDeleted      BIT               NOT NULL CONSTRAINT DF_ScoringScale_IsDeleted DEFAULT (0),

    CONSTRAINT PK_ScoringScale PRIMARY KEY CLUSTERED (ScoringScaleId),
    CONSTRAINT UQ_ScoringScale_Code UNIQUE (ScaleCode),
    CONSTRAINT CK_ScoringScale_Range CHECK (MaxValue > MinValue)
);
GO

CREATE TABLE game.ScoringScaleLevel
(
    ScoringScaleLevelId INT IDENTITY(1,1) NOT NULL,
    ScoringScaleId      INT               NOT NULL,
    LevelValue          DECIMAL(5,2)      NOT NULL,
    LevelLabel          NVARCHAR(100)     NOT NULL,   -- e.g. 'Needs Development' ... 'Exceptional'
    LevelDescription    NVARCHAR(MAX)     NULL,

    CONSTRAINT PK_ScoringScaleLevel PRIMARY KEY CLUSTERED (ScoringScaleLevelId),
    CONSTRAINT UQ_ScoringScaleLevel UNIQUE (ScoringScaleId, LevelValue),
    CONSTRAINT FK_ScoringScaleLevel_Scale FOREIGN KEY (ScoringScaleId)
        REFERENCES game.ScoringScale (ScoringScaleId)
);
GO

-- Seed the default 1-5 scale used by SELVA/HERMIOS/Crisis Orbit/Wave Riding today.
-- Additional scales (e.g. 0-10) can be added later with no schema change.
INSERT INTO game.ScoringScale (ScaleCode, Name, MinValue, MaxValue, StepValue, Description)
VALUES ('1-5-STANDARD', 'Standard 1 to 5 Scale', 1, 5, 1, 'Default indicator scoring scale (1 = Needs Development, 5 = Exceptional)');
GO

INSERT INTO game.ScoringScaleLevel (ScoringScaleId, LevelValue, LevelLabel, LevelDescription)
SELECT ScoringScaleId, v.LevelValue, v.LevelLabel, NULL
FROM game.ScoringScale s
CROSS APPLY (VALUES
    (1, N'Needs Development'),
    (2, N'Emerging'),
    (3, N'Developing'),
    (4, N'Proficient'),
    (5, N'Exceptional')
) AS v(LevelValue, LevelLabel)
WHERE s.ScaleCode = '1-5-STANDARD';
GO

-- The 5 (or N) main analytical indicators per game version. Count is NOT hardcoded:
-- it is simply however many rows exist for a given GameVersionId.
CREATE TABLE game.IndicatorDefinition
(
    IndicatorDefinitionId INT IDENTITY(1,1) NOT NULL,
    GameVersionId         INT               NOT NULL,
    IndicatorCode         NVARCHAR(50)      NOT NULL,   -- ACTIVE_LISTENING, EMPATHY...
    Title                 NVARCHAR(200)     NOT NULL,   -- native-language title
    TitleEn               NVARCHAR(200)     NOT NULL,   -- English title
    ShortDefinition       NVARCHAR(MAX)     NULL,
    DisplayOrder          INT               NOT NULL CONSTRAINT DF_IndicatorDefinition_Order DEFAULT (0),
    ScoringScaleId         INT              NOT NULL,
    IsActive              BIT               NOT NULL CONSTRAINT DF_IndicatorDefinition_IsActive DEFAULT (1),

    CreatedAt             DATETIME2(3)      NOT NULL CONSTRAINT DF_IndicatorDefinition_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy             INT               NULL,
    UpdatedAt             DATETIME2(3)      NULL,
    UpdatedBy             INT               NULL,
    IsDeleted             BIT               NOT NULL CONSTRAINT DF_IndicatorDefinition_IsDeleted DEFAULT (0),

    CONSTRAINT PK_IndicatorDefinition PRIMARY KEY CLUSTERED (IndicatorDefinitionId),
    CONSTRAINT UQ_IndicatorDefinition UNIQUE (GameVersionId, IndicatorCode),
    CONSTRAINT FK_IndicatorDefinition_GameVersion FOREIGN KEY (GameVersionId)
        REFERENCES game.GameVersion (GameVersionId),
    CONSTRAINT FK_IndicatorDefinition_ScoringScale FOREIGN KEY (ScoringScaleId)
        REFERENCES game.ScoringScale (ScoringScaleId)
);
GO
CREATE NONCLUSTERED INDEX IX_IndicatorDefinition_GameVersion
    ON game.IndicatorDefinition (GameVersionId, DisplayOrder) WHERE IsDeleted = 0;
GO

-- Behavioral patterns: global catalogue (Analyst, Observer, Mediator...), reusable across games
CREATE TABLE game.BehavioralPattern
(
    BehavioralPatternId INT IDENTITY(1,1) NOT NULL,
    PatternCode         NVARCHAR(50)      NOT NULL,   -- ANALYST, OBSERVER, MEDIATOR...
    Name                NVARCHAR(150)     NOT NULL,
    NameEn              NVARCHAR(150)     NOT NULL,
    Description         NVARCHAR(MAX)     NULL,
    IsGlobal            BIT               NOT NULL CONSTRAINT DF_BehavioralPattern_IsGlobal DEFAULT (1),

    CreatedAt           DATETIME2(3)      NOT NULL CONSTRAINT DF_BehavioralPattern_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy           INT               NULL,
    UpdatedAt           DATETIME2(3)      NULL,
    UpdatedBy           INT               NULL,
    IsDeleted           BIT               NOT NULL CONSTRAINT DF_BehavioralPattern_IsDeleted DEFAULT (0),

    CONSTRAINT PK_BehavioralPattern PRIMARY KEY CLUSTERED (BehavioralPatternId),
    CONSTRAINT UQ_BehavioralPattern_Code UNIQUE (PatternCode)
);
GO

-- Which patterns apply to which game version, with optional per-game override text
CREATE TABLE game.GamePattern
(
    GamePatternId       INT IDENTITY(1,1) NOT NULL,
    GameVersionId       INT               NOT NULL,
    BehavioralPatternId INT               NOT NULL,
    CustomName          NVARCHAR(150)     NULL,     -- override display name for this game
    CustomDescription   NVARCHAR(MAX)     NULL,
    DisplayOrder        INT               NOT NULL CONSTRAINT DF_GamePattern_Order DEFAULT (0),

    CreatedAt           DATETIME2(3)      NOT NULL CONSTRAINT DF_GamePattern_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy           INT               NULL,

    CONSTRAINT PK_GamePattern PRIMARY KEY CLUSTERED (GamePatternId),
    CONSTRAINT UQ_GamePattern UNIQUE (GameVersionId, BehavioralPatternId),
    CONSTRAINT FK_GamePattern_GameVersion FOREIGN KEY (GameVersionId)
        REFERENCES game.GameVersion (GameVersionId),
    CONSTRAINT FK_GamePattern_Pattern FOREIGN KEY (BehavioralPatternId)
        REFERENCES game.BehavioralPattern (BehavioralPatternId)
);
GO

/* =====================================================================================
   SCHEMA: session -- what actually happened: sessions, teams, participants, media,
                      transcripts, observations, key moments
   ===================================================================================== */

CREATE TABLE session.GameSession
(
    SessionId     INT IDENTITY(1,1) NOT NULL,
    OrganizationId INT              NOT NULL,
    GameVersionId INT               NOT NULL,
    SessionCode   NVARCHAR(50)      NULL,
    SessionName   NVARCHAR(200)     NULL,
    SessionDate   DATE              NOT NULL,
    StartTime     DATETIME2(3)      NULL,
    EndTime       DATETIME2(3)      NULL,
    Location      NVARCHAR(200)     NULL,
    Status        NVARCHAR(30)      NOT NULL CONSTRAINT DF_GameSession_Status DEFAULT ('Scheduled'),

    CreatedAt     DATETIME2(3)      NOT NULL CONSTRAINT DF_GameSession_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy     INT               NULL,
    UpdatedAt     DATETIME2(3)      NULL,
    UpdatedBy     INT               NULL,
    IsDeleted     BIT               NOT NULL CONSTRAINT DF_GameSession_IsDeleted DEFAULT (0),
    DeletedAt     DATETIME2(3)      NULL,
    DeletedBy     INT               NULL,

    CONSTRAINT PK_GameSession PRIMARY KEY CLUSTERED (SessionId),
    CONSTRAINT UQ_GameSession_Code UNIQUE (SessionCode),
    CONSTRAINT FK_GameSession_Organization FOREIGN KEY (OrganizationId)
        REFERENCES core.Organization (OrganizationId),
    CONSTRAINT FK_GameSession_GameVersion FOREIGN KEY (GameVersionId)
        REFERENCES game.GameVersion (GameVersionId),
    CONSTRAINT CK_GameSession_Status CHECK (Status IN ('Scheduled','InProgress','Completed','Cancelled'))
);
GO
CREATE NONCLUSTERED INDEX IX_GameSession_Org_Date ON session.GameSession (OrganizationId, SessionDate) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_GameSession_GameVersion ON session.GameSession (GameVersionId);
GO

CREATE TABLE session.SessionFacilitator
(
    SessionFacilitatorId INT IDENTITY(1,1) NOT NULL,
    SessionId            INT               NOT NULL,
    UserId               INT               NOT NULL,
    RoleInSession        NVARCHAR(50)      NULL,   -- Lead Facilitator, Co-Facilitator, Observer

    CreatedAt            DATETIME2(3)      NOT NULL CONSTRAINT DF_SessionFacilitator_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy            INT               NULL,

    CONSTRAINT PK_SessionFacilitator PRIMARY KEY CLUSTERED (SessionFacilitatorId),
    CONSTRAINT UQ_SessionFacilitator UNIQUE (SessionId, UserId),
    CONSTRAINT FK_SessionFacilitator_Session FOREIGN KEY (SessionId)
        REFERENCES session.GameSession (SessionId),
    CONSTRAINT FK_SessionFacilitator_User FOREIGN KEY (UserId)
        REFERENCES core.AppUser (UserId)
);
GO

CREATE TABLE session.Team
(
    TeamId    INT IDENTITY(1,1) NOT NULL,
    SessionId INT               NOT NULL,
    TeamName  NVARCHAR(150)     NOT NULL,
    TeamCode  NVARCHAR(50)      NULL,

    CreatedAt DATETIME2(3)      NOT NULL CONSTRAINT DF_Team_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy INT               NULL,
    IsDeleted BIT               NOT NULL CONSTRAINT DF_Team_IsDeleted DEFAULT (0),

    CONSTRAINT PK_Team PRIMARY KEY CLUSTERED (TeamId),
    CONSTRAINT UQ_Team UNIQUE (SessionId, TeamName),
    CONSTRAINT FK_Team_Session FOREIGN KEY (SessionId) REFERENCES session.GameSession (SessionId)
);
GO

CREATE TABLE session.SessionParticipant
(
    SessionParticipantId INT IDENTITY(1,1) NOT NULL,
    SessionId            INT               NOT NULL,
    TeamId                INT              NULL,
    ParticipantId         INT              NOT NULL,

    CreatedAt             DATETIME2(3)     NOT NULL CONSTRAINT DF_SessionParticipant_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy              INT             NULL,
    IsDeleted              BIT             NOT NULL CONSTRAINT DF_SessionParticipant_IsDeleted DEFAULT (0),

    CONSTRAINT PK_SessionParticipant PRIMARY KEY CLUSTERED (SessionParticipantId),
    CONSTRAINT UQ_SessionParticipant UNIQUE (SessionId, ParticipantId),
    CONSTRAINT FK_SessionParticipant_Session FOREIGN KEY (SessionId)
        REFERENCES session.GameSession (SessionId),
    CONSTRAINT FK_SessionParticipant_Team FOREIGN KEY (TeamId)
        REFERENCES session.Team (TeamId),
    CONSTRAINT FK_SessionParticipant_Participant FOREIGN KEY (ParticipantId)
        REFERENCES core.Participant (ParticipantId)
);
GO
CREATE NONCLUSTERED INDEX IX_SessionParticipant_Participant ON session.SessionParticipant (ParticipantId);
CREATE NONCLUSTERED INDEX IX_SessionParticipant_Team ON session.SessionParticipant (TeamId);
GO

CREATE TABLE session.MediaAsset
(
    MediaAssetId    BIGINT IDENTITY(1,1) NOT NULL,
    SessionId       INT                  NOT NULL,
    MediaType       NVARCHAR(20)         NOT NULL,   -- Video, Audio
    FileName        NVARCHAR(300)        NOT NULL,
    StoragePath     NVARCHAR(500)        NOT NULL,    -- blob/URL reference
    DurationSeconds INT                  NULL,
    RecordedAt      DATETIME2(3)         NULL,

    CreatedAt       DATETIME2(3)         NOT NULL CONSTRAINT DF_MediaAsset_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy       INT                  NULL,
    IsDeleted       BIT                  NOT NULL CONSTRAINT DF_MediaAsset_IsDeleted DEFAULT (0),

    CONSTRAINT PK_MediaAsset PRIMARY KEY CLUSTERED (MediaAssetId),
    CONSTRAINT FK_MediaAsset_Session FOREIGN KEY (SessionId) REFERENCES session.GameSession (SessionId),
    CONSTRAINT CK_MediaAsset_Type CHECK (MediaType IN ('Video','Audio'))
);
GO
CREATE NONCLUSTERED INDEX IX_MediaAsset_Session ON session.MediaAsset (SessionId);
GO

CREATE TABLE session.Transcript
(
    TranscriptId  INT IDENTITY(1,1) NOT NULL,
    SessionId     INT               NOT NULL,
    MediaAssetId  BIGINT            NULL,
    Language      NVARCHAR(10)      NULL,
    FullText      NVARCHAR(MAX)     NULL,

    CreatedAt     DATETIME2(3)      NOT NULL CONSTRAINT DF_Transcript_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy     INT               NULL,
    IsDeleted     BIT               NOT NULL CONSTRAINT DF_Transcript_IsDeleted DEFAULT (0),

    CONSTRAINT PK_Transcript PRIMARY KEY CLUSTERED (TranscriptId),
    CONSTRAINT FK_Transcript_Session FOREIGN KEY (SessionId) REFERENCES session.GameSession (SessionId),
    CONSTRAINT FK_Transcript_MediaAsset FOREIGN KEY (MediaAssetId) REFERENCES session.MediaAsset (MediaAssetId)
);
GO

CREATE TABLE session.TranscriptSegment
(
    TranscriptSegmentId  BIGINT IDENTITY(1,1) NOT NULL,
    TranscriptId         INT                  NOT NULL,
    SpeakerParticipantId  INT                 NULL,
    SpeakerLabel          NVARCHAR(100)       NULL,   -- e.g. 'Facilitator', used if participant unknown
    StartTimeMs           INT                 NOT NULL,
    EndTimeMs             INT                 NULL,
    SegmentText           NVARCHAR(MAX)       NOT NULL,

    CreatedAt             DATETIME2(3)        NOT NULL CONSTRAINT DF_TranscriptSegment_CreatedAt DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT PK_TranscriptSegment PRIMARY KEY CLUSTERED (TranscriptSegmentId),
    CONSTRAINT FK_TranscriptSegment_Transcript FOREIGN KEY (TranscriptId)
        REFERENCES session.Transcript (TranscriptId),
    CONSTRAINT FK_TranscriptSegment_Participant FOREIGN KEY (SpeakerParticipantId)
        REFERENCES core.Participant (ParticipantId)
);
GO
CREATE NONCLUSTERED INDEX IX_TranscriptSegment_Transcript_Time
    ON session.TranscriptSegment (TranscriptId, StartTimeMs);
GO

CREATE TABLE session.Observation
(
    ObservationId         BIGINT IDENTITY(1,1) NOT NULL,
    SessionId             INT                  NOT NULL,
    ParticipantId         INT                  NULL,
    TeamId                INT                  NULL,
    ObservedByUserId      INT                  NOT NULL,
    IndicatorDefinitionId  INT                 NULL,
    BehavioralPatternId    INT                 NULL,
    ObservationTimeMs      INT                 NULL,
    ObservationText        NVARCHAR(MAX)       NOT NULL,

    CreatedAt              DATETIME2(3)        NOT NULL CONSTRAINT DF_Observation_CreatedAt DEFAULT (SYSUTCDATETIME()),
    IsDeleted               BIT                NOT NULL CONSTRAINT DF_Observation_IsDeleted DEFAULT (0),

    CONSTRAINT PK_Observation PRIMARY KEY CLUSTERED (ObservationId),
    CONSTRAINT FK_Observation_Session FOREIGN KEY (SessionId) REFERENCES session.GameSession (SessionId),
    CONSTRAINT FK_Observation_Participant FOREIGN KEY (ParticipantId) REFERENCES core.Participant (ParticipantId),
    CONSTRAINT FK_Observation_Team FOREIGN KEY (TeamId) REFERENCES session.Team (TeamId),
    CONSTRAINT FK_Observation_User FOREIGN KEY (ObservedByUserId) REFERENCES core.AppUser (UserId),
    CONSTRAINT FK_Observation_Indicator FOREIGN KEY (IndicatorDefinitionId)
        REFERENCES game.IndicatorDefinition (IndicatorDefinitionId),
    CONSTRAINT FK_Observation_Pattern FOREIGN KEY (BehavioralPatternId)
        REFERENCES game.BehavioralPattern (BehavioralPatternId)
);
GO
CREATE NONCLUSTERED INDEX IX_Observation_Session ON session.Observation (SessionId);
CREATE NONCLUSTERED INDEX IX_Observation_Participant ON session.Observation (ParticipantId);
GO

CREATE TABLE session.KeyMoment
(
    KeyMomentId           INT IDENTITY(1,1) NOT NULL,
    SessionId             INT               NOT NULL,
    TeamId                INT               NULL,
    ParticipantId         INT               NULL,
    MediaAssetId          BIGINT            NULL,
    TranscriptSegmentId   BIGINT            NULL,
    TimestampMs           INT               NOT NULL,
    Title                 NVARCHAR(200)     NOT NULL,
    Description           NVARCHAR(MAX)     NULL,
    Significance          NVARCHAR(MAX)     NULL,

    CreatedAt             DATETIME2(3)      NOT NULL CONSTRAINT DF_KeyMoment_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy             INT               NULL,
    IsDeleted             BIT               NOT NULL CONSTRAINT DF_KeyMoment_IsDeleted DEFAULT (0),

    CONSTRAINT PK_KeyMoment PRIMARY KEY CLUSTERED (KeyMomentId),
    CONSTRAINT FK_KeyMoment_Session FOREIGN KEY (SessionId) REFERENCES session.GameSession (SessionId),
    CONSTRAINT FK_KeyMoment_Team FOREIGN KEY (TeamId) REFERENCES session.Team (TeamId),
    CONSTRAINT FK_KeyMoment_Participant FOREIGN KEY (ParticipantId) REFERENCES core.Participant (ParticipantId),
    CONSTRAINT FK_KeyMoment_MediaAsset FOREIGN KEY (MediaAssetId) REFERENCES session.MediaAsset (MediaAssetId),
    CONSTRAINT FK_KeyMoment_TranscriptSegment FOREIGN KEY (TranscriptSegmentId)
        REFERENCES session.TranscriptSegment (TranscriptSegmentId)
);
GO
CREATE NONCLUSTERED INDEX IX_KeyMoment_Session_Time ON session.KeyMoment (SessionId, TimestampMs);
GO

/* =====================================================================================
   SCHEMA: template -- reusable report structure, decoupled from generated content
   ===================================================================================== */

CREATE TABLE template.ReportModuleType
(
    ReportModuleTypeId INT IDENTITY(1,1) NOT NULL,
    ModuleCode          NVARCHAR(50)     NOT NULL,  -- COVER, INDICATOR_OVERVIEW, INDICATOR_DETAIL,
                                                      -- BEHAVIORAL_PATTERNS, KEY_MOMENTS, GROWTH_MAP,
                                                      -- ACTION_PLAN, FINAL_SUMMARY
    Name                NVARCHAR(150)    NOT NULL,
    Description         NVARCHAR(MAX)    NULL,

    CONSTRAINT PK_ReportModuleType PRIMARY KEY CLUSTERED (ReportModuleTypeId),
    CONSTRAINT UQ_ReportModuleType_Code UNIQUE (ModuleCode)
);
GO
INSERT INTO template.ReportModuleType (ModuleCode, Name) VALUES
    ('COVER',               'Cover Page'),
    ('INDICATOR_OVERVIEW',  'Indicator Overview'),
    ('INDICATOR_DETAIL',    'Indicator Detail Page'),
    ('BEHAVIORAL_PATTERNS', 'Behavioral Patterns'),
    ('KEY_MOMENTS',         'Key Moments'),
    ('GROWTH_MAP',          'Growth Map'),
    ('ACTION_PLAN',         'Practical Action Plan'),
    ('FINAL_SUMMARY',       'Final Summary');
GO

CREATE TABLE template.ReportTemplate
(
    ReportTemplateId INT IDENTITY(1,1) NOT NULL,
    GameVersionId    INT               NULL,     -- NULL = generic/cross-game template (e.g. org rollup)
    ReportScope      NVARCHAR(20)      NOT NULL, -- Individual, Team, Organization
    TemplateName     NVARCHAR(200)     NOT NULL,
    VersionNumber    NVARCHAR(20)      NOT NULL,
    IsActive         BIT               NOT NULL CONSTRAINT DF_ReportTemplate_IsActive DEFAULT (1),

    CreatedAt        DATETIME2(3)      NOT NULL CONSTRAINT DF_ReportTemplate_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy        INT               NULL,
    UpdatedAt        DATETIME2(3)      NULL,
    UpdatedBy        INT               NULL,
    IsDeleted        BIT               NOT NULL CONSTRAINT DF_ReportTemplate_IsDeleted DEFAULT (0),

    CONSTRAINT PK_ReportTemplate PRIMARY KEY CLUSTERED (ReportTemplateId),
    CONSTRAINT UQ_ReportTemplate UNIQUE (GameVersionId, ReportScope, VersionNumber),
    CONSTRAINT FK_ReportTemplate_GameVersion FOREIGN KEY (GameVersionId)
        REFERENCES game.GameVersion (GameVersionId),
    CONSTRAINT CK_ReportTemplate_Scope CHECK (ReportScope IN ('Individual','Team','Organization'))
);
GO

CREATE TABLE template.ReportTemplateModule
(
    ReportTemplateModuleId INT IDENTITY(1,1) NOT NULL,
    ReportTemplateId       INT               NOT NULL,
    ReportModuleTypeId     INT               NOT NULL,
    DisplayOrder           INT               NOT NULL CONSTRAINT DF_ReportTemplateModule_Order DEFAULT (0),
    IsRequired             BIT               NOT NULL CONSTRAINT DF_ReportTemplateModule_Required DEFAULT (1),
    ConfigJson             NVARCHAR(MAX)     NULL,   -- layout/presentation config only, see JSON notes

    CreatedAt              DATETIME2(3)      NOT NULL CONSTRAINT DF_ReportTemplateModule_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy               INT              NULL,

    CONSTRAINT PK_ReportTemplateModule PRIMARY KEY CLUSTERED (ReportTemplateModuleId),
    CONSTRAINT UQ_ReportTemplateModule UNIQUE (ReportTemplateId, ReportModuleTypeId),
    CONSTRAINT FK_ReportTemplateModule_Template FOREIGN KEY (ReportTemplateId)
        REFERENCES template.ReportTemplate (ReportTemplateId),
    CONSTRAINT FK_ReportTemplateModule_ModuleType FOREIGN KEY (ReportModuleTypeId)
        REFERENCES template.ReportModuleType (ReportModuleTypeId),
    CONSTRAINT CK_ReportTemplateModule_ConfigJson CHECK (ConfigJson IS NULL OR ISJSON(ConfigJson) = 1)
);
GO

/* =====================================================================================
   SCHEMA: report -- generated report content (the actual deliverable per participant/
                     team/organization)
   ===================================================================================== */

CREATE TABLE report.Report
(
    ReportId           BIGINT IDENTITY(1,1) NOT NULL,
    ReportScope        NVARCHAR(20)         NOT NULL,  -- Individual, Team, Organization
    ReportTemplateId   INT                  NOT NULL,
    SessionId          INT                  NOT NULL,
    ParticipantId      INT                  NULL,       -- set when ReportScope = Individual
    TeamId             INT                  NULL,       -- set when ReportScope = Team
    OrganizationId     INT                  NOT NULL,   -- always set; org roll-up context
    GameVersionId      INT                  NOT NULL,

    ReportTitle        NVARCHAR(300)        NULL,
    GeneratedAt        DATETIME2(3)         NULL,
    GeneratedByUserId  INT                  NULL,
    GenerationMethod   NVARCHAR(20)         NOT NULL CONSTRAINT DF_Report_GenMethod DEFAULT ('Manual'), -- Manual, AI, Hybrid
    Status             NVARCHAR(30)         NOT NULL CONSTRAINT DF_Report_Status DEFAULT ('Draft'),      -- Draft, Generated, Reviewed, Published, Archived

    CreatedAt          DATETIME2(3)         NOT NULL CONSTRAINT DF_Report_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy          INT                  NULL,
    UpdatedAt          DATETIME2(3)         NULL,
    UpdatedBy          INT                  NULL,
    IsDeleted          BIT                  NOT NULL CONSTRAINT DF_Report_IsDeleted DEFAULT (0),
    DeletedAt          DATETIME2(3)         NULL,
    DeletedBy          INT                  NULL,

    CONSTRAINT PK_Report PRIMARY KEY CLUSTERED (ReportId),
    CONSTRAINT FK_Report_Template FOREIGN KEY (ReportTemplateId) REFERENCES template.ReportTemplate (ReportTemplateId),
    CONSTRAINT FK_Report_Session FOREIGN KEY (SessionId) REFERENCES session.GameSession (SessionId),
    CONSTRAINT FK_Report_Participant FOREIGN KEY (ParticipantId) REFERENCES core.Participant (ParticipantId),
    CONSTRAINT FK_Report_Team FOREIGN KEY (TeamId) REFERENCES session.Team (TeamId),
    CONSTRAINT FK_Report_Organization FOREIGN KEY (OrganizationId) REFERENCES core.Organization (OrganizationId),
    CONSTRAINT FK_Report_GameVersion FOREIGN KEY (GameVersionId) REFERENCES game.GameVersion (GameVersionId),
    CONSTRAINT FK_Report_GeneratedBy FOREIGN KEY (GeneratedByUserId) REFERENCES core.AppUser (UserId),
    CONSTRAINT CK_Report_Scope CHECK (ReportScope IN ('Individual','Team','Organization')),
    CONSTRAINT CK_Report_GenMethod CHECK (GenerationMethod IN ('Manual','AI','Hybrid')),
    CONSTRAINT CK_Report_Status CHECK (Status IN ('Draft','Generated','Reviewed','Published','Archived')),
    -- Enforce scope/foreign-key consistency
    CONSTRAINT CK_Report_ScopeConsistency CHECK (
        (ReportScope = 'Individual' AND ParticipantId IS NOT NULL) OR
        (ReportScope = 'Team'       AND TeamId IS NOT NULL) OR
        (ReportScope = 'Organization')
    )
);
GO
CREATE NONCLUSTERED INDEX IX_Report_Session ON report.Report (SessionId);
CREATE NONCLUSTERED INDEX IX_Report_Participant ON report.Report (ParticipantId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Report_Team ON report.Report (TeamId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Report_Organization ON report.Report (OrganizationId) WHERE IsDeleted = 0;
CREATE NONCLUSTERED INDEX IX_Report_Status ON report.Report (Status);
GO

-- Generic narrative content per report module (cover text, overview narrative,
-- patterns narrative, key-moments narrative, growth-map narrative, action-plan
-- narrative, final summary...). One row per module instance; for INDICATOR_DETAIL
-- there is one row per indicator (IndicatorDefinitionId populated).
CREATE TABLE report.ReportModule
(
    ReportModuleId        BIGINT IDENTITY(1,1) NOT NULL,
    ReportId              BIGINT               NOT NULL,
    ReportModuleTypeId    INT                  NOT NULL,
    IndicatorDefinitionId INT                  NULL,     -- used only for INDICATOR_DETAIL modules
    DisplayOrder          INT                  NOT NULL CONSTRAINT DF_ReportModule_Order DEFAULT (0),
    ContentText           NVARCHAR(MAX)        NULL,
    ContentHtml           NVARCHAR(MAX)        NULL,

    IsAiGenerated         BIT                  NOT NULL CONSTRAINT DF_ReportModule_IsAi DEFAULT (0),
    AiModel               NVARCHAR(100)        NULL,
    AiPromptRef           NVARCHAR(200)        NULL,      -- traceability id/ref to the prompt used
    ReviewedByUserId      INT                  NULL,
    ReviewedAt            DATETIME2(3)         NULL,

    CreatedAt             DATETIME2(3)         NOT NULL CONSTRAINT DF_ReportModule_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy             INT                  NULL,
    UpdatedAt             DATETIME2(3)         NULL,
    UpdatedBy             INT                  NULL,
    IsDeleted             BIT                  NOT NULL CONSTRAINT DF_ReportModule_IsDeleted DEFAULT (0),

    CONSTRAINT PK_ReportModule PRIMARY KEY CLUSTERED (ReportModuleId),
    CONSTRAINT FK_ReportModule_Report FOREIGN KEY (ReportId) REFERENCES report.Report (ReportId),
    CONSTRAINT FK_ReportModule_ModuleType FOREIGN KEY (ReportModuleTypeId)
        REFERENCES template.ReportModuleType (ReportModuleTypeId),
    CONSTRAINT FK_ReportModule_Indicator FOREIGN KEY (IndicatorDefinitionId)
        REFERENCES game.IndicatorDefinition (IndicatorDefinitionId),
    CONSTRAINT FK_ReportModule_ReviewedBy FOREIGN KEY (ReviewedByUserId) REFERENCES core.AppUser (UserId)
);
GO
CREATE NONCLUSTERED INDEX IX_ReportModule_Report ON report.ReportModule (ReportId, ReportModuleTypeId);
GO

-- Structured indicator scoring + narrative fields (title/definition come from
-- game.IndicatorDefinition; this table holds the per-participant, per-report data)
CREATE TABLE report.ReportIndicatorScore
(
    ReportIndicatorScoreId  BIGINT IDENTITY(1,1) NOT NULL,
    ReportId                BIGINT               NOT NULL,
    IndicatorDefinitionId   INT                  NOT NULL,
    ScoreValue              DECIMAL(5,2)         NOT NULL,
    ScoringScaleId          INT                  NOT NULL,   -- snapshot of scale used at scoring time
    ObservedEvidence        NVARCHAR(MAX)        NULL,
    IndicatorBehaviors      NVARCHAR(MAX)        NULL,
    GrowthOpportunities     NVARCHAR(MAX)        NULL,
    Strengths               NVARCHAR(MAX)        NULL,
    PracticalRecommendation NVARCHAR(MAX)        NULL,
    IsAiGenerated           BIT                  NOT NULL CONSTRAINT DF_ReportIndicatorScore_IsAi DEFAULT (0),

    CreatedAt               DATETIME2(3)         NOT NULL CONSTRAINT DF_ReportIndicatorScore_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy               INT                  NULL,
    UpdatedAt               DATETIME2(3)         NULL,
    UpdatedBy               INT                  NULL,

    CONSTRAINT PK_ReportIndicatorScore PRIMARY KEY CLUSTERED (ReportIndicatorScoreId),
    CONSTRAINT UQ_ReportIndicatorScore UNIQUE (ReportId, IndicatorDefinitionId),
    CONSTRAINT FK_ReportIndicatorScore_Report FOREIGN KEY (ReportId) REFERENCES report.Report (ReportId),
    CONSTRAINT FK_ReportIndicatorScore_Indicator FOREIGN KEY (IndicatorDefinitionId)
        REFERENCES game.IndicatorDefinition (IndicatorDefinitionId),
    CONSTRAINT FK_ReportIndicatorScore_Scale FOREIGN KEY (ScoringScaleId)
        REFERENCES game.ScoringScale (ScoringScaleId)
);
GO
CREATE NONCLUSTERED INDEX IX_ReportIndicatorScore_Indicator ON report.ReportIndicatorScore (IndicatorDefinitionId);
GO

CREATE TABLE report.ReportPatternScore
(
    ReportPatternScoreId  BIGINT IDENTITY(1,1) NOT NULL,
    ReportId              BIGINT               NOT NULL,
    BehavioralPatternId   INT                  NOT NULL,
    ScoreValue            DECIMAL(5,2)         NULL,      -- optional numeric strength/prevalence
    IsDominantPattern     BIT                  NOT NULL CONSTRAINT DF_ReportPatternScore_Dominant DEFAULT (0),
    Description           NVARCHAR(MAX)        NULL,

    CreatedAt             DATETIME2(3)         NOT NULL CONSTRAINT DF_ReportPatternScore_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy             INT                  NULL,

    CONSTRAINT PK_ReportPatternScore PRIMARY KEY CLUSTERED (ReportPatternScoreId),
    CONSTRAINT UQ_ReportPatternScore UNIQUE (ReportId, BehavioralPatternId),
    CONSTRAINT FK_ReportPatternScore_Report FOREIGN KEY (ReportId) REFERENCES report.Report (ReportId),
    CONSTRAINT FK_ReportPatternScore_Pattern FOREIGN KEY (BehavioralPatternId)
        REFERENCES game.BehavioralPattern (BehavioralPatternId)
);
GO

CREATE TABLE report.ReportKeyMoment
(
    ReportKeyMomentId  BIGINT IDENTITY(1,1) NOT NULL,
    ReportId           BIGINT               NOT NULL,
    KeyMomentId        INT                  NOT NULL,
    DisplayOrder       INT                  NOT NULL CONSTRAINT DF_ReportKeyMoment_Order DEFAULT (0),
    NarrativeText      NVARCHAR(MAX)        NULL,   -- how this moment is interpreted in this report

    CreatedAt          DATETIME2(3)         NOT NULL CONSTRAINT DF_ReportKeyMoment_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy          INT                  NULL,

    CONSTRAINT PK_ReportKeyMoment PRIMARY KEY CLUSTERED (ReportKeyMomentId),
    CONSTRAINT UQ_ReportKeyMoment UNIQUE (ReportId, KeyMomentId),
    CONSTRAINT FK_ReportKeyMoment_Report FOREIGN KEY (ReportId) REFERENCES report.Report (ReportId),
    CONSTRAINT FK_ReportKeyMoment_KeyMoment FOREIGN KEY (KeyMomentId) REFERENCES session.KeyMoment (KeyMomentId)
);
GO

CREATE TABLE report.GrowthMapEntry
(
    GrowthMapEntryId       BIGINT IDENTITY(1,1) NOT NULL,
    ReportId               BIGINT               NOT NULL,
    IndicatorDefinitionId  INT                  NULL,
    Area                   NVARCHAR(200)        NOT NULL,
    CurrentState           NVARCHAR(MAX)        NULL,
    TargetState            NVARCHAR(MAX)        NULL,
    DisplayOrder           INT                  NOT NULL CONSTRAINT DF_GrowthMapEntry_Order DEFAULT (0),

    CreatedAt              DATETIME2(3)         NOT NULL CONSTRAINT DF_GrowthMapEntry_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy              INT                  NULL,

    CONSTRAINT PK_GrowthMapEntry PRIMARY KEY CLUSTERED (GrowthMapEntryId),
    CONSTRAINT FK_GrowthMapEntry_Report FOREIGN KEY (ReportId) REFERENCES report.Report (ReportId),
    CONSTRAINT FK_GrowthMapEntry_Indicator FOREIGN KEY (IndicatorDefinitionId)
        REFERENCES game.IndicatorDefinition (IndicatorDefinitionId)
);
GO

CREATE TABLE report.ActionPlanItem
(
    ActionPlanItemId       BIGINT IDENTITY(1,1) NOT NULL,
    ReportId               BIGINT               NOT NULL,
    IndicatorDefinitionId  INT                  NULL,
    ActionDescription      NVARCHAR(MAX)        NOT NULL,
    TimeFrame              NVARCHAR(50)         NULL,   -- '30 days', 'Short-term'...
    Priority               NVARCHAR(20)         NULL,   -- High, Medium, Low
    DisplayOrder           INT                  NOT NULL CONSTRAINT DF_ActionPlanItem_Order DEFAULT (0),

    CreatedAt              DATETIME2(3)         NOT NULL CONSTRAINT DF_ActionPlanItem_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CreatedBy              INT                  NULL,

    CONSTRAINT PK_ActionPlanItem PRIMARY KEY CLUSTERED (ActionPlanItemId),
    CONSTRAINT FK_ActionPlanItem_Report FOREIGN KEY (ReportId) REFERENCES report.Report (ReportId),
    CONSTRAINT FK_ActionPlanItem_Indicator FOREIGN KEY (IndicatorDefinitionId)
        REFERENCES game.IndicatorDefinition (IndicatorDefinitionId),
    CONSTRAINT CK_ActionPlanItem_Priority CHECK (Priority IS NULL OR Priority IN ('High','Medium','Low'))
);
GO

/* =====================================================================================
   END OF SCRIPT
   ===================================================================================== */
