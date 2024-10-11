FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ./Smooth.Identity/Smooth.Identity.csproj ./Smooth.Identity/
RUN dotnet restore "./Smooth.Identity/Smooth.Identity.csproj"

COPY . ./

RUN dotnet publish "./Smooth.Identity/Smooth.Identity.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose HTTP port
EXPOSE 443

ENTRYPOINT ["dotnet", "Smooth.Identity.dll"]