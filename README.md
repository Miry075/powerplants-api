# Power Plants Challenge

A .NET REST API that solves the **unit-commitment problem**: given a target load and a set of power plants with different fuel costs, determine how much power each plant should produce using the **merit-order** dispatch algorithm.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [API Reference](#api-reference)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Run locally](#run-locally)
  - [Run with Docker](#run-with-docker)
- [Project Structure](#project-structure)
- [Running Tests](#running-tests)

---

## Overview

The API accepts a production plan request containing:
- A **load** (MWh) that must be met.
- **Fuel prices** (gas, kerosine, CO₂, wind availability).
- A list of **power plants** (gas-fired, turbojet, wind turbine).

It returns the optimal allocation of production across plants by ordering them by **marginal cost** (merit order) and greedily assigning load, cheapest first.

### Supported plant types

| Type | Fuel | Marginal cost formula |
|---|---|---|
| `gasfired` | Gas | `gas_price / efficiency` |
| `turbojet` | Kerosine | `kerosine_price / efficiency` |
| `windturbine` | Wind | `0` (free) — effective PMax = `pmax × wind%` |

---

## Architecture

The solution follows a clean layered structure:

```
Domain      →  Models (request/response/entities)
Application →  Service interfaces
Api         →  Controllers, concrete services, helpers
Tests       →  Unit tests
```

```
┌─────────────────────┐
│  HTTP Client        │
└────────┬────────────┘
         │  POST /productionplan
┌────────▼────────────┐
│  ProductionPlan     │
│  Controller         │
└────────┬────────────┘
         │
┌────────▼────────────┐
│  DispatchService    │  orders plants by marginal cost (CostHelper)
└────────┬────────────┘
         │
┌────────▼────────────┐
│  ProductionHelper   │  greedy merit-order dispatch
└─────────────────────┘
```

---

## API Reference

### `POST /productionplan`

**Request body**

```json
{
  "load": 480,
  "fuels": {
    "gas(euro/MWh)": 13.4,
    "kerosine(euro/MWh)": 50.8,
    "co2(euro/ton)": 20,
    "wind(%)": 60
  },
  "powerplants": [
    {
      "name": "gasfiredbig1",
      "type": "gasfired",
      "efficiency": 0.53,
      "pmin": 100,
      "pmax": 460
    },
    {
      "name": "windpark1",
      "type": "windturbine",
      "efficiency": 1,
      "pmin": 0,
      "pmax": 150
    }
  ]
}
```

**Response `200 OK`**

```json
[
  { "name": "windpark1",    "p": 90.0  },
  { "name": "gasfiredbig1", "p": 390.0 }
]
```

**Error responses**

| Status | Cause |
|---|---|
| `400 Bad Request` | Request body is null or contains invalid data |
| `500 Internal Server Error` | Unknown plant type or unexpected error |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- (Optional) [Docker](https://www.docker.com/)

### Run locally

```bash
# Clone the repository
git clone <repo-url>
cd PowerPlants

# Restore & run
dotnet restore
dotnet run --project Powerplants.Challenge.Api
```

The API will be available at `http://localhost:5000` (or the port shown in the console).

You can test it directly with the included HTTP file:
```
Powerplants.Challenge.Api/Powerplants.Challenge.Api.http
```

### Run with Docker

```bash
# Build the image
docker build -t powerplants-challenge .

# Run the container
docker run -p 8080:8080 powerplants-challenge
```

The API will be available at `http://localhost:8080`.

---

## Project Structure

```
PowerPlants/
├── Powerplants.Challenge.Api/          # ASP.NET Core Web API
│   ├── Controllers/
│   │   └── ProductionPlanController.cs  # POST /productionplan endpoint
│   ├── Services/
│   │   └── DispatchService.cs           # Orchestrates dispatch logic
│   └── Helpers/
│       ├── CostHelper.cs                # Marginal cost calculation
│       └── ProductionHelper.cs          # Merit-order dispatch algorithm
├── Powerplants.Challenge.Application/  # Service interfaces (IDispatchService)
├── Powerplants.Challenge.Domain/       # Domain models
│   └── Models/
│       ├── FuelsInfo.cs
│       ├── Powerplant.cs
│       ├── PowerplantProduction.cs
│       ├── ProductionPlanRequest.cs
│       └── ProductionPlanResponse.cs
├── Powerplants.Challenge.Tests/        # xUnit unit tests
│   ├── Helpers/
│   │   ├── CostHelperShould.cs
│   │   └── ProductionHelperShould.cs
│   └── Services/
│       └── DispatchServiceShould.cs
└── Dockerfile
```

---

## Running Tests

```bash
dotnet test
```
