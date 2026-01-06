<script setup lang="ts">
import { computed, reactive, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import type { AxiosError } from "axios";

import SlideTransition from "@src/components/ui/transitions/SlideTransition.vue";
import PasswordSection from "@src/components/views/AccessView/RegisterForm/PasswordSection.vue";
import PersonalSection from "@src/components/views/AccessView/RegisterForm/PersonalSection.vue";
import AuthService from "@src/services/authService";
import type { RegisterPersonalForm, RegisterPasswordForm } from "./types";

const router = useRouter();
const activeSectionName = ref("password-section");
const animation = ref("slide-left");
const personalForm = reactive<RegisterPersonalForm>({
  email: "",
  firstName: "",
  lastName: "",
});
const passwordForm = reactive<RegisterPasswordForm>({
  password: "",
  confirmPassword: "",
});
const errorMessage = ref<string | null>(null);
const isSubmitting = ref(false);

const ActiveSection = computed((): any => {
  if (activeSectionName.value === "personal-section") {
    return PersonalSection;
  }

  return PasswordSection;
});

const changeActiveSection = (event: {
  sectionName: string;
  animationName: string;
}) => {
  animation.value = event.animationName;
  activeSectionName.value = event.sectionName;
  errorMessage.value = null;
};

const personalInfoIsComplete = computed(
  () =>
    personalForm.email.trim().length > 0 &&
    personalForm.firstName.trim().length > 0 &&
    personalForm.lastName.trim().length > 0
);

const passwordsMatch = computed(
  () =>
    passwordForm.password.length > 0 &&
    passwordForm.password === passwordForm.confirmPassword
);

const updatePersonalForm = (payload: RegisterPersonalForm) => {
  errorMessage.value = null;
  Object.assign(personalForm, payload);
};

const updatePasswordForm = (payload: RegisterPasswordForm) => {
  errorMessage.value = null;
  Object.assign(passwordForm, payload);
};

const submitRegistration = async () => {
  if (isSubmitting.value) {
    return;
  }

  errorMessage.value = null;

  if (!personalInfoIsComplete.value) {
    activeSectionName.value = "personal-section";
    animation.value = "slide-right";
    errorMessage.value = "Please complete all personal details.";
    return;
  }

  if (!passwordsMatch.value) {
    errorMessage.value = "Passwords must match.";
    return;
  }

  isSubmitting.value = true;

  try {
    const payload = {
      email: personalForm.email.trim(),
      displayName: `${personalForm.firstName} ${personalForm.lastName}`.trim(),
      password: passwordForm.password,
    };

    await AuthService.register(payload);
    await router.push("/chat/no-chat/");
  } catch (error) {
    const axiosError = error as AxiosError<{ message?: string }>;
    errorMessage.value =
      axiosError?.response?.data?.message ??
      "Unable to create an account. Please try again.";
  } finally {
    isSubmitting.value = false;
  }
};

const sectionProps = computed(() => {
  if (activeSectionName.value === "personal-section") {
    return {
      personalForm,
    };
  }

  return {
    passwordForm,
    isSubmitting: isSubmitting.value,
    errorMessage: errorMessage.value,
  };
});

const sectionListeners = computed(() => {
  const listeners: Record<string, (...args: any[]) => void> = {
    "onActive-section-change": changeActiveSection,
  };

  if (activeSectionName.value === "personal-section") {
    listeners["onUpdate:personalForm"] = updatePersonalForm;
  } else {
    listeners["onUpdate:passwordForm"] = updatePasswordForm;
    listeners["onSubmit"] = submitRegistration;
  }

  return listeners;
});
</script>

<template>
  <div
    class="p-5 md:basis-1/2 xs:basis-full flex flex-col justify-center items-center"
  >
    <div class="w-full md:px-[26%] xs:px-[10%]">
      <!--header-->
      <div class="mb-6 flex flex-col">
        <img
          src="@src/assets/vectors/logo-gradient.svg"
          class="w-5.5 h-4.5 mb-5 opacity-70"
        />
        <p class="heading-2 text-black/70 dark:text-white/70 mb-4">
          Chat Application
        </p>
        <p class="body-3 text-black/75 dark:text-white/70 font-light">
          Sign in to start using messaging!
        </p>
      </div>

      <!--form section-->
      <p
        v-if="errorMessage && activeSectionName === 'personal-section'"
        class="body-3 text-red-500 text-center mb-4"
      >
        {{ errorMessage }}
      </p>
      <SlideTransition :animation="animation">
        <component
          @active-section-change="changeActiveSection"
          :is="ActiveSection"
          :key="activeSectionName"
          v-bind="sectionProps"
          v-on="sectionListeners"
        />
      </SlideTransition>

      <!--bottom text-->
      <div class="flex justify-center">
        <p class="body-2 text-black/70 dark:text-white/70">
          Already have an account?
          <RouterLink to="/access/sign-in/" class="text-indigo-400 opacity-100">
            Sign in
          </RouterLink>
        </p>
      </div>
    </div>
  </div>
</template>
