# MailSender API

API REST pour l'envoi d'emails via SMTP (Gmail).

## Configuration

### 1. Configuration Gmail pour SMTP

Pour utiliser Gmail avec SMTP, vous devez :

1. **Activer l'authentification à deux facteurs** sur votre compte Google
2. **Générer un mot de passe d'application** :
   - Allez dans les paramètres de votre compte Google
   - Sécurité > Authentification à 2 facteurs > Mots de passe d'application
   - Générez un nouveau mot de passe d'application pour "MailSender"
   - Copiez le mot de passe généré (16 caractères)

### 2. Configuration de l'application

Modifiez le fichier `appsettings.json` ou `appsettings.Development.json` :

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

### 3. Variables d'environnement (Production)

Pour la production, utilisez des variables d'environnement :

```bash
SMTP__USERNAME=votre-email@gmail.com
SMTP__PASSWORD=votre-mot-de-passe-application
SMTP__FROMNAME="Votre Nom"
SMTP__FROMEMAIL=votre-email@gmail.com
```

## Endpoints

### Health Check
- **GET** `/api/health`
- Retourne le statut de l'application

### Envoi d'email
- **POST** `/api/email/send`
- Body:
```json
{
  "to": "destinataire@example.com",
  "cc": "copie@example.com", // optionnel (plusieurs emails séparés par ;)
  "bcc": "copie-cachee@example.com", // optionnel (plusieurs emails séparés par ;)
  "subject": "Sujet de l'email",
  "message": "Contenu de l'email"
}
```

## Utilisation

1. Configurez vos paramètres SMTP dans `appsettings.json`
2. Démarrez l'application : `dotnet run`
3. Ouvrez votre navigateur sur `https://localhost:7xxx` pour accéder à Swagger UI
4. Testez l'endpoint `/api/health` pour vérifier que l'API fonctionne
5. Utilisez l'endpoint `/api/email/send` pour envoyer des emails

## Fonctionnalités

- ? Envoi d'emails via SMTP Gmail
- ? Support CC et BCC (plusieurs destinataires séparés par ;)
- ? Messages en texte brut et HTML
- ? Configuration flexible via appsettings.json
- ? Logging détaillé
- ? Health check endpoint
- ? Documentation Swagger/OpenAPI

## Sécurité

- Utilisez toujours des mots de passe d'application Gmail (pas votre mot de passe principal)
- Ne commitez jamais vos identifiants dans le code source
- Utilisez des variables d'environnement ou Azure Key Vault en production
- Activez HTTPS en production