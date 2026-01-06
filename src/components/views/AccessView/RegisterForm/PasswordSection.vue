<script setup lang="ts">
import PasswordInput from "@src/components/ui/inputs/PasswordInput.vue";
import Button from "@src/components/ui/inputs/Button.vue";
import type { RegisterPasswordForm } from "./types";

const props = defineProps<{
  passwordForm: RegisterPasswordForm;
  isSubmitting?: boolean;
  errorMessage?: string | null;
}>();
const emit = defineEmits<{
  (event: "active-section-change", payload: { sectionName: string; animationName: string }): void;
  (event: "update:passwordForm", payload: RegisterPasswordForm): void;
  (event: "submit"): void;
}>();

const updateField = (field: keyof RegisterPasswordForm, value: string) => {
  emit("update:passwordForm", {
    ...props.passwordForm,
    [field]: value,
  });
};

const handleSignUp = () => {
  emit('submit');
};
</script>

<template>
  <div>
    <div class="mb-5">
      <!--form-->
      <PasswordInput
        @value-changed="(value) => updateField('password', value)"
        :value="props.passwordForm.password"
        label="Password (must be at least 8 characters)"
        placeholder="Enter your password"
        class="mb-4"
      />

      <PasswordInput
        @value-changed="(value) => updateField('confirmPassword', value)"
        :value="props.passwordForm.confirmPassword"
        label="Confirm Password"
        placeholder="Enter your password"
      />
    </div>

    <!--controls-->
    <div class="mb-5">
      <Button class="contained-primary contained-text w-full mb-4"
      :loading="props.isSubmitting"
      @click="handleSignUp"
        >Sign up</Button
      >
      <Button
        class="outlined-primary outlined-text w-full"
        @click="
          $emit('active-section-change', {
            sectionName: 'personal-section',
            animationName: 'slide-right',
          })
        "
      >
        Back
      </Button>
    </div>
  </div>
</template>
