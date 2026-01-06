<script setup lang="ts">
import type { IContact, IConversation } from "@src/types";

import { computed, ref } from "vue";

import { getAvatar, getName } from "@src/utils";

import {
  ArrowLeftOnRectangleIcon,
  PencilIcon,
  UserGroupIcon,
} from "@heroicons/vue/24/outline";
import { ArrowUturnLeftIcon } from "@heroicons/vue/24/solid";
import IconAndText from "@src/components/shared/blocks/IconAndText.vue";
import ImageViewer from "@src/components/shared/modals/ConversationInfoModal/ConversationInfoTab/ImageViewer.vue";
import Button from "@src/components/ui/inputs/Button.vue";
import IconButton from "@src/components/ui/inputs/IconButton.vue";

const props = defineProps<{
  conversation: IConversation;
  contact?: IContact;
  closeModal: () => void;
}>();

const openImageViewer = ref(false);

const imageUrl = computed(() => {
  if (props.contact) {
    return props.contact.avatar;
  } else {
    return getAvatar(props.conversation);
  }
});
</script>

<template>
  <div>
    <div class="mb-6 px-5 flex justify-between items-center">
      <!--title-->
      <p
        class="heading-1 text-black/70 dark:text-white/70"
        id="modal-title"
        tabindex="0"
      >
        Group Info
      </p>

      <!--close button-->
      <Button
        v-if="!props.contact"
        @click="props.closeModal"
        class="outlined-danger ghost-text py-2 px-4"
      >
        Esc
      </Button>

      <!--return button-->
      <IconButton
        v-else
        @click="
          $emit('active-page-change', {
            tabName: 'members',
            animationName: 'slide-right',
          })
        "
        class="ic-btn-outlined-danger p-2"
      >
        <ArrowUturnLeftIcon
          class="w-5 h-5 text-black opacity-50 dark:text-white dark:opacity-70 group-focus:text-red-500 dark:group-focus:text-white group-hover:text-red-500 group-hover:opacity-100 dark:group-hover:text-white"
        />
      </IconButton>
    </div>

    <!--top-->
    <div class="w-full p-5 pb-6">
      <div class="flex">
        <!--avatar-->
        <div class="mr-5">
          <button
            @click="openImageViewer = true"
            class="outline-none"
            aria-label="view avatar"
          >
            <div
              :style="{
                backgroundImage: `url(${getAvatar(props.conversation)})`,
              }"
              class="w-9.5 h-9.5 rounded-full bg-cover bg-center"
            ></div>
          </button>
        </div>

        <!--name-->
        <div class="w-full flex justify-between">
          <div>
            <p
              class="heading-2 text-black/70 dark:text-white/70 mb-3 mr-5 text-start"
            >
              <span>
                {{ getName(props.conversation) }}
              </span>
            </p>

            <p
              class="body-2 text-black/70 dark:text-white/70 font-extralight text-start"
            >
              {{
                `${props.conversation.contacts.length} members`
              }}
            </p>
          </div>

          <IconButton
            title="Edit group"
            class="ic-btn-ghost-primary w-7 h-7"
            @click="
              $emit('active-page-change', {
                tabName: 'edit-group',
                animationName: 'slide-left',
              })
            "
          >
            <PencilIcon class="w-5 h-5" />
          </IconButton>
        </div>
      </div>
    </div>

    <!--middle-->
    <div class="w-full py-5 border-t border-gray-100 dark:border-gray-700">
      <!-- members-->
      <div
        class="px-5 flex items-center"
      >
        <IconAndText
          :icon="UserGroupIcon"
          title="Members"
          link
          chevron
          @click="
            $emit('active-page-change', {
              tabName: 'members',
              animationName: 'slide-left',
            })
          "
        />
      </div>
    </div>

    <!--bottom-->
    <div class="w-full border-t border-gray-100 dark:border-gray-700">
      <!--(group) exit group-->
      <div
        class="px-5 pt-5 flex items-center group"
      >
        <IconAndText :icon="ArrowLeftOnRectangleIcon" title="Exit group" link />
      </div>
    </div>

    <!--image viewer-->
    <ImageViewer
      :image-url="imageUrl"
      :open="openImageViewer"
      :close-image="() => (openImageViewer = false)"
    />
  </div>
</template>
