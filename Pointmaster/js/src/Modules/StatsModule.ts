import StatsService from "@/Services/statsService";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const useStatsStore = defineStore('Stats', () => {
    const statsService = new StatsService();

    const PointStats = ref<pointStats[]>([]);
    const LatestMatches = ref<Point[]>([]);

    const POINT_STATS = computed(() => PointStats.value ?? []);
    const LATEST_MATCHES = computed(() => LatestMatches.value ?? [])

    async function GET_POINT_STATS() {
        var data = await statsService.points();
        PointStats.value = data;
        return data;
    }

    async function GET_LATEST_MATCHES() {
        var data = await statsService.latestMatches();
        LatestMatches.value = data;
        return data;
    }

    return {
        PointStats, LatestMatches,
        POINT_STATS, LATEST_MATCHES,
        GET_POINT_STATS, GET_LATEST_MATCHES
    }
});