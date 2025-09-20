import { createApp } from 'vue'
// import './style.css'
import './assets/styles/main.scss'
import App from './App.vue'
import { router } from './router'
import components from './components/UI'
import PageHeader from './components/layouts/PageHeader.vue'
import Sidebar from './components/layouts/Sidebar.vue'
import axios from 'axios'
import store from './store'

const app = createApp(App)
// ниже регестрируются компоненты из массива UI
components.forEach(component => {
    app.component(component.name, component) //первое поле определяет имя компонента, второе - сам компонент
})
app.component("PageHeader", PageHeader)
app.component("Sidebar", Sidebar)


axios.defaults.baseURL = 'https://vacya.onrender.com';

axios.interceptors.request.use((config) => {
  const token = localStorage.getItem('tokenoken');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});


app.use(router)
app.use(store)

app.mount('#app')
