# Dockerfile pour MailSender MCP Server .NET 8

# Étape 1: Image de base pour le runtime
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Créer un utilisateur non-root pour la sécurité
RUN adduser --disabled-password --gecos "" appuser

# Étape 2: Image de build avec SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier les fichiers projet et restaurer les dépendances
COPY ["MailSender.Mcp/MailSender.Mcp.csproj", "MailSender.Mcp/"]
COPY ["MailSender.Core/MailSender.Core.csproj", "MailSender.Core/"]
RUN dotnet restore "MailSender.Mcp/MailSender.Mcp.csproj"

# Copier tout le code source
COPY MailSender.Mcp/ MailSender.Mcp/
COPY MailSender.Core/ MailSender.Core/

WORKDIR "/src/MailSender.Mcp"

# Build de l'application
RUN dotnet build "MailSender.Mcp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Étape 3: Publication
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MailSender.Mcp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape 4: Image finale
FROM base AS final
WORKDIR /app

# Copier les fichiers publiés
COPY --from=publish /app/publish .

# Changer vers l'utilisateur non-root
USER appuser

# Variables d'environnement SMTP (à surcharger)
ENV SMTP__HOST=smtp.gmail.com
ENV SMTP__PORT=587
ENV SMTP__USERNAME=""
ENV SMTP__PASSWORD=""
ENV SMTP__USESSL=true
ENV SMTP__FROMNAME="MailSender MCP"
ENV SMTP__FROMEMAIL=""
ENV SMTP__TIMEOUTMS=30000

# Point d'entrée
ENTRYPOINT ["dotnet", "mailsender-mcp.dll"]
