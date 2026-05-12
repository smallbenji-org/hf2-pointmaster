# Pointmaster
The master of controlling points

## Prerequisite

## Requirements
- Node.js v20.20.0
- PostgreSQL v16 (Docker version)
- .NET 10

### Build frontend
```bash
cd Pointmaster/js
npm install
npm run build
```
### Build backend
```bash
dotnet restore
dotnet build
```

### Local development
Start PostgreSQL first:
```bash
docker compose -f docker-compose.dev.yaml up -d
```

If you prefer Podman:
```bash
podman compose -f docker-compose.dev.yaml up -d
```

The backend reads its connection string from `PointMaster__ConnectionString`.
Copy `.env.example` to `.env` and set it before running the API locally, for example:
```bash
export PointMaster__ConnectionString="Host=localhost;Port=5432;Database=pointmaster;Username=postgres;Password=mysecretpassword"
```

Then run the backend:
```bash
dotnet run --project Pointmaster/Pointmaster.csproj
```

For frontend development:
```bash
cd Pointmaster/js
npm install
npm run dev
```

### Notes
- The repository includes `.env.example` with `PointMaster__ConnectionString`.
- The Docker compose file only starts PostgreSQL; it does not run the API or frontend.

