param(
    [Parameter(Mandatory=$true)]
    [string]$MigrationName
)

dotnet ef migrations add $MigrationName `
    --project "FitnessFox.Components" `
    --context "FitnessFox.Data.ApplicationDbContext" `
    --output-dir "Data/Migrations"