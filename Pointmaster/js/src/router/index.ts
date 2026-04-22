import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import Home from "@/Views/Home.vue";
import { usePatruljeStore } from "@/Modules/PatruljeModule";
import Patruljer from "@/Views/Patruljer.vue";
import Point from "@/Views/Point.vue";
import { usePointStore } from "@/Modules/PointModule";
import Post from "@/Views/Post.vue";
import { usePostStore } from "@/Modules/PostModule";
import { useStatsStore } from "@/Modules/StatsModule";

const routes: RouteRecordRaw[] = [
    {
        path: "/",
        component: Home,
        beforeEnter: async () => {
            const statsStore = useStatsStore();
            await statsStore.GET_POINT_STATS();
        }
    },
    {
        path: "/patruljer",
        component: Patruljer,
        beforeEnter: async () => {
            const patruljeStore = usePatruljeStore();
            await patruljeStore.GET_PATRULJER();
        }
    },
    {
        path: "/point",
        component: Point,
        beforeEnter: async () => {
            const pointStore = usePointStore();
            const postStore = usePostStore();
            const patruljeStore = usePatruljeStore();
            await pointStore.GET_POINTS();
            await postStore.GET_POSTS();
            await patruljeStore.GET_PATRULJER();
        }
    },
    {
        path: "/poster",
        component: Post,
        beforeEnter: async () => {
            const postStore = usePostStore();
            await postStore.GET_POSTS();
        }
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
});

export default router;