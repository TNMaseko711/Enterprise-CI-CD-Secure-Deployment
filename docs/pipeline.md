# Pipeline Details

## Continuous Integration (CI)
Triggered on pull requests to `main`:
- Restore dependencies
- Build the API
- Run unit tests
- Run dependency review
- Run gitleaks secret scanning

## Continuous Deployment (CD)
Triggered on pushes to `main`:
- Build Docker image using multi-stage Dockerfile
- Tag image with the short commit SHA
- Push image to GitHub Container Registry (GHCR)
- Deploy with Docker Compose using environment variables

## Security Gates
- Dependency review blocks newly introduced vulnerable packages.
- Gitleaks blocks accidental secret commits.

## Failure Handling
- If build or tests fail, the CI pipeline fails and prevents merge.
- If security checks fail, the PR cannot be merged.
- The CD pipeline only runs for successful merges to `main`.
