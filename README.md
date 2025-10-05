# ?? MailSender API

[![Build and Push Docker Image](https://github.com/cmoi936/MailSender/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/cmoi936/MailSender/actions/workflows/docker-publish.yml)
[![Docker Image](https://img.shields.io/badge/docker-ghcr.io%2Fcmoi936%2Fmailsender-blue)](https://github.com/cmoi936/MailSender/pkgs/container/mailsender)

Une API REST simple et efficace pour envoyer des emails via SMTP, construite avec .NET 8 et entièrement containerisée.

## ?? Démarrage rapide avec Docker

### Option 1: Utiliser l'image pré-construite (Recommandé)

```bash
# Télécharger et lancer en une commande
docker run -d \
  --name mailsender-api \
  -p 5000:8080 \
  -e SMTP__USERNAME="votre-email@gmail.com" \
  -e SMTP__PASSWORD="votre-mot-de-passe-app" \
  -e SMTP__FROMEMAIL="votre-email@gmail.com" \
  ghcr.io/cmoi936/mailsender:latest
```

### Option 2: Avec Docker Compose

1. **Clonez le repository** :
```bash
git clone https://github.com/cmoi936/MailSender.git
cd MailSender
```

2. **Configurez vos variables d'environnement** :
```bash
cp .env.example .env
# Éditez .env avec vos vraies valeurs SMTP
```

3. **Déployez** :
```bash
# Windows
.\deploy.ps1

# Linux/macOS  
./deploy.sh

# Ou manuellement
docker-compose -f docker-compose.production.yml up -d
```

## ??? Images Docker disponibles

| Tag | Description | Plateforme |
|-----|-------------|------------|
| `latest` | Dernière version stable | `linux/amd64`, `linux/arm64` |
| `v1.0.0` | Version taguée | `linux/amd64`, `linux/arm64` |
| `master` | Branche master | `linux/amd64`, `linux/arm64` |

Toutes les images sont disponibles sur : **`ghcr.io/cmoi936/mailsender`**

## ?? Fonctionnalités

- ? **API REST** simple pour l'envoi d'emails
- ? **Support SMTP** (Gmail, Outlook, etc.)
- ? **Containerisé** avec Docker
- ? **Multi-architecture** (AMD64, ARM64)
- ? **Health checks** intégrés
- ? **Support CC/BCC** (plusieurs destinataires)
- ? **Messages HTML et texte**
- ? **Logging** configuré
- ? **Sécurisé** (utilisateur non-root)
- ? **CI/CD** automatisé avec GitHub Actions
- ? **Documentation Swagger/OpenAPI**

## ?? Configuration

### Variables d'environnement requises

| Variable | Description | Exemple |
|----------|-------------|---------|
| `SMTP__USERNAME` | Votre email SMTP | `user@gmail.com` |
| `SMTP__PASSWORD` | Mot de passe d'application | `abcd efgh ijkl mnop` |
| `SMTP__FROMEMAIL` | Email expéditeur | `user@gmail.com` |

### Variables optionnelles

| Variable | Défaut | Description |
|----------|---------|-------------|
| `SMTP__HOST` | `smtp.gmail.com` | Serveur SMTP |
| `SMTP__PORT` | `587` | Port SMTP |
| `SMTP__FROMNAME` | `MailSender API` | Nom expéditeur |
| `SMTP__USESSL` | `true` | Utiliser SSL/TLS |
| `SMTP__TIMEOUTMS` | `30000` | Timeout en ms |

### Configuration appsettings.json (développement local)

```json
{
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Username": "votre-email@gmail.com",
    "Password": "votre-mot-de-passe-application",
    "UseSsl": "true",
    "FromName": "Votre Nom",
    "FromEmail": "votre-email@gmail.com",
    "TimeoutMs": "30000"
  }
}
```

## ?? Configuration Gmail

1. **Activez l'authentification à 2 facteurs** sur votre compte Google
2. **Générez un mot de passe d'application** :
   - Google Account ? Sécurité ? Authentification à 2 facteurs
   - Mots de passe d'application ? Créer un nouveau mot de passe pour "MailSender"
   - Copiez le mot de passe généré (16 caractères)
   - Utilisez ce mot de passe dans `SMTP__PASSWORD`

## ?? Utilisation de l'API

### Health Check
```bash
GET http://localhost:5000/api/health
```

### Envoyer un email
```bash
POST http://localhost:5000/api/email/send
Content-Type: application/json

{
  "to": "destinataire@example.com",
  "cc": "copie@example.com", // optionnel (plusieurs emails séparés par ;
  "bcc": "copie-cachee@example.com", // optionnel (plusieurs emails séparés par ;
  "subject": "Hello from MailSender!",
  "message": "Ceci est un message de test."
}
```

### Exemple avec curl
```bash
curl -X POST http://localhost:5000/api/email/send \
  -H "Content-Type: application/json" \
  -d '{
    "to": "test@example.com",
    "subject": "Test depuis Docker",
    "message": "Hello World depuis MailSender API!"
  }'
```

## ??? Développement local

### Build avec .NET CLI
```bash
# Cloner le repo
git clone https://github.com/cmoi936/MailSender.git
cd MailSender

# Restaurer les dépendances
dotnet restore

# Build et run
dotnet build
dotnet run

# L'API sera disponible sur https://localhost:7xxx
```

### Build avec Docker
```bash
# Build local
docker build -t mailsender-local .

# Run local
docker run -p 5000:8080 mailsender-local
```

### Structure du projet
```
MailSender/
??? Controllers/         # Contrôleurs API
??? Services/           # Services métier (EmailService)
??? Models/             # Modèles de données (EmailRequest)
??? Dockerfile          # Configuration Docker
??? docker-compose.yml  # Configuration développement
??? docker-compose.production.yml  # Configuration production
??? .github/workflows/  # CI/CD GitHub Actions
??? deploy.ps1         # Script de déploiement Windows
??? deploy.sh          # Script de déploiement Linux/macOS
```

## ?? Déploiement

### Déploiement automatique

Le projet utilise **GitHub Actions** pour automatiser :
- ? Build multi-architecture (AMD64, ARM64)
- ? Tests automatisés
- ? Publication sur GitHub Container Registry
- ? Signature cryptographique des images avec Cosign
- ? Versioning automatique avec les tags Git

### Créer une nouvelle version
```bash
# Créer et pousser un tag
git tag v1.0.0
git push origin v1.0.0

# L'image sera automatiquement construite et publiée sur ghcr.io
```

### Déploiement manuel sur serveur
```bash
# Télécharger la dernière version
docker pull ghcr.io/cmoi936/mailsender:latest

# Utiliser le script de déploiement
.\deploy.ps1 latest

# Ou Docker Compose directement
docker-compose -f docker-compose.production.yml up -d
```

## ?? Monitoring et Debugging

```bash
# Logs en temps réel
docker logs -f mailsender-api

# Statistiques de performance
docker stats mailsender-api

# Health check
curl http://localhost:5000/api/health

# Accéder à Swagger UI (en développement)
# http://localhost:5000/swagger
```

## ?? Sécurité

- ? Utilisateur non-root dans le conteneur Docker
- ? Variables d'environnement pour les secrets
- ? Support HTTPS (configurable)
- ? Images Docker signées cryptographiquement
- ? Vulnérabilités scannées automatiquement
- ?? **Important** : N'utilisez jamais votre mot de passe Gmail principal
- ?? **Important** : Utilisez toujours des mots de passe d'application

## ?? Documentation complète

- ?? **Guide de déploiement Docker** : [DOCKER_DEPLOYMENT.md](DOCKER_DEPLOYMENT.md)
- ?? **Images Docker** : [GitHub Container Registry](https://github.com/cmoi936/MailSender/pkgs/container/mailsender)
- ?? **CI/CD Pipeline** : [GitHub Actions](https://github.com/cmoi936/MailSender/actions)

## ?? Contribution

1. Fork le projet
2. Créez une branche feature (`git checkout -b feature/AmazingFeature`)
3. Committez vos changements (`git commit -m 'Add some AmazingFeature'`)
4. Push vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrez une Pull Request

## ?? Support

- ?? **Documentation** : Voir les fichiers de documentation dans le repo
- ?? **Issues** : [GitHub Issues](https://github.com/cmoi936/MailSender/issues)
- ?? **Discussions** : [GitHub Discussions](https://github.com/cmoi936/MailSender/discussions)

## ?? Licence

Ce projet est sous licence MIT. Voir le fichier `LICENSE` pour plus de détails.

---

**Développé avec ?? et .NET 8**