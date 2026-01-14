<script setup lang="ts">
import { onUnmounted, provide, ref, watch } from "vue";

import useStore from "../../../store/store";

import FadeTransition from "@src/components/ui/transitions/FadeTransition.vue";
import Navigation from "@src/components/views/HomeView/Navigation/Navigation.vue";
import Sidebar from "@src/components/views/HomeView/Sidebar/Sidebar.vue";
import { getActiveConversationId } from "@src/utils";
import {
  fetchGroupMessages,
  fetchGroups,
  groupDtoToConversation,
  joinGroupConversations,
  mapRealtimeMessage,
} from "@src/services/conversationService";
import {
  createSignalRClient,
  type ReceiveMessagePayload,
  type CreateGroupRequest,
} from "@src/services/signalRService";
import { signalRCreateGroupKey } from "@src/services/signalRContext";
import { useRouter } from "vue-router";

const store = useStore();

const signalRClient = ref<ReturnType<typeof createSignalRClient> | null>(null);
const isInitializing = ref(false);
const hasInitialized = ref(false);
const router = useRouter();

const handleRealtimeMessage = (payload: ReceiveMessagePayload) => {
  const message = mapRealtimeMessage(payload);
  store.addMessageToConversation(payload.groupId, message);
};

const createGroupHandler: CreateGroupHandler = async (payload: CreateGroupRequest) => {
  const client = signalRClient.value;
  if (!client) {
    throw new Error("SignalR client is not ready");
  }

  const groupDto = await client.createGroup(payload);
  const conversation = groupDtoToConversation(groupDto);
  store.addConversation(conversation);
  store.setConversationMessages(conversation.groupId, []);
  await client.joinConversation(conversation.groupId);
  void router.push({ path: `/chat/${conversation.groupId}/` });
  return conversation;
};

provide(signalRCreateGroupKey, createGroupHandler);

const stopConnection = async () => {
  hasInitialized.value = false;
  if (signalRClient.value) {
    try {
      await signalRClient.value.disconnect();
    } catch (error) {
      console.error("SignalR disconnect error", error);
    }
    signalRClient.value = null;
  }
  store.setConversations([]);
  store.status = "idle";
};

const startConnection = async () => {
  if (!store.token || isInitializing.value || hasInitialized.value) {
    return;
  }

  isInitializing.value = true;
  store.status = "loading";

  const client = createSignalRClient({
    getAccessToken: () => store.token ?? "",
    onMessage: handleRealtimeMessage,
    log: (msg, data) => console.debug(msg, data),
  });

  signalRClient.value = client;

  try {
    await client.connect();

    const groups = await fetchGroups();
    store.setConversations(groups);

    await joinGroupConversations(
      groups.map((group) => group.groupId),
      (groupId) => client.joinConversation(groupId)
    );
    
    await Promise.all(
      groups.map(async (group) => {
        const messages = await fetchGroupMessages(group.groupId);
        store.setConversationMessages(group.groupId, messages);
      })
    );

    store.status = "success";
    hasInitialized.value = true;
  } catch (error) {
    console.error("Failed to initialize chat", error);
    store.status = "error";
  } finally {
    isInitializing.value = false;
  }
};

watch(
  () => store.token,
  (token) => {
    if (token) {
      void startConnection();
    } else {
      void stopConnection();
    }
  },
  { immediate: true }
);

onUnmounted(() => {
  void stopConnection();
});
</script>

<template>
  <KeepAlive>
    <div
      class="xs:relative md:static h-full flex xs:flex-col md:flex-row overflow-hidden"
    >
      <!--navigation-bar-->
      <Navigation class="xs:order-1 md:order-none" />
      <!--sidebar-->
      <Sidebar
        class="xs:grow-1 md:grow-0 xs:overflow-y-scroll md:overflow-visible scrollbar-hidden"
      />
      <!--chat-->
      <div
        id="mainContent"
        class="xs:absolute xs:z-10 md:static grow h-full xs:w-full md:w-fit scrollbar-hidden bg-white dark:bg-gray-800 transition-all duration-500"
        :class="
          getActiveConversationId()
            ? ['xs:-left-[0rem]', 'xs:static']
            : ['xs:left-250']
        "
        role="region"
      >
        <router-view v-slot="{ Component }">
          <FadeTransition name="fade" mode="out-in">
            <component :is="Component" :key="getActiveConversationId()" />
          </FadeTransition>
        </router-view>
      </div>
    </div>
  </KeepAlive>
</template>
