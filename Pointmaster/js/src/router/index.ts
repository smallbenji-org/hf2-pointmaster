import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import Home from "../Views/Home.vue";
import Test from "../Views/Test.vue";
import { usePatruljeStore } from "../Modules/PatruljeModule";
import Patruljer from "../Views/Patruljer.vue";

const routes: RouteRecordRaw[] = [
    {
        path: "/",
        component: Home,
        beforeEnter: async () => {
            const patruljeStore = usePatruljeStore();
            patruljeStore.GET_PATRULJER();
        }
    },
    {
        path: "/test",
        component: Test,
        beforeEnter: async () => {
            console.log("entering Test");
        }
    },
    {
        path: "/patruljer",
        component: Patruljer,
        beforeEnter: async () => {
            const patruljeStore = usePatruljeStore();
            patruljeStore.GET_PATRULJER();
        }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;