import { defineStore } from "pinia";
import PatruljeService from "../Services/PatruljeService";
import { computed, ref } from "vue";

export const usePatruljeStore = defineStore('Patrulje', () => {
    const patruljeService = new PatruljeService();

    const Patruljer = ref<Patrulje[]>([]);

    const PATRULJER = computed(() => Patruljer.value ?? []);

    async function GET_PATRULJER() {
        const data = await patruljeService.getAll();
        Patruljer.value = data ?? [];

        return data;
    }

    async function ADD_PATRULJE(name: string){
        const data = await patruljeService.addPatrulje(name);
        return data;
    }

    return {
        Patruljer,
        PATRULJER,
        GET_PATRULJER, ADD_PATRULJE
    }
});