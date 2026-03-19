# Exam — Web Services with .NET

## Overview

The exam for **Web Services with .NET** is a project-based assessment. You will work in teams to design and implement a web service application using the technologies covered in this course.

## Format

- **Duration**: 45 minutes per team (including 10-15 minutes Q&A)
- **Team Size**: 5-6 students
- **Presentation**: Live demo + brief presentation of architecture and design decisions

## Requirements

Your project must demonstrate proficiency in the following areas:

### Mandatory
- [ ] **.NET-based API** with proper route organization (route groups) — minimum 8–12 endpoints
- [ ] **Entity Framework Core** with Database of your choice (at least 3 related entities)
- [ ] **Validation** (DataAnnotations and/or FluentValidation)
- [ ] **Error Handling** with Problem Details (RFC 9457)
- [ ] **Authentication & Authorization** (JWT/Keycloak/...)


### Options
- [ ] **HybridCache** with L1/L2 caching
- [ ] **SignalR** real-time communication
- [ ] **Background Services** (BackgroundService or worker project)
- [ ] **Integration Tests** with WebApplicationFactory + Testcontainers
- [ ] **API Versioning**
- [ ] **Resilience**
- [ ] **gRPC** service
- [ ] **GraphQL** 
- [ ] **OData** 
- [ ] **MCP** 

## Grading Criteria

### Milestone 1: Project Idea and Use Cases

#### Timing
Workshop Day 3 (morning block)

#### Content and Format
- 5-10 minute team presentation
- Present your project idea and planned use cases/user stories
- No detailed architecture or technology decisions required yet

#### Points
- **5 points**

### Milestone 2: Architecture and API Definition

#### Timing
Workshop Day 5

#### Content and Format
- 10-15 minute team presentation
- Architecture overview
- Interface/API specification
- UI workflow (if applicable)
- Test specification (if available)

#### Points
- **25 points**

## Milestone 3: Final Presentation and Source Code Review

#### Timing
Exam day (Workshop Day 6 / exam slot)

#### Content and Format
- ~45 minute presentation including 10-15 minutes Q&A (**50 points**)
- Every team member must present a meaningful part
- Source code must be available in a shared repository
- Every team member must have contributed to the codebase

#### Points
- Presentation: **50 points**
- Source code quality and contribution: **20 points**

## Total Grading Overview
- Milestone 1: 5 points
- Milestone 2: 25 points
- Milestone 3 presentation: 50 points
- Milestone 3 source code: 20 points
- **Total: 100 points**

## Tips and Examples
- [Example final presentation 1](Example%20Presentation1.pdf)
- [Example final presentation 2](Example%20Presentation2.pdf)

## Important Notes

> [!IMPORTANT]
> - Attendance is **mandatory** for all teams from 8:30 until the end of the exam session
> - Please be on time — late arrival may result in point deductions
> - Have your project ready to demo (Docker running, database seeded, etc.)
> - Prepare a brief (5 min) presentation covering your architecture decisions

## Submission

- Push your complete project to a repository
- Share the repository link with the instructor before the exam date
- Include a `README.md` in your project with setup instructions and API documentation
