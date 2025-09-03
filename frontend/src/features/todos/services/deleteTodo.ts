import {API_URL} from "@/config.ts";

type DeleteTodoRequest = {
    id: string;
}

export const deleteTodo = async (request: DeleteTodoRequest) => {
    const response = await fetch(`${API_URL}/todos/${request.id}`, {
        method: "DELETE"
    });

    return response;
}