rmdir /S /Q "Data/Migrations"

dotnet ef migrations add Users -c ApplicationDbContext -o Data/Migrations/
dotnet ef migrations add OperationalData -c PersistedGrantDbContext -o Data/Migrations

dotnet ef database update 