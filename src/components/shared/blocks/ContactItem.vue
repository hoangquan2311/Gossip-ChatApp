<script setup lang="ts">
import type { IUser } from "@src/types";

import useStore from "../../../store/store";

defineEmits(["contactSelected"]);

const props = defineProps<{
  member: IUser;
}>();

const store = useStore();
</script>

<template>
  <div>
    <div
      class="w-full p-5 flex transition duration-200 ease-out outline-none"
    >
      <!--profile image-->
      <div class="mr-4">
        <div
          :style="{ backgroundImage: `url(${props.member.avatarUrl})` }"
          class="w-7 h-7 rounded-full bg-cover bg-center"
        ></div>
      </div>

      <div class="w-full flex flex-col items-start">
        <div class="w-full mb-3 flex justify-between items-center">
          <!--contact name-->
          <p class="heading-2 text-black/70 dark:text-white/70">
            {{
              props.member.displayName + (store.user && store.user.userId === props.member.userId ? " (You)" : "")
            }}
          </p>

        </div>

        <!--contact email -->
        <p class="body-2 text-black/70 dark:text-white/70">{{ props.member.email }}</p>
      </div>

    </div>
  </div>
</template>
