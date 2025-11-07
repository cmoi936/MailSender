# Script PowerShell pour le déploiement MailSender API
# Utilise l'image Docker depuis GitHub Container Registry

param(
    [string]$ImageTag = "latest",
    [string]$ComposeFile = "docker-compose.production.yml"
)

$ErrorActionPreference = "Stop"

Write-Host "?? Déploiement de MailSender API depuis GitHub Container Registry" -ForegroundColor Green

# Configuration
$GitHubRepo = "cmoi936/mailsender"  # Modifiez avec votre repo
$ImageName = "ghcr.io/$GitHubRepo`:$ImageTag"

Write-Host "?? Image: $ImageName" -ForegroundColor Cyan

# Vérifier que le fichier .env existe
if (-not (Test-Path ".env")) {
    Write-Host "? Fichier .env manquant. Copiez .env.example vers .env et configurez vos variables." -ForegroundColor Red
    exit 1
}

# Arrêter les conteneurs existants
Write-Host "?? Arrêt des conteneurs existants..." -ForegroundColor Yellow
docker-compose -f $ComposeFile down --remove-orphans

# Télécharger la dernière image
Write-Host "?? Téléchargement de l'image $ImageName..." -ForegroundColor Blue
docker pull $ImageName

# Démarrer les nouveaux conteneurs
Write-Host "?? Démarrage des conteneurs..." -ForegroundColor Green
docker-compose -f $ComposeFile up -d

# Attendre que le service soit prêt
Write-Host "? Attente que le service soit prêt..." -ForegroundColor Yellow
Start-Sleep 10

# Vérifier le health check
Write-Host "?? Vérification du health check..." -ForegroundColor Blue
$maxAttempts = 30
for ($i = 1; $i -le $maxAttempts; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-Host "? Service démarré avec succès!" -ForegroundColor Green
            break
        }
    }
    catch {
        if ($i -eq $maxAttempts) {
            Write-Host "? Le service ne répond pas après $maxAttempts tentatives" -ForegroundColor Red
            docker-compose -f $ComposeFile logs mailsender-api
            exit 1
        }
        Write-Host "Tentative $i/$maxAttempts..." -ForegroundColor Yellow
        Start-Sleep 2
    }
}

Write-Host "?? Déploiement terminé!" -ForegroundColor Green
Write-Host "?? Logs en temps réel: docker-compose -f $ComposeFile logs -f" -ForegroundColor Cyan
Write-Host "?? État: docker-compose -f $ComposeFile ps" -ForegroundColor Cyan