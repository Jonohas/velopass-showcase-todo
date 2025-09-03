import { create } from 'zustand'
import type {TodoItemType} from "@/features/todos/types/TodoItemType.ts";
import {getTodos} from "@/features/todos/services/getTodos.ts";

interface TodoState {
    loading: boolean
    todos: TodoItemType[];
    setTodos: (todos: TodoItemType[]) => void;
    fetchTodos: () => Promise<void>;
}

export const useTodoStore = create<TodoState>((set) => ({
    loading: false,
    todos: [],
    setTodos: (todos) => set({ todos }),
    fetchTodos: async () => {
        set({ loading: true });
        const todosResult = await getTodos();
        if (todosResult.ok) {
            const todosArray = await todosResult.json();
            set({
                todos: todosArray,
                loading: false,
            })
        }
    }
}))