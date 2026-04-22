<template>
    <div class="container tenants-page">
        <h1 class="title is-4">Events</h1>

        <div class="box">
            <h2 class="subtitle is-6">Dine events</h2>
            <BTable :data="tenants">
                <BTableColumn label="Navn" field="name" v-slot="props">
                    {{ props.row.name }}
                </BTableColumn>
                <BTableColumn label="Handling" v-slot="props">
                    <BButton type="is-primary is-small" @click="chooseTenant(props.row.id)">
                        Vælg
                    </BButton>
                </BTableColumn>
            </BTable>
        </div>

        <div class="box">
            <h2 class="subtitle is-6">Opret nyt event</h2>
            <BField label="Navn">
                <BInput v-model="newTenantName"></BInput>
            </BField>
            <BButton type="is-warning" @click="createTenant">Opret event</BButton>
        </div>
    </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/Modules/AuthModule';
import { BButton, BField, BInput, BTable, BTableColumn } from 'buefy';
import { storeToRefs } from 'pinia';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const router = useRouter();
const { TENANTS } = storeToRefs(authStore);

const newTenantName = ref('');
const tenants = TENANTS;

const chooseTenant = async (tenantId: string) => {
    authStore.SET_ACTIVE_TENANT(tenantId);
    await authStore.GET_ME();
    await router.push('/');
};

const createTenant = async () => {
    if (!newTenantName.value.trim()) {
        return;
    }

    const tenant = await authStore.CREATE_TENANT(newTenantName.value.trim());
    if (tenant) {
        await router.push('/');
    }

    newTenantName.value = '';
};
</script>

<style scoped lang="scss">
.tenants-page {
    padding-top: 1rem;
}
</style>
