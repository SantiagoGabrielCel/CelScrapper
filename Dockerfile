# -----------------------
# Etapa 1 - Build
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copiamos la solución y el proyecto
COPY . . 

# Restauramos dependencias
RUN dotnet restore CelTechScrapper.sln

# Publicamos el proyecto principal
RUN dotnet publish ./CelTechScrapper/CelTechScrapper.csproj -c Release -o /app/publish

# -----------------------
# Etapa 2 - Runtime
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "CelTechScrapper.dll"]
