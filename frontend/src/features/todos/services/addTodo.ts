import {API_URL} from "@/config.ts";

type AddTodoRequest = {
    name: string;
}

export const addTodo = async (request: AddTodoRequest) => {
    const response = await fetch(`${API_URL}/todos`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });

    return response;
}