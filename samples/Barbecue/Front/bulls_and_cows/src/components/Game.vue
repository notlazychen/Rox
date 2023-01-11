<script setup>
import { result } from 'lodash';
import { ref } from 'vue'
import * as signalR from '@microsoft/signalr'

defineProps({
  msg: String,
})

const count = ref(0)
</script>

<template>
  <div class="common-layout">
    <el-container>
      <el-container>
        <el-header>
          {{ text }}
        </el-header>
        <!-- <a href="#/assistant">试试问下聪明小伙</a> -->
        <el-main>
          <el-row>
            <el-button class="grid-content" @click="start()">重开</el-button>
          </el-row>
          <el-row>
            <el-button :disabled="diss[7]" class="grid-content" @click="touch(7)">7</el-button>
            <el-button :disabled="diss[8]" class="grid-content" @click="touch(8)">8</el-button>
            <el-button :disabled="diss[9]" class="grid-content" @click="touch(9)">9</el-button>
          </el-row>
          <el-row>
            <el-button :disabled="diss[4]" class="grid-content" @click="touch(4)">4</el-button>
            <el-button :disabled="diss[5]" class="grid-content" @click="touch(5)">5</el-button>
            <el-button :disabled="diss[6]" class="grid-content" @click="touch(6)">6</el-button>
          </el-row>
          <el-row>
            <el-button :disabled="diss[1]" class="grid-content" @click="touch(1)">1</el-button>
            <el-button :disabled="diss[2]" class="grid-content" @click="touch(2)">2</el-button>
            <el-button :disabled="diss[3]" class="grid-content" @click="touch(3)">3</el-button>
          </el-row>
          <el-row>
            <el-button :disabled="dissBack" class="grid-content" @click="touch('Backspace')">
              <el-icon>
                <Back />
              </el-icon>
            </el-button>
            <el-button :disabled="diss[0]" class="grid-content" @click="touch(0)">0</el-button>
            <el-button :disabled="dissEnter" class="grid-content" @click="touch('Enter')"><el-icon>
                <Check />
              </el-icon></el-button>
          </el-row>
        </el-main>
      </el-container>
      <el-aside v-html="consoleText">

      </el-aside>
    </el-container>
  </div>
</template>

<script>

export default {
  data() {
    return {
      text: "____",
      consoleText: "",
      round: 0,
      diss: [false,false,false,false,false,false,false,false,false,false],
    }
  },
  computed:{
      dissEnter(){        
        let index = this.text.indexOf('_');
        return !(index < 0);//找不到的时候可以提交
      },
      dissBack(){
        let index = this.text.indexOf('_');
        return !(index < 0 || index > 0);
      },
  },
  created() {
    let app = this;
    document.onkeydown = function (e) {
      window.event.preventDefault()
      // let key = window.event.keyCode; // 获得keyCode
      // console.log(e);
      app.touch(e.key);
    }
  },
  async mounted() {
    let app = this;
    let connection = new signalR.HubConnectionBuilder()
      .withUrl(this.$baseURL + "bac")
      .build();
    connection.on("info", data => {
      console.log(data);
      app.consoleText += data + `<br>`;
      if (data.indexOf("4A0B") >= 0) {
        app.consoleText += 'YOU ARE RIGHT!'
        app.$alert('YOU ARE RIGHT!', 'WIN');
      }
    });
    connection.onclose((err)=>{
      console.log(err);
      app.$alert('CONNECTION CLOSED!', 'ERROR');
    })
    await connection.start();
    this.connection = connection;
    await this.start();
  },
  methods: {
    touch(key) {
      if (isNaN(key)) {
        switch (key) {
          case 'Enter': {
            let index = this.text.indexOf('_');
            if (index < 0)
              this.guess(this.text);
          }
            break;
          case 'Backspace':
            let index = this.text.indexOf('_');
            if (index < 0) {
              index = this.text.length;
            }
            if (index > 0) {
              this.text = this.setCharOnIndex(this.text, index - 1, '_')
            }
            break;
        }
      } else if (key >= 0 && key < 10) {
        // console.log(`number ${key}`)
        let index = this.text.indexOf('_');
        if (index < 0) {
          index = this.text.length - 1;
        }
        this.text = this.setCharOnIndex(this.text, index, key)
      }
    },
    setCharOnIndex(string, index, char) {
      if (index > string.length - 1) return string;
      var old = string[index];
      // console.log(old, index, char)
      if(old == '_'){
        this.diss[Number(char)] = true;
      }else if(index == 3){
        this.diss[Number(char)] = true;
        this.diss[Number(old)] = false;
      }else{
        this.diss[Number(old)] = false;
      }
      return string.substring(0, index) + char + string.substring(index + 1);
    },
    async start() {
      // const { data: res } = await this.$http({
      //   method: 'post',
      //   url: '/BullsAndCows/Start'
      // })
      this.connection.invoke("start");
      this.round = 0;
      for(let i = 3 ; i>= 0; i--){
        this.text = this.setCharOnIndex(this.text, i, "_");
      }
      this.consoleText = "";
    },
    async guess(number) {
      // const { data: res } = await this.$http({
      //   method: 'post',
      //   url: '/BullsAndCows/Guess',
      //   data: {
      //     guessNumber: number
      //   },
      // })      
      this.connection.invoke("guess", number);
      
      this.round += 1;
      for(let i = 4 ; i>= 0; i--){
        this.text = this.setCharOnIndex(this.text, i, "_");
      }
    },
  }
}
</script>

<style lang="scss">
.common-layout {
  position: relative;
  width: 100%;
  height: 100%;
}

.el-aside {
  display: flex;
  flex-wrap: nowrap;
  max-width: 36%;
  height: 100vh;
  overflow-y: auto;
  overflow-x: auto;
  white-space: nowrap;
  background-color:ivory
}

.el-header {
  display: flex;
  background-color: lightgoldenrodyellow;
  align-items: center;
  justify-content: center;
  height: 200px;
  font-size: 40px;
  font-weight: 600;
  padding: 5px;
  letter-spacing: 15px;
}

.el-main {
  display: flex;
  flex-direction: column;
  flex-wrap: nowrap;
  justify-content: center;
  // background-color: lightskyblue;
  padding: 6px;
}

.el-row {
  display: flex;
  flex-wrap: nowrap;
  margin-bottom: 10px;
}

.el-row:last-child {
  margin-bottom: 0;
}

.grid-content {
  border-color: black;
  border-radius: 8px;
  width: 30%;
  height: 60px;
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
