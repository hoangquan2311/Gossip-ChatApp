<script setup lang="ts">

import useStore from "@src/store/store";

import FadeTransition from "@src/components/ui/transitions/FadeTransition.vue";
import Conversations from "@src/components/views/HomeView/Sidebar/Conversations/Conversations.vue";

const store = useStore();

// the selected sidebar component (e.g message/notifications/settings)
// const ActiveComponent = computed((): any => {
//   if (store.activeSidebarComponent === "messages") {
//     return Conversations;
//   } else if (store.activeSidebarComponent === "contacts") {
//     return Contacts;
//   } else if (store.activeSidebarComponent === "notifications") {
//     return Notifications;
//   } else if (store.activeSidebarComponent === "phone") {
//     return Calls;
//   } else if (store.activeSidebarComponent === "settings") {
//     return Settings;
//   }
// });
</script>

<template>
  <aside
    class="xs:w-full md:w-72.5 h-full xs:px-5 md:p-0 flex flex-col overflow-visible transition-all duration-500"
  >
    <FadeTransition>
      <Conversations class="h-full flex flex-col" />
    </FadeTransition>
  </aside>
</template>
