<script setup lang="ts">
import type { IConversation} from "@src/types";
import type { Ref } from "vue";
import { computed, ref } from "vue";

import useStore from "../../../../../store/store";
import {
  getActiveConversationId,
  getConversationIndex,
  shorten,
} from "@src/utils";
import router from "@src/router";

const props = defineProps<{
  conversation: IConversation;
}>();

const store = useStore();


// (event) select this conversation.
const handleSelectConversation = () => {
  router.push({ path: `/chat/${props.conversation.groupId}/` });
};

// last message in conversation to display
const lastMessage = computed(
  () => props.conversation.messages[props.conversation.messages.length - 1],
);

// (event) remove the unread indicator when opening the conversation
const handleRemoveUnread = () => {
  let index = getConversationIndex(props.conversation.groupId);
  if (index !== undefined) {
    store.conversations[index].unread = 0;
  }
};

// (computed property) determines if this conversation is active.
const isActive = computed(
  () => getActiveConversationId() === props.conversation.groupId,
);
</script>

<template>
  <div class="select-none">
    <button
      :aria-label="'conversation with' + props.conversation.title"
      tabindex="0"
      @contextmenu.prevent=""
      @click="
        () => {
          handleSelectConversation();
          handleRemoveUnread();
        }
      "
      class="w-full h-23 px-5 py-6 mb-3 flex rounded focus:bg-indigo-50 dark:active:bg-gray-600 dark:focus:bg-gray-600 dark:hover:bg-gray-600 hover:bg-indigo-50 active:bg-indigo-100 focus:outline-none transition duration-500 ease-out"
      :class="{
        'md:bg-indigo-50': isActive,
        'md:dark:bg-gray-600': isActive,
      }"
    >
      <!--profile image-->
      <div class="mr-4">
        <div
          :style="{ backgroundImage: `url(${props.conversation.avatarUrl})` }"
          class="w-7 h-7 rounded-full bg-cover bg-center"
        ></div>
      </div>

      <div class="w-full flex flex-col">
        <div class="w-full">
          <!--conversation name-->
          <div class="flex items-start">
            <div class="grow mb-3 text-start">
              <p class="heading-2 text-black/70 dark:text-white/70">
                {{ props.conversation.title }}
              </p>
            </div>

            <!--last message date-->
            <p class="body-1 text-black/70 dark:text-white/70">
              {{ lastMessage?.sentAt }}
            </p>
          </div>
        </div>

        <div class="flex justify-between">
          <div>
            <!--last message content -->
            <p
              class="body-2 text-black/70 dark:text-white/70 flex justify-start items-center"
              :class="{ 'text-indigo-400': props.conversation.unread }"
            >
              <span :class="{ 'text-indigo-400': props.conversation.unread }">
                {{ shorten(lastMessage) }}
              </span>
            </p>
          </div>

          <div v-if="props.conversation.unread">
            <div
              class="w-4.5 h-4.5 flex justify-center items-center rounded-[50%] bg-indigo-300"
            >
              <p class="body-1 text-white">
                {{ props.conversation.unread }}
              </p>
            </div>
          </div>
        </div>
      </div>
    </button>
  </div>
</template>
