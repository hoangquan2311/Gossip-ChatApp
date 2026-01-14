<script setup lang="ts">
import { computed, inject, ref } from "vue";

import GroupInfo from "@src/components/shared/modals/ComposeModal/GroupInfo.vue";
import GroupMembers from "@src/components/shared/modals/ComposeModal/GroupMembers.vue";
import SlideTransition from "@src/components/ui/transitions/SlideTransition.vue";
import { signalRCreateGroupKey, type CreateGroupHandler } from "@src/services/signalRContext";

const props = defineProps<{
  closeModal: () => void;
}>();

// used to determine whether to slide left or right
const animation = ref("slide-left");

// name of the active modal page
const activePageName = ref("group-info");

const groupTitle = ref("");
const isCreating = ref(false);
const errorMessage = ref<string | null>(null);
const createGroupHandler = inject<CreateGroupHandler | undefined>(signalRCreateGroupKey);

// the active page component
const ActivePage = computed((): any => {
  if (activePageName.value === "group-info") return GroupInfo;
  else if (activePageName.value === "group-members") return GroupMembers;
});

// (event) to move between modal pages
const handleChangeActiveTab = (event: {
  tabName: string;
  animationName: string;
}) => {
  animation.value = event.animationName;
  activePageName.value = event.tabName;
};

const handleCreateGroup = async () => {
  if (!groupTitle.value.trim()) {
    errorMessage.value = "Group name is required";
    activePageName.value = "group-info";
    return;
  }

  if (!createGroupHandler) {
    errorMessage.value = "Unable to reach chat server";
    return;
  }

  isCreating.value = true;
  errorMessage.value = null;

  try {
    await createGroupHandler({
      title: groupTitle.value.trim(),
      participantIds: [],
    });
    groupTitle.value = "";
    activePageName.value = "group-info";
    props.closeModal();
  } catch (error) {
    errorMessage.value =
      error instanceof Error ? error.message : "Failed to create group";
  } finally {
    isCreating.value = false;
  }
};
</script>

<template>
  <div>
    <!--content-->
    <div class="overflow-x-hidden">
      <SlideTransition :animation="animation">
        <component
          :is="ActivePage"
          :group-title="groupTitle"
          :is-creating="isCreating"
          :error-message="errorMessage"
          @update:groupTitle="(value) => (groupTitle = value)"
          @active-page-change="handleChangeActiveTab"
          @create-group="handleCreateGroup"
          :key="activePageName"
        />
      </SlideTransition>
    </div>
  </div>
</template>
