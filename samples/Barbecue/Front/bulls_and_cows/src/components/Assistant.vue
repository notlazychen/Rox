<script setup>
import * as signalR from '@microsoft/signalr'
</script>

<template>
  <div class="flex-container">
    <ul>
      <li>你找张纸写下一个四位数（数字不重复）</li>
      <li>聪明小伙猜一次</li>
      <li>你告诉他几A几B</li>
      <li>A代表数字和位置都正确</li>
      <li>B代表仅数字正确，位置错误</li>
    </ul>
  </div>
  <el-container>
    <el-container>
      <el-main>
        <el-row>
          <div class="console-panel" v-html="consoleText">
          </div>
        </el-row>
        <el-row>
          <div>
            {{ a }}A{{ b }}B
          </div>
        </el-row>
        <el-row>
          <el-button class="grid-content" @click="start()">重开</el-button>
        </el-row>
        <el-row>
          <el-button :disabled="diss[0]" class="grid-content" @click="touch(0)">0</el-button>
          <el-button :disabled="diss[1]" class="grid-content" @click="touch(1)">1</el-button>
          <el-button :disabled="diss[2]" class="grid-content" @click="touch(2)">2</el-button>
          <el-button :disabled="diss[3]" class="grid-content" @click="touch(3)">3</el-button>
          <el-button :disabled="diss[4]" class="grid-content" @click="touch(4)">4</el-button>
        </el-row>
        <el-row>
          <el-button :disabled="dissBack" class="grid-content" @click="touch('Backspace')">
            <el-icon>
              <Back />
            </el-icon>
          </el-button>
          <el-button :disabled="dissEnter" class="grid-content" @click="touch('Enter')"><el-icon>
              <Check />
            </el-icon></el-button>
        </el-row>
      </el-main>
    </el-container>
  </el-container>
</template>
  
<script>

export default {
  data() {
    return {
      a: '_',
      b: '_',
      consoleText: "",
      round: 0,
      diss: [false, false, false, false, false, false, false, false, false, false],
    }
  },
  async mounted() {
    let app = this;
    let connection = new signalR.HubConnectionBuilder()
      .withUrl(this.$baseURL + "bac")
      .build();
    connection.on("info", data => {
      console.log(data);
      app.consoleText += data;
    });
    connection.onclose((err) => {
      console.log(err);
      app.$alert('CONNECTION CLOSED!', 'ERROR');
    })
    await connection.start();
    this.connection = connection;
    this.start()
  },
  computed: {
    dissEnter() {
      return this.b == '_';//找不到的时候可以提交
    },
    dissBack() {
      return this.a == '_';
    },
  },
  methods: {
    touch(key) {
      if (isNaN(key)) {
        switch (key) {
          case 'Enter':
            if (this.b != '_') {
              this.guess(`${this.a}A${this.b}B`);
            }
            break;
          case 'Backspace':
            if (this.b != '_') {
              this.b = '_';
            } else if (this.a != '_') {
              this.a = '_';
            }
            break;
        }
      } else if (key >= 0 && key < 10) {
        if (this.a == '_') {
          this.a = key;
        } else {
          this.b = key;
        }
      }
    },
    async start() {
      // const { data: res } = await this.$http({
      //   method: 'post',
      //   url: '/BullsAndCows/Start'
      // })
      this.connection.invoke("startRobot");
      this.round = 0;
      this.a = '_';
      this.b = '_';
      this.round += 1;
    },
    async guess(res) {
      this.consoleText += `->${res}<br>`;
      if (res[0] == 4) {
        this.consoleText += '聪明小伙猜对了！'
        this.$alert('聪明小伙猜对了！', 'WIN');
        return;
      }
      this.connection.invoke("askRobot", res);
      // const { data: answer } = await this.$http({
      //   method: 'post',
      //   url: '/BullsAndCows/AskRobot',
      //   data: {
      //     result: res
      //   },
      // })
      this.round += 1;
      // this.consoleText += `[${this.round}]${answer}`;
      this.a = '_';
      this.b = '_';
    },
  }
}
</script>

<style lang="scss">
.console-panel {
  background-color: lightgoldenrodyellow;
  // display: flex;
  // align-items: center;
  // justify-content: center;
  height: 40vh;
  width: 100vw;
  overflow-y: auto;
  // font-size: 40px;
  // font-weight: 600;
  // letter-spacing: 15px;
  padding: 5px;
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
  height: 60px;
  width: 100%;
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
