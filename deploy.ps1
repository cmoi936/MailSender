# Script PowerShell pour le d�ploiement MailSender API
# Utilise l'image Docker depuis GitHub Container Registry

param(
    [string]$ImageTag = "latest",
    [string]$ComposeFile = "docker-compose.production.yml"
)

$ErrorActionPreference = "Stop"

Write-Host "?? D�ploiement de MailSender API depuis GitHub Container Registry" -ForegroundColor Green

# Configuration
$GitHubRepo = "cmoi936/mailsender"  # Modifiez avec votre repo
$ImageName = "ghcr.io/$GitHubRepo`:$ImageTag"

Write-Host "?? Image: $ImageName" -ForegroundColor Cyan

# V�rifier que le fichier .env existe
if (-not (Test-Path ".env")) {
    Write-Host "? Fichier .env manquant. Copiez .env.example vers .env et configurez vos variables." -ForegroundColor Red
    exit 1
}

# Arr�ter les conteneurs existants
Write-Host "?? Arr�t des conteneurs existants..." -ForegroundColor Yellow
docker-compose -f $ComposeFile down --remove-orphans

# T�l�charger la derni�re image
Write-Host "?? T�l�chargement de l'image $ImageName..." -ForegroundColor Blue
docker pull $ImageName

# D�marrer les nouveaux conteneurs
Write-Host "?? D�marrage des conteneurs..." -ForegroundColor Green
docker-compose -f $ComposeFile up -d

# Attendre que le service soit pr�t
Write-Host "? Attente que le service soit pr�t..." -ForegroundColor Yellow
Start-Sleep 10

# V�rifier le health check
Write-Host "?? V�rification du health check..." -ForegroundColor Blue
$maxAttempts = 30
for ($i = 1; $i -le $maxAttempts; $i++) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5000/api/health" -UseBasicParsing -TimeoutSec 5
        if ($response.StatusCode -eq 200) {
            Write-Host "? Service d�marr� avec succ�s!" -ForegroundColor Green
            break
        }
    }
    catch {
        if ($i -eq $maxAttempts) {
            Write-Host "? Le service ne r�pond pas apr�s $maxAttempts tentatives" -ForegroundColor Red
            docker-compose -f $ComposeFile logs mailsender-api
            exit 1
        }
        Write-Host "Tentative $i/$maxAttempts..." -ForegroundColor Yellow
        Start-Sleep 2
    }
}

Write-Host "?? D�ploiement termin�!" -ForegroundColor Green
Write-Host "?? Logs en temps r�el: docker-compose -f $ComposeFile logs -f" -ForegroundColor Cyan
Write-Host "?? �tat: docker-compose -f $ComposeFile ps" -ForegroundColor Cyan