# CQRS Showcase

![CQRS Arquitecture image](images/cqrs_show_case_architecture_diagram.svg)

This project is a demonstration of the Command and Query Responsibility Segregation (CQRS) pattern using .NET 6. The showcase application uses Azure Cosmos DB Emulator as the read database and MongoDB as the write database. To ensure eventual consistency between the read and write databases, Kafka is utilized as an event-driven synchronization mechanism. By integrating Kafka into the architecture, the system can effectively propagate updates and synchronize data across the databases in an asynchronous and scalable manner.
The project includes sample code for implementing CQRS in a .NET application, along with instructions for setting up the required databases and deploying the application. This showcase can serve as a starting point for developers who are interested in implementing the CQRS pattern in their .NET applications.

In this showcase project, I will design and implement a social media post microservice using the CQRS pattern to handle all post-related functions, such as creating, editing, and displaying posts. By using CQRS, we can optimize the performance of the microservice and achieve scalability, flexibility, and maintainability in our application architecture.

## Table of Contents

- [CQRS Showcase](#cqrs-showcase)
  - [Table of Contents](#table-of-contents)
  - [Installation](#installation)
    - [Set Azure Cosmos DB Emulator Primary Key as Environment Variable](#set-azure-cosmos-db-emulator-primary-key-as-environment-variable)
    - [Set Kafka Topic Name as Environment Variable](#set-kafka-topic-name-as-environment-variable)
    - [Run Apache Kafka with Docker Compose](#run-apache-kafka-with-docker-compose)
  - [Usage](#usage)
  - [Domain-Driven Design](#domain-driven-design)
    - [Bounded Context](#bounded-context)
  - [Azure Cosmos DB](#azure-cosmos-db)
  - [Synchronization](#synchronization)
  - [Troubleshooting](#troubleshooting)
  - [External documentation links](#external-documentation-links)

## Installation

Instructions for installing the project.

### Set Azure Cosmos DB Emulator Primary Key as Environment Variable

Open a powershell terminal in as Administrator and run the following command:
```ps1
setx CosmosDbSettings__PrimaryKey "your primary key"
```

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


## Usage

Instructions for using the project.

## Domain-Driven Design

### Bounded Context
In DDD, a bounded context defines a specific problem area within a domain and may correspond to a separate microservice. For example, in the context of a social media application, a bounded context could be defined for post-related functions. This bounded context could then be implemented as a social media post microservice, which would handle all the functions related to creating, managing, and displaying posts.

## Azure Cosmos DB

## Synchronization

I created this topic on the discussion tab [Updating Read Databases in a CQRS Architecture](https://github.com/reinaldogez/cqrs-showcase/discussions/1), where the main options are deeply discussed.

## Troubleshooting

- **Error: "message error"** - If you're seeing this error message, you need to bla bla

## External documentation links
[Use the Azure Cosmos DB Emulator on Docker for Linux](https://learn.microsoft.com/en-us/azure/cosmos-db/docker-emulator-linux)
