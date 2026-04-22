<template>
    <div class="members container">
        <div class="members-topbar">
            <h1 class="title is-4">Medlemmer</h1>
            <BButton type="is-warning" @click="open = true">Tilfoej bruger</BButton>
        </div>

        <BTable :data="Members">
            <BTableColumn label="Bruger" field="username" v-slot="props">
                {{ props.row.username }}
            </BTableColumn>
            <BTableColumn label="Rolle" field="roleName" v-slot="props">
                <div class="is-flex is-align-items-center is-gap-2">
                    <span>{{ props.row.roleName }}</span>
                    <div class="select is-small">
                        <select
                            :value="props.row.roleName"
                            @change="updateRole(props.row.userId, ($event.target as HTMLSelectElement).value as TenantRole)">
                            <option value="Administrator">Administrator</option>
                            <option value="PostUser">PostUser</option>
                        </select>
                    </div>
                </div>
            </BTableColumn>
        </BTable>

        <BModal v-model="open" has-modal-card trap-focus>
            <div class="modal-card">
                <section class="modal-card-body">
                    <BField label="Brugernavn">
                        <BInput v-model="newUsername"></BInput>
                    </BField>
                    <BField label="Rolle">
                        <BSelect v-model="newRole" expanded>
                            <option value="Administrator">Administrator</option>
                            <option value="PostUser">PostUser</option>
                        </BSelect>
                    </BField>
                    <BButton type="is-primary" @click="addMember">Tilfoej</BButton>
                </section>
            </div>
        </BModal>
    </div>
</template>

<script setup lang="ts">
import { useAuthStore } from '@/Modules/AuthModule';
import { BButton, BField, BInput, BModal, BSelect, BTable, BTableColumn } from 'buefy';
import { storeToRefs } from 'pinia';
import { ref } from 'vue';

const authStore = useAuthStore();
const { Members } = storeToRefs(authStore);

const open = ref(false);
const newUsername = ref('');
const newRole = ref<TenantRole>('PostUser');

const addMember = async () => {
    if (!newUsername.value) {
        return;
    }

    await authStore.ADD_MEMBER(newUsername.value, newRole.value);
    newUsername.value = '';
    newRole.value = 'PostUser';
    open.value = false;
};

const updateRole = async (userId: string, roleName: TenantRole) => {
    await authStore.UPDATE_MEMBER_ROLE(userId, roleName);
};
</script>

<style scoped lang="scss">
.members {
    padding-top: 1rem;
}

.members-topbar {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 1rem;
}
</style>
