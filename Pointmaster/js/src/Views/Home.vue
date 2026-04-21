<template>
    <div class="home">
        <h1>Welcome home</h1>
        <p v-for="patrulje in PATRULJER">
            {{ patrulje.name }}
        </p>
    </div>
</template>
<script lang="ts" setup>
import { storeToRefs } from 'pinia';
import { usePatruljeStore } from '@/Modules/PatruljeModule';

const patruljeStore = usePatruljeStore();
const { PATRULJER } = storeToRefs(patruljeStore);
</script>