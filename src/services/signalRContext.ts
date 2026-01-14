import type { CreateGroupRequest } from "@src/services/signalRService";
import type { IConversation } from "@src/types";

export type CreateGroupHandler = (
  payload: CreateGroupRequest,
) => Promise<IConversation>;

export const signalRCreateGroupKey = Symbol("signalr-create-group");
