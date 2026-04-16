# Pointmaster
The master of controlling points

## How to run?
- Node: v20.20.0
- Postgresql: v16

### Build frontend
```bash
cd Pointmaster/js
npm i
npm run build
```
### Build backend
```bash
dotnet restore
dotnet build
```

### Run inside Docker instead
How to TBD...

### Environment variables needed
When running inside Docker, no configuration needed.
When running locally in vscode, use .env file, example file is in the repository
When running locally in visual studio:
```
DB__DefaultConnection: <postgresql connection string>
```
