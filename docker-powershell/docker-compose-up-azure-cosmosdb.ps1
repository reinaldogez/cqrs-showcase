$env:hostDirectory = 'C:\emulator\bind-mount'
if (-not (Test-Path $env:hostDirectory)) {
    New-Item -ItemType Directory -Path $env:hostDirectory
}
docker-compose -f ../docker-compose-azure-cosmosdb.yaml up