# Architecture Overview

## System Context
The system is a single internal API that exposes health and employee CRUD endpoints. It is packaged into a container image and deployed via Docker Compose to a simulated production environment. The CI/CD pipelines enforce build, test, and security gates before deployment.

## Components
- **Employee API**: ASP.NET Core minimal API providing `/health` and `/employees` endpoints.
- **Container Image**: Multi-stage Docker build that runs as a non-root user and includes a health check.
- **CI Pipeline**: Builds and tests the API and runs dependency/secret scanning.
- **CD Pipeline**: Builds the Docker image, tags it with the commit hash, pushes to GHCR, and deploys via Docker Compose.

## Data Flow
1. Client calls the API endpoints.
2. The API performs in-memory CRUD operations.
3. Logs are emitted in JSON format for structured logging consumption.

## Security Considerations
- The container runs as a non-root user.
- Secrets are supplied via GitHub Secrets and environment variables, not committed to the repository.
- CI security gates include dependency review and secret scanning.
