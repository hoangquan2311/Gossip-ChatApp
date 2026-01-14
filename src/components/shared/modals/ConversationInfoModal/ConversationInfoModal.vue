<script setup lang="ts">
import type { Ref } from "vue";
import type { IConversation, IUser } from "@src/types";

import { computed, ref } from "vue";

import ConversationInfoTab from "@src/components/shared/modals/ConversationInfoModal/ConversationInfoTab/ConversationInfoTab.vue";
import EditGroupInfoTab from "@src/components/shared/modals/ConversationInfoModal/EditGroupInfoTab.vue";
import ConversationMembersTab from "@src/components/shared/modals/ConversationInfoModal/ConversationMembersTab.vue";
import Modal from "@src/components/ui/utils/Modal.vue";
import SlideTransition from "@src/components/ui/transitions/SlideTransition.vue";

defineEmits(["activePageChange"]);

const props = defineProps<{
  open: boolean;
  conversation: IConversation;
  closeModal: () => void;
}>();

// used to determine whether to slide left or right
const animation = ref("slide-left");

// name of the active modal page
const activePageName = ref("conversation-info");

// the active modal page component
const ActiveTab = computed((): any => {
  if (activePageName.value === "conversation-info") return ConversationInfoTab;
  else if (activePageName.value === "members") return ConversationMembersTab;
  else if (activePageName.value === "edit-group") return EditGroupInfoTab;
});

// (event) move between modal pages
const handleChangeActiveTab = (event: {
  tabName: string;
  animationName: string;
}) => {
  animation.value = event.animationName;
  activePageName.value = event.tabName;
};
</script>

<template>
  <Modal :open="props.open" :close-modal="props.closeModal">
    <template v-slot:content>
      <div class="overflow-x-hidden">
        <div class="w-75 bg-white dark:bg-gray-800 rounded py-6">
          <!--content-->
          <SlideTransition :animation="animation">
            <component
              @active-page-change="handleChangeActiveTab"
              :is="ActiveTab"
              :conversation="props.conversation"
              :close-modal="props.closeModal"
              :key="activePageName"
            />
          </SlideTransition>
        </div>
      </div>
    </template>
  </Modal>
</template>
