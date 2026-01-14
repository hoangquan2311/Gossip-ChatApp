import api from "@src/services/api";
import type { ReceiveMessagePayload } from "@src/services/signalRService";
import type { GroupDto } from "@src/services/serverDtos";
import type { IConversation, IMessage, IUser } from "@src/types";

export type MessageResponse = {
  id: string;
  groupId: string;
  senderId: string;
  senderName: string;
  content: string;
  sentAt: string;
};

export const formatTimestamp = (value: string) => {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    return value;
  }
  return date.toLocaleString();
};

const mapMemberToUser = (member: GroupDto["members"][number]): IUser => ({
  userId: member.userId,
  displayName: member.displayName,
  email: member.email,
  avatarUrl: member.avatarUrl,
});

const mapMessageDto = (message: MessageResponse): IMessage => ({
  id: message.id,
  content: message.content,
  sentAt: formatTimestamp(message.sentAt),
  sender: {
    userId: message.senderId,
    displayName: message.senderName,
    email: "",
    avatarUrl: undefined,
  },
});

export const groupDtoToConversation = (group: GroupDto): IConversation => ({
  groupId: group.id,
  title: group.title,
  avatarUrl: group.avatarUrl,
  messages: [],
  // unread: 0,
  members: group.members.map(mapMemberToUser),
});

export const fetchGroups = async (): Promise<IConversation[]> => {
  const { data } = await api.get<GroupDto[]>("/api/groups");
  console.log("Fetched groups:", data);
  return data.map(groupDtoToConversation);
};

export const fetchGroupMessages = async (
  groupId: string,
): Promise<IMessage[]> => {
  const { data } = await api.get<MessageResponse[]>(
    `/api/groups/${groupId}/messages`,
  );
  console.log(`Fetched messages for group ${groupId}:`, data);
  return data.map(mapMessageDto);
};

export const joinGroupConversations = async (
  groupIds: string[],
  joiner: (groupId: string) => Promise<void>,
) => {
  for (const groupId of groupIds) {
    await joiner(groupId);
  }
};

export const mapRealtimeMessage = (
  payload: ReceiveMessagePayload,
): IMessage => ({
  id: payload.id,
  content: payload.content,
  sentAt: formatTimestamp(payload.sentAt),
  sender: {
    userId: payload.senderId,
    displayName: payload.senderName,
    email: "",
    avatarUrl: undefined,
  },
});
