# Guide de Déploiement Docker - MailSender API

## 🚀 Déploiement avec Docker

### Prérequis
- Docker installé
- Docker Compose (optionnel mais recommandé)
- Compte Gmail avec mot de passe d'application généré

### 📦 Images Docker Disponibles

L'image Docker est automatiquement construite et publiée sur GitHub Container Registry :
- **Production** : `ghcr.io/cmoi936/mailsender:latest`
- **Versions taguées** : `ghcr.io/cmoi936/mailsender:v1.0.0`
- **Branches** : `ghcr.io/cmoi936/mailsender:master`

### ⚡ Déploiement rapide avec l'image pré-construite

#### 1. Télécharger l'image depuis GitHub Container Registry
```bash
docker pull ghcr.io/cmoi936/mailsender:latest
```

#### 2. Lancement du conteneur
```bash
docker run -d \
  --name mailsender-api \
  -p 5000:8080 \
  -e SMTP__USERNAME="votre-email@gmail.com" \
  -e SMTP__PASSWORD="votre-mot-de-passe-application" \
  -e SMTP__FROMNAME="Votre Nom" \
  -e SMTP__FROMEMAIL="votre-email@gmail.com" \
  ghcr.io/cmoi936/mailsender:latest
```

### 🐳 Déploiement avec Docker Compose (Recommandé)

#### 1. Configuration des variables d'environnement
```bash
# Copiez le fichier d'exemple
cp .env.example .env

# Éditez le fichier .env avec vos vraies valeurs
nano .env
```

#### 2. Déploiement automatisé
```bash
# Avec le script PowerShell (Windows)
.\deploy.ps1

# Ou avec le script Bash (Linux/macOS)
./deploy.sh

# Ou manuellement avec Docker Compose
docker-compose -f docker-compose.production.yml up -d
```

### 🔧 Build local (Développement uniquement)

Si vous voulez construire l'image localement :

#### 1. Build de l'image
```bash
docker build -t mailsender-api .
```

#### 2. Utilisation du docker-compose de développement
```bash
docker-compose up -d
```

### ⚙️ Variables d'environnement importantes

| Variable | Description | Valeur par défaut | Obligatoire |
|----------|-------------|-------------------|-------------|
| `SMTP__USERNAME` | Email Gmail | | ✅ |
| `SMTP__PASSWORD` | Mot de passe d'application Gmail | | ✅ |
| `SMTP__FROMNAME` | Nom de l'expéditeur | "MailSender API" | ❌ |
| `SMTP__FROMEMAIL` | Email de l'expéditeur | | ✅ |
| `SMTP__HOST` | Serveur SMTP | smtp.gmail.com | ❌ |
| `SMTP__PORT` | Port SMTP | 587 | ❌ |
| `SMTP__USESSL` | Utiliser SSL | true | ❌ |
| `SMTP__TIMEOUTMS` | Timeout en ms | 30000 | ❌ |

### 📧 Configuration Gmail

1. **Activez l'authentification à 2 facteurs** sur votre compte Google
2. **Générez un mot de passe d'application** :
   - Allez dans les paramètres Google → Sécurité
   - Authentification à 2 facteurs → Mots de passe d'application
   - Créez un nouveau mot de passe pour "MailSender"
   - Utilisez ce mot de passe dans `SMTP__PASSWORD`

### 🏥 Health Check

L'API expose un endpoint de santé :
```bash
# Vérifier que l'API fonctionne
curl http://localhost:5000/api/health
```

### 📊 Surveillance et Logging

```bash
# Voir les logs en temps réel
docker-compose -f docker-compose.production.yml logs -f mailsender-api

# Voir l'état des conteneurs
docker-compose -f docker-compose.production.yml ps

# Statistiques de ressources
docker stats mailsender-api
```

### 🔄 CI/CD et Publication Automatique

L'image Docker est automatiquement construite et publiée via **GitHub Actions** :

- ✅ **Sur chaque push** vers `master` → `ghcr.io/cmoi936/mailsender:latest`
- 🏷️ **Sur chaque tag** `v*.*.*` → `ghcr.io/cmoi936/mailsender:v1.0.0`
- 🌐 **Support multi-architecture** (AMD64, ARM64)
- 🔐 **Signature cryptographique** avec Cosign
- 🚀 **Cache optimisé** pour des builds rapides

#### Pour publier une nouvelle version :
```bash
# Créer et pousser un tag
git tag v1.0.0
git push origin v1.0.0

# L'image sera automatiquement construite et publiée
```

### 🔑 Authentification GitHub Container Registry

Pour télécharger des images privées :
```bash
# Se connecter avec un Personal Access Token
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Ou utiliser Docker Compose avec login automatique
docker-compose -f docker-compose.production.yml pull
```

### 💡 Commandes utiles

```bash
# Utiliser une version spécifique
docker pull ghcr.io/cmoi936/mailsender:v1.0.0

# Mise à jour vers la dernière version
docker-compose -f docker-compose.production.yml pull
docker-compose -f docker-compose.production.yml up -d

# Redémarrer uniquement l'API
docker-compose -f docker-compose.production.yml restart mailsender-api

# Voir les logs d'erreur uniquement
docker-compose -f docker-compose.production.yml logs mailsender-api | grep -i error

# Entrer dans le conteneur
docker exec -it mailsender-api /bin/bash

# Nettoyer les anciennes images
docker image prune -a
```

### 🧪 Test de l'API

Une fois déployée, testez l'API :

```bash
# Health Check
curl http://localhost:5000/api/health

# Envoi d'email de test
curl -X POST http://localhost:5000/api/email/send \
  -H "Content-Type: application/json" \
  -d '{
    "to": "destinataire@example.com",
    "subject": "Test depuis Docker",
    "message": "Ceci est un test depuis l API containerisée !"
  }'
```

### 🛡️ Sécurité

- ✅ L'API s'exécute avec un utilisateur non-root
- 🔒 Variables d'environnement pour les secrets
- 🔌 Port interne différent du port externe
- 🏥 Health check configuré
- 📊 Limites de ressources définies
- 🔐 Images signées cryptographiquement
- 🔍 Vulnérabilités scannées automatiquement

### 📈 Mise à l'échelle

Pour déployer plusieurs instances :
```bash
docker-compose -f docker-compose.production.yml up -d --scale mailsender-api=3
```

### 🔄 Mise à jour et Rollback

```bash
# Mise à jour automatique vers la dernière version
.\deploy.ps1 latest

# Utiliser une version spécifique
.\deploy.ps1 v1.0.0

# Rollback vers une version précédente
docker-compose -f docker-compose.production.yml down
docker run -d --name mailsender-api -p 5000:8080 ghcr.io/cmoi936/mailsender:v0.9.0