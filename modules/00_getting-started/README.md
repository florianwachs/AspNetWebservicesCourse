# Module 00: Getting Started

Welcome to the Web Services with .NET course! This module will help you set up your development environment and get ready for the course.

## 📋 Prerequisites

Before starting this course, you should have:
- Basic programming knowledge (any language)
- Understanding of web concepts (HTTP, APIs)
- Willingness to learn and experiment!

## 🛠️ Required Software

### 1. .NET 10 SDK

Download and install the latest .NET 10 SDK:
- **Download**: https://dotnet.microsoft.com/download/dotnet/10.0
- **Verify installation**: Open a terminal and run:
  ```bash
  dotnet --version
  ```
  You should see version 10.0.x or higher.

### 2. Development IDE

Choose one of the following:

#### Option A: Visual Studio 2025 (Recommended for Windows)
- **Download**: https://visualstudio.microsoft.com/
- **Edition**: Community (free) or higher
- **Workloads to install**:
  - ASP.NET and web development
  - .NET desktop development
  - .NET Aspire SDK

#### Option B: Visual Studio Code (Cross-platform)
- **Download**: https://code.visualstudio.com/
- **Required Extensions**:
  - C# Dev Kit (Microsoft)
  - .NET Aspire (Microsoft)
  - REST Client (Huachao Mao)
  - Docker (Microsoft)

### 3. Docker Desktop

Required for running containerized services and .NET Aspire:
- **Download**: https://www.docker.com/products/docker-desktop
- **System Requirements**:
  - Windows: WSL 2 enabled
  - macOS: Recent version (10.15 or later)
  - Linux: Docker Engine

**Verify installation**:
```bash
docker --version
docker compose version
```

### 4. Git

For version control:
- **Download**: https://git-scm.com/downloads
- **Configure Git** (first time only):
  ```bash
  git config --global user.name "Your Name"
  git config --global user.email "your.email@example.com"
  ```

### 5. Additional Tools (Optional but Recommended)

- **Postman** or **Insomnia**: For API testing
  - Postman: https://www.postman.com/downloads/
  - Insomnia: https://insomnia.rest/download
  
- **Azure Data Studio** or **SQL Server Management Studio**: For database management
  - Azure Data Studio: https://docs.microsoft.com/sql/azure-data-studio/download

## 🔧 .NET Aspire Setup

.NET Aspire is a core component of this course. Install the Aspire workload:

```bash
dotnet workload update
dotnet workload install aspire
```

Verify Aspire installation:
```bash
dotnet workload list
```

You should see `aspire` in the installed workloads list.

## 📦 Verify Your Setup

Let's verify everything is working correctly:

### 1. Create a Test Aspire Project

```bash
# Create a new directory for testing
mkdir aspire-test
cd aspire-test

# Create a new Aspire starter app
dotnet new aspire-starter -n MyTestApp

# Navigate to the AppHost project
cd MyTestApp/MyTestApp.AppHost

# Run the application
dotnet run
```

If everything is set up correctly:
- The Aspire dashboard should open in your browser (usually at http://localhost:15888)
- You should see the application components in the dashboard
- No error messages should appear

Press `Ctrl+C` to stop the application.

### 2. Clean Up Test Project

```bash
cd ../../..
rm -rf aspire-test
```

## 🚀 Clone the Course Repository

Clone this repository to your local machine:

```bash
git clone https://github.com/florianwachs/AspNetWebservicesCourse.git
cd AspNetWebservicesCourse
```

## 📚 Familiarize Yourself with the Structure

Take a moment to explore the repository structure:

```
AspNetWebservicesCourse/
├── modules/              # Course modules with exercises and documentation
│   ├── 00_getting-started/
│   ├── 01_csharp/
│   ├── 02_aspnet_basics/
│   └── ...
├── course/               # Workshop day schedules
├── 00_cheatsheets/       # Quick reference guides
├── 00_prerequisites/     # Setup instructions
└── README.md             # Course overview
```

## ✅ Quick Start Checklist

Use this checklist to ensure you're ready for the course:

- [ ] .NET 10 SDK installed and verified (`dotnet --version`)
- [ ] Visual Studio 2025 or VS Code with extensions installed
- [ ] Docker Desktop installed and running
- [ ] Git installed and configured
- [ ] .NET Aspire workload installed (`dotnet workload list`)
- [ ] Successfully ran an Aspire test project
- [ ] Course repository cloned to local machine
- [ ] Familiar with the repository structure

## 🆘 Troubleshooting

### Docker not starting
- **Windows**: Ensure WSL 2 is enabled and updated
- **macOS**: Check that Docker Desktop has sufficient resources allocated
- **All platforms**: Restart Docker Desktop

### .NET SDK version issues
```bash
# List all installed SDKs
dotnet --list-sdks

# If you have multiple versions, you can create a global.json to pin the version
dotnet new globaljson --sdk-version 10.0.100 --force
```

### Aspire workload installation fails
```bash
# Try updating first
dotnet workload update

# Then clean and reinstall
dotnet workload install aspire --skip-sign-check
```

### Port conflicts
If ports 5000, 5001, or 15888 are already in use, you may need to:
1. Stop other applications using these ports
2. Configure different ports in your Aspire projects

## 📖 Additional Resources

- [.NET 10 Documentation](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-10)
- [.NET Aspire Documentation](https://learn.microsoft.com/dotnet/aspire/)
- [C# 13 What's New](https://learn.microsoft.com/dotnet/csharp/whats-new/csharp-13)
- [Docker Getting Started](https://docs.docker.com/get-started/)

## 🎯 Next Steps

Once your environment is set up:
1. Review the [C# Cheat Sheet](../../00_cheatsheets/csharplanguage/csharp_cheat_sheet.md)
2. Explore [Module 01: C# & .NET Fundamentals](../01_csharp/)
3. Join the first workshop session!

---

Need help? Don't hesitate to ask questions during the course or reach out to the instructor!
