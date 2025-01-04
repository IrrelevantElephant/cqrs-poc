# CQRS Proof of Concept

A simple todo application using CQRS concepts:

```mermaid
flowchart TD;
    UI-->BFF;
    BFF-- writes --->WriteApi;
    WriteApi-- sends commands --->MessageQueue([MessageQueue]);
    BFF-- reads --->ReadApi;
    ReadApi-->Cache[(Cache)]
    MessageQueue-- handles CacheUpdated events --->WebSocketHub
    MessageQueue-- handles commands --->Handlers
    Handlers-- publishes PersistenceUpdated events --->MessageQueue
    MessageQueue-- handles PersistenceUpdated events --->CacheUpdater
    CacheUpdater-- updates cache with denormalised view data --->Cache
    CacheUpdater-- publishes CacheUpdated events --->MessageQueue
    Handlers-- persists normalised data --->Database[(Database)]
    WebSocketHub-- prompts UI to reload --->UI
```

## Run

To run the application in watch mode:

```shell
docker compose -f docker/docker-compose.yaml up --watch
```

Navigate to http://localhost:8000 to view the UI

Grafana for logs, traces & metrics @ http://localhost:3000
