FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app/src

COPY "RednitDev.csproj" .

RUN dotnet restore "RednitDev.csproj"

COPY "Program.cs" "appsettings.json" "appsettings.Development.json" ./
COPY "Properties/" "./Properties"

# cause of docker COPY can't copy directory itself, so separate them to copy content of all directory instead
COPY "Models/" "./Models"
COPY "Views/" "./Views"
COPY "Services/" "./Services"
COPY "wwwroot/" "./wwwroot"
COPY "Controllers/" "./Controllers"
COPY "Datacenter/" "./Datacenter"

RUN dotnet publish --no-restore -c Release -o "/app/publish"

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish/ .

ENV ASPNETCORE_URLS=http://0.0.0.0:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "RednitDev.dll"]