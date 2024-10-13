rmdir /S /Q "Data/Migrations"

dotnet ef migrations add initial_identity_data -c ApplicationDbContext -o Data/Migrations
dotnet ef migrations add initial_Operational_data -c PersistedGrantDbContext -o Data/Migrations

dotnet ef database update -c ApplicationDbContext --no-build
dotnet ef database update -c PersistedGrantDbContext --no-build

dotnet ef database update -c ApplicationDbContext 


REM add-migration initial_identity_data -Context ApplicationDbContext -o Data/Migrations/IdentityData
REM add-migration initial_Operational_data -c PersistedGrantDbContext -o Data/Migrations/OperationalData
REM update-database -Context ApplicationDbContext
REM update-database -Context PersistedGrantDbContext