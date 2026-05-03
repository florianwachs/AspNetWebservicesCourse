# Day 5 — API Evolution, Integration Testing & Alternative API Styles

## 🎯 Learning Objectives

By the end of Day 5, you will be able to:

TODO: I still want to add a AI sample for my students!!

- evolve public APIs safely by recognizing breaking changes and introducing explicit API versions
- choose between focused open-box tests with `WebApplicationFactory` and closed-box distributed-app tests with `Aspire.Hosting.Testing`
- write integration tests against realistic infrastructure using PostgreSQL, Testcontainers, and reliable database reset strategies
- compare when REST should stay simple and when protocols like MCP, gRPC, OData, or GraphQL are a better fit
- explore multiple ways to expose the same domain to browsers, integrations, and AI assistants

## 📖 Topics

1. **[API Versioning — Evolving Without Breaking](01-api-versioning.md)** — breaking vs non-breaking changes, versioning strategies, URL-path versioning with `Asp.Versioning`, deprecation, and separate OpenAPI documents per version
2. **[Integration Testing — Confidence in Your API](02-integration-testing.md)** — testing pyramid, `WebApplicationFactory` vs `Aspire.Hosting.Testing`, open-box vs closed-box testing, real infrastructure with Testcontainers, and cleanup with Respawn

## 🧪 Labs

**Core track — [Version the TechConf API](../../labs/lab-api-versioning/) and [Integration testing lab](../../labs/lab-testing/)**: Practice side-by-side API versions, version-aware OpenAPI docs, focused API tests, and distributed-app integration tests against real infrastructure.

**Optional advanced API track — [MCP server lab](../../labs/lab-mcp/), [gRPC streaming lab](../../labs/lab-grpc/), [OData lab](../../labs/lab-odata/), and [GraphQL lab](../../labs/lab-graphql/)**: Explore AI tool integration with MCP, live event streaming with gRPC and gRPC-Web, standardized queryable REST with OData, and schema-first APIs with GraphQL.

## 📚 Key Packages Introduced Today

| Package | Purpose |
|---------|---------|
| `Asp.Versioning.Http` | API versioning for ASP.NET Core HTTP endpoints |
| `Microsoft.AspNetCore.Mvc.Testing` | In-process integration testing for a single ASP.NET Core service |
| `Aspire.Hosting.Testing` | Closed-box integration testing through the Aspire AppHost |
| `Testcontainers.PostgreSql` | Real PostgreSQL infrastructure for integration tests |
| `Respawn` | Fast database reset between integration tests |
| `ModelContextProtocol` | Build MCP servers that expose tools and prompts to AI assistants |
| `Grpc.AspNetCore` / `Grpc.AspNetCore.Web` | Build gRPC services and enable browser access through gRPC-Web |
| `Microsoft.AspNetCore.OData` | Add standardized query and metadata capabilities to REST APIs |
| `HotChocolate.AspNetCore` | Build GraphQL APIs with queries, mutations, filtering, and pagination |
