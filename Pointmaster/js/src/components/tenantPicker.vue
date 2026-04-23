<template>
    <div class="select is-small" v-if="ME?.authenticated">
        <select v-model="selectedTenant" @change="onTenantChanged">
            <option value="" disabled>Vælg event</option>
            <option v-for="tenant in TENANTS" :Key="tenant.id" :value="tenant.id">
                {{ tenant.name }}
            </option>
        </select>
    </div>
</template>
<script lang="ts" setup>
import { useAuthStore } from '@/Modules/AuthModule';
import { storeToRefs } from 'pinia';
import { ref, watch } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();

const authStore = useAuthStore();
const { TENANTS, ME} = storeToRefs(authStore);

const selectedTenant = ref<string>('');

const refreshAuthStatus = async () => {
    selectedTenant.value = authStore.ACTIVE_TENANT_ID ?? '';
}

watch(
    () => route.fullPath,
    async () => {
        await refreshAuthStatus();
    },
    { immediate: true}
)

const onTenantChanged = async () => {
    if (!selectedTenant.value) {
        return;
    }

    authStore.SET_ACTIVE_TENANT(selectedTenant.value);
    await authStore.GET_ME();
};
</script>