import PointService from "@/Services/PointService";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const usePointStore = defineStore('Point', () => {
    const pointService = new PointService();

    const Point = ref<Point[]>([]);
    const GivPointOverview = ref<GivPointOverview | null>(null);
    const GivPointMessage = ref<string | null>(null);
    const GivPointLoading = ref(false);

    const POINT = computed(() => Point.value ?? []);
    const GIVPOINT_OVERVIEW = computed(() => GivPointOverview.value);
    const GIVPOINT_MESSAGE = computed(() => GivPointMessage.value);
    const GIVPOINT_LOADING = computed(() => GivPointLoading.value);

    async function GET_POINTS() {
        const result = await pointService.getAll();
        Point.value = result.data ?? [];

        return result;
    }

    async function CREATE_POINT(data: PointDTO) {
        const retval = await pointService.createPoint(data);

        return retval;
    }

    async function DELETE_POINT(data: number) {
        const retval = await pointService.deletePoint(data);

        return retval;
    }

    async function GET_GIVPOINT_OVERVIEW(patruljeId: number) {
        GivPointLoading.value = true;
        const result = await pointService.getGivPointOverview(patruljeId);
        GivPointOverview.value = result.data;
        GivPointMessage.value = result.message;
        GivPointLoading.value = false;

        return result;
    }

    async function SAVE_GIVPOINT(patruljeId: number, data: GivPointUpdateDTO) {
        const result = await pointService.saveGivPoint(patruljeId, data);
        GivPointMessage.value = result.message;

        return result;
    }

    return {
        Point,
        POINT,
        GivPointOverview,
        GIVPOINT_OVERVIEW,
        GIVPOINT_MESSAGE,
        GIVPOINT_LOADING,
        GET_POINTS,
        CREATE_POINT,
        DELETE_POINT,
        GET_GIVPOINT_OVERVIEW,
        SAVE_GIVPOINT
    }
});