# -----------------------
# Etapa 1 - Build (SDK 8.0)
# -----------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos la solución y el proyecto
COPY . .

# Restauramos dependencias (usa el SDK 8 para proyectos net8.0)
RUN dotnet restore CelTechScrapper.sln

# Publicamos el proyecto principal
RUN dotnet publish ./CelTechScrapper/CelTechScrapper.csproj -c Release -o /app/publish /p:UseAppHost=false

# -----------------------
# Etapa 2 - Runtime (ASP.NET 8.0)
# -----------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Render inyecta $PORT; obligamos a Kestrel a escucharlo
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

COPY --from=build /app/publish .

# EXPOSE es opcional en Render
EXPOSE 80

ENTRYPOINT ["dotnet", "CelTechScrapper.dll"]
