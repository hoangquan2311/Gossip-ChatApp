import { defineStore } from "pinia";
import type { Ref } from "vue";
import { computed, ref } from "vue";

import type {
  IConversation,
  IUser,
  IMessage,
} from "@src/types";

const useStore = defineStore("chat", () => {
  // local storage
  const storage = JSON.parse(localStorage.getItem("chat") || "{}");

  // app status refs
  const status = ref("idle");
  const darkMode = ref(storage.darkMode || false);

  // auth refs
  const token: Ref<string | null> = ref(storage.token || null);
  const isAuthenticated = computed(() => !!token.value);

  // data refs
  const user: Ref<IUser | null> = ref(null);
  const conversations: Ref<IConversation[]> = ref([]);
  // ui refs
  const activeSidebarComponent: Ref<string> = ref(
    storage.activeSidebarComponent || "messages"
  );
  const delayLoading = ref(false);
  const conversationOpen: Ref<string | undefined> = ref(
    storage.conversationOpen
  );

  const getStatus = computed(() => status);

  const setToken = (newToken: string | null) => {
    token.value = newToken;
  };

  const clearTokens = () => {
    token.value = null;
    user.value = null;
    conversations.value = [];
  };

  const setUser = (newUser: IUser | null) => {
    user.value = newUser;
  };

  const setConversations = (items: IConversation[]) => {
    conversations.value = items;
  };

  const addConversation = (conversation: IConversation) => {
    const existingIndex = conversations.value.findIndex(
      (item) => item.groupId === conversation.groupId,
    );
    if (existingIndex !== -1) {
      conversations.value[existingIndex] = conversation;
    } else {
      conversations.value = [conversation, ...conversations.value];
    }
  };

  const addMessageToConversation = (groupId: string, message: IMessage) => {
    const conversation = conversations.value.find(
      (item) => item.groupId === groupId
    );
    if (conversation) {
      conversation.messages.push(message);
    }
  };

  const setConversationMessages = (
    groupId: string,
    messages: IMessage[]
  ) => {
    const conversation = conversations.value.find(
      (item) => item.groupId === groupId
    );
    if (conversation) {
      conversation.messages = messages;
    }
  };

  return {
    // status refs
    status,
    getStatus,
    darkMode,

    // auth refs
    token,
    isAuthenticated,

    // data refs
    user,
    conversations,

    // ui refs
    activeSidebarComponent,
    delayLoading,
    conversationOpen,

    // auth methods
    clearTokens,
    setUser,
    setToken,
    setConversations,
    setConversationMessages,
    addMessageToConversation,
    addConversation,
  };
});

export default useStore;
