<script setup lang="ts">
import type { IConversation } from "@src/types";
import type { Ref } from "vue";

import { ref } from "vue";

import useStore from "../../../../store/store";

import { ArrowUturnLeftIcon } from "@heroicons/vue/24/solid";
import ContactItem from "@src/components/shared/blocks/ContactItem.vue";
import IconButton from "@src/components/ui/inputs/IconButton.vue";
import ScrollBox from "@src/components/ui/utils/ScrollBox.vue";

const props = defineProps<{
  closeModal: () => void;
  conversation: IConversation;
}>();

const store = useStore();
</script>

<template>
  <div>
    <!--header-->
    <div class="flex justify-between items-center mb-6 px-5">
      <p id="modal-title" class="heading-1 text-black/70 dark:text-white/70">
        Members
      </p>

      <!--return button-->
      <IconButton
        @click="
          $emit('active-page-change', {
            tabName: 'conversation-info',
            animationName: 'slide-right',
            removeContact: true,
          })
        "
        class="ic-btn-outlined-danger p-2"
      >
        <ArrowUturnLeftIcon class="w-5 h-5" />
      </IconButton>
    </div>

    <!-- ADD BUTTON ADD NEW MEMBER HERE -->
    <!--search-->
    <!-- <div class="mb-5 mx-5">
      <SearchInput />
    </div> -->

    <!--members-->
    <div ref="contactContainer">
      <ScrollBox class="max-h-58 overflow-y-scroll">
        <ContactItem
          v-for="(member, index) in props.conversation.members"
          :member="member"
          :key="index"
        >
        </ContactItem>
      </ScrollBox>
    </div>
  </div>
</template>
