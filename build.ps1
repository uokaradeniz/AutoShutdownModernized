param (
    [switch]$Run = $false
)

$projectPath = "AutoShutdownModernized.csproj"

Write-Host "Restoring packages..." -ForegroundColor Cyan
dotnet restore $projectPath

Write-Host "Building project..." -ForegroundColor Cyan
dotnet build $projectPath -c Release --no-restore

if ($Run) {
    Write-Host "Running application..." -ForegroundColor Cyan
    dotnet run --project $projectPath -c Release --no-build
}

