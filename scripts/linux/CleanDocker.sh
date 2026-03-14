#!/usr/bin/env bash

set +e

echo "========================================"
echo "   🚨 DOCKER FULL WIPE (ALL DATA LOST)  "
echo "========================================"

echo "[1/6] Stopping all containers..."
docker stop $(docker ps -q) 2>/dev/null

echo "[2/6] Removing all containers..."
docker rm -f $(docker ps -aq) 2>/dev/null

echo "[3/6] Removing all images..."
docker rmi -f $(docker images -aq) 2>/dev/null

echo "[4/6] Removing all volumes..."
docker volume rm $(docker volume ls -q) 2>/dev/null

echo "[5/6] Removing all non-default networks..."
docker network prune -f

echo "[6/6] Removing build cache & dangling data..."
docker system prune -a -f --volumes

echo "========================================"
echo "   ✅ DOCKER ENVIRONMENT IS NOW CLEAN   "
echo "========================================"