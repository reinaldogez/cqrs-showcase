# CQRS Showcase

![CQRS Arquitecture image](images/cqrs_show_case_architecture_diagram.svg)

This project is a demonstration of the Command and Query Responsibility Segregation (CQRS) pattern using .NET 6. The showcase application uses MS SQL Server as the read database and MongoDB as the write database. To ensure eventual consistency between the read and write databases, Kafka is utilized as an event-driven synchronization mechanism. By integrating Kafka into the architecture, the system can effectively propagate updates and synchronize data across the databases in an asynchronous and scalable manner.
The project includes sample code for implementing CQRS in a .NET application, along with instructions for setting up the required databases and deploying the application. This showcase can serve as a starting point for developers who are interested in implementing the CQRS pattern in their .NET applications.

In this showcase project, I will design and implement a social media post microservice using the CQRS pattern to handle all post-related functions, such as creating, editing, and displaying posts. By using CQRS, we can optimize the performance of the microservice and achieve scalability, flexibility, and maintainability in our application architecture.

## Table of Contents

- [CQRS Showcase](#cqrs-showcase)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
    - [Set Kafka Topic Name as Environment Variable](#set-kafka-topic-name-as-environment-variable)
    - [Run Apache Kafka with Docker Compose](#run-apache-kafka-with-docker-compose)
    - [Set SQL Server Password as Environment Variable](#set-sql-server-password-as-environment-variable)
    - [Creating .env File for Docker Compose](#creating-env-file-for-docker-compose)
  - [Usage](#usage)
  - [Domain-Driven Design](#domain-driven-design)
    - [Bounded Context](#bounded-context)
      - [Key Concepts](#key-concepts)
        - [1. Logical Boundaries](#1-logical-boundaries)
        - [2. Consistency and Integrity](#2-consistency-and-integrity)
        - [3. Autonomy and Modular Development](#3-autonomy-and-modular-development)
        - [4. Ubiquitous Language](#4-ubiquitous-language)
    - [Aggregate and Aggregate Root](#aggregate-and-aggregate-root)
      - [Aggregate](#aggregate)
      - [Aggregate Root](#aggregate-root)
      - [Example Usage in Project](#example-usage-in-project)
      - [Simplified Aggregate Structure](#simplified-aggregate-structure)
        - [Key Characteristics of `PostAggregate` as an Aggregate Root](#key-characteristics-of-postaggregate-as-an-aggregate-root)
          - [Singular Aggregate Composition](#singular-aggregate-composition)
          - [Role of `PostAggregate` in the Domain Model](#role-of-postaggregate-in-the-domain-model)
          - [Lack of Additional Entities](#lack-of-additional-entities)
  - [Event Sourcing](#event-sourcing)
    - [Why Event Sourcing?](#why-event-sourcing)
    - [Event Store](#event-store)
      - [Key Characteristics of an Event Store:](#key-characteristics-of-an-event-store)
      - [Optimistic Concurrency Control](#optimistic-concurrency-control)
  - [Synchronization](#synchronization)
  - [Troubleshooting](#troubleshooting)

## Installation

Instructions for installing the project.

### Set Kafka Topic Name as Environment Variable

Open a powershell terminal in as Administrator and run the following command:
```ps1
setx KAFKA_TOPIC "SocialMediaSyncQueueEvents"
```
### Run Apache Kafka with Docker Compose

Open a powershell terminal in as Administrator and run the following command:
```ps1
docker-compose -f docker-compose-apache-kafka.yaml up -d
```

### Set SQL Server Password as Environment Variable

Open a powershell terminal in as Administrator and run the following command:
```ps1
setx SQLSERVER_PASSWORD "YourStrongPasswordHere"
```

### Creating .env File for Docker Compose

For Docker Compose to up the SQL Server with the necessary environment variables, create a .env file in the root directory of your project. This file should contain the SQL Server password that the Docker container will use:

```
# .env file
SA_PASSWORD=YourStrongPasswordHere
```

Replace YourStrongPasswordHere with your actual SQL Server password. This file will be read by Docker Compose, as specified in your docker-compose.yml, to set the password for SQL Server within the container.

## Usage

Instructions for using the project.

## Domain-Driven Design

### Bounded Context
In DDD, a bounded context defines a specific problem area within a domain and may correspond to a separate microservice. For example, in the context of a social media application, a bounded context could be defined for post-related functions. This bounded context could then be implemented as a social media post microservice, which would handle all the functions related to creating, managing, and displaying posts.

#### Key Concepts

##### 1. Logical Boundaries
This Bounded Context specifically encapsulates all operations related to social media posts, ensuring all functionalities are cohesively organized and independent from other features like user management.

##### 2. Consistency and Integrity
By using the Command Query Responsibility Segregation (CQRS) pattern, this context maintains a consistent model by separating read and write operations, which enhances data integrity and system performance.

##### 3. Autonomy and Modular Development
The social media post microservice can be independently developed, deployed, and scaled, allowing teams to work autonomously and focus solely on improving post-related functionalities without cross-interference.

##### 4. Ubiquitous Language
Within this Bounded Context, all stakeholders—developers, business analysts, and users—use a common language to describe features and processes related to posts. This ubiquitous language ensures that everyone understands and communicates effectively about the system functionalities, reducing misunderstandings and aligning project goals across different teams.

### Aggregate and Aggregate Root

#### Aggregate
"An Aggregate is an explicit grouping of domain objects designed to support the behaviors and invariants of a domain model while acting as a consistency and transactional boundary." This definition is provided by Scott Millett and Nick Tune in their book Patterns, Principles, and Practices of Domain-Driven Design.

#### Aggregate Root
The `AggregateRoot` is a specific entity contained within an Aggregate and acts as the gatekeeper to the Aggregate. This entity has global identity and is responsible for checking all modifications and state changes in the Aggregate. The `AggregateRoot` ensures that the Aggregate remains in a valid state throughout its lifecycle. In the project code, `AggregateRoot` is an abstract class that contains a list of changes (`_changes`) which track events that have not yet been committed to the store, demonstrating how changes within the Aggregate are managed and applied.

#### Example Usage in Project

```csharp
public abstract class AggregateRoot
{
    protected Guid _id;
    private readonly List<BaseEvent> _changes = new();

    // ...other methods...

    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }
}

```

```csharp
public class PostAggregate : AggregateRoot
{
    // Post-specific properties and methods
    public void AddComment(string comment, string username)
    {
        // Method implementation
    }

    // Event application methods
    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }
}

```

#### Simplified Aggregate Structure

The `PostAggregate` serves both as the Aggregate Root and, effectively, as the aggregate itself due to the absence of other distinct entities within its boundary. This setup allows the `PostAggregate` to encapsulate all related functionalities and operations, such as managing comments and likes directly within its class structure.
##### Key Characteristics of `PostAggregate` as an Aggregate Root

###### Singular Aggregate Composition
It manages not only the state of a social media post but also operations related to it like comments and likes, handled directly through methods and local collections.

###### Role of `PostAggregate` in the Domain Model
t plays the dual role of being both the manager of the aggregate and the primary entity within it. This is common in simpler domain models or when a domain entity has limited relationships or complexity.

###### Lack of Additional Entities
There are no other domain entities that are part of the `PostAggregate` beyond basic data attributes and behaviors, making it effectively the aggregate in its entirety.

## Event Sourcing
"Event Sourcing ensures that all changes to application state are stored as a sequence of events. Not just can we query these events, we can also use the event log to reconstruct past states, and as a foundation to automatically adjust the state to cope with retroactive changes." Martin Fowler

### Why Event Sourcing?
Event Sourcing has gained popularity because it maintains a full history of system activities, providing a comprehensive log of all events that have led to the current state of your domain model. This approach is valuable because, if you only have the current state, you lack a way to understand the sequence of actions and decisions that brought the system to its present configuration. By storing each change as a unique event, Event Sourcing not only facilitates troubleshooting and system analysis by allowing to replay and reconstruct past states, but it also enhances auditability and can simplify the process of implementing new features or making retroactive changes to the system.

### Event Store

An **Event Store** is a specialized type of database designed specifically for the storage of events as part of an Event Sourcing architecture. It differs from traditional databases in that it captures all changes to an application's state as a series of events. These events are stored in a sequential order, creating an immutable historical record of all actions taken within the system. In this project, the **Event Store** utilizes MongoDB as the underlying write database.

#### Key Characteristics of an Event Store:

- **Immutable Log:** The Event Store functions as an immutable log where events are recorded sequentially. Once an event is recorded, it cannot be changed or deleted, which ensures the historical accuracy and integrity of the log.
  
- **Event-Driven:** The store captures the entire series of actions and events that affect the state of the application, not just the current state. Each event in the store represents a state change in the domain.

- **Reconstruct State:** By replaying the events from the Event Store, the state of the application at any point in time can be reconstructed. This capability is critical for debugging, auditing, historical analysis, and system recovery.

#### Optimistic Concurrency Control

In this project, Optimistic Concurrency Control is implemented within the `EventStore` class to manage simultaneous access to the Event Store. This method does not prevent conflicts during the execution of transactions but instead checks for conflicts at the time of saving the events. It ensures that if the state of the aggregate has changed since it was last read (which is indicated by the `expectedVersion`), the transaction will not be allowed to proceed.

- **Code Implementation:**
```csharp
    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

        if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
            throw new ConcurrencyException();
    
        // Rest of the method implementation
    }
```

- **Pros:**
  - **Performance:** Optimistic Concurrency Control typically offers better performance than pessimistic locking in scenarios with low to moderate contention, as it allows multiple processes to proceed without waiting for locks.
  - **Deadlock Avoidance:** This approach avoids deadlocks by design, as transactions don't hold locks while they are working on data and only validate data at commit time.

- **Cons:**
  - **Conflict Handling:** If conflicts occur, the transaction may need to be rolled back and restarted, which can lead to wasted resources, especially in high contention scenarios.
  - **Complexity in Transaction Management:** Managing retries and handling failures due to concurrency conflicts can increase the complexity of application logic.


## Synchronization

I created this topic on the discussion tab [Updating Read Databases in a CQRS Architecture](https://github.com/reinaldogez/cqrs-showcase/discussions/1), where the main options are deeply discussed.

## Troubleshooting

- **Error: "message error"** - Template message

