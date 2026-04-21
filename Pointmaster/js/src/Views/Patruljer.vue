<template>
    <div class="patruljer">
        <div class="container patruljer-topbar">
            <h1 class="title is-4">
                Patruljer
            </h1>
            <div class="actions">
                <BButton type="is-warning" @click="open = true">Opret patrulje</BButton>
            </div>
        </div>
        <div class="container">
            <BTable :data="PATRULJER" :columns="columns"></BTable>
        </div>
        <BModal
            v-model="open"
            has-modal-card
            trap-focus
        >
            <div class="modal-card">
                <section class="modal-card-body">
                    <BField label="Navn">
                        <BInput v-model="newName"></BInput>
                    </BField>
                    <BButton type="is-primary" @click="createPatrulje">Opret Patrulje</BButton>
                </section>
            </div>
        </BModal>
    </div>
</template>
<script lang="ts" setup>
import { storeToRefs } from 'pinia';
import { usePatruljeStore } from '@/Modules/PatruljeModule';
import { BButton, BField, BModal, BTable } from 'buefy';
import { ref } from 'vue';

const patruljeStore = usePatruljeStore();
const { PATRULJER } = storeToRefs(patruljeStore);

const columns = [
    {
        field: "id",
        label: "ID"
    },
    {
        field: "name",
        label: "Navn"
    }
];

const open = ref(false);

const newName = ref<string>("");

const createPatrulje = async () => {
    open.value = false;
    await patruljeStore.ADD_PATRULJE(newName.value);
    await patruljeStore.GET_PATRULJER();
    newName.value = "";
}

</script>
<style lang="scss">
.patruljer {
    // border: 1px solid rgba(0, 0, 0, 0.2);
    display: flex;
    flex-direction: column;

    th {
        text-align: left;
    }

    &-topbar {
        display: flex;
        justify-content: space-between;
        // border-top: 1px rgba(0, 0, 0, 0.8);
        // z-index: 99;
        // background-color: rgba(0,0,0, 0.05);
        padding: 1rem;
        align-items: center;
        h1 {
            margin-bottom: 0 !important;
        }
        // border-bottom: 1px solid rgba(0, 0, 0, 0.1);
    }
}
</style>