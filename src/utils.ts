import useStore from "./store/store";
import type {
  IConversation,
  IMessage,
  IUser,
} from "@src/types";
import { useRoute } from "vue-router";


/**
 * trim a string when it reaches a certain length and adds three dots
 * at the end.
 * @param text
 * @param maxLength
 * @returns A string that is trimmed according the length provided
 */
export const shorten = (message: IMessage | string, maxLength: number = 23) => {
  let text: string | undefined;

  if (typeof message === "string") {
    text = message;
  } else {
    text = message.content;
  }

  if (text && typeof text === "string") {
    let trimmedString = text;
    if (text.length > maxLength) {
      // trim the string to the maximum length.
      trimmedString = trimmedString.slice(0, maxLength);
      // add three dots to indicate that there is more to the message.
      trimmedString += "...";
    }
    return trimmedString;
  }

  return "";
};


/**
 * get index of the conversation inside the conversations array
 * @param conversationId
 * @returns A number indicating the index of the conversation.
 */
export const getConversationIndex = (
  conversationId: string
): number | undefined => {
  let conversationIndex;
  const store = useStore();

  store.conversations.forEach((conversation, index) => {
    if (conversation.groupId === conversationId) {
      conversationIndex = index;
    }
  });

  return conversationIndex;
};

/**
 * extract the id of the active conversaiton from the url
 */
export const getActiveConversationId = () => {
  const route = useRoute();
  return route.params.id ? String(route.params.id) : undefined;
};

export const getMessageById = (
  conversation: IConversation,
  messageId?: string
) => {
  if (messageId) {
    return conversation.messages.find((message) => message.id === messageId);
  }
};

// Get first character of user's display name or null if avatarUrl exists
export const getUserNameFirstChar = (user : IUser | null) => {
  if (!user) return "";
  if(user.avatarUrl && user.avatarUrl.trim().length > 0) return "";
  const name = user.displayName;
  const nameParts = name.trim().split(' ');
  const lastName = nameParts[nameParts.length - 1];
  return lastName.charAt(0).toUpperCase();
};