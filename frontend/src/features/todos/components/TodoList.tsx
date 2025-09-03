import {useEffect} from "react";
import {getTodos} from "@/features/todos/services/getTodos.ts";
import {TodoItem} from "@/features/todos/components/TodoItem.tsx";
import {useTodoStore} from "@/features/todos/stores/useTodoStore.ts";

export const TodoList = () => {
    const {todos, fetchTodos} = useTodoStore();
    
    useEffect(() => {
        fetchTodos();
    }, [fetchTodos])
    
    return (
        <div className={"todoList"}>
            {todos.map((todo, index) => <TodoItem key={index} todo={todo} />)}
        </div>
    )
}