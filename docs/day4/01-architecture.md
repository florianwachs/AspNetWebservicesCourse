# Architecture Patterns

The detailed architecture material lives in the split reference set under [`../../extras/architecture/`](../../extras/architecture/).
Links below are relative to this file. The labs themselves live under `/labs/` at the repository root.

The most important message for this day is simple:

> **It depends**. No really. Don`t apply patterns and concepts just for the patterns sake. Every pattern has pro and cons. The art is to know about them and especially when they are helpful...and when not.


## What you should take away

You are **not** expected to apply every architecture pattern in the folder.

The practical goal is smaller:

- notice when a layered API is starting to hurt,
- refactor toward a cleaner feature-oriented structure,
- and learn the next step that makes change easier without adding unnecessary ceremony.

For most course projects, that next step is **Vertical Slice Architecture with MediatR**. Onion is the follow-up move when the domain rules themselves need stronger protection.

## Start here

- [Architecture overview](../../extras/architecture/00-overview.md)
- [Architecture styles](../../extras/architecture/01-architecture-styles.md)
- [MediatR, CQRS, and pipeline behaviors](../../extras/architecture/02-mediatr-cqrs-and-pipeline-behaviors.md)

## Reading order by question

| If you are asking... | Read |
| --- | --- |
| "Why should I care about architecture at all?" | [Overview](../../extras/architecture/00-overview.md) |
| "When should I stay layered, move to VSA, or add Onion?" | [Architecture styles](../../extras/architecture/01-architecture-styles.md) |
| "How do I implement thin endpoints and request handlers in practice?" | [MediatR, CQRS, and pipeline behaviors](../../extras/architecture/02-mediatr-cqrs-and-pipeline-behaviors.md) |
| "Do I need repositories everywhere?" | [Repository pattern](../../extras/architecture/03-repository-pattern.md) |
| "What are the advanced patterns and when are they justified?" | [Event sourcing](../../extras/architecture/04-event-sourcing.md), [Actor model](../../extras/architecture/05-actor-model.md), and [Companion patterns](../../extras/architecture/06-companion-patterns-and-anti-patterns.md) |

## Recommended reading order

1. Read the [overview](../../extras/architecture/00-overview.md) for the decision context.
2. Read [architecture styles](../../extras/architecture/01-architecture-styles.md) to compare layered architecture, VSA, and Onion.
3. Read [MediatR, CQRS, and pipeline behaviors](../../extras/architecture/02-mediatr-cqrs-and-pipeline-behaviors.md) for the practical request/handler workflow used in the dedicated MediatR follow-up lab and treat pipeline behaviors as the next step after that baseline.
4. Use the remaining pages in `extras/architecture/` as deeper reference material:
   - [Repository pattern](../../extras/architecture/03-repository-pattern.md)
   - [Event sourcing](../../extras/architecture/04-event-sourcing.md)
   - [Actor model](../../extras/architecture/05-actor-model.md)
   - [Companion patterns and anti-patterns](../../extras/architecture/06-companion-patterns-and-anti-patterns.md)

## Where this shows up in the course

- [WorkshopPlanner VSA lab](../../labs/lab-architecture-vertical-slices/) focuses on feature-first organization.
- [WorkshopPlanner MediatR/CQRS lab](../../labs/lab-mediatr-cqrs/) focuses on thin endpoints, request handlers, validators, and pipeline behaviors inside a small Aspire + React app.
- [WorkshopPlanner Onion lab](../../labs/lab-architecture-onion/) focuses on protecting a richer domain core.
- [Akka.NET seat-reservation lab](../../labs/lab-akkanet/) applies the actor-model branch with a focused Akka.Hosting + Aspire example.

## Takeaway

For most services in this course:

1. start from a simple baseline,
2. move to **Vertical Slice Architecture** when change cost rises,
3. use **MediatR** and **logical CQRS** to structure commands and queries,
4. add validators early, and treat **pipeline behaviors** for validation/logging as an optional follow-up once the request/handler split is already helping,
5. add **Onion** when the domain becomes rich enough to justify stronger boundaries around the core,
6. remember that **Onion complements VSA** instead of replacing it,
7. and only reach for heavier tools such as repositories, event sourcing, or actors when the problem actually needs them.
