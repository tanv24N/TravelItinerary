# Travel Itinerary API (.NET 8) — GitHub-Only Setup

A minimal **.NET 8 Web API** that generates day-by-day travel itineraries using **OpenAI** and enriches
them with **Google Places**. Uses **SQLite** locally, and includes **GitHub Actions CI** (build + test) —
**no cloud hosting required**. You can demo it via **GitHub Codespaces** (public port) or run locally.

## Tech
- ASP.NET Core 8 (Controllers) + Swagger
- EF Core + SQLite (local)
- OpenAI Chat Completions API
- Google Places Text Search API
- GitHub Actions (build/test)
- Optional: Dockerfile
- Optional: GitHub Codespaces dev container

## Run locally
```bash
# from repo root
dotnet restore ./src/Travel.Api/Travel.Api.csproj
# set env vars for secrets (on mac/linux, use 'export'; on Windows cmd use 'set')
export OPENAI_API_KEY=sk-xxxx
export MAPS_API_KEY=AIza-xxxx

dotnet run --project ./src/Travel.Api/Travel.Api.csproj
# Swagger: http://localhost:5242/swagger  (port may vary)
```

### Sample request
`POST /api/itineraries/generate`
```json
{
  "tripName": "Seattle 3-day",
  "destination": "Seattle, WA",
  "startDate": "2025-11-10",
  "days": 3,
  "interests": ["coffee","museums","waterfront"],
  "budgetLevel": "mid"
}
```

## GitHub CI (no deploy)
- CI workflow: `.github/workflows/ci.yml` (restore, build, publish artifact).
- Optional release workflow: `.github/workflows/release.yml` to attach build artifact to a GitHub Release.

## Codespaces (share demo link)
This repo includes a `.devcontainer` config. In **GitHub → Code → Create Codespace**, then:
- The API runs automatically via `postCreateCommand`, or start with `dotnet run`.
- Forward the port and toggle **"Public"** to share a temporary URL for interviews/demos.

## Docker (optional)
```bash
docker build -t travelapi:dev -f Dockerfile .
docker run -p 8080:8080 -e OPENAI_API_KEY=sk-xxx -e MAPS_API_KEY=AIza-xxx travelapi:dev
# Swagger at http://localhost:8080/swagger
```

## Environment
Secrets read from process environment:
- `OPENAI_API_KEY`
- `MAPS_API_KEY`

Create a local `.env` if you want (not committed). For Codespaces, set secrets via **Codespaces → Secrets**.

## Notes
- The project uses `EnsureCreated()` to bootstrap the SQLite DB file (`itineraries.db`) in the working dir.
- If you add tests later, CI will run them automatically.
