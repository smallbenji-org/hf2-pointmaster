import PointService from "@/Services/PointService";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const usePointStore = defineStore('Point', () => {
    const pointService = new PointService();

    const Point = ref<Point[]>([]);

    const POINT = computed(() => Point.value ?? []);

    async function GET_POINTS() {
        const data = await pointService.getAll();
        Point.value = data;

        return data;
    }

    async function CREATE_POINT(data: PointDTO) {
        const retval = await pointService.createPoint(data);

        return retval;
    }

    async function DELETE_POINT(data: number) {
        const retval = await pointService.deletePoint(data);

        return retval;
    }

    return {
        Point,
        POINT,
        GET_POINTS, CREATE_POINT, DELETE_POINT
    }
});