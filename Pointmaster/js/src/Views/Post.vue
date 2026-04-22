<template>
    <div class="Post">
        <div class="container post-topbar">
            <h1 class="title is-4">
                Post
            </h1>
            <div class="actions">
                <BButton type="is-warning" @click="open = true">Opret Post</BButton>
            </div>
        </div>
        <div class="container">
            <BTable :data="POST">
                <BTableColumn label="ID" field="id" v-slot="props">
                    {{ props.row.id }}
                </BTableColumn>
                <BTableColumn label="Navn" field="name" v-slot="props">
                    {{ props.row.name }}
                </BTableColumn>
                <BTableColumn label="Handlinger" v-slot="props">
                    <div class="buttons">
                        <BButton type="is-small is-danger" @click="deletePost(props.row.id)">Slet</BButton>
                    </div>
                </BTableColumn>
            </BTable>
        </div>
        <BModal
            v-model="open"
            has-modal-card
            trap-focus
        >
            <div class="modal-card">
                <section class="modal-card-body">
                    <BField label="Navn">
                        <BInput v-model="newName"></BInput>
                    </BField>
                    <BButton type="is-primary" @click="createPost">Opret Post</BButton>
                </section>
            </div>
        </BModal>
    </div>
</template>
<script lang="ts" setup>
import { usePostStore } from '@/Modules/PostModule';
import { BButton, BTable, BModal, BField } from 'buefy';
import { storeToRefs } from 'pinia';
import { ref } from 'vue';

const postStore = usePostStore();
const { POST } = storeToRefs(postStore);

const open = ref(false);
const newName = ref<string>("");

const createPost = async () => {
    open.value = false;
    await postStore.ADD_POST(newName.value);
    await postStore.GET_POSTS();
    newName.value = "";
};

const deletePost = async (id: number) => {
    await postStore.DELETE_POST(id);
    await postStore.GET_POSTS();
}

</script>
<style lang="scss">
.post {
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