import PostService from "@/Services/PostService";
import { defineStore } from "pinia";
import { computed, ref } from "vue";

export const usePostStore = defineStore('Post', () => {
    const postService = new PostService();

    const Post = ref<Post[]>([]);
    const SelectedPostId = ref<string | null>(localStorage.getItem('pm_selected_post_id'));

    const POST = computed(() => Post.value ?? []);
    const SELECTED_POST_ID = computed(() => SelectedPostId.value);

    async function GET_POSTS() {
        const data = await postService.getAll();
        Post.value = data;

        return data;
    }

    function SET_SELECTED_POST(postId: string | number | null) {
        if (postId == null) {
            SelectedPostId.value = null;
            localStorage.removeItem('pm_selected_post_id');
            return;
        }

        const value = String(postId);
        SelectedPostId.value = value;
        localStorage.setItem('pm_selected_post_id', value);
    }

    function CLEAR_SELECTED_POST() {
        SET_SELECTED_POST(null);
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
        SelectedPostId,
        SELECTED_POST_ID,
        GET_POSTS,
        ADD_POST,
        DELETE_POST,
        SET_SELECTED_POST,
        CLEAR_SELECTED_POST
    }
});