<script setup lang="ts">
import type {
  IConversation,
  IMessage,
} from "@src/types";

import linkifyStr from "linkify-string";
import { inject } from "vue";

import Receipt from "@src/components/views/HomeView/Chat/ChatMiddle/Message/Receipt.vue";

const props = defineProps<{
  message: IMessage;
  followUp: boolean;
  self: boolean;
  divider?: boolean;
}>();

const activeConversation = <IConversation>inject("activeConversation");

// decide whether to show or hide avatar next to the image.
const hideAvatar = () => {
  if (props.divider && !props.self) {
    return false;
  } else {
    if (props.followUp) {
      return true;
    }
    if (props.self) {
      return true;
    }
  }
};

</script>

<template>
  <div class="select-none">
    <div class="xs:mb-6 md:mb-5 flex" :class="{ 'justify-end': props.self }">
      <!--avatar-->
      <div class="mr-4" :class="{ 'ml-[2.25rem]': props.followUp && !divider }">
        <div
          v-if="!hideAvatar()"
          :aria-label="props.message.sender.avatarUrl"
          class="outline-none"
        >
          <div
            :style="{ backgroundImage: `url(${props.message.sender.avatarUrl})` }"
            class="w-[2.25rem] h-[2.25rem] bg-cover bg-center rounded-full"
          ></div>
        </div>
      </div>

      <div class="flex items-end">
        <!--bubble-->
        <div
          @contextmenu.prevent
          class="group max-w-125 p-5 rounded-b-xl transition duration-500"
        >
          <!--reply to-->
          <!-- <MessagePreview
            v-if="replyMessage"
            :message="replyMessage"
            :self="props.self"
            class="mb-5 px-3"
          /> -->

          <!--content-->
          <!-- Convert url into clickable links with sliced text -->
          <p
            v-if="props.message.content && props.message.type !== 'recording'"
            class="body-2 outline-none text-black opacity-60 dark:text-white dark:opacity-70"
            v-html="
              linkifyStr(props.message.content as string, {
                className: props.self
                  ? 'text-black opacity-50'
                  : 'text-indigo-500 dark:text-indigo-300',
                format: {
                  url: (value) =>
                    value.length > 50 ? value.slice(0, 50) + `…` : value,
                },
              })
            "
            tabindex="0"
          ></p>
        </div>

        <!--date-->
        <div :class="props.self ? ['ml-4', 'order-1'] : ['mr-4']">
          <p class="body-1 text-black/70 dark:text-white/70 whitespace-pre">
            {{ props.message.sentAt }}
          </p>
        </div>

        <!--read receipt-->
        <Receipt v-if="props.self" :state="props.message.sentAt" />
      </div>
    </div>
    
    <!-- <MessageContextMenu
      :selected="props.selected"
      :message="props.message"
      :show="showContextMenu"
      :left="contextMenuCoordinations.x"
      :top="contextMenuCoordinations.y"
      :handle-close-context-menu="handleCloseContextMenu"
      :handle-select-message="handleSelectMessage"
      :handle-deselect-message="handleDeselectMessage"
    /> -->
  </div>
</template>
