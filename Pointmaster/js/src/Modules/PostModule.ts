import PostService from "@/Services/PostService";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const usePostStore = defineStore('Post', () => {
    const postService = new PostService();

    const Post = ref<Post[]>([]);

    const POST = computed(() => Post.value ?? []);

    async function GET_POSTS() {
        const data = await postService.getAll();
        Post.value = data;

        return data;
    }

    async function ADD_POST(name: string) {
        const data = await postService.addPost(name);

        return data;
    }

    async function DELETE_POST(id: number) {
        const data = await postService.deletePost(id);

        return data;
    }

    return {
        Post,
        POST,
        GET_POSTS, ADD_POST, DELETE_POST
    }
});