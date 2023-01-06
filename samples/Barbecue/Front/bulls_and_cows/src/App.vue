<script setup>
import Game from './components/Game.vue'
import Assistant from './components/Assistant.vue'
</script>

<script>
const routes = {
  '/': Game,
  '/assistant': Assistant
}
export default {
  data() {
    return {
      currentPath: window.location.hash
    }
  },
  computed: {
    currentView() {
      return routes[this.currentPath.slice(1) || '/'] || NotFound
    }
  },
  mounted() {
    window.addEventListener('hashchange', () => {
      this.currentPath = window.location.hash
    })
  }
}
</script>

<template>
  <div style="height: 100%">
      <!-- <a href="#/">Game</a>
      <a href="#/assistant">Smart Guy</a>  -->
      <component :is="currentView" />
  </div>
</template>

<style scoped>

</style>
