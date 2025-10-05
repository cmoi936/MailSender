#!/bin/bash

# Script de déploiement pour MailSender API
# Utilise l'image Docker depuis GitHub Container Registry

set -e

echo "?? Déploiement de MailSender API depuis GitHub Container Registry"

# Configuration
GITHUB_REPO="cmoi936/mailsender"  # Modifiez avec votre repo
IMAGE_TAG="${1:-latest}"
COMPOSE_FILE="docker-compose.production.yml"

echo "?? Image: ghcr.io/$GITHUB_REPO:$IMAGE_TAG"

# Vérifier que le fichier .env existe
if [ ! -f .env ]; then
    echo "? Fichier .env manquant. Copiez .env.example vers .env et configurez vos variables."
    exit 1
fi

# Arrêter les conteneurs existants
echo "?? Arrêt des conteneurs existants..."
docker-compose -f $COMPOSE_FILE down --remove-orphans

# Télécharger la dernière image
echo "?? Téléchargement de l'image ghcr.io/$GITHUB_REPO:$IMAGE_TAG..."
docker pull ghcr.io/$GITHUB_REPO:$IMAGE_TAG

# Démarrer les nouveaux conteneurs
echo "?? Démarrage des conteneurs..."
docker-compose -f $COMPOSE_FILE up -d

# Attendre que le service soit prêt
echo "? Attente que le service soit prêt..."
sleep 10

# Vérifier le health check
echo "?? Vérification du health check..."
for i in {1..30}; do
    if curl -sf http://localhost:5000/api/health > /dev/null; then
        echo "? Service démarré avec succès!"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "? Le service ne répond pas après 30 tentatives"
        docker-compose -f $COMPOSE_FILE logs mailsender-api
        exit 1
    fi
    echo "Tentative $i/30..."
    sleep 2
done

echo "?? Déploiement terminé!"
echo "?? Logs en temps réel: docker-compose -f $COMPOSE_FILE logs -f"
echo "?? État: docker-compose -f $COMPOSE_FILE ps"