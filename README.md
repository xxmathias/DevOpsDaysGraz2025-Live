# DevOpsDays Graz 2025 Sample Application
This is the companion repository for the workshop "Modern Delivery for Not-so-modern Software" on the DevOpsDays Graz 2025.

## Repository Structure
The `main` branch contains the start of the workshop on which the participant should work while following along the workshop.

The `workshop-steps` branch will contain the results of the steps in the workshop as separate git commits and can be used as a reference.

## Sample Application
The sample application is a simple ToDo List application. However, there are a few things that that make it hard to build: copying files, manually entering version numbers, manual signing, etc.

In the workshop we will take the application and modernize its delivery step by step.

## Folder Structure
* `data/`: contains the file `default-tasks.xml`, which is a dev fallback for the template engine
* `scripts/`: contains the signing script (only works on windows)
* `src/`: source code of the application
  * `DevOpsDaysTasks.Core/`: models, services and database access of the app
  * `DevOpsDaysTasks.Core.Tests/`: tests for models and services
  * `DevOpsDaysTasks.IntegrationTests/`: tests that actually query a database
  * `DevOpsDaysTasks.UI/`: UI and main application
* `workshop_material/`: additional material for participants, e.g., the file that should be copied with a release
  * `release_steps.md`: instruction on how to perform a manual release

## How to Start the Application
⚠️ This application requires .NET 8 SDK or later

Run `dotnet run --project .\src\DevOpsDaysTasks.UI\`.

### Switching between MSSQL and SQLite
By default the application uses localDB. For cross-platform compatibility also a SQLite adapter is implemented. Replace `DbKind.SqlServer` with `DbKind.Sqlite` in `MainWindow.axaml.cs` and `MSSQLFixture.cs` to use SQLite.