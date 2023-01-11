import { createApp } from 'vue'
import ElementPlus from 'element-plus'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import 'element-plus/dist/index.css'
import './style.css'
import App from './App.vue'
import axios from 'axios' 

const app = createApp(App)
app.use(ElementPlus)
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}

axios.defaults.baseURL = import.meta.env.PROD ? '/' : 'http://127.0.0.1:8080/';
app.config.globalProperties.$baseURL = axios.defaults.baseURL
app.config.globalProperties.$http = axios 
app.mount('#app')
