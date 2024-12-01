# CQRS Proof of Concept

A simple todo application using CQRS concepts:

```mermaid
flowchart TD;
    User-->UI;
    UI-->BFF;
    BFF-- Sends Commands --->MessageQueue;
    BFF-- Queries --->QueryApi;
    QueryApi-->Cache[(Cache)]
    MessageQueue-- handles commands --->Handlers
    Handlers-- publish events --->MessageQueue
    MessageQueue-- handles events --->CacheUpdater
    CacheUpdater-- updates cache with denormalised view data --->Cache
    Handlers-- Persists normalised data --->Database[(Database)]
```