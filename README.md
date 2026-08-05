
# PAS2 — Proof of Concept: Vertical Slice Architecture / CQRS / DDD

### Summary
This repository is a small proof of concept (POC) demonstrating how to apply several architectural principles and patterns together: Vertical Slice Architecture (VSA), CQRS, SOLID and Domain-Driven Design (DDD). The goal is to provide a simple and reproducible structure.

Why VSA? VSA emerged to combat the high **maintenance overhead and rigid abstractions of Clean Architecture**. Instead of forcing code into strict horizontal technical layers (such as controllers, services, and repositories) that scatter a single feature across multiple files and cause accidental coupling between unrelated use cases, VSA cuts through the stack vertically. By grouping everything needed for a specific business feature into a single, self-contained slice, it eliminates boilerplate code, minimizes navigation fatigue, and ensures that changes to one feature cannot unexpectedly break another.

### Principles and patterns implemented
- Vertical Slice Architecture (VSA)
  - Unlike a layered Clean Architecture approach where code is separated by technical layers (Application, Infrastructure, Domain), this solution groups code by feature (slice). 
  - Each slice owns everything required to implement a single use case or endpoint.

- CQRS
  - Commands (mutations) and Queries (reads) are separated. Handlers live under `src/*/Application/Commands` and `src/*/Application/Queries`.
  - HTTP endpoints are thin and send Commands/Queries to the message bus (Wolverine) instead of embedding business logic.

- Domain-Driven Design (DDD)
  - Aggregates (e.g., Fund, Currency) and Value Objects (e.g., Isin) encapsulate business rules and invariants.
  - Domain events are used (e.g., FundNavUpdatedDomainEvent) and collected from entities via Entity.DomainEvents.

- SOLID
  - Single Responsibility: handlers, repositories, entities and endpoints have distinct responsibilities.
  - Dependency Inversion: repository interfaces are defined in the domain and implemented in Infrastructure; the implementations are injected via DI at composition time.

- Wolverine (messaging + transactional integration)
  - Wolverine is configured (see `src/Common/Api/WolverineExtensions.cs`) to:
	- use SQL Server for durable transport and for the transactional outbox/inbox;
	- integrate EF Core and automatically apply transactions to command handlers;
	- publish DomainEvents extracted from EF-tracked entities;
	- enable durable outbox/inbox to ensure message delivery.
  - Practical consequence: handlers add/modify entities through repositories but should not call `SaveChanges` directly — Wolverine applies the commit (SaveChangesAsync) within its pipeline.

---

# Historique : comment est née la Vertical Slice Architecture (VSA) ?
La VSA a été créée pour résoudre les rigidités et le coût de maintenance de la Clean Architecture (et de l'architecture en couches traditionnelle) sur les projets en constante évolution.

### Faiblesses de la Clean Architecture qui ont mené à l'apparition de la VSA
1. **La dispersion du code (la "taxe de navigation")**
    - Clean Architecture : le code est découpé par couches techniques. Pour modifier ou ajouter une seule fonctionnalité, vous devez ouvrir et modifier entre 4 et 8 fichiers répartis dans des dossiers ou projets différents.
    - Conséquence : une perte de temps considérable en navigation et une surcharge cognitive pour le développeur.
2. **Le couplage horizontal excessif**
    - Clean Architecture : elle pousse à maximiser la réutilisation du code. Les services d'une même couche partagent souvent les mêmes objets ou méthodes.
    - Conséquence : modifier une règle métier pour la fonctionnalité "A" risque de casser involontairement la fonctionnalité "B" qui partage le même code. Les effets de bord sont fréquents.
3. **L'abstraction prématurée et inutile**
    - Clean Architecture : elle impose des règles strictes d'isolation (utilisation systématique d'interfaces, d'abstractions, et de patterns comme les DTO de mapping entre chaque couche).
    - Conséquence : pour les fonctionnalités simples, cette structure est lourde, inutile et crée du code "passe-plat" (boilerplate code) sans valeur ajoutée.
4. **L'alignement manqué avec la livraison de valeur**
    - Clean Architecture : elle organise le code selon la technique, pas selon le métier.
    - Conséquence : les équipes techniques travaillent souvent par "briques" (faire la base de données, puis l'API), ce qui retarde la livraison d'une fonctionnalité complète et testable de bout en bout par les utilisateurs.

### La réponse de la VSA
Face à ces limites, la VSA change radicalement de paradigme :
- **Organisation par fonctionnalité** : au lieu de découper horizontalement par couches, on découpe verticalement par cas d'utilisation (ex: EnregistrerUtilisateur, GenererFacture).
- **Autonomie** : chaque "tranche" (slice) contient tout ce dont elle a besoin pour fonctionner (L'API, la logique métier, et l'accès aux données).
- **Principe de cohésion** : les choses qui changent ensemble doivent être verrouillées ensemble. Si une fonctionnalité est supprimée ou modifiée, seule sa tranche est impactée.
