<script setup lang="ts">
import { computed, reactive, ref } from "vue";
import { RouterLink, useRouter } from "vue-router";
import type { AxiosError } from "axios";

import SlideTransition from "@src/components/ui/transitions/SlideTransition.vue";
import PasswordSection from "@src/components/views/AccessView/RegisterForm/PasswordSection.vue";
import PersonalSection from "@src/components/views/AccessView/RegisterForm/PersonalSection.vue";
import { useAuth } from "@src/composables/useAuth";
import useStore from "@src/store/store";

import type { RegisterPersonalForm, RegisterPasswordForm } from "./types";
import { AuthResponse } from "@src/services/authService";


// defineEmits(["activeSectionChange"]);
const auth = useAuth();
const router = useRouter();
const store = useStore();
// determines what form section to use.
const activeSectionName = ref("personal-section");

// determines what direction the slide animation should use.
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
// holds any error messages to show to the user
const errorMessage = ref<string | null>(null);
// indicates whether the form is being submitted
const isSubmitting = ref(false);

// get the active section component from the section name
const ActiveSection = computed((): any => {
  if (activeSectionName.value === "personal-section") {
    return PersonalSection;
  } else if (activeSectionName.value === "password-section") {
    return PasswordSection;
  }
});

// (event) to move between modal pages
const changeActiveSection = (event: {
  sectionName: string;
  animationName: string;
}) => {
  animation.value = event.animationName;
  activeSectionName.value = event.sectionName;
};
// checks if the personal info form is complete
const personalInfoIsComplete = computed(
  () =>
    personalForm.email.trim().length > 0 &&
    personalForm.firstName.trim().length > 0 &&
    personalForm.lastName.trim().length > 0
);

const emailIsValid = computed(() =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(personalForm.email.trim())
);

const handlePersonalInfoError = (message? : string) => {
  activeSectionName.value = "personal-section";
    animation.value = "slide-right";
    if (message) errorMessage.value = message;
};

const passwordFormIsComplete = computed(
  () =>
    passwordForm.password.trim().length > 0 &&
    passwordForm.confirmPassword.trim().length > 0
);

// checks if the passwords match
const passwordsMatch = computed(
  () =>
    passwordForm.password.length > 0 &&
    passwordForm.password === passwordForm.confirmPassword
);

const passwordLengthIsValid = computed(
  () => passwordForm.password.length >= 8
);

const passwordHasNoAccents = computed(
  () => !/[^\x00-\x7F]/.test(passwordForm.password)
);
// updates the personal form data
const updatePersonalForm = (payload: RegisterPersonalForm) => {
  errorMessage.value = null;
  Object.assign(personalForm, payload);
};
// updates the password form data
const updatePasswordForm = (payload: RegisterPasswordForm) => {
  errorMessage.value = null;
  Object.assign(passwordForm, payload);
};
// submits the registration form
const submitRegistration = async () => {
  if (isSubmitting.value) {
    return;
  }

  errorMessage.value = null;

  if (!personalInfoIsComplete.value) {
    handlePersonalInfoError("Please type in your personal information.");
    return;
  }

  if (!emailIsValid.value) {
    handlePersonalInfoError("Please enter a valid email address.");
    return;
  }
  
  if (!passwordFormIsComplete.value) {
    errorMessage.value = "Please type in your password.";
    return;
  }

  if (!passwordLengthIsValid.value) {
    errorMessage.value = "Passwords is too short.";
    return;
  }

  if (!passwordHasNoAccents.value) {
    errorMessage.value = "Password contains invalid characters.";
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

    const response : AuthResponse = await auth.register(payload);
    console.log("Registration successful. Token: " + response.accessToken);
  } catch (error) {
    const axiosError = error as AxiosError;
    errorMessage.value =
      axiosError?.response?.data as string || "Registration failed. Please try again.";
    if (axiosError?.response?.status === 400) {
      handlePersonalInfoError();
    }
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
          Get started with Avian
        </p>
        <p class="body-3 text-black/75 dark:text-white/70 font-light">
          Sign in to start using messaging!
        </p>
      </div>

      <!--form section-->
      <p
        v-if="errorMessage"
        class="body-3 text-red-500 text-center mb-4"
      >
        {{ errorMessage }}
      </p>
      <SlideTransition :animation="animation">
        <component
          @active-section-change="changeActiveSection"
          @update:personalForm="updatePersonalForm"
          @update:passwordForm="updatePasswordForm"
          @submit="submitRegistration"
          :is="ActiveSection"
          :key="activeSectionName"
          v-bind="sectionProps"
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
