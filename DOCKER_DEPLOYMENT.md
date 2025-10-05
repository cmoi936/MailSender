# Guide de D�ploiement Docker - MailSender API

## ?? D�ploiement avec Docker

### Pr�requis
- Docker install�
- Docker Compose (optionnel mais recommand�)
- Compte Gmail avec mot de passe d'application g�n�r�

### ?? Images Docker Disponibles

L'image Docker est automatiquement construite et publi�e sur GitHub Container Registry :
- **Production** : `ghcr.io/cmoi936/mailsender:latest`
- **Versions tagu�es** : `ghcr.io/cmoi936/mailsender:v1.0.0`
- **Branches** : `ghcr.io/cmoi936/mailsender:master`

### ?? D�ploiement rapide avec l'image pr�-construite

#### 1. T�l�charger l'image depuis GitHub Container Registry
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

### ?? D�ploiement avec Docker Compose (Recommand�)

#### 1. Configuration des variables d'environnement
```bash
# Copiez le fichier d'exemple
cp .env.example .env

# �ditez le fichier .env avec vos vraies valeurs
nano .env
```

#### 2. D�ploiement automatis�
```bash
# Avec le script PowerShell (Windows)
.\deploy.ps1

# Ou avec le script Bash (Linux/macOS)
./deploy.sh

# Ou manuellement avec Docker Compose
docker-compose -f docker-compose.production.yml up -d
```

### ??? Build local (D�veloppement uniquement)

Si vous voulez construire l'image localement :

#### 1. Build de l'image
```bash
docker build -t mailsender-api .
```

#### 2. Utilisation du docker-compose de d�veloppement
```bash
docker-compose up -d
```

### ?? Variables d'environnement importantes

| Variable | Description | Valeur par d�faut | Obligatoire |
|----------|-------------|-------------------|-------------|
| `SMTP__USERNAME` | Email Gmail | | ? |
| `SMTP__PASSWORD` | Mot de passe d'application Gmail | | ? |
| `SMTP__FROMNAME` | Nom de l'exp�diteur | "MailSender API" | ? |
| `SMTP__FROMEMAIL` | Email de l'exp�diteur | | ? |
| `SMTP__HOST` | Serveur SMTP | smtp.gmail.com | ? |
| `SMTP__PORT` | Port SMTP | 587 | ? |
| `SMTP__USESSL` | Utiliser SSL | true | ? |
| `SMTP__TIMEOUTMS` | Timeout en ms | 30000 | ? |

### ?? Configuration Gmail

1. **Activez l'authentification � 2 facteurs** sur votre compte Google
2. **G�n�rez un mot de passe d'application** :
   - Allez dans les param�tres Google ? S�curit�
   - Authentification � 2 facteurs ? Mots de passe d'application
   - Cr�ez un nouveau mot de passe pour "MailSender"
   - Utilisez ce mot de passe dans `SMTP__PASSWORD`

### ?? Health Check

L'API expose un endpoint de sant� :
```bash
# V�rifier que l'API fonctionne
curl http://localhost:5000/api/health
```

### ?? Surveillance et Logging

```bash
# Voir les logs en temps r�el
docker-compose -f docker-compose.production.yml logs -f mailsender-api

# Voir l'�tat des conteneurs
docker-compose -f docker-compose.production.yml ps

# Statistiques de ressources
docker stats mailsender-api
```

### ?? CI/CD et Publication Automatique

L'image Docker est automatiquement construite et publi�e via **GitHub Actions** :

- ? **Sur chaque push** vers `master` ? `ghcr.io/cmoi936/mailsender:latest`
- ? **Sur chaque tag** `v*.*.*` ? `ghcr.io/cmoi936/mailsender:v1.0.0`
- ? **Support multi-architecture** (AMD64, ARM64)
- ? **Signature cryptographique** avec Cosign
- ? **Cache optimis�** pour des builds rapides

#### Pour publier une nouvelle version :
```bash
# Cr�er et pousser un tag
git tag v1.0.0
git push origin v1.0.0

# L'image sera automatiquement construite et publi�e
```

### ?? Authentification GitHub Container Registry

Pour t�l�charger des images priv�es :
```bash
# Se connecter avec un Personal Access Token
echo $GITHUB_TOKEN | docker login ghcr.io -u USERNAME --password-stdin

# Ou utiliser Docker Compose avec login automatique
docker-compose -f docker-compose.production.yml pull
```

### ??? Commandes utiles

```bash
# Utiliser une version sp�cifique
docker pull ghcr.io/cmoi936/mailsender:v1.0.0

# Mise � jour vers la derni�re version
docker-compose -f docker-compose.production.yml pull
docker-compose -f docker-compose.production.yml up -d

# Red�marrer uniquement l'API
docker-compose -f docker-compose.production.yml restart mailsender-api

# Voir les logs d'erreur uniquement
docker-compose -f docker-compose.production.yml logs mailsender-api | grep -i error

# Entrer dans le conteneur
docker exec -it mailsender-api /bin/bash

# Nettoyer les anciennes images
docker image prune -a
```

### ?? Test de l'API

Une fois d�ploy�e, testez l'API :

```bash
# Health Check
curl http://localhost:5000/api/health

# Envoi d'email de test
curl -X POST http://localhost:5000/api/email/send \
  -H "Content-Type: application/json" \
  -d '{
    "to": "destinataire@example.com",
    "subject": "Test depuis Docker",
    "message": "Ceci est un test depuis l API containeris�e !"
  }'
```

### ?? S�curit�

- ? L'API s'ex�cute avec un utilisateur non-root
- ? Variables d'environnement pour les secrets
- ? Port interne diff�rent du port externe
- ? Health check configur�
- ? Limites de ressources d�finies
- ? Images sign�es cryptographiquement
- ? Vuln�rabilit�s scann�es automatiquement

### ?? Mise � l'�chelle

Pour d�ployer plusieurs instances :
```bash
docker-compose -f docker-compose.production.yml up -d --scale mailsender-api=3
```

### ?? Mise � jour et Rollback

```bash
# Mise � jour automatique vers la derni�re version
.\deploy.ps1 latest

# Utiliser une version sp�cifique
.\deploy.ps1 v1.0.0

# Rollback vers une version pr�c�dente
docker-compose -f docker-compose.production.yml down
docker run -d --name mailsender-api -p 5000:8080 ghcr.io/cmoi936/mailsender:v0.9.0