import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from "@microsoft/signalr";

export type ReceiveMessagePayload = {
  id: string;
  groupId: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
  readers: Array<{ userId: string; userName: string }>;
};

export type SendMessageRequest = {
  groupId: string;
  content: string;
};

type SignalRClientOptions = {
  baseUrl?: string;
  getAccessToken?: () => string | undefined;
  onMessage?: (payload: ReceiveMessagePayload) => void;
  log?: (msg: string, data?: unknown) => void;
};

export const defaultHubUrl =
  import.meta.env.VITE_CHAT_HUB_URL ?? "http://localhost:5079/hubs/chat";

export function createSignalRClient(opts: SignalRClientOptions = {}) {
  const {
    baseUrl = defaultHubUrl,
    getAccessToken,
    onMessage,
    log = () => undefined,
  } = opts;

  let connection: HubConnection | null = null;
  let startPromise: Promise<void> | null = null;

  const ensureConnection = async () => {
    if (!connection) {
      connection = new HubConnectionBuilder()
        .withUrl(baseUrl, {
          accessTokenFactory: () => getAccessToken?.() ?? "",
          withCredentials: true,
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      connection.onreconnecting((err) => log("SignalR reconnecting", err));
      connection.onreconnected((id) => log("SignalR reconnected", id));
      connection.onclose((err) => log("SignalR closed", err));

      connection.on("ReceiveMessage", (payload: ReceiveMessagePayload) => {
        onMessage?.(payload);
      });
    }

    if (
      connection.state === HubConnectionState.Connecting ||
      connection.state === HubConnectionState.Reconnecting
    ) {
      return startPromise ?? Promise.resolve();
    }

    if (connection.state === HubConnectionState.Disconnected) {
      startPromise = connection.start().catch((err) => {
        startPromise = null;
        throw err;
      });
      await startPromise;
      startPromise = null;
    }
  };

  const connect = async () => {
    await ensureConnection();
    log("SignalR connected");
  };

  const disconnect = async () => {
    if (connection) {
      await connection.stop();
      log("SignalR stopped");
    }
  };

  const joinConversation = async (conversationId: string) => {
    await ensureConnection();
    await connection!.invoke("JoinConversation", conversationId);
  };

  const sendMessage = async (payload: SendMessageRequest) => {
    await ensureConnection();
    await connection!.invoke("SendMessage", {
      groupId: payload.groupId,
      content: payload.content,
    });
  };

  return {
    connect,
    disconnect,
    joinConversation,
    sendMessage,
    getConnection: () => connection,
  };
}