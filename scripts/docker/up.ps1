$ErrorActionPreference = "Stop"

docker compose pull
docker compose up -d --build --remove-orphans

docker compose ps
