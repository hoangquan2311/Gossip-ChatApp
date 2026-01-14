<script setup lang="ts">
import Button from "@src/components/ui/inputs/Button.vue";
import DropFileUpload from "@src/components/ui/inputs/DropFileUpload.vue";
import LabeledTextInput from "@src/components/ui/inputs/LabeledTextInput.vue";

const props = defineProps<{
  groupTitle: string;
}>();

const emit = defineEmits<{
  (event: "update:groupTitle", value: string): void;
  (event: "active-page-change", payload: { tabName: string; animationName: string }): void;
}>();

const handleNext = () => {
  if (!props.groupTitle.trim()) return;
  emit("active-page-change", {
    tabName: "group-members",
    animationName: "slide-left",
  });
};
</script>

<template>
  <div class="px-5">
    <!--inputs-->
    <div class="mb-5">
      <div class="mb-5">
        <LabeledTextInput
          type="text"
          placeholder="Enter group name"
          label="Group name"
          :value="props.groupTitle"
          @value-changed="(value) => emit('update:groupTitle', value)"
        />
      </div>

      <div>
        <DropFileUpload label="Avatar" />
      </div>
    </div>

    <!--next button-->
    <div class="flex pb-6">
      <div class="grow"></div>
      <Button
        @button-clicked="handleNext"
        class="contained-primary contained-text"
      >
        Next
      </Button>
    </div>
  </div>
</template>
