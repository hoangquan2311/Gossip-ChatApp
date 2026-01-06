<script setup lang="ts">

import GroupTab from "@src/components/shared/modals/ComposeModal/GroupTab/GroupTab.vue";
import Button from "@src/components/ui/inputs/Button.vue";
import FadeTransition from "@src/components/ui/transitions/FadeTransition.vue";
import Modal from "@src/components/ui/utils/Modal.vue";

const props = defineProps<{
  open: boolean;
  closeModal: () => void;
}>();

</script>

<template>
  <Modal :open="props.open" :close-modal="props.closeModal">
    <template v-slot:content>
      <div class="w-75 bg-white dark:bg-gray-800 rounded pt-6">
        <!--header-->
        <div class="flex justify-between items-center mb-6 px-5">
          <p
            id="modal-title"
            class="heading-1 text-black/70 dark:text-white/70"
            ref="modalTitle"
            tabindex="0"
          >
            Add Group
          </p>

          <Button
            @click="props.closeModal"
            class="outlined-danger ghost-text py-2 px-4"
          >
            esc
          </Button>
        </div>

        <!--ActiveTab-->
        <FadeTransition>
          <component :is="GroupTab"/>
        </FadeTransition>
      </div>
    </template>
  </Modal>
</template>
