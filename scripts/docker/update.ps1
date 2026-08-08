$ErrorActionPreference = "Stop"

docker compose pull
docker compose up -d --build --force-recreate --remove-orphans

docker compose ps
