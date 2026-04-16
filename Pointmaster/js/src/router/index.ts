import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import Home from "../Views/Home.vue";
import Test from "../Views/Test.vue";

const routes: RouteRecordRaw[] = [
    {
        path: "/",
        component: Home,
        beforeEnter: async () => {
            console.log("entering Home");
        }
    },
    {
        path: "/test",
        component: Test,
        beforeEnter: async () => {
            console.log("entering Test");
        }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;