<script setup>
// import HelloWorld from './components/HelloWorld.vue'
import LayoutPanel from './components/LayoutPanel.vue'
</script>

<template>
  <!-- <HelloWorld msg="Vite + Vue" /> -->
  
  <div class="common-layout">
    <el-container>
      <el-header>Header</el-header>
      <el-container>
        <el-aside width="200px">Aside</el-aside>
        <el-container>
          <el-main><LayoutPanel /></el-main>
          <el-footer>Footer</el-footer>
        </el-container>
      </el-container>
    </el-container>
  </div>

</template>

<style scoped>
</style>
