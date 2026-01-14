<script setup lang="ts">
import type { Ref } from "vue";

import useStore from "../../../../store/store";
import { computed, provide, ref } from "vue";

import { getActiveConversationId } from "@src/utils";

import NoChatSelected from "@src/components/states/empty-states/NoChatSelected.vue";
import Spinner from "@src/components/states/loading-states/Spinner.vue";
import ChatBottom from "@src/components/views/HomeView/Chat/ChatBottom/ChatBottom.vue";
import ChatMiddle from "@src/components/views/HomeView/Chat/ChatMiddle/ChatMiddle.vue";
import ChatTop from "@src/components/views/HomeView/Chat/ChatTop/ChatTop.vue";

const store = useStore();

// search the selected conversation using activeConversationId.
const activeConversation = computed(() => {
  return store.conversations.find(
    (conversation) => conversation.groupId === getActiveConversationId(),
  );
});

// provide the active conversation to all children.
provide("activeConversation", activeConversation.value);

</script>

<template>
  <Spinner v-if="store.status === 'loading' || store.delayLoading" />

  <div
    v-else-if="getActiveConversationId() && activeConversation"
    class="h-full flex flex-col scrollbar-hidden"
  >
    <ChatTop/>
    <ChatMiddle/>
    <ChatBottom />
  </div>

  <NoChatSelected v-else />
</template>
