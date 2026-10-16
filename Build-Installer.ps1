[CmdletBinding()]
param(
    [ValidateSet('win-x64')]
    [string] $Runtime = 'win-x64',
    [string] $InnoSetupCompiler
)

$ErrorActionPreference = 'Stop'
$projectRoot = $PSScriptRoot
$artifactsRoot = [IO.Path]::GetFullPath((Join-Path $projectRoot 'artifacts'))
$publishDir = [IO.Path]::GetFullPath((Join-Path $artifactsRoot "publish\$Runtime"))
$installerScript = Join-Path $projectRoot 'installer\PdfOcr.iss'

if (-not $publishDir.StartsWith($artifactsRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
    throw "La carpeta de publicacion no pertenece a artifacts: $publishDir"
}

if (Test-Path -LiteralPath $publishDir) {
    Remove-Item -LiteralPath $publishDir -Recurse -Force
}

dotnet publish (Join-Path $projectRoot 'PdfOcr.csproj') `
    --configuration Release `
    --runtime $Runtime `
    --self-contained true `
    --output $publishDir `
    -p:PublishReadyToRun=false

if ($LASTEXITCODE -ne 0) {
    throw "La publicacion de la aplicacion fallo con codigo $LASTEXITCODE."
}

if (-not $InnoSetupCompiler) {
    $candidates = @(
        (Join-Path $env:LOCALAPPDATA 'Programs\Inno Setup 6\ISCC.exe'),
        (Join-Path $env:ProgramFiles 'Inno Setup 6\ISCC.exe'),
        (Join-Path ${env:ProgramFiles(x86)} 'Inno Setup 6\ISCC.exe')
    )
    $InnoSetupCompiler = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
}

if (-not $InnoSetupCompiler -or -not (Test-Path $InnoSetupCompiler)) {
    throw 'No se encontro ISCC.exe. Instala Inno Setup 6 o pasa -InnoSetupCompiler con su ruta.'
}

& $InnoSetupCompiler /Q $installerScript
if ($LASTEXITCODE -ne 0) {
    throw "La compilacion del instalador fallo con codigo $LASTEXITCODE."
}

$output = Join-Path $projectRoot 'artifacts\installer\PdfOcr-Setup-1.0.1-win-x64.exe'
Write-Host "Instalador generado: $output"
