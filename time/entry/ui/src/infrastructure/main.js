import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import {init} from "./auth";

const app = createApp(App)

init().then(() =>
{
    app.use(router);
    app.mount('#app');
});
