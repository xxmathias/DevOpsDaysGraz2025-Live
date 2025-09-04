param(
    [Parameter(Mandatory)][String]$ExeFile,
    [Parameter(Mandatory)][String]$PfxFile,
    [Parameter(Mandatory)][String]$PfxPassword
)
$signtool = "C:\Program Files (x86)\Microsoft SDKs\ClickOnce\SignTool\signtool.exe"

# Sign the .exe
$exe = Get-ChildItem $ExeFile | Select-Object -First 1
& $signtool sign `
  /fd SHA256 `
  /f $PfxFile `
  /p $PfxPassword `
  /tr http://timestamp.digicert.com `
  /td SHA256 `
  $exe.FullName

# Verify
Get-AuthenticodeSignature $exe.FullName | Format-List
