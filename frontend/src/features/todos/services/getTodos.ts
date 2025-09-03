import {API_URL} from "@/config.ts";

export const getTodos = async () => {
    return await fetch(`${API_URL}/todos`);
}