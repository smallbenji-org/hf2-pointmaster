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
import { useAuthStore } from "@/Modules/AuthModule";
import Members from "@/Views/Members.vue";
import Tenants from "@/Views/Tenants.vue";

const routes: RouteRecordRaw[] = [
    {
        path: "/",
        component: Home,
        beforeEnter: async () => {
            const statsStore = useStatsStore();
            await statsStore.GET_POINT_STATS();
        },
        meta: {
            requiresAuth: true,
            requiresTenant: true
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
            requiresAuth: true,
            requiresTenant: true,
            allowRoles: ['Administrator', 'PostUser']
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
            requiresAuth: true,
            requiresTenant: true,
            allowRoles: ['Administrator', 'PostUser']
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
            requiresAuth: true,
            requiresTenant: true,
            allowRoles: ['Administrator', 'PostUser']
        }
    },
    {
        path: "/members",
        component: Members,
        beforeEnter: async () => {
            const authStore = useAuthStore();
            await authStore.LOAD_MEMBERS();
        },
        meta: {
            requiresAuth: true,
            requiresTenant: true,
            allowRoles: ['Administrator']
        }
    },
    {
        path: "/tenants",
        component: Tenants,
        beforeEnter: async () => {
            const authStore = useAuthStore();
            await authStore.REFRESH_TENANTS();
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

router.beforeEach(async (to, _from, next) => {
    const authStore = useAuthStore();

    if (!authStore.ME) {
        await authStore.GET_ME();
    }

    const result = authStore.ME;

    const isProtected = to.matched.some(record => record.meta.requiresAuth);
    const requiresTenant = to.matched.some(record => record.meta.requiresTenant);
    const roleRequirements = to.matched
        .flatMap(record => (record.meta.allowRoles as TenantRole[] | undefined) ?? []);

    const isAuthenticated = result?.authenticated ?? false;
    const activeTenant = authStore.ACTIVE_TENANT_ID;
    const role = authStore.ROLE;
    const isSuperUser = result?.isSuperUser ?? false;

    if (isProtected && !isAuthenticated) {
        return next({ path: "/login"});
    }

    if (isProtected && requiresTenant && !activeTenant) {
        return next({ path: "/tenants" });
    }

    if (isProtected && roleRequirements.length > 0 && !isSuperUser) {
        const allowed = roleRequirements.some(r => r === role);
        if (!allowed) {
            return next({ path: "/" });
        }
    }

    return next();
});

export default router;