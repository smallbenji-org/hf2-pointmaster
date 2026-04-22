<script setup lang="ts">
import { useAuthStore } from '@/Modules/AuthModule';
import { BNavbar, BNavbarItem } from 'buefy';
import { storeToRefs } from 'pinia';
import { computed, ref, watch } from 'vue';
import { useRoute } from 'vue-router';

const authStore = useAuthStore();
const auth = storeToRefs(authStore);

const route = useRoute();

const isLoggedIn = ref(false);
const username = ref<string | null>(null);
const selectedTenant = ref<string>('');

const tenants = computed(() => authStore.TENANTS);
const isAdmin = computed(() => authStore.IS_ADMIN);

const refreshAuthStatus = async () => {
    if (!auth.ME.value) {
        await authStore.GET_ME();
    }

    const result = auth.ME.value;
    if (result) {
        isLoggedIn.value = result.authenticated;
        username.value = result.username;
        selectedTenant.value = authStore.ACTIVE_TENANT_ID ?? '';
    }
};

const logout = async () => {
    const success = await authStore.LOGOUT();
    if (success) {
        await refreshAuthStatus();
    }
};

const onTenantChanged = async () => {
    if (!selectedTenant.value) {
        return;
    }

    authStore.SET_ACTIVE_TENANT(selectedTenant.value);
    await authStore.GET_ME();
};

watch(
    () => route.fullPath,
    async () => {
        await refreshAuthStatus();
    },
    { immediate: true }
);

</script>

<template>
    <b-navbar :shadow="true">
        <template #brand>
            <b-navbar-item tag="router-link" :to="{ path: '/' }">
                <h1 class="title">
                    PointMaster
                </h1>
            </b-navbar-item>
        </template>
        <template #end>
            <b-navbar-item tag="div" v-if="isLoggedIn">
                <router-link to="/patruljer" class="button is-primary">
                    Patruljer
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div" v-if="isLoggedIn">
                <router-link to="/poster" class="button is-primary">
                    Poster
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div" v-if="isLoggedIn">
                <router-link to="/point" class="button is-primary">
                    Point
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div" v-if="isLoggedIn">
                <router-link to="/tenants" class="button is-info">
                    Events
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div" v-if="isLoggedIn && isAdmin">
                <router-link to="/members" class="button is-link">
                    Medlemmer
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div" v-if="isLoggedIn">
                <div class="select is-small">
                    <select v-model="selectedTenant" @change="onTenantChanged">
                        <option value="" disabled>Vaelg event</option>
                        <option v-for="tenant in tenants" :key="tenant.id" :value="tenant.id">
                            {{ tenant.name }}
                        </option>
                    </select>
                </div>
            </b-navbar-item>
            <b-navbar-item tag="div" v-if="!isLoggedIn">
                <router-link to="/login" class="button is-light">
                    Login
                </router-link>
            </b-navbar-item>
            <b-navbar-item tag="div">
                <router-link to="/register" class="button is-warning" v-if="!isLoggedIn">
                    Register
                </router-link>
                <button class="button is-danger" v-else @click="logout">
                    Logout
                </button>
            </b-navbar-item>
        </template>
    </b-navbar>
</template>
<style lang="scss">
.auth-state {
    font-size: 0.95rem;
    color: #2f855a;
    font-weight: 600;
}

.is-offline {
    color: #4a5568;
}

// .topbar {
//     // border-bottom: 1px solid black;
//     box-shadow: 0 5px 5px 0 rgba(0, 0, 0, 0.2);
//     padding: 1rem;
//     display: flex;
//     z-index: 1;
//     a {
//         // color: black;
//         text-decoration: none;
//     }

//     &-links {
//         margin-left: auto;
//         display: flex;
//         gap: 5px;
//     }
// }
</style>