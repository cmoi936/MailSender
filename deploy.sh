#!/bin/bash

# Script de d�ploiement pour MailSender API
# Utilise l'image Docker depuis GitHub Container Registry

set -e

echo "?? D�ploiement de MailSender API depuis GitHub Container Registry"

# Configuration
GITHUB_REPO="cmoi936/mailsender"  # Modifiez avec votre repo
IMAGE_TAG="${1:-latest}"
COMPOSE_FILE="docker-compose.production.yml"

echo "?? Image: ghcr.io/$GITHUB_REPO:$IMAGE_TAG"

# V�rifier que le fichier .env existe
if [ ! -f .env ]; then
    echo "? Fichier .env manquant. Copiez .env.example vers .env et configurez vos variables."
    exit 1
fi

# Arr�ter les conteneurs existants
echo "?? Arr�t des conteneurs existants..."
docker-compose -f $COMPOSE_FILE down --remove-orphans

# T�l�charger la derni�re image
echo "?? T�l�chargement de l'image ghcr.io/$GITHUB_REPO:$IMAGE_TAG..."
docker pull ghcr.io/$GITHUB_REPO:$IMAGE_TAG

# D�marrer les nouveaux conteneurs
echo "?? D�marrage des conteneurs..."
docker-compose -f $COMPOSE_FILE up -d

# Attendre que le service soit pr�t
echo "? Attente que le service soit pr�t..."
sleep 10

# V�rifier le health check
echo "?? V�rification du health check..."
for i in {1..30}; do
    if curl -sf http://localhost:5000/api/health > /dev/null; then
        echo "? Service d�marr� avec succ�s!"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "? Le service ne r�pond pas apr�s 30 tentatives"
        docker-compose -f $COMPOSE_FILE logs mailsender-api
        exit 1
    fi
    echo "Tentative $i/30..."
    sleep 2
done

echo "?? D�ploiement termin�!"
echo "?? Logs en temps r�el: docker-compose -f $COMPOSE_FILE logs -f"
echo "?? �tat: docker-compose -f $COMPOSE_FILE ps"