<template>
    <div class="home">
        <div class="hero is-info">
            <div class="hero-body">
                <p class="title is-4">Velkommen tilbage</p>
            </div>
        </div>
        <section class="section">
            <div class="container">

                <div class="grid is-col-min-10">
                    <div class="cell">
                        <div class="box">
                            <p class="subtitle is-5">Turnout</p>
                            <hr>
                            <p v-for="item in turnoutSorted" :key="item.patruljeName">
                                <strong>{{ item.patruljeName }}:</strong> {{ item.totalTurnout }}
                            </p>
                        </div>
                    </div>

                    <div class="cell">
                        <div class="box">
                            <p class="subtitle is-5">Samlet Score</p>
                            <hr>
                            <p v-for="item in totalSorted" :key="item.patruljeName">
                                <strong>{{ item.patruljeName }}:</strong> {{ item.combinedTotal }}
                            </p>
                        </div>
                    </div>

                    <div class="cell">
                        <div class="box">
                            <p class="subtitle is-5">Post Point</p>
                            <hr>
                            <p v-for="item in pointsSorted" :key="item.patruljeName">
                                <strong>{{ item.patruljeName }}:</strong> {{ item.totalPoints }}
                            </p>
                        </div>
                    </div>
                </div>
            </div>
        </section>
        <section class="section">
            <div class="container">
                <p class="title is-6">
                    Sidste 5 kampe
                </p>
                <p v-for="match in LATEST_MATCHES">
                    {{ match.patrulje.name }} - {{ match.post.name }} - p: {{ match.points }} | t: {{ match.turnout }} | s: {{ match.points + match.turnout }}
                </p>
            </div>
        </section>
    </div>
</template>
<script lang="ts" setup>
import { useStatsStore } from '@/Modules/StatsModule';
import { storeToRefs } from 'pinia';
import { computed } from 'vue';

const statsStore = useStatsStore();
const { POINT_STATS, LATEST_MATCHES } = storeToRefs(statsStore);

const pointsSorted = computed(() =>
    [...POINT_STATS.value].sort((a, b) => b.totalPoints - a.totalPoints)
);

const turnoutSorted = computed(() =>
    [...POINT_STATS.value].sort((a, b) => b.totalTurnout - a.totalTurnout)
);

const totalSorted = computed(() =>
    [...POINT_STATS.value].sort((a, b) => b.combinedTotal - a.combinedTotal)
);
</script>