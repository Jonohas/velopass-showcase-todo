import {API_URL} from "@/config.ts";

type UpdateTodoRequest = {
    id: string;
    done: boolean;
    name?: string;
};

export const updateTodo = async (request: UpdateTodoRequest) => {
    const response = await fetch(`${API_URL}/todos/${request.id}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify(request),
    });

    return response;
}