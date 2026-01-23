# Exercise 5: Deployment to Azure

## 🎯 Objectives

In this exercise, you will:

- Setup Azure Developer CLI (azd)
- Deploy .NET Aspire application to Azure Container Apps
- Configure Azure Key Vault for secrets management
- Setup Azure Application Insights for monitoring
- Implement CI/CD with GitHub Actions
- Configure scaling and resilience
- Implement blue-green deployment

## ⏱️ Estimated Time

90-120 minutes

## 📋 Prerequisites

- Completed previous exercises
- Azure subscription ([Free trial](https://azure.microsoft.com/free/))
- Azure CLI installed
- Azure Developer CLI installed
- GitHub account
- Basic understanding of Azure services

## 🔧 Part 1: Setup and Initial Deployment

### Step 1: Install Azure Developer CLI

```bash
# Windows (PowerShell)
winget install microsoft.azd

# macOS
brew tap azure/azd && brew install azd

# Linux
curl -fsSL https://aka.ms/install-azd.sh | bash

# Verify installation
azd version
```

### Step 2: Create a Deployable Aspire Application

```bash
cd aspire-labs
dotnet new aspire-starter -n DeploymentDemo
cd DeploymentDemo
```

### Step 3: Initialize Azure Developer CLI

```bash
# Initialize azd in your project
azd init

# When prompted:
# - Select: "Use code in the current directory"
# - Environment name: "dev" (or your preferred name)
```

This creates:
- `azure.yaml` - Azure Developer CLI configuration
- `.azure/` - Environment configuration directory
- `infra/` - Bicep infrastructure as code templates (if not present)

### Step 4: Review azure.yaml

The `azure.yaml` file should look like this:

```yaml
name: DeploymentDemo
metadata:
  template: aspire-starter

services:
  apiservice:
    project: ./DeploymentDemo.ApiService
    language: dotnet
    host: containerapp

  webfrontend:
    project: ./DeploymentDemo.Web
    language: dotnet
    host: containerapp
```

### Step 5: Login to Azure

```bash
# Login to Azure
azd auth login

# Set your subscription (if you have multiple)
az account list --output table
az account set --subscription "Your-Subscription-Name"
```

### Step 6: Deploy to Azure

```bash
# Provision and deploy everything
azd up

# When prompted:
# - Select your Azure subscription
# - Select a region (e.g., "eastus", "westeurope")
# - Confirm deployment
```

This command will:
1. Build your application
2. Create Azure resources (Container Apps, Container Registry, Log Analytics, etc.)
3. Push container images
4. Deploy your services

**Note:** Initial deployment takes 5-10 minutes.

### ✅ Verification Point 1

After deployment completes:

```bash
# Show deployed resources
azd show

# Open the web frontend
azd show --endpoint webfrontend
```

**Check in Azure Portal:**
1. Navigate to your resource group
2. Verify these resources exist:
   - Container Apps Environment
   - Container Apps (apiservice, webfrontend)
   - Container Registry
   - Log Analytics Workspace
   - Application Insights (if configured)

## 🔐 Part 2: Secrets Management with Azure Key Vault

### Step 1: Add Key Vault to Infrastructure

Create or update `infra/main.bicep` to include Key Vault:

```bicep
param location string = resourceGroup().location
param principalId string

// Existing resources...

// Add Key Vault
resource keyVault 'Microsoft.KeyVault/vaults@2023-02-01' = {
  name: 'kv-${resourceGroup().name}'
  location: location
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enableRbacAuthorization: true
    enabledForDeployment: true
  }
}

// Grant access to the deployment principal
resource keyVaultRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, principalId, 'KeyVaultSecretsUser')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: principalId
    principalType: 'ServicePrincipal'
  }
}

output keyVaultName string = keyVault.name
output keyVaultEndpoint string = keyVault.properties.vaultUri
```

### Step 2: Add Secrets to Key Vault

```bash
# Get Key Vault name
KV_NAME=$(azd env get-values | grep AZURE_KEY_VAULT_NAME | cut -d'=' -f2 | tr -d '"')

# Add secrets
az keyvault secret set --vault-name $KV_NAME --name "DatabasePassword" --value "YourSecurePassword123!"
az keyvault secret set --vault-name $KV_NAME --name "ApiKey" --value "your-api-key-here"
az keyvault secret set --vault-name $KV_NAME --name "ConnectionString" --value "Server=...;Database=...;"
```

### Step 3: Configure Application to Use Key Vault

Update `DeploymentDemo.ApiService/Program.cs`:

```csharp
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Key Vault configuration
if (!builder.Environment.IsDevelopment())
{
    var keyVaultEndpoint = builder.Configuration["AZURE_KEY_VAULT_ENDPOINT"];
    
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultEndpoint),
            new DefaultAzureCredential());
    }
}

// Now you can access secrets like regular configuration
var dbPassword = builder.Configuration["DatabasePassword"];
var apiKey = builder.Configuration["ApiKey"];

var app = builder.Build();

app.MapGet("/config", (IConfiguration config) =>
{
    // Don't expose secrets in real applications!
    return new
    {
        KeyVaultConfigured = !string.IsNullOrEmpty(config["AZURE_KEY_VAULT_ENDPOINT"]),
        HasDatabasePassword = !string.IsNullOrEmpty(config["DatabasePassword"]),
        Environment = builder.Environment.EnvironmentName
    };
});

app.MapDefaultEndpoints();

app.Run();
```

### Step 4: Add Required Package

```bash
cd DeploymentDemo.ApiService
dotnet add package Azure.Identity
dotnet add package Azure.Extensions.AspNetCore.Configuration.Secrets
```

### Step 5: Update and Redeploy

```bash
cd ..
azd deploy
```

### ✅ Verification Point 2

Test Key Vault integration:

```bash
# Get the API endpoint
API_ENDPOINT=$(azd show --endpoint apiservice)

# Test configuration endpoint
curl $API_ENDPOINT/config
```

Expected response:
```json
{
  "keyVaultConfigured": true,
  "hasDatabasePassword": true,
  "environment": "Production"
}
```

## 📊 Part 3: Monitoring with Application Insights

### Step 1: Verify Application Insights Setup

Application Insights should be automatically configured by Aspire. Verify:

```bash
# Get Application Insights connection string
azd env get-values | grep APPLICATIONINSIGHTS
```

### Step 2: View Telemetry in Azure Portal

1. Open Azure Portal
2. Navigate to your Application Insights resource
3. Explore:
   - **Live Metrics** - Real-time telemetry
   - **Application Map** - Service dependencies
   - **Performance** - Request/response times
   - **Failures** - Error rates and exceptions
   - **Logs** - Query logs with KQL

### Step 3: Create Custom KQL Queries

In Application Insights Logs, try these queries:

```kql
// All requests in the last hour
requests
| where timestamp > ago(1h)
| summarize count() by name, resultCode
| order by count_ desc

// Slow requests (>1 second)
requests
| where duration > 1000
| project timestamp, name, duration, resultCode
| order by duration desc

// Error rate over time
requests
| where timestamp > ago(24h)
| summarize 
    total = count(),
    errors = countif(success == false)
    by bin(timestamp, 1h)
| extend errorRate = errors * 100.0 / total
| project timestamp, errorRate
| render timechart

// Dependency calls
dependencies
| where timestamp > ago(1h)
| summarize count() by name, type, resultCode
| order by count_ desc

// Custom events and metrics
customMetrics
| where timestamp > ago(1h)
| project timestamp, name, value
| render timechart
```

### Step 4: Create Alerts

Create an alert for high error rates:

```bash
# Get Application Insights ID
APPINSIGHTS_ID=$(az monitor app-insights component show \
  --app YourAppInsightsName \
  --resource-group YourResourceGroup \
  --query id -o tsv)

# Create action group (for notifications)
az monitor action-group create \
  --name "AspireAlerts" \
  --resource-group YourResourceGroup \
  --short-name "AspireAlrt" \
  --email-receiver name=admin email=admin@example.com

# Create alert rule
az monitor scheduled-query create \
  --name "HighErrorRate" \
  --resource-group YourResourceGroup \
  --scopes $APPINSIGHTS_ID \
  --condition "count 'Placeholder' > 10" \
  --condition-query "requests | where success == false | count" \
  --description "Alert when error rate exceeds threshold" \
  --action-groups "AspireAlerts" \
  --evaluation-frequency 5m \
  --window-size 15m
```

### ✅ Verification Point 3

**Generate traffic and check monitoring:**

```bash
# Generate requests
for i in {1..100}; do
  curl $API_ENDPOINT/weatherforecast
done
```

**In Application Insights:**
- Verify requests appear in Live Metrics
- Check Application Map shows services
- View request details in Performance tab

## 🚀 Part 4: CI/CD with GitHub Actions

### Step 1: Create GitHub Repository

```bash
# Initialize git (if not already done)
git init
git add .
git commit -m "Initial commit"

# Create GitHub repository and push
gh repo create DeploymentDemo --public --source=. --remote=origin --push
```

Or create manually and push:

```bash
git remote add origin https://github.com/yourusername/DeploymentDemo.git
git branch -M main
git push -u origin main
```

### Step 2: Configure GitHub for Azure Deployment

```bash
# Configure azd for GitHub Actions
azd pipeline config

# This will:
# 1. Create Azure service principal
# 2. Add GitHub secrets (AZURE_CREDENTIALS, etc.)
# 3. Generate workflow file
```

### Step 3: Review GitHub Actions Workflow

Check `.github/workflows/azure-dev.yml`:

```yaml
name: Azure Dev Deploy

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
        dotnet-version: '8.0.x'

    - name: Install azd
      uses: Azure/setup-azd@v1.0.0

    - name: Azure Login
      uses: azure/login@v2
      with:
        client-id: ${{ secrets.AZURE_CLIENT_ID }}
        tenant-id: ${{ secrets.AZURE_TENANT_ID }}
        subscription-id: ${{ secrets.AZURE_SUBSCRIPTION_ID }}

    - name: Deploy with azd
      run: azd up --no-prompt
      env:
        AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
        AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
        AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}
```

### Step 4: Trigger Deployment

```bash
# Make a change and push
echo "# Deployment Demo" > README.md
git add README.md
git commit -m "Add README"
git push

# Watch the workflow in GitHub Actions
gh run watch
```

### ✅ Verification Point 4

**Check GitHub Actions:**
1. Go to your GitHub repository
2. Click "Actions" tab
3. Verify workflow runs successfully
4. Check deployment logs

## ⚖️ Part 5: Scaling and Resilience

### Step 1: Configure Auto-Scaling

Update Container App configuration in `infra/main.bicep`:

```bicep
resource apiService 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'apiservice'
  location: location
  properties: {
    environmentId: containerAppsEnvironment.id
    configuration: {
      ingress: {
        external: true
        targetPort: 8080
        transport: 'http'
      }
    }
    template: {
      containers: [
        {
          name: 'apiservice'
          image: '${containerRegistry.properties.loginServer}/apiservice:latest'
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 10
        rules: [
          {
            name: 'http-scaling'
            http: {
              metadata: {
                concurrentRequests: '100'
              }
            }
          }
          {
            name: 'cpu-scaling'
            custom: {
              type: 'cpu'
              metadata: {
                type: 'Utilization'
                value: '70'
              }
            }
          }
        ]
      }
    }
  }
}
```

### Step 2: Configure Health Probes

Add health checks to Container App:

```bicep
template: {
  containers: [
    {
      name: 'apiservice'
      image: '...'
      probes: [
        {
          type: 'Liveness'
          httpGet: {
            path: '/health'
            port: 8080
          }
          initialDelaySeconds: 10
          periodSeconds: 10
        }
        {
          type: 'Readiness'
          httpGet: {
            path: '/health'
            port: 8080
          }
          initialDelaySeconds: 5
          periodSeconds: 5
        }
        {
          type: 'Startup'
          httpGet: {
            path: '/health'
            port: 8080
          }
          initialDelaySeconds: 0
          periodSeconds: 1
          failureThreshold: 30
        }
      ]
    }
  ]
}
```

### Step 3: Deploy Updated Configuration

```bash
azd deploy
```

### Step 4: Test Auto-Scaling

Generate load to trigger scaling:

```bash
# Install hey for load testing
go install github.com/rakyll/hey@latest

# Or use Apache Bench
sudo apt-get install apache2-utils  # Linux
brew install httpie                   # macOS

# Generate load
hey -z 2m -c 50 $API_ENDPOINT/weatherforecast

# Watch replicas scale
az containerapp replica list \
  --name apiservice \
  --resource-group YourResourceGroup \
  --query "[].{Name:name, Status:properties.runningState}" \
  --output table
```

### ✅ Verification Point 5

**Monitor scaling:**
1. Azure Portal → Container App → Metrics
2. View "Replica Count" metric
3. Verify it scales up under load
4. Verify it scales down after load decreases

## 🔄 Part 6: Blue-Green Deployment

### Step 1: Create Deployment Script

Create `scripts/blue-green-deploy.sh`:

```bash
#!/bin/bash
set -e

RESOURCE_GROUP=$1
APP_NAME=$2
NEW_IMAGE=$3

# Get current active revision
CURRENT_REVISION=$(az containerapp revision list \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "[?properties.trafficWeight > 0].name" \
  -o tsv | head -n 1)

echo "Current active revision: $CURRENT_REVISION"

# Create new revision with new image
echo "Creating new revision..."
az containerapp update \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --image $NEW_IMAGE \
  --revision-suffix $(date +%Y%m%d%H%M%S)

# Get new revision name
NEW_REVISION=$(az containerapp revision list \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --query "[0].name" \
  -o tsv)

echo "New revision created: $NEW_REVISION"

# Split traffic 50/50 for canary testing
echo "Splitting traffic for canary testing..."
az containerapp ingress traffic set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --revision-weight $CURRENT_REVISION=50 $NEW_REVISION=50

echo "Canary deployment active. Monitor metrics..."
echo "Run './scripts/complete-deployment.sh $RESOURCE_GROUP $APP_NAME $NEW_REVISION' to complete"
echo "Run './scripts/rollback-deployment.sh $RESOURCE_GROUP $APP_NAME $CURRENT_REVISION' to rollback"
```

### Step 2: Create Completion Script

Create `scripts/complete-deployment.sh`:

```bash
#!/bin/bash
set -e

RESOURCE_GROUP=$1
APP_NAME=$2
NEW_REVISION=$3

echo "Switching all traffic to new revision..."
az containerapp ingress traffic set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --revision-weight $NEW_REVISION=100

echo "Deployment completed successfully!"
```

### Step 3: Create Rollback Script

Create `scripts/rollback-deployment.sh`:

```bash
#!/bin/bash
set -e

RESOURCE_GROUP=$1
APP_NAME=$2
OLD_REVISION=$3

echo "Rolling back to previous revision..."
az containerapp ingress traffic set \
  --name $APP_NAME \
  --resource-group $RESOURCE_GROUP \
  --revision-weight $OLD_REVISION=100

echo "Rollback completed!"
```

### Step 4: Make Scripts Executable

```bash
chmod +x scripts/*.sh
```

### Step 5: Perform Blue-Green Deployment

```bash
# Build and push new image
azd deploy --no-prompt

# Get the new image name
IMAGE_NAME=$(az acr repository show-tags \
  --name YourACRName \
  --repository apiservice \
  --orderby time_desc \
  --top 1 \
  -o tsv)

# Start blue-green deployment
./scripts/blue-green-deploy.sh YourResourceGroup apiservice YourACRName.azurecr.io/apiservice:$IMAGE_NAME

# Monitor metrics and errors for 5-10 minutes

# If everything looks good, complete deployment
./scripts/complete-deployment.sh YourResourceGroup apiservice NewRevisionName

# Or rollback if there are issues
./scripts/rollback-deployment.sh YourResourceGroup apiservice OldRevisionName
```

### ✅ Verification Point 6

**Test blue-green deployment:**
1. Make a visible change to your API
2. Deploy using blue-green script
3. Verify traffic split in Azure Portal
4. Test both versions are responding
5. Complete or rollback deployment

## 🎯 Challenge Tasks

1. **Setup staging environment** with separate azd environment
2. **Implement feature flags** with Azure App Configuration
3. **Add database migrations** in deployment pipeline
4. **Configure custom domains** and SSL certificates
5. **Implement disaster recovery** with multi-region deployment

## 📝 Summary

In this exercise, you:

- ✅ Deployed Aspire app to Azure Container Apps
- ✅ Configured Azure Key Vault for secrets
- ✅ Setup Application Insights monitoring
- ✅ Implemented CI/CD with GitHub Actions
- ✅ Configured auto-scaling and health checks
- ✅ Performed blue-green deployment

## 🧹 Cleanup

To avoid Azure charges, delete resources:

```bash
# Delete all Azure resources
azd down

# Or manually delete resource group
az group delete --name YourResourceGroup --yes --no-wait
```

## 🎓 Key Takeaways

1. **azd simplifies** Azure deployment
2. **Container Apps** provide serverless scaling
3. **Key Vault** secures sensitive data
4. **Application Insights** provides production monitoring
5. **GitHub Actions** automate deployments
6. **Blue-green deployment** enables safe releases

## 🎉 Congratulations!

You've completed all .NET Aspire exercises! You now know how to:
- Build cloud-native applications with Aspire
- Integrate databases, caching, and messaging
- Implement comprehensive observability
- Deploy and scale in production

## 📚 Additional Resources

- [Azure Developer CLI Documentation](https://learn.microsoft.com/azure/developer/azure-developer-cli/)
- [Azure Container Apps Documentation](https://learn.microsoft.com/azure/container-apps/)
- [.NET Aspire Deployment](https://learn.microsoft.com/dotnet/aspire/deployment/)
