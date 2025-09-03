import {memo, useCallback} from "react";
import type {TodoItemType} from "@/features/todos/types/TodoItemType.ts";
import {updateTodo} from "@/features/todos/services/updateTodo.ts";
import {useTodoStore} from "@/features/todos/stores/useTodoStore.ts";
import classNames from "classnames";
import {Checkbox} from "@/features/todos/components/Checkbox.tsx";
import {deleteTodo} from "@/features/todos/services/deleteTodo.ts";

type TodoItemProps = {
    todo: TodoItemType;
}

export const TodoItem = memo((props: TodoItemProps) => {
    const {todo} = props;
    const {fetchTodos} = useTodoStore();
    
    const updateFinishedState = useCallback(() => {
        const update = async () => {
            const result = await updateTodo({
                id: todo.id,
                done: !todo.done
            });
            
            if (result.ok) {
                fetchTodos();
            }
        }
        
        update();
    }, [todo, fetchTodos, todo.id, todo.done])
    
    const removeTodo = useCallback(async () => {
        const result = await deleteTodo({
            id: todo.id
        })
        
        if (result.ok) {
            fetchTodos();
        }
    }, [fetchTodos, todo.id])
    
    return (
        <div className={classNames("text-body1", "todoItem", todo.done ? "done" : null)}>
            <Checkbox
                label={todo.name}
                checked={todo.done}
                onChange={() => updateFinishedState()}
                />
            <button onClick={removeTodo}>remove</button>
        </div>
    )
})