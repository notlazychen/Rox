import { createApp } from 'vue'
import './style.css'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import App from './App.vue'

const app = createApp(App)

app.config.globalProperties.$baseURL = import.meta.env.PROD ? '/' : 'http://127.0.0.1:8080/';
app.use(ElementPlus)
app.mount('#app')
