<script setup>
import * as signalR from '@microsoft/signalr'
import {
  ArrowUp,
  ArrowDown,
  ArrowLeft,
  ArrowRight,
} from '@element-plus/icons-vue'
</script>

<template>
  <el-container>
    <el-container>
      <el-main>
        <div class="console-panel" v-html="consoleText">
        </div>
        <div class="control-panel">
          <el-row>
            <el-col :span="8" :offset="8">
              <el-button :icon="ArrowUp" @click="move('u')"/>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="8">
              <el-button :icon="ArrowLeft" @click="move('l')"/>
            </el-col>
            <el-col :span="8" :offset="8">
              <el-button :icon="ArrowRight" @click="move('r')"/>
            </el-col>
          </el-row>
          <el-row>
            <el-col :span="8" :offset="8">
              <el-button :icon="ArrowDown" @click="move('d')"/>
            </el-col>
          </el-row>
        </div>
      </el-main>
    </el-container>
  </el-container>
</template>
  
<script>

export default {
  data() {
    return {
      consoleText: "",
      round: 0
    }
  },
  async mounted() {
    let app = this;
    document.onkeydown = function (e) {
      app.touch(e.key);
    }
    let connection = new signalR.HubConnectionBuilder()
      .withUrl(this.$baseURL + "tetris")
      .build();
    connection.on("info", data => {
      app.consoleText = data;
    });
    connection.onclose((err) => {
      console.log(err);
      app.$alert('CONNECTION CLOSED!', 'ERROR');
    })
    await connection.start();
    this.connection = connection;
  },
  methods: {
    touch(key) {
      var res = "d";
      switch (key) {
        case 'ArrowUp':
          res = 'u';
          break;
        case 'ArrowDown':
          res = 'd';
          break;
        case 'ArrowLeft':
          res = 'l';
          break;
        case 'ArrowRight':
          res = 'r';
          break;
      }
      this.connection.invoke("move", res);
    },
    async move(res) {
      this.connection.invoke("move", res);
    },
  }
}
</script>

<style lang="scss">
.console-panel {
  background-color: lightgoldenrodyellow;
  margin-top: 20vh;
  height: 50vh;
  white-space: pre-wrap;
}

.control-panel {
  background-color: lightgray;
  height: 20vh;
}

.el-button {
  font-size: 16px;
  font-weight: 600;
  padding: 5px;

  color: #606266;
  border-color: #DCDFE6;
  background-color: #fff;
}

.el-button:hover {
  color: #606266;
  border-color: #DCDFE6;
  background-color: #fff;
}
</style>
