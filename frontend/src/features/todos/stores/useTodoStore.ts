import { create } from 'zustand'
import type {TodoItemType} from "@/features/todos/types/TodoItemType.ts";
import {getTodos} from "@/features/todos/services/getTodos.ts";

interface TodoState {
    todos: TodoItemType[];
    setTodos: (todos: TodoItemType[]) => void;
    fetchTodos: () => Promise<void>;
}

export const useTodoStore = create<TodoState>((set) => ({
    todos: [],
    setTodos: (todos) => set({ todos }),
    fetchTodos: async () => {
        const todosResult = await getTodos();
        if (todosResult.ok) {
            const todosArray = await todosResult.json();
            set({
                todos: todosArray.sort((a: TodoItemType, b: TodoItemType) => {
                    if (a.done !== b.done) {
                        return a.done ? 1 : -1;
                    }
                    return new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime();
                })
            })
        }
    }
}))