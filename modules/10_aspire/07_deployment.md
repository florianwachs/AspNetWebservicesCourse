# Lesson 7: Deployment - Taking Aspire to Production

## Introduction

While .NET Aspire excels at local development, its real power shines when deploying applications to production. Aspire provides built-in deployment support through the Azure Developer CLI (azd), making it easy to deploy your distributed applications to Azure Container Apps. The same applications that run locally can be deployed to production with minimal configuration changes.

In this lesson, we'll explore deployment strategies, learn how to use Azure Developer CLI, configure production environments, manage secrets, and implement CI/CD pipelines.

## Table of Contents

1. [Deployment Overview](#deployment-overview)
2. [Azure Developer CLI (azd)](#azure-developer-cli-azd)
3. [Deploying to Azure Container Apps](#deploying-to-azure-container-apps)
4. [Infrastructure as Code](#infrastructure-as-code)
5. [Environment Configuration](#environment-configuration)
6. [Secrets Management](#secrets-management)
7. [CI/CD Integration](#cicd-integration)
8. [Production Considerations](#production-considerations)
9. [Monitoring Production](#monitoring-production)
10. [Alternative Deployment Options](#alternative-deployment-options)
11. [Best Practices](#best-practices)

## Deployment Overview

### From Development to Production

```
Development                  Production
┌─────────────┐             ┌──────────────────┐
│ AppHost     │             │ Azure Container  │
│ Orchestrates│  ──────▶    │ Apps             │
│ • Services  │   Deploy    │ • Auto-scaling   │
│ • Resources │             │ • Load balancing │
│ • Config    │             │ • Health checks  │
└─────────────┘             └──────────────────┘
```

### What Gets Deployed

```
Local Development          Azure Container Apps
├─ Projects               ├─ Container Images
│  ├─ Frontend            │  ├─ Frontend Container
│  ├─ CatalogApi          │  ├─ CatalogApi Container
│  └─ OrderApi            │  └─ OrderApi Container
│                          │
├─ Resources              ├─ Azure Resources
│  ├─ PostgreSQL          │  ├─ Azure DB for PostgreSQL
│  ├─ Redis               │  ├─ Azure Cache for Redis
│  └─ RabbitMQ            │  └─ Azure Service Bus
│                          │
└─ Configuration          └─ Azure Key Vault
   ├─ Connection Strings      ├─ Secrets
   └─ Settings                └─ Configuration
```

### Deployment Architecture

```
┌────────────────────────────────────────────────────────────────┐
│                      Azure Container Apps                      │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │  Frontend   │  │ CatalogApi  │  │  OrderApi   │           │
│  │  Container  │  │  Container  │  │  Container  │           │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘           │
│         │                │                │                   │
│         └────────────────┴────────────────┘                   │
│                          │                                    │
│         ┌────────────────┴────────────────┐                   │
│         │                                 │                   │
├─────────▼─────────────┐  ┌────────────────▼─────────────┐    │
│ Azure DB for          │  │ Azure Cache for Redis        │    │
│ PostgreSQL            │  │                              │    │
│ • Managed database    │  │ • Managed cache              │    │
│ • Auto backups        │  │ • High availability          │    │
│ • Scaling             │  │ • Persistence                │    │
└───────────────────────┘  └──────────────────────────────┘    │
│                                                                │
├────────────────────────────────────────────────────────────────┤
│                    Supporting Services                         │
├────────────────────────────────────────────────────────────────┤
│                                                                │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐        │
│  │ Azure Key    │  │ App Insights │  │ Azure        │        │
│  │ Vault        │  │              │  │ Monitor      │        │
│  │ • Secrets    │  │ • Telemetry  │  │ • Logs       │        │
│  └──────────────┘  └──────────────┘  └──────────────┘        │
│                                                                │
└────────────────────────────────────────────────────────────────┘
```

## Azure Developer CLI (azd)

### What is azd?

Azure Developer CLI (azd) is a command-line tool that:
- Initializes Aspire projects for deployment
- Provisions Azure infrastructure
- Builds and containerizes applications
- Deploys to Azure Container Apps
- Manages environments (dev, staging, prod)
- Integrates with CI/CD pipelines

### Installing azd

```bash
# Windows (PowerShell)
powershell -ex AllSigned -c "Invoke-RestMethod 'https://aka.ms/install-azd.ps1' | Invoke-Expression"

# macOS/Linux
curl -fsSL https://aka.ms/install-azd.sh | bash

# Verify installation
azd version
```

Output:
```
azd version 1.10.0 (stable)
```

### azd Prerequisites

```bash
# Required tools:
- Azure CLI (az)
- Docker Desktop
- .NET 10 SDK

# Install Azure CLI
# Windows: Download from https://aka.ms/installazurecliwindows
# macOS: brew install azure-cli
# Linux: curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash

# Verify
az --version
docker --version
dotnet --version
```

### Initializing a Project for Deployment

Navigate to your AppHost project:

```bash
cd MyApp.AppHost
azd init
```

You'll be prompted:

```
? Select an environment name: (my-app-env)
> my-app-dev

Initializing project...
✓ Project initialized

Next steps:
  1. Review the generated files in ./infra
  2. Run 'azd up' to provision and deploy to Azure
```

### Generated Files

```
MyApp.AppHost/
├── infra/                      # Infrastructure as Code
│   ├── main.bicep             # Main infrastructure definition
│   ├── main.parameters.json   # Parameters
│   └── resources/             # Resource modules
│       ├── containerApp.bicep
│       ├── database.bicep
│       └── cache.bicep
├── azure.yaml                  # azd project definition
└── .azure/                    # Azure environment configs
    └── my-app-dev/
        ├── .env               # Environment variables
        └── config.json        # Environment configuration
```

### azure.yaml Structure

```yaml
# azure.yaml
name: myapp
metadata:
  template: aspire@1.0.0

services:
  frontend:
    project: ../MyApp.Frontend
    language: csharp
    host: containerapp
  
  catalogapi:
    project: ../MyApp.CatalogApi
    language: csharp
    host: containerapp
  
  orderapi:
    project: ../MyApp.OrderApi
    language: csharp
    host: containerapp
```

## Deploying to Azure Container Apps

### First Deployment

```bash
# Login to Azure
azd auth login

# Provision infrastructure and deploy (one command!)
azd up
```

The `azd up` command:
1. Prompts for Azure subscription and region
2. Provisions all Azure resources (Bicep)
3. Builds Docker images for each service
4. Pushes images to Azure Container Registry
5. Deploys containers to Azure Container Apps
6. Configures service connections
7. Outputs endpoint URLs

### Deployment Output

```
Provisioning Azure resources (azd provision)
┌───────────────────────────────────────────┐
│ Resource Group: rg-myapp-dev              │
│ Location: eastus                          │
│                                           │
│ Creating resources:                       │
│ ✓ Container Apps Environment             │
│ ✓ Azure Container Registry               │
│ ✓ Azure Database for PostgreSQL          │
│ ✓ Azure Cache for Redis                  │
│ ✓ Application Insights                   │
│ ✓ Log Analytics Workspace                │
│ ✓ Key Vault                              │
└───────────────────────────────────────────┘

Building and deploying (azd deploy)
┌───────────────────────────────────────────┐
│ Building images:                          │
│ ✓ frontend                                │
│ ✓ catalogapi                              │
│ ✓ orderapi                                │
│                                           │
│ Deploying to Container Apps:              │
│ ✓ frontend                                │
│ ✓ catalogapi                              │
│ ✓ orderapi                                │
└───────────────────────────────────────────┘

SUCCESS: Your application was provisioned and deployed to Azure

Endpoints:
  frontend: https://frontend.happytree-12345678.eastus.azurecontainerapps.io
  catalogapi: https://catalogapi.happytree-12345678.eastus.azurecontainerapps.io
```

### Subsequent Deployments

After initial setup, deploy updates with:

```bash
# Deploy code changes only (faster)
azd deploy

# Provision infrastructure changes and deploy
azd up

# Deploy specific service
azd deploy frontend
```

### Viewing Deployment Status

```bash
# Show environment details
azd env list

# Show resource status
azd show

# Open Azure Portal
azd show --portal
```

## Infrastructure as Code

### Bicep Overview

Aspire generates Bicep (Azure's IaC language) to define infrastructure.

#### main.bicep

```bicep
targetScope = 'resourceGroup'

@description('Primary location for resources')
param location string = resourceGroup().location

@description('Name of the environment')
@minLength(1)
@maxLength(64)
param environmentName string

// Container Apps Environment
module containerAppsEnvironment 'resources/containerAppsEnvironment.bicep' = {
  name: 'containerAppsEnvironment'
  params: {
    name: '${environmentName}-env'
    location: location
  }
}

// Azure Container Registry
module containerRegistry 'resources/containerRegistry.bicep' = {
  name: 'containerRegistry'
  params: {
    name: replace('${environmentName}acr', '-', '')
    location: location
  }
}

// PostgreSQL Database
module database 'resources/database.bicep' = {
  name: 'database'
  params: {
    name: '${environmentName}-db'
    location: location
    administratorLogin: 'dbadmin'
    administratorPassword: databasePassword
  }
}

// Redis Cache
module cache 'resources/cache.bicep' = {
  name: 'cache'
  params: {
    name: '${environmentName}-cache'
    location: location
  }
}

// Application Insights
module appInsights 'resources/appInsights.bicep' = {
  name: 'appInsights'
  params: {
    name: '${environmentName}-insights'
    location: location
  }
}

// Frontend Container App
module frontendApp 'resources/containerApp.bicep' = {
  name: 'frontend'
  params: {
    name: 'frontend'
    location: location
    environmentId: containerAppsEnvironment.outputs.environmentId
    containerImage: '${containerRegistry.outputs.loginServer}/frontend:latest'
    environmentVariables: [
      {
        name: 'ASPNETCORE_ENVIRONMENT'
        value: 'Production'
      }
      {
        name: 'ConnectionStrings__catalogapi'
        value: 'https://${catalogApiApp.outputs.fqdn}'
      }
    ]
  }
}

// CatalogApi Container App
module catalogApiApp 'resources/containerApp.bicep' = {
  name: 'catalogapi'
  params: {
    name: 'catalogapi'
    location: location
    environmentId: containerAppsEnvironment.outputs.environmentId
    containerImage: '${containerRegistry.outputs.loginServer}/catalogapi:latest'
    environmentVariables: [
      {
        name: 'ConnectionStrings__catalogdb'
        value: database.outputs.connectionString
      }
      {
        name: 'ConnectionStrings__cache'
        value: cache.outputs.connectionString
      }
    ]
  }
}

// Outputs
output frontendUrl string = frontendApp.outputs.fqdn
output catalogApiUrl string = catalogApiApp.outputs.fqdn
```

### Customizing Infrastructure

Edit Bicep files to customize:

```bicep
// Scale configuration
module catalogApiApp 'resources/containerApp.bicep' = {
  params: {
    // ...
    minReplicas: 1
    maxReplicas: 10
    targetCpu: 70
    targetMemory: 70
  }
}

// Resource sizes
module database 'resources/database.bicep' = {
  params: {
    // ...
    sku: 'Standard_B1ms'  // Choose appropriate SKU
    storageSizeGB: 32
  }
}

// Custom domains
module frontendApp 'resources/containerApp.bicep' = {
  params: {
    // ...
    customDomains: [
      'www.myapp.com'
      'myapp.com'
    ]
  }
}
```

### Applying Infrastructure Changes

```bash
# Provision infrastructure changes
azd provision

# Or provision and deploy
azd up
```

## Environment Configuration

### Managing Multiple Environments

Create separate environments for dev, staging, and production:

```bash
# Create dev environment
azd env new dev

# Create staging environment
azd env new staging

# Create production environment
azd env new prod

# List environments
azd env list

# Switch environment
azd env select prod
```

### Environment-Specific Configuration

Each environment has its own configuration:

```
.azure/
├── dev/
│   ├── .env
│   └── config.json
├── staging/
│   ├── .env
│   └── config.json
└── prod/
    ├── .env
    └── config.json
```

#### .env File

```bash
# .azure/dev/.env
AZURE_LOCATION=eastus
AZURE_SUBSCRIPTION_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
DATABASE_SKU=Standard_B1ms
CACHE_SKU=Basic
MIN_REPLICAS=1
MAX_REPLICAS=3
```

```bash
# .azure/prod/.env
AZURE_LOCATION=eastus
AZURE_SUBSCRIPTION_ID=xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx
DATABASE_SKU=Standard_D4s_v3
CACHE_SKU=Premium
MIN_REPLICAS=3
MAX_REPLICAS=20
```

### Using Environment Variables in Bicep

```bicep
@description('Database SKU')
param databaseSku string = 'Standard_B1ms'

@description('Minimum replicas')
param minReplicas int = 1

@description('Maximum replicas')
param maxReplicas int = 10

// Use parameters from environment
module database 'resources/database.bicep' = {
  params: {
    sku: databaseSku
  }
}

module app 'resources/containerApp.bicep' = {
  params: {
    minReplicas: minReplicas
    maxReplicas: maxReplicas
  }
}
```

### Feature Flags

```csharp
// Program.cs
builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(builder.Configuration["AZURE_APP_CONFIG_CONNECTION_STRING"])
           .UseFeatureFlags();
});

builder.Services.AddFeatureManagement();

// Usage
public class ProductsController : ControllerBase
{
    private readonly IFeatureManager _featureManager;
    
    [HttpGet]
    public async Task<IActionResult> GetProducts()
    {
        if (await _featureManager.IsEnabledAsync("NewProductDisplay"))
        {
            return Ok(await GetProductsV2());
        }
        
        return Ok(await GetProductsV1());
    }
}
```

## Secrets Management

### Azure Key Vault Integration

Aspire automatically provisions Azure Key Vault for secrets:

```bicep
// Automatically generated
module keyVault 'resources/keyVault.bicep' = {
  name: 'keyVault'
  params: {
    name: '${environmentName}-kv'
    location: location
  }
}

// Container Apps configured to access Key Vault
module app 'resources/containerApp.bicep' = {
  params: {
    // ...
    secrets: [
      {
        name: 'database-password'
        keyVaultUrl: '${keyVault.outputs.vaultUri}secrets/database-password'
      }
    ]
  }
}
```

### Setting Secrets

```bash
# Set secret via azd
azd env set DATABASE_PASSWORD 'MySecurePassword123!'

# Or use Azure CLI
az keyvault secret set \
  --vault-name myapp-dev-kv \
  --name database-password \
  --value 'MySecurePassword123!'
```

### Using Secrets in Code

```csharp
// Secrets are injected as environment variables
var connectionString = builder.Configuration
    .GetConnectionString("catalogdb");

// Or from Key Vault directly
builder.Configuration.AddAzureKeyVault(
    new Uri(builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"]!),
    new DefaultAzureCredential());

// Access secret
var apiKey = builder.Configuration["ApiKeys:OpenAI"];
```

### Managed Identities

Container Apps use managed identities to access Azure resources:

```bicep
// Container App with managed identity
resource app 'Microsoft.App/containerApps@2024-03-01' = {
  identity: {
    type: 'SystemAssigned'
  }
  // ...
}

// Grant Key Vault access
resource keyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: app.identity.tenantId
        objectId: app.identity.principalId
        permissions: {
          secrets: ['get', 'list']
        }
      }
    ]
  }
}
```

No passwords needed in code or configuration!

## CI/CD Integration

### GitHub Actions

azd generates GitHub Actions workflows:

```bash
# Configure GitHub Actions
azd pipeline config

# Follow prompts to set up GitHub integration
```

Generated workflow:

```yaml
# .github/workflows/azure-dev.yml
name: Azure Dev

on:
  push:
    branches:
      - main
  workflow_dispatch:

permissions:
  id-token: write
  contents: read

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'
      
      - name: Install azd
        uses: Azure/setup-azd@v1.0.0
      
      - name: Azure Login
        uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      
      - name: Deploy
        run: azd up --no-prompt
        env:
          AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
          AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
          AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}
```

### Multi-Environment Pipeline

```yaml
name: Multi-Environment Deploy

on:
  push:
    branches:
      - main
      - develop

jobs:
  deploy-dev:
    if: github.ref == 'refs/heads/develop'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: Azure/setup-azd@v1.0.0
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - run: azd env select dev
      - run: azd up --no-prompt
  
  deploy-staging:
    if: github.ref == 'refs/heads/main'
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: Azure/setup-azd@v1.0.0
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - run: azd env select staging
      - run: azd up --no-prompt
  
  deploy-prod:
    if: github.ref == 'refs/heads/main'
    needs: deploy-staging
    runs-on: ubuntu-latest
    environment: production
    steps:
      - uses: actions/checkout@v4
      - uses: Azure/setup-azd@v1.0.0
      - uses: azure/login@v2
        with:
          client-id: ${{ secrets.AZURE_CLIENT_ID }}
          tenant-id: ${{ secrets.AZURE_TENANT_ID }}
          subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}
      - run: azd env select prod
      - run: azd up --no-prompt
```

### Azure DevOps Pipeline

```yaml
# azure-pipelines.yml
trigger:
  - main

pool:
  vmImage: 'ubuntu-latest'

steps:
  - task: setup-azd@0
    displayName: 'Install azd'
  
  - task: AzureCLI@2
    displayName: 'Deploy to Azure'
    inputs:
      azureSubscription: 'Azure Service Connection'
      scriptType: 'bash'
      scriptLocation: 'inlineScript'
      inlineScript: |
        azd up --no-prompt
    env:
      AZURE_ENV_NAME: $(AZURE_ENV_NAME)
      AZURE_LOCATION: $(AZURE_LOCATION)
```

## Production Considerations

### Scaling and Performance

```bicep
// Configure auto-scaling
module app 'resources/containerApp.bicep' = {
  params: {
    minReplicas: 3              // Always run at least 3
    maxReplicas: 30             // Scale up to 30
    targetCpu: 70               // Scale at 70% CPU
    targetMemory: 70            // Or 70% memory
    targetHttpRequests: 1000    // Or 1000 req/sec
  }
}
```

### High Availability

```bicep
// Deploy to multiple regions
module appEastUS 'regions/eastus.bicep' = {
  params: {
    location: 'eastus'
  }
}

module appWestUS 'regions/westus.bicep' = {
  params: {
    location: 'westus'
  }
}

// Use Azure Front Door for global load balancing
module frontDoor 'resources/frontdoor.bicep' = {
  params: {
    backends: [
      appEastUS.outputs.endpoint
      appWestUS.outputs.endpoint
    ]
  }
}
```

### Database Considerations

```bicep
// Production database configuration
module database 'resources/database.bicep' = {
  params: {
    sku: 'Standard_D4s_v3'      // Production SKU
    storageSizeGB: 256          // Adequate storage
    backupRetentionDays: 35     // Long retention
    geoRedundantBackup: true    // Geo-redundancy
    highAvailability: true      // HA configuration
  }
}
```

### Monitoring and Alerts

```bicep
// Application Insights with alerts
module appInsights 'resources/appInsights.bicep' = {
  params: {
    name: '${environmentName}-insights'
    location: location
  }
}

// Alert rules
module alerts 'resources/alerts.bicep' = {
  params: {
    appInsightsId: appInsights.outputs.id
    alerts: [
      {
        name: 'High Error Rate'
        condition: 'exceptions/count > 10'
        severity: 1
      }
      {
        name: 'High Response Time'
        condition: 'requests/duration > 5000'
        severity: 2
      }
      {
        name: 'Low Availability'
        condition: 'availabilityResults/availabilityPercentage < 99'
        severity: 0
      }
    ]
  }
}
```

### Security

```bicep
// Network isolation
module vnet 'resources/vnet.bicep' = {
  params: {
    name: '${environmentName}-vnet'
    location: location
  }
}

module containerAppsEnv 'resources/containerAppsEnvironment.bicep' = {
  params: {
    vnetSubnetId: vnet.outputs.subnetId
    internal: true  // Internal-only environment
  }
}

// Web Application Firewall
module waf 'resources/waf.bicep' = {
  params: {
    frontendUrl: frontendApp.outputs.fqdn
  }
}
```

## Monitoring Production

### Application Insights Integration

Automatically configured by Aspire:

```csharp
// Already configured in ServiceDefaults
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddAzureMonitorTraceExporter());
```

### Querying Logs

```kusto
// KQL queries in Application Insights

// Error rate over time
requests
| where timestamp > ago(1h)
| summarize ErrorRate = countif(success == false) * 100.0 / count() by bin(timestamp, 5m)
| render timechart

// Slow requests
requests
| where duration > 5000
| project timestamp, name, duration, resultCode
| order by duration desc
| take 100

// Exception analysis
exceptions
| where timestamp > ago(1h)
| summarize Count = count() by type, outerMessage
| order by Count desc
```

### Distributed Tracing in Production

View traces in Application Insights:

```
Azure Portal → Application Insights → Performance
→ Select operation → View end-to-end transaction
```

### Dashboards

Create Azure Dashboards for monitoring:

```json
{
  "tiles": [
    {
      "type": "metrics",
      "metric": "requests/count",
      "aggregation": "sum",
      "timeRange": "PT1H"
    },
    {
      "type": "metrics",
      "metric": "requests/duration",
      "aggregation": "avg",
      "timeRange": "PT1H"
    },
    {
      "type": "logs",
      "query": "exceptions | summarize count()"
    }
  ]
}
```

## Alternative Deployment Options

### Docker Compose

For non-Azure deployments:

```bash
# Generate Dockerfile for each service (if not exists)
dotnet publish -c Release

# Create docker-compose.yml manually or use Aspire's manifest
dotnet run --project MyApp.AppHost -- --output-path manifest.json --publisher manifest

# Convert to docker-compose (manual or tooling)
```

### Kubernetes

Deploy to any Kubernetes cluster:

```bash
# Generate Kubernetes manifests
# (Aspire doesn't generate these directly - use tools like Helm or Kustomize)

# Build images
docker build -t myapp/frontend:latest ./MyApp.Frontend
docker build -t myapp/catalogapi:latest ./MyApp.CatalogApi

# Push to registry
docker push myapp/frontend:latest
docker push myapp/catalogapi:latest

# Apply manifests
kubectl apply -f k8s/
```

### AWS

Deploy to AWS using:
- ECS (Elastic Container Service)
- EKS (Elastic Kubernetes Service)
- App Runner

```bash
# Use Docker images and deploy via AWS CLI or CDK
```

### Self-Hosted

Deploy to your own infrastructure:

```bash
# Build and run with Docker
docker compose up -d

# Or run directly (not recommended for production)
dotnet MyApp.Frontend.dll
dotnet MyApp.CatalogApi.dll
```

## Best Practices

### 1. Use azd for Consistency

```bash
# Always use azd for deployments
azd up  # ✅

# Avoid manual deployments
az containerapp update ...  # ❌ Use only for debugging
```

### 2. Separate Environments

```bash
# Never share environments between dev/staging/prod
azd env new dev      # ✅
azd env new staging  # ✅
azd env new prod     # ✅
```

### 3. Version Container Images

```yaml
# Tag images with version or commit SHA
- run: azd deploy --image-tag ${{ github.sha }}
```

### 4. Use Managed Identities

```csharp
// Avoid connection strings with passwords
builder.Services.AddAzureBlobClient("storage"); // ✅ Uses managed identity

// Avoid
var connectionString = "DefaultEndpointsProtocol=https;AccountName=...;AccountKey=..."; // ❌
```

### 5. Monitor Costs

```bash
# Check resource costs regularly
az consumption usage list --start-date 2024-01-01

# Use Azure Cost Management for alerts
```

### 6. Implement Health Checks

```csharp
builder.Services.AddHealthChecks()
    .AddNpgsql(connectionString, name: "database")
    .AddRedis(redisConnectionString, name: "cache");

app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");
```

### 7. Configure Logging Levels

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "System": "Warning"
    }
  }
}
```

### 8. Use Application Insights Sampling

```csharp
builder.Services.Configure<TelemetryConfiguration>(config =>
{
    config.DefaultTelemetrySink.TelemetryProcessorChainBuilder
        .UseAdaptiveSampling(maxTelemetryItemsPerSecond: 5);
});
```

### 9. Plan for Disaster Recovery

```bicep
// Enable geo-replication
module database 'resources/database.bicep' = {
  params: {
    geoRedundantBackup: true
    backupRetentionDays: 35
  }
}

// Regular backup verification
// Test restore procedures
```

### 10. Document Runbooks

Create operational documentation:

```markdown
# Deployment Runbook

## Standard Deployment
1. Run tests: `dotnet test`
2. Deploy: `azd up`
3. Verify health: Check Application Insights
4. Monitor for 15 minutes

## Rollback Procedure
1. Identify previous working version
2. Deploy: `azd deploy --image-tag <previous-sha>`
3. Verify rollback successful

## Incident Response
1. Check Application Insights for errors
2. Review logs in Azure Portal
3. Scale up if needed: Update Bicep and redeploy
```

## Summary

Deploying Aspire applications to production:

✅ **Azure Developer CLI** simplifies deployment to Azure Container Apps  
✅ **Infrastructure as Code** with Bicep ensures reproducible deployments  
✅ **Multiple environments** (dev, staging, prod) are easily managed  
✅ **Secrets management** with Azure Key Vault and managed identities  
✅ **CI/CD integration** with GitHub Actions or Azure DevOps  
✅ **Monitoring** with Application Insights provides full observability  
✅ **Alternative deployments** to Kubernetes, Docker, or other clouds  

Key takeaways:
- `azd up` deploys entire application stack with one command
- Aspire generates production-ready Bicep infrastructure
- Managed identities eliminate password management
- Application Insights provides production telemetry
- Same code runs locally and in production

This completes the comprehensive .NET Aspire module. You now have the knowledge to build, develop, and deploy cloud-native distributed applications with .NET Aspire!
