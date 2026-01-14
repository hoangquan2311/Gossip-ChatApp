<script setup lang="ts">
import { ref } from "vue";

import LabeledTextInput from "@src/components/ui/inputs/LabeledTextInput.vue";
import Button from "@src/components/ui/inputs/Button.vue";

const props = defineProps<{
  groupTitle: string;
  isCreating: boolean;
  errorMessage?: string | null;
}>();

const emit = defineEmits<{
  (event: "active-page-change", payload: { tabName: string; animationName: string }): void;
  (event: "create-group"): void;
}>();

const emailInput = ref<string>("");

const handleBack = () => {
  emit("active-page-change", {
    tabName: "group-info",
    animationName: "slide-right",
  });
};

const handleCreate = () => emit("create-group");
</script>

<template>
  <div>
    <div class="px-5 mb-3">
      <p class="body-3 text-black/60 dark:text-white/60 mb-2">Group name</p>
      <p class="heading-3 text-black/80 dark:text-white/80">
        {{ props.groupTitle || "Untitled group" }}
      </p>
    </div>

    <div class="px-5 mb-7">
      <LabeledTextInput
        type="text"
        placeholder="example@email.com"
        label="Add member"
        :value="emailInput"
        @value-changed="(value) => (emailInput.value = value)"
      />
    </div>

    <div v-if="props.errorMessage" class="px-5 mb-4">
      <p class="body-3 text-red-500">{{ props.errorMessage }}</p>
    </div>

    <div class="flex px-5 mt-5 pb-6">
      <div class="grow"></div>
      <Button
        @button-clicked="handleBack"
        class="ghost-primary ghost-text mr-4"
      >
        <p class="body-5">Previous</p>
      </Button>

      <Button
        @button-clicked="handleCreate"
        class="contained-text contained-primary"
        :loading="props.isCreating"
      >
        Create
      </Button>
    </div>
  </div>
</template>
