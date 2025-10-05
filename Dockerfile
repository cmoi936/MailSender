# Dockerfile pour MailSender API .NET 8

# �tape 1: Image de base pour le runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Cr�er un utilisateur non-root pour la s�curit�
RUN adduser --disabled-password --gecos "" appuser

# �tape 2: Image de build avec SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copier le fichier projet et restaurer les d�pendances
COPY ["MailSender.csproj", "."]
RUN dotnet restore "./MailSender.csproj"

# Copier tout le code source
COPY . .
WORKDIR "/src/."

# Build de l'application
RUN dotnet build "./MailSender.csproj" -c $BUILD_CONFIGURATION -o /app/build

# �tape 3: Publication
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MailSender.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# �tape 4: Image finale
FROM base AS final
WORKDIR /app

# Copier les fichiers publi�s
COPY --from=publish /app/publish .

# Changer vers l'utilisateur non-root
USER appuser

# Variables d'environnement par d�faut
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Variables d'environnement SMTP (� surcharger)
ENV SMTP__HOST=smtp.gmail.com
ENV SMTP__PORT=587
ENV SMTP__USERNAME=""
ENV SMTP__PASSWORD=""
ENV SMTP__USESSL=true
ENV SMTP__FROMNAME="MailSender API"
ENV SMTP__FROMEMAIL=""
ENV SMTP__TIMEOUTMS=30000

# Point d'entr�e
ENTRYPOINT ["dotnet", "MailSender.dll"]