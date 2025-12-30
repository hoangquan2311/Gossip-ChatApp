<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { RouterView } from 'vue-router';
import FadeTransition from './components/ui/transitions/FadeTransition.vue';


const darkMode = ref(false);

const height = ref(`${window.innerHeight}px`);

const resizeWindow = () => {
  height.value = `${window.innerHeight}px`;
};
// and add the resize event when the component mounts.
onMounted(() => {
  window.addEventListener("resize", resizeWindow);
});

// remove the event when un-mounting the component.
onUnmounted(() => {
  window.removeEventListener("resize", resizeWindow);
});
</script>

<template>
  <div :class="darkMode ? 'dark' : ''">
    <div class="bg-white dark:bg-gray-800 transition-colors duration-500"
      :style="{ height: height }">
      <div class="bg-neon">Test</div>
      <button @click="() => {darkMode = !darkMode}">Light switch</button>
      <RouterView v-slot="{ Component }">
        <FadeTransition>
          <component :is="Component" />
        </FadeTransition>
      </RouterView>
    </div>
  </div>
  
</template>

<style scoped>
  
</style>
