# Release Steps

1. Checkout the most recent commit
2. Run all tests in the `DevOpsDaysTasks.Core.Tests` project (`dotnet test .\src\DevOpsDaysTasks.Core.Tests`)
3. Run all tests in the `DevOpsDaysTasks.IntegrationTests` project (`dotnet test .\src\DevOpsDaysTasks.IntegrationTests`)
4. Build a Release version with `dotnet publish -c Release src\DevOpsDaysTasks.UI\ -o DevOpsDaysTasks /p:Version=<version> /p:VersionName=<versionName>` into the `DevOpsDaysTasks` folder. Don't forget to set the correct `<version>` and `<versionName>`
5. Copy the `workshop_material\default-tasks.xml` into `DevOpsDaysTasks\Templates\`
6. Copy the `workshop_material\Help.pdf` into `DevOpsDaysTasks\Help\`
7. (Windows only) Sign the `DevOpsDaysTasks.UI.exe` with `.\scripts\sign.ps1 .\DevOpsDaysTasks\DevOpsDaysTasks.UI.exe -PfxFile .\workshop_material\devopsdays_tasks_codesign.pfx -PfxPassword Password`
8. Manually test that everything is working (e.g. help can be displayed, correct templates) by running `DevOpsDaysTasks.UI.exe`
9. Create a zip file out of the `DevOpsDaysTasks` folder
10. Upload the zip file to GitHub Releases