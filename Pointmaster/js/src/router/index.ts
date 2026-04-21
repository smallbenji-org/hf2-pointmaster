import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import Home from "@/Views/Home.vue";
import { usePatruljeStore } from "@/Modules/PatruljeModule";
import Patruljer from "@/Views/Patruljer.vue";
import Point from "@/Views/Point.vue";
import { usePointStore } from "@/Modules/PointModule";
import Post from "@/Views/Post.vue";
import { usePostStore } from "@/Modules/PostModule";

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
        path: "/patruljer",
        component: Patruljer,
        beforeEnter: async () => {
            const patruljeStore = usePatruljeStore();
            patruljeStore.GET_PATRULJER();
        }
    },
    {
        path: "/point",
        component: Point,
        beforeEnter: async () => {
            const pointStore = usePointStore();
            const postStore = usePostStore();
            const patruljeStore = usePatruljeStore();
            pointStore.GET_POINTS();
            postStore.GET_POSTS();
            patruljeStore.GET_PATRULJER();
        }
    },
    {
        path: "/poster",
        component: Post,
        beforeEnter: async () => {
            const postStore = usePostStore();
            postStore.GET_POSTS();
        }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;