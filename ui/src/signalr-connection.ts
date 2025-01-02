import * as signalR from "@microsoft/signalr";

const URL = import.meta.env.VITE_HUB_URL ?? "http://localhost:8086/hub";

class Connector {
  private connection: signalR.HubConnection;

  public events: (onMessageReceived: (message: string) => void) => void;

  static instance: Connector;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(URL)
      .withAutomaticReconnect()
      .build();
    this.connection.start().catch((err) => document.write(err));
    this.events = (onMessageReceived) => {
      this.connection.on("CacheUpdated", (message) => {
        onMessageReceived(message);
      });
    };
  }

  public static getInstance(): Connector {
    if (!Connector.instance) Connector.instance = new Connector();
    return Connector.instance;
  }
}

export default Connector.getInstance;
