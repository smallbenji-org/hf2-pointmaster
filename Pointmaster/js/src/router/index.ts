import { createRouter, createWebHistory, type RouteRecordRaw } from "vue-router";
import Home from "@/Views/Home.vue";
import { usePatruljeStore } from "@/Modules/PatruljeModule";
import Patruljer from "@/Views/Patruljer.vue";
import Point from "@/Views/Point.vue";
import { usePointStore } from "@/Modules/PointModule";
import Post from "@/Views/Post.vue";
import { usePostStore } from "@/Modules/PostModule";
import { useStatsStore } from "@/Modules/StatsModule";
import Login from "@/Views/Login.vue";
import Register from "@/Views/Register.vue";
import AuthService from "@/Services/AuthService";

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
        },
        meta: {
            requiresAuth: true
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
        },
        meta: {
            requiresAuth: true
        }
    },
    {
        path: "/poster",
        component: Post,
        beforeEnter: async () => {
            const postStore = usePostStore();
            await postStore.GET_POSTS();
        },
        meta: {
            requiresAuth: true
        }
    },
    {
        path: "/login",
        component: Login
    },
    {
        path: "/register",
        component: Register
    }
]

const router = createRouter({
    history: createWebHistory(),
    routes
});

router.beforeEach(async (to, from, next) => {
    const authService = new AuthService();
    const result = await authService.me();

    const isProtected = to.matched.some(record => record.meta.requiresAuth);
    const isAuthenticated = result.authenticated;

    if (isProtected && !isAuthenticated) {
        next({ path: "/login"})
    } else {
        next();
    }
});

export default router;