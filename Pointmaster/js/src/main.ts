import { createApp } from 'vue'
import App from './App.vue'
import router from './router';
import { createPinia } from 'pinia';
import Buefy from 'buefy';
// import 'bulma/bulma.scss'
import 'buefy/dist/css/buefy.css'
const app = createApp(App);

const pinia = createPinia();

app.use(router);
app.use(pinia);
app.use(Buefy);

app.mount('#app')
