rmdir /S /Q "Data/Migrations"

dotnet ef migrations add initial_identity_data -c IdentityServerDbContext -o Data/Migrations
dotnet ef migrations add initial_Operational_data -c PersistedGrantDbContext -o Data/Migrations

dotnet ef database update -c IdentityServerDbContext --no-build
dotnet ef database update -c PersistedGrantDbContext --no-build

REM dotnet ef database update -c IdentityServerDbContext 


REM add-migration initial_identity_data -Context ApplicationDbContext -o Data/Migrations/IdentityData
REM add-migration initial_Operational_data -c PersistedGrantDbContext -o Data/Migrations/OperationalData
REM update-database -Context ApplicationDbContext
REM update-database -Context PersistedGrantDbContext