<template>
    <div class="patruljer">
        <div class="patruljer-topbar">
            Patruljer
            <div class="actions">
                <button class="button primary">Opret patrulje</button>
            </div>
        </div>
        <table>
            <tr>
                <th>
                    Id
                </th>
                <th>
                    Navn
                </th>
            </tr>
            <tr v-for="patrulje in PATRULJER">
                <td>{{ patrulje.id }}</td>
                <td>{{ patrulje.name }}</td>
            </tr>
        </table>
    </div>
</template>
<script lang="ts" setup>
import { storeToRefs } from 'pinia';
import { usePatruljeStore } from '../Modules/PatruljeModule';

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
    }
}
</style>