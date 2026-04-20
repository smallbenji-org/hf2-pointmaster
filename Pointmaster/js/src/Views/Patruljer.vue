<template>
    <div class="patruljer">
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