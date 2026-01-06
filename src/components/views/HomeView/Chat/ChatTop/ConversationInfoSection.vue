<script setup lang="ts">
import type { IConversation } from "@src/types";

import { inject } from "vue";

import router from "@src/router";
import useStore from "@src/store/store";
import { getAvatar, getName } from "@src/utils";

import {
  ChevronLeftIcon,
} from "@heroicons/vue/24/outline";
import IconButton from "@src/components/ui/inputs/IconButton.vue";

const props = defineProps<{
  handleOpenInfo: () => void;
}>();

const store = useStore();

const activeConversation = <IConversation>inject("activeConversation");

// (event) navigate to the /chat/ url
const handleCloseConversation = () => {
  router.push({ path: "/chat/" });
};

</script>

<template>
  <!--conversation info-->
  <div class="w-full flex justify-center items-center">
    <div class="group mr-4 md:hidden">
      <IconButton
        class="ic-btn-ghost-primary w-7 h-7"
        @click="handleCloseConversation"
        title="close conversation"
        aria-label="close conversation"
      >
        <ChevronLeftIcon class="w-[1.25rem] h-[1.25rem]" />
      </IconButton>
    </div>

    <div v-if="store.status !== 'loading'" class="flex grow">
      <!--avatar-->
      <button
        class="mr-5 outline-none"
        @click="props.handleOpenInfo"
        aria-label="profile avatar"
      >
        <div
          :style="{
            backgroundImage: `url(${getAvatar(activeConversation)})`,
          }"
          class="w-[2.25rem] h-[2.25rem] rounded-full bg-cover bg-center"
        ></div>
      </button>

      <!--name and last seen-->
      <div class="flex flex-col">
        <p
          class="w-fit heading-2 text-black/70 dark:text-white/70 mb-2 cursor-pointer"
          @click="props.handleOpenInfo"
          tabindex="0"
        >
          {{ getName(activeConversation) }}
        </p>

        <p
          class="body-2 text-black/70 dark:text-white/70 font-extralight rounded-[.25rem]"
          tabindex="0"
          aria-label="Last seen december 16, 2019"
        >
          Last seen Dec 16, 2019
        </p>
      </div>
    </div>

  </div>
</template>
