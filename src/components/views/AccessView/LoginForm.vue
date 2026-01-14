<script setup lang="ts">
import { ref, reactive, computed } from "vue";

import Button from "@src/components/ui/inputs/Button.vue";
import LabeledTextInput from "@src/components/ui/inputs/LabeledTextInput.vue";
import PasswordInput from "@src/components/ui/inputs/PasswordInput.vue";
import { RouterLink } from "vue-router";
import type { LoginForm } from "@src/types";
import { LoginRequest } from "@src/services/AuthService/authService";
import { useAuth } from "@src/services/AuthService/useAuth";
import { AxiosError } from "axios";

const auth = useAuth();

const loginForm = reactive<LoginForm>({
  email: "",
  password: "",
});

const errorMessage = ref<string | null>(null);
const isSubmitting = ref(false);

const emailIsValid = computed(() =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(loginForm.email.trim())
);

const passwordHasNoAccents = computed(
  () => !/[^\x00-\x7F]/.test(loginForm.password)
);

const loginFormIsComplete = computed(
  () => loginForm.email.trim().length > 0 && loginForm.password.trim().length > 0
);

const passwordLengthIsValid = computed(
  () => loginForm.password.trim().length >= 8
);

const submitLogin = async () => {
  // Prevent multiple submissions
  if (isSubmitting.value) return;

  // Reset error message
  errorMessage.value = null;

  if (!loginFormIsComplete.value) {
    errorMessage.value = "Please fill in all required fields.";
    return;
  }
  // Basic validation
  if (!emailIsValid.value) {
    errorMessage.value = "Please enter a valid email address.";
    return;
  }
  if (!passwordHasNoAccents.value) {
    errorMessage.value = "Password must not contain accented characters.";
    return;
  }
  if (!passwordLengthIsValid.value) {
    errorMessage.value = "Password must be at least 8 characters long.";
    return;
  }

  // passed all validations
  isSubmitting.value = true;
  try {
    const payload: LoginRequest = {
      email: loginForm.email.trim(),
      password: loginForm.password,
    };

    await auth.login(payload);
  } catch (error) {
    const axiosError = error as AxiosError;
    errorMessage.value =
      axiosError?.response?.data as string || "Login failed. Please try again.";
  } finally {
    isSubmitting.value = false;
  }
};
</script>

<template>
  <div
    class="p-5 md:basis-1/2 xs:basis-full flex flex-col justify-center items-center"
  >
    <div class="w-full md:px-[26%] xs:px-[10%]">
      <!--header-->
      <div class="mb-6 flex flex-col">
        <!-- <img
          src="@src/assets/vectors/logo-gradient.svg"
          class="w-5.5 h-4.5 mb-4 opacity-70"
          alt="bird logo"
        /> -->
        <p class="heading-2 text-black/70 dark:text-white/70 mb-4">
          Welcome back
        </p>
        <p class="body-3 text-black/75 dark:text-white/70 font-light">
          Create an account a start messaging now!
        </p>
      </div>

      <!--form-->
      <p
        v-if="errorMessage"
        class="body-3 text-red-500 text-center mb-4"
      >
        {{ errorMessage }}
      </p>
      <div class="mb-6">
        <LabeledTextInput
          @value-changed="
            (value) => {
              loginForm.email = value;
            }
          "
          :value="loginForm.email"
          label="Email"
          placeholder="Enter your email"
          class="mb-5"
        />
        <PasswordInput
          @value-changed="
            (value) => {
              loginForm.password = value;
            }
          "
          :value="loginForm.password"
          label="Password"
          placeholder="Enter your password"
        />
      </div>

      <!--local controls-->
      <div class="mb-6">
        <Button
          @click="submitLogin"
          :loading="isSubmitting"
          class="contained-primary contained-text w-full mb-4"
          >Sign in</Button
        >
      </div>

      <!--divider-->
      <!-- <div class="mb-6 flex items-center">
        <span
          class="w-full border border-dashed border-gray-100 dark:border-gray-600 rounded-[.0625rem]"
        ></span>
        <p class="body-3 text-black/75 dark:text-white/70 px-4 font-light">
          or
        </p>
        <span
          class="w-full border border-dashed border-gray-100 dark:border-gray-600 rounded-[.0625rem]"
        ></span>
      </div> -->
      

      <!--oauth controls-->
      <div>
        <!-- <Button class="outlined-primary outlined-text w-full mb-5">
          <img
            src="@src/assets/vectors/google-logo.svg"
            class="mr-3"
            alt="google logo"
          />
          Sign in with google
        </Button> -->

        <!--bottom text-->
        <div class="flex justify-center">
          <p class="body-2 text-black/70 dark:text-white/70">
            Don't have an account?
            <RouterLink
              to="/access/sign-up/"
              class="text-indigo-400 opacity-100"
            >
              Sign up
            </RouterLink>
          </p>
        </div>
      </div>
    </div>
  </div>
</template>
