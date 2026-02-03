# Enterprise CI/CD & Secure Deployment Pipeline

## What Problem This Solves
This project demonstrates how a regulated internal API can move safely from code to production with explicit build, test, and security gates. It shows how to containerize a service, enforce quality checks, and deploy in a controlled way without leaking secrets.

## Architecture Summary
- ASP.NET Core minimal API with health and employee CRUD endpoints
- Multi-stage Docker build running as a non-root user
- CI pipeline for build/test/security gates
- CD pipeline that publishes to GHCR and deploys via Docker Compose

For a deeper view, see [docs/architecture.md](docs/architecture.md).

## CI vs CD Responsibilities
**CI (Pull Requests)**
- Restores dependencies
- Builds the API
- Runs unit tests
- Runs dependency review and secret scanning

**CD (Main Branch)**
- Builds Docker image
- Tags with commit hash
- Pushes to GHCR
- Deploys with Docker Compose

Details are documented in [docs/pipeline.md](docs/pipeline.md).

## Running Locally
```bash
# Build the Docker image
docker build -f docker/Dockerfile -t employee-api:local .

# Run with Docker Compose
docker compose -f docker/docker-compose.yml up -d
```

## Secrets Handling
- No secrets are committed to the repository.
- Pipeline secrets (registry credentials, tokens) are injected through GitHub Secrets.
- Runtime configuration uses environment variables (see `docker/production.env` as an example).

## Failure Scenarios and Rollback Strategy
- **Build/test failure**: CI fails; merge is blocked.
- **Security gate failure**: dependency review or secret scan blocks merge.
- **Deployment failure**: CD job fails; previous running container remains healthy.
- **Rollback**: redeploy by pinning the prior image tag (commit hash) in the deployment environment.

## What I Would Improve With a Real Cloud Environment
- Use managed container hosting (ECS/AKS/App Service) with auto-scaling.
- Add centralized logging and alerting (e.g., SIEM or managed log analytics).
- Implement secrets rotation with a managed vault service.
- Add blue/green or canary deployment strategies.

## Repository Structure
```
/
├── src/
├── tests/
├── docker/
│   ├── Dockerfile
│   ├── docker-compose.yml
│   └── production.env
├── .github/
│   └── workflows/
│       ├── ci.yml
│       └── cd.yml
├── docs/
│   ├── architecture.md
│   └── pipeline.md
└── README.md
```
