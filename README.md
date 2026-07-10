# Behavioral Report Engine — ASP.NET Core MVC App

A full CRUD web application for the Behavioral Report Engine SQL Server schema
(SELVA, HERMIOS, Crisis Orbit, Wave Riding, and future games). Built with
**.NET 8**, **ASP.NET Core MVC**, and **EF Core (SQL Server provider)**.

It gives you a working web UI (list / details / create / edit / delete) for
every table in every schema:

- **core** — Organizations, Users, User Roles, Participants
- **game** — Games, Game Versions, Scoring Scales & Levels, Indicator
  Definitions, Behavioral Patterns, Game↔Pattern mapping
- **session** — Game Sessions, Facilitators, Teams, Session Participants,
  Media Assets, Transcripts & Segments, Observations, Key Moments
- **template** — Report Module Types, Report Templates, Template Modules
- **report** — Reports, Report Modules, Indicator Scores, Pattern Scores,
  Report Key Moments, Growth Map Entries, Action Plan Items

## Project layout

```
BehavioralReportEngine.Web/
├── Database/schema.sql          # the SQL Server schema script (run this first)
├── Models/                      # one POCO class per table (31 total)
├── Data/ApplicationDbContext.cs # EF Core DbContext + Fluent API mappings
├── Controllers/                 # one MVC controller per table (CRUD actions)
├── Views/<Entity>/              # Index / Details / Create / Edit / Delete views
├── Views/Shared/_Layout.cshtml  # nav grouped by schema (core/game/session/template/report)
├── Views/Home/Index.cshtml      # dashboard with live record counts
├── Program.cs                   # app bootstrap + EF Core registration
└── appsettings.json             # connection string lives here
```

## 1. Create the database

Open `Database/schema.sql` in SQL Server Management Studio or Azure Data
Studio (against a SQL Server 2019+ instance) and execute it. This creates the
`BehavioralReportEngine` database, all five schemas, every table, and the
seed data (roles, scoring scale, report module types).

## 2. Point the app at your database

Edit `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=BehavioralReportEngine;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Replace `YOUR_SERVER_NAME` with your SQL Server instance (e.g. `localhost`,
`.\SQLEXPRESS`, or a full connection string with SQL auth if you're not using
Windows auth).

## 3. Run it

```bash
cd BehavioralReportEngine.Web
dotnet restore
dotnet run
```

Then open the URL shown in the console (e.g. `http://localhost:5254`).

## Notes on design choices

- **No EF migrations** are included on purpose — the database is created by
  the authoritative SQL script, and the EF model is hand-mapped (via Fluent
  API) to match those exact tables/columns/schemas. If you evolve the SQL
  schema later, update the matching model/DbContext mapping (or switch to
  `dotnet ef dbcontext scaffold` against the live database to regenerate).
- **Foreign keys** are rendered as dropdowns on Create/Edit screens (e.g.
  choosing an Organization when creating a User), populated from the related
  table.
- **Soft delete columns** (`IsDeleted`, `DeletedAt`, `DeletedBy`) exist in the
  models to mirror the schema, but the `Delete` action in this scaffold
  performs a **hard delete** for simplicity. If you want soft-delete
  behavior in the UI, change `DeleteConfirmed` in the relevant controller to
  set `IsDeleted = true` and save, instead of `_context.Remove(...)`, and add
  a global query filter (`HasQueryFilter(x => !x.IsDeleted)`) in
  `ApplicationDbContext.OnModelCreating`.
- **Cascade deletes** are disabled (`DeleteBehavior.Restrict`) on every
  relationship to avoid SQL Server's "multiple cascade paths" error, since
  many tables reference `GameSession`, `Report`, etc. through several
  different foreign keys.
- Scoring/decimal columns use `decimal(5,2)` to match the SQL script's
  `DECIMAL(5,2)` columns.

## Extending it

Everything here was generated from a single metadata table describing all 31
entities (name, schema, primary key, columns, foreign keys, audit columns).
If you add a table to the SQL schema, add a matching entry to that table and
regenerate the model/DbContext/controller/view for just that entity, or add
the class/mapping by hand following the pattern of any existing entity (e.g.
`Models/Participant.cs`, `Controllers/ParticipantController.cs`,
`Views/Participant/*.cshtml`).
