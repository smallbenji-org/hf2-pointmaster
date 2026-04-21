<template>
    <div class="point">
        <div class="container point-topbar">
            <h1 class="title is-4">
                Point
            </h1>
            <div class="actions">
                <BButton type="is-warning" @click="open = true">Opret Point</BButton>
            </div>
        </div>
        <div class="container">
            <BTable :data="POINT" :columns="columns"></BTable>
        </div>
        <BModal
            v-model="open"
            has-modal-card
            trap-focus
        >
            <div class="modal-card">
                <section class="modal-card-body">
                    <BField label="Point">
                        <BInput type="number" v-model="data.point"></BInput>
                    </BField>
                    <BField label="Turnout">
                        <BInput type="number" v-model="data.turnout"></BInput>
                    </BField>
                    <BField label="Patrulje">
                        <BSelect label="Patrulje" expanded v-model="data.patrulje">
                            <option
                                v-for="patrulje in PATRULJER"
                                :value="patrulje.id"
                                :key="patrulje.id+patrulje.name"
                            >
                                {{ patrulje.name }}
                            </option>
                        </BSelect>
                    </BField>
                    <BField label="Post">
                        <BSelect label="Post" expanded v-model="data.post">
                            <option
                            v-for="post in POST"
                            :value="post.id"
                            :key="post.id+post.name"
                            >
                                {{ post.name }}
                            </option>
                        </BSelect>
                    </BField>
                    <BButton type="is-primary" @click="createPoint">
                        Opret Point
                    </BButton>
                </section>
            </div>
        </BModal>
    </div>
</template>
<script lang="ts" setup>
import { usePatruljeStore } from '@/Modules/PatruljeModule';
import { usePointStore } from '@/Modules/PointModule';
import { usePostStore } from '@/Modules/PostModule';
import { BButton, BTable, BModal, BSelect, BInput, BField } from 'buefy';
import { storeToRefs } from 'pinia';
import { ref } from 'vue';

const pointStore = usePointStore();
const patruljeStore = usePatruljeStore();
const postStore = usePostStore();
const { POINT } = storeToRefs(pointStore);
const { PATRULJER } = storeToRefs(patruljeStore);
const { POST } = storeToRefs(postStore);

const columns = [
    {
        field: "id",
        label: "ID"
    },
    {
        field: "points",
        label: "Point"
    },
    {
        field: "turnout",
        label: "Turnout"
    },
    {
        field: "patrulje.name",
        label: "Patrulje"
    },
    {
        field: "post.name",
        label: "Post"
    }
];

const open = ref(false);
const data = ref<PointDTO>({
    patrulje: 0,
    point: 0,
    post: 0,
    turnout: 0
});

const createPoint = async () => {
    if (data.value){
        await pointStore.CREATE_POINT(data.value);
    }

    data.value = {
        patrulje: 0,
        point: 0,
        post: 0,
        turnout: 0
    };

    open.value = false;

    await pointStore.GET_POINTS();
}

</script>
<style lang="scss">
.point {
    &-topbar {
        display: flex;
        justify-content: space-between;
        padding: 1rem;
        align-items: center;

        h1 {
            margin-bottom: 0 !important;
        }
    }
}
</style>