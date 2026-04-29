<template>
    <section class="patrulje-stats">
        <div class="patrulje-stats__summary" v-if="overview">
            <div>
                <strong>{{ overview.posts.filter(entry => entry.visited).length }}</strong>
                <span>Visited</span>
            </div>
            <div>
                <strong>{{ overview.posts.filter(entry => !entry.visited).length }}</strong>
                <span>Open</span>
            </div>
            <div>
                <strong>{{ overview.posts.length }}</strong>
                <span>Total posts</span>
            </div>
        </div>

        <div v-if="GIVPOINT_LOADING" class="patrulje-stats__state">
            Loading overview...
        </div>

        <div v-else-if="!overview" class="patrulje-stats__state">
            {{ GIVPOINT_MESSAGE ?? 'No overview data available.' }}
        </div>

        <div v-else class="patrulje-stats__grid">
            <article
                v-for="entry in overview.posts"
                :key="entry.post.id"
                class="patrulje-stats__card"
                :class="{ 'is-visited': entry.visited }"
            >
                <div class="patrulje-stats__card-header">
                    <div>
                        <p class="patrulje-stats__label">Post</p>
                        <h3>{{ entry.post.name }}</h3>
                    </div>
                    <span class="patrulje-stats__badge">
                        {{ entry.visited ? 'Visited' : 'Unvisited' }}
                    </span>
                </div>

                <div v-if="entry.point" class="patrulje-stats__metrics">
                    <div>
                        <span>Point</span>
                        <strong>{{ entry.point.points }}</strong>
                    </div>
                    <div>
                        <span>Turnout</span>
                        <strong>{{ entry.point.turnout }}</strong>
                    </div>
                </div>

                <p v-else class="patrulje-stats__missing">
                    No points recorded yet for this post.
                </p>
            </article>
        </div>
    </section>
</template>
<script lang="ts" setup>
import { computed } from 'vue';
import { storeToRefs } from 'pinia';
import { usePointStore } from '@/Modules/PointModule';

const pointStore = usePointStore();
const { GIVPOINT_OVERVIEW, GIVPOINT_LOADING, GIVPOINT_MESSAGE } = storeToRefs(pointStore);

const overview = computed(() => GIVPOINT_OVERVIEW.value);
</script>
<style lang="scss">
.patrulje-stats {
    display: grid;
    gap: 1rem;

    &__summary {
        display: grid;
        grid-template-columns: repeat(3, minmax(0, 1fr));
        gap: 0.75rem;

        div {
            background: white;
            border: 1px solid #e5e7eb;
            border-radius: 14px;
            padding: 0.85rem 1rem;
            display: grid;
            gap: 0.15rem;
        }

        strong {
            font-size: 1.35rem;
        }

        span {
            color: #6b7280;
            font-size: 0.9rem;
        }
    }

    &__state {
        border-radius: 12px;
        padding: 0.85rem 1rem;
        background: #f3f4f6;
        color: #111827;
    }

    &__grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(240px, 1fr));
        gap: 0.85rem;
    }

    &__card {
        background: white;
        border-radius: 16px;
        border: 1px solid #e5e7eb;
        padding: 1rem;
        box-shadow: 0 10px 24px rgba(15, 23, 42, 0.05);

        &.is-visited {
            border-color: #a7f3d0;
            background: #ecfdf5;
        }
    }

    &__card-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        gap: 1rem;
        margin-bottom: 0.85rem;
    }

    &__label {
        font-size: 0.75rem;
        text-transform: uppercase;
        letter-spacing: 0.08em;
        color: #6b7280;
        margin-bottom: 0.2rem;
    }

    &__badge {
        font-size: 0.75rem;
        text-transform: uppercase;
        letter-spacing: 0.08em;
        padding: 0.35rem 0.55rem;
        border-radius: 999px;
        background: #e5e7eb;
        color: #374151;
        white-space: nowrap;
    }

    &__metrics {
        display: grid;
        grid-template-columns: repeat(2, minmax(0, 1fr));
        gap: 0.75rem;

        div {
            display: grid;
            gap: 0.15rem;
        }

        span {
            font-size: 0.85rem;
            color: #6b7280;
        }

        strong {
            font-size: 1.1rem;
        }
    }

    &__missing {
        color: #6b7280;
        margin-bottom: 0;
    }
}
</style>
