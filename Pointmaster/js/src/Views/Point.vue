<template>
    <div class="point">
        <div class="container point-topbar">
            <h1 class="title is-4">
                Point
            </h1>
            <div class="actions">
                <BButton type="is-warning">Opret Point</BButton>
            </div>
        </div>
        <div class="container">
            <BTable :data="POINT" :columns="columns"></BTable>
        </div>
    </div>
</template>
<script lang="ts" setup>
import { usePointStore } from '@/Modules/PointModule';
import { BButton, BTable } from 'buefy';
import { storeToRefs } from 'pinia';

const pointStore = usePointStore();
const { POINT } = storeToRefs(pointStore);

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

</script>
<style lang="scss">
.point {
    &-topbar {
        display: flex;
        justify-content: space-between;
        padding: 1rem;
        align-items: center;

        h1 {
            margin-bottom: 0 !important;
        }
    }
}
</style>