<template>
    <div class="givpoint-page">
        <div class="container givpoint-shell">
            <div v-if="GIVPOINT_LOADING" class="givpoint-state givpoint-state--loading">
                <p>Loading patrulje data...</p>
            </div>

            <div v-else-if="!overview || !overview.patrulje" class="givpoint-state givpoint-state--empty">
                <p>{{ GIVPOINT_MESSAGE ?? 'No patrulje data is available for the selected tenant.' }}</p>
            </div>

            <template v-else>
                <header class="givpoint-header">
                    <div>
                        <p class="givpoint-kicker">GivPoint</p>
                        <h1 class="title is-3">{{ overview.patrulje?.name ?? 'Unknown patrulje' }}</h1>
                        <!-- <p class="givpoint-subtitle">
                            {{ isAuthenticated ? 'Choose a post, review the current values, and submit an update.' : 'Public tenant-scoped overview of visited posts and recorded points.' }}
                        </p> -->
                    </div>

                    <!-- <BMessage v-if="GIVPOINT_MESSAGE" type="is-info" has-icon :closable="false" class="givpoint-message">
                        {{ GIVPOINT_MESSAGE }}
                    </BMessage> -->
                </header>

                <PatruljeStats v-if="!isAuthenticated" />

                <section v-else class="givpoint-auth-grid">
                    <!-- <article class="givpoint-card givpoint-card--posts">
                        <div class="givpoint-card__header">
                            <h2 class="title is-5">Selected Post</h2>
                            <p>Post selection is locked on this page.</p>
                        </div>

                        <BMessage v-if="selectedPostEntry" type="is-primary" :closable="false" has-icon>
                            <p class="title is-6">{{ selectedPostEntry.post.name }}</p>
                            <p>{{ selectedPostEntry.visited ? 'Existing point data available.' : 'No point data yet for this post.' }}</p>
                        </BMessage>

                        <BMessage v-else type="is-danger" :closable="false" has-icon>
                            <p class="title is-6">No post is set</p>
                            <p>Set a post through <strong>/selectpost/:postId</strong> before using GivPoint.</p>
                        </BMessage>
                    </article> -->

                    <article class="givpoint-card givpoint-card--editor">
                        <template v-if="selectedPostEntry">
                            <div class="givpoint-card__header">
                                <h2 class="title is-5">Editing {{ selectedPostEntry.post.name }}</h2>
                                <p>{{ selectedPostEntry.visited ? 'Existing values are prefilled below.' : 'Create the first point entry for this post.' }}</p>
                            </div>

                            <BField label="Point">
                                <BInput v-model="form.point" type="number" min="0"></BInput>
                            </BField>

                            <BField label="Turnout">
                                <BInput v-model="form.turnout" type="number" min="0"></BInput>
                            </BField>

                            <div class="buttons is-right">
                                <BButton type="is-warning" :loading="saving" @click="savePoint">
                                    Save point
                                </BButton>
                            </div>
                        </template>

                        <BMessage v-else type="is-warning" :closable="false" has-icon>
                            <p class="title is-5">No post selected</p>
                            <p>Set a post through <strong>/selectpost/:postId</strong> to load and submit values.</p>
                        </BMessage>
                    </article>
                </section>
            </template>
        </div>
    </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref, watch } from 'vue';
import { storeToRefs } from 'pinia';
import { useRoute } from 'vue-router';
import { BButton, BField, BInput, BMessage } from 'buefy';
import PatruljeStats from '@/components/GivPoint/PatruljeStats.vue';
import { useAuthStore } from '@/Modules/AuthModule';
import { usePointStore } from '@/Modules/PointModule';
import { usePostStore } from '@/Modules/PostModule';

const route = useRoute();
const authStore = useAuthStore();
const pointStore = usePointStore();
const postStore = usePostStore();

const { ME } = storeToRefs(authStore);
const { SELECTED_POST_ID } = storeToRefs(postStore);
const { GIVPOINT_OVERVIEW, GIVPOINT_MESSAGE, GIVPOINT_LOADING } = storeToRefs(pointStore);

const patruljeId = computed(() => Number(route.params.patruljeId));
const isAuthenticated = computed(() => ME.value?.authenticated ?? false);
const overview = computed(() => GIVPOINT_OVERVIEW.value);
const selectedPostId = computed(() => SELECTED_POST_ID.value);
const saving = ref(false);
const form = ref<GivPointUpdateDTO>({
    post: 0,
    point: 0,
    turnout: 0
});

const selectedPostEntry = computed(() => {
    if (!overview.value || !selectedPostId.value) {
        return null;
    }

    return overview.value.posts.find(entry => String(entry.post.id) === selectedPostId.value) ?? null;
});

function syncForm() {
    if (!selectedPostEntry.value) {
        form.value = {
            post: selectedPostId.value ? Number(selectedPostId.value) : 0,
            point: 0,
            turnout: 0
        };
        return;
    }

    form.value = {
        post: selectedPostEntry.value.post.id,
        point: selectedPostEntry.value.point?.points ?? 0,
        turnout: selectedPostEntry.value.point?.turnout ?? 0
    };
}

async function loadOverview() {
    if (Number.isNaN(patruljeId.value)) {
        return;
    }

    await pointStore.GET_GIVPOINT_OVERVIEW(patruljeId.value);
}

async function savePoint() {
    if (!selectedPostId.value) {
        return;
    }

    saving.value = true;
    const result = await pointStore.SAVE_GIVPOINT(patruljeId.value, {
        post: Number(selectedPostId.value),
        point: Number(form.value.point),
        turnout: Number(form.value.turnout)
    });

    if (result.success) {
        await loadOverview();
    }

    saving.value = false;
}

watch([selectedPostEntry, selectedPostId], () => {
    syncForm();
}, { immediate: true });

onMounted(async () => {
    await loadOverview();
});
</script>

<style lang="scss">
.givpoint-page {
    padding: 1.5rem 0 2rem;
}

.givpoint-shell {
    display: grid;
    gap: 1.25rem;
}

.givpoint-header {
    display: flex;
    justify-content: space-between;
    gap: 1rem;
    align-items: flex-start;
    flex-wrap: wrap;
}

.givpoint-kicker {
    text-transform: uppercase;
    letter-spacing: 0.08em;
    font-size: 0.75rem;
    color: #6b7280;
    margin-bottom: 0.25rem;
}

.givpoint-subtitle {
    max-width: 56rem;
    color: #4b5563;
}

.givpoint-message,
.givpoint-state {
    border-radius: 12px;
    padding: 0.85rem 1rem;
    background: #f3f4f6;
    color: #111827;
}

.givpoint-auth-grid {
    display: grid;
    grid-template-columns: 1.1fr 0.9fr;
    gap: 1rem;
}

.givpoint-card {
    background: white;
    border: 1px solid #e5e7eb;
    border-radius: 16px;
    padding: 1rem;
    box-shadow: 0 14px 32px rgba(15, 23, 42, 0.06);
}

.givpoint-card__header {
    margin-bottom: 0.85rem;
}

.givpoint-empty-selection {
    min-height: 100%;
    display: grid;
    place-content: center;
    gap: 0.5rem;
    text-align: center;
    color: #4b5563;
}

@media (max-width: 900px) {
    .givpoint-auth-grid {
        grid-template-columns: 1fr;
    }
}

@media (max-width: 600px) {
    .givpoint-card {
        padding: 0.85rem;
    }
}
</style>
