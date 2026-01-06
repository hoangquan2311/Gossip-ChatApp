<script setup lang="ts">
import type { IConversation } from "@src/types";

import { inject, ref } from "vue";

import ConversationInfoModal from "@src/components/shared/modals/ConversationInfoModal/ConversationInfoModal.vue";
import ConversationInfoSection from "./ConversationInfoSection.vue";



const activeConversation = <IConversation>inject("activeConversation");

const openInfo = ref(false);


// (event) open info modal
const handleOpenInfo = () => {
  openInfo.value = true;
};
</script>

<template>
  <div class="w-full">
    <!--Top section-->
    <div class="w-full min-h-21 px-5 py-6">
      <ConversationInfoSection
        :handle-open-info="handleOpenInfo"
      />
    </div>
    <!--Contact info modal-->
    <ConversationInfoModal
      :open="openInfo"
      :closeModal="() => (openInfo = false)"
      :conversation="activeConversation"
    />
  </div>
</template>
