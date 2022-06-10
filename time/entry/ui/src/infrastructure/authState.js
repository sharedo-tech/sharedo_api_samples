import { reactive } from "vue";

const state = reactive(
    {
        isAuthenticated: false,
        username: null
    });

export default state;