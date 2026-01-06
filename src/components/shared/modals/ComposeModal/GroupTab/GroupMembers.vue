<script setup lang="ts">
import type { Ref } from "vue";
import type { IContact } from "@src/types";

import { ref } from "vue";

import useStore from "@src/store/store";

import LabeledTextInput from "@src/components/ui/inputs/LabeledTextInput.vue";
import Button from "@src/components/ui/inputs/Button.vue";

const store = useStore();

// a list of contacts selected to make a call
const selectedContacts: Ref<IContact[]> = ref([]);

// checks whether a contact is selected or not
const isContactSelected = (contact: IContact) => {
  if (contact) {
    return Boolean(
      selectedContacts.value.find((item) => item.id === contact.id),
    );
  } else {
    return false;
  }
};

// (event) change the value of selected contacts
const handleSelectedContactsChange = (contact: IContact) => {
  let contactIndex = selectedContacts.value.findIndex(
    (item) => item.id === contact.id,
  );
  if (contactIndex !== -1) {
    selectedContacts.value.splice(contactIndex, 1);
  } else {
    selectedContacts.value.push(contact);
  }
};

const emailInput = ref<string>("");

</script>

<template>
  <div>
    <!--search-->
    <!-- <div class="mx-5 mt-3 mb-5">
      <SearchInput />
    </div> -->
    
    <div class="px-5 mb-7">
      <!-- <p class="body-2 text-black/70 dark:text-white/70 mb-3">Member email:</p> -->
      <LabeledTextInput type="text" placeholder="example@email.com" label="Add member" @value-changed="emailInput = $event" />
    </div>

    <div class="flex px-5 mt-5 pb-6">
      <div class="grow"></div>
      <!--previous button-->
      <Button
        @click="
          $emit('active-page-change', {
            tabName: 'group-info',
            animationName: 'slide-right',
          })
        "
        class="ghost-primary ghost-text mr-4"
      >
        <p class="body-5">Previous</p>
      </Button>

      <!--next button-->
      <Button class="contained-text contained-primary" :class="(emailInput != '' ? 'bg-indigo-400' : '')" :disabled="emailInput === ''"> Create </Button>
    </div>
  </div>
</template>
