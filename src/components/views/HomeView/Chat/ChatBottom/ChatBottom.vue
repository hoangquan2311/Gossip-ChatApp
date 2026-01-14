<script setup lang="ts">
import type { Ref } from "vue";
import type { IConversation } from "@src/types";

import useStore from "../../../../../store/store";
import { ref, inject, onMounted } from "vue";
import { getConversationIndex } from "@src/utils";

import {
  PaperAirplaneIcon,
} from "@heroicons/vue/24/outline";
import IconButton from "@src/components/ui/inputs/IconButton.vue";
import ReplyMessage from "@src/components/views/HomeView/Chat/ChatBottom/ReplyMessage.vue";
import Textarea from "@src/components/ui/inputs/Textarea.vue";

const store = useStore();

const activeConversation = <IConversation>inject("activeConversation");

// the content of the message.
const value: Ref<string> = ref("");

onMounted(() => {
});
</script>

<template>
  <div class="w-full">

    <div
      class="h-auto min-h-21 p-5 flex items-end"
      v-if="store.status !== 'loading'"
    >
      <!--message textarea-->
      <div class="grow md:mr-5 xs:mr-4 self-end">
        <div class="relative">
          <Textarea
            class="max-h-[5rem] pr-12.5 resize-none scrollbar-hidden"
            @value-changed="(newValue: string) => (value = newValue)"
            @input=""
            :value="value"
            auto-resize
            cols="30"
            rows="1"
            placeholder="Write your message here"
            aria-label="Write your message here"
          />

          
        </div>
      </div>

      <div class="min-h-[2.75rem] flex">

        <!--send message button-->
        <IconButton
          class="ic-btn-contained-primary w-7 h-7 active:scale-110"
          title="send message"
          aria-label="send message"
        >
          <PaperAirplaneIcon class="w-4.25 h-4.25" />
        </IconButton>
      </div>
    </div>
  </div>
</template>