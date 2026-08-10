# Poll & Survey Builder

A real-time Poll & Survey Builder developed for the AMD201 Advanced .NET Development group assignment. Live deployment verified.

The application allows users to create multiple-choice polls, share a unique poll link, submit votes, view live results, and close polls. Results update in real time using SignalR.

## Live Application

Frontend:
https://poll-survey-frontend.vercel.app

Backend API:
https://poll-survey-backend-production.up.railway.app

## Features

- Create a poll with up to 6 answer options
- Generate a unique shareable poll code
- Vote without creating an account
- Prevent duplicate voting using a voter token
- View poll results
- Real-time result updates using SignalR
- Creator can close a poll
- Data stored in PostgreSQL
- RESTful ASP.NET Core Web API
- React/Vite frontend
- Automated CI/CD deployment
- Unit and integration testing

## Technology Stack

### Backend
- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- PostgreSQL
- SignalR
- xUnit

### Frontend
- React
- Vite
- Axios
- SignalR Client

### DevOps & Deployment
- Docker
- Docker Compose
- GitHub Actions
- GitHub Container Registry (GHCR)
- Railway
- Vercel

## Architecture

The deployed application follows this architecture:

```text
Frontend (Vercel)
        |
        | HTTPS / REST API / SignalR
        v
ASP.NET Core Backend (Railway)
        |
        | Entity Framework Core
        v
PostgreSQL Database (Railway)
```

The frontend communicates with the ASP.NET Core backend using REST API requests.

SignalR is used for real-time communication so poll results can update immediately when another user submits a vote.

The backend uses Entity Framework Core to store polls, poll options, and votes in PostgreSQL.

## CI/CD Pipeline

The backend uses GitHub Actions for automated CI/CD.

On every push to the `main` branch:

1. GitHub Actions checks out the repository.
2. .NET dependencies are restored.
3. The solution is built.
4. Static analysis is performed using `dotnet format`.
5. Unit and integration tests are executed.
6. A Docker image is built.
7. The Docker image is pushed to GitHub Container Registry.
8. Railway is automatically triggered to deploy the new backend image.

Pipeline:

```text
Git Push
    |
    v
GitHub Actions
    |
    +--> Build
    |
    +--> Static Analysis
    |
    +--> Unit & Integration Tests
    |
    +--> Docker Build
    |
    v
GitHub Container Registry
    |
    v
Railway Deployment
```

The frontend is deployed separately through Vercel.
## Docker

The backend uses a multi-stage Docker build.

The first stage uses the .NET SDK to restore and publish the application.

The second stage uses the smaller ASP.NET runtime image to run the published application.

Example local build:

```bash
docker build -t poll-survey-backend .
```

Run the full local backend and database environment using:

```bash
docker compose up --build
```

The API is then available at:

```text
http://localhost:8080
```

## Running the Backend Locally

### Requirements

- .NET 10 SDK
- Docker Desktop
- Git
- PostgreSQL or Docker Compose

### Clone the repository

```bash
git clone https://github.com/bobbbb-source/poll-survey-backend.git
cd poll-survey-backend
```

### Restore packages

```bash
dotnet restore
```

### Build

```bash
dotnet build
```

### Run tests

```bash
dotnet test PollSurveyBuilder.Tests/PollSurveyBuilder.Tests.csproj
```

### Run using Docker Compose

```bash
docker compose up --build
```

## Running the Frontend Locally

Clone the frontend repository:

```bash
git clone https://github.com/bobbbb-source/poll-survey-frontend.git
cd poll-survey-frontend
```

Install dependencies:

```bash
npm install
```

Start the development server:

```bash
npm run dev
```

The frontend normally runs at:

```text
http://localhost:5173
```

## API Endpoints

Main poll endpoints include:

```text
POST /api/polls
GET /api/polls/{code}
POST /api/polls/{code}/vote
GET /api/polls/{code}/results
```

The backend also includes functionality for closing polls and SignalR communication.

## Testing

The backend includes both unit tests and integration tests.

### Unit Tests

Unit tests validate core poll creation and validation logic.

Examples include:

- Rejecting polls with fewer than 2 options
- Creating a valid poll successfully
- Rejecting more than 6 options
- Rejecting duplicate options

### Integration Test

The integration test starts the ASP.NET Core application using `WebApplicationFactory`, sends a real HTTP request to the API, and verifies that creating a poll returns HTTP `201 Created`.

Run all tests with:

```bash
dotnet test PollSurveyBuilder.Tests/PollSurveyBuilder.Tests.csproj
```

## Deployment

### Backend

The backend is deployed on Railway:

https://poll-survey-backend-production.up.railway.app

The Docker image is stored in GitHub Container Registry:

```text
ghcr.io/bobbbb-source/poll-survey-backend:latest
```

### Frontend

The React frontend is deployed on Vercel:

https://poll-survey-frontend.vercel.app

## Repositories

Backend:  
https://github.com/bobbbb-source/poll-survey-backend

Frontend:  
https://github.com/bobbbb-source/poll-survey-frontend

## Project Topic

**AMD201 — Poll & Survey Builder**

The project was developed using:

- ASP.NET Core
- React
- Docker
- GitHub Actions
- PostgreSQL
- SignalR