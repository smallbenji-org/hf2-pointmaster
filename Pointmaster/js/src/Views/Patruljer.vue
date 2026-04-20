<template>
    <div class="patruljer">
        <div class="patruljer-topbar">
            Patruljer
            <div class="actions">
                <BButton type="primary">Opret patrulje</BButton>
            </div>
        </div>
        <BTable :data="PATRULJER" :columns="[{ field: 'id' }, { field: 'name' }]">

        </BTable>
    </div>
</template>
<script lang="ts" setup>
import { storeToRefs } from 'pinia';
import { usePatruljeStore } from '../Modules/PatruljeModule';
import { BButton } from 'buefy';

const patruljeStore = usePatruljeStore();
const { PATRULJER } = storeToRefs(patruljeStore);
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
        border-top: 1px rgba(0, 0, 0, 0.8);
        z-index: 99;
        background-color: rgba(0,0,0, 0.05);
        padding: 1rem;
        align-items: center;
        border-bottom: 1px solid rgba(0, 0, 0, 0.1);
    }
}
</style>