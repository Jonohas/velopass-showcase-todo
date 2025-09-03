import {API_URL} from "@/config.ts";

export const getTodos = async () => {
    console.log("getTodos", API_URL);
    
    return await fetch(`${API_URL}/todos`);
}