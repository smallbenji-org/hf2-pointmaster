import StatsService from "@/Services/statsService";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const useStatsStore = defineStore('Stats', () => {
    const postService = new StatsService();

    const PointStats = ref<pointStats[]>([]);

    const POINT_STATS = computed(() => PointStats.value ?? []);

    async function GET_POINT_STATS() {
        var data = await postService.points();
        PointStats.value = data;
        return data;
    }


    return {
        PointStats,
        POINT_STATS,
        GET_POINT_STATS
    }
});