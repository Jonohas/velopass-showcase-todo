import {type FormEvent, memo, useCallback, useState} from "react";
import {addTodo} from "@/features/todos/services/addTodo.ts";
import {useTodoStore} from "@/features/todos/stores/useTodoStore.ts";
import classNames from "classnames";

interface FormElements extends HTMLFormControlsCollection {
    todoName: HTMLInputElement
}

interface FormElement extends HTMLFormElement {
    readonly elements: FormElements
}

export const AddTodoForm = () => {
    const {todos, fetchTodos} = useTodoStore();
    const [todoInput, setTodoInput] = useState("");
    
    const submit = useCallback((event: FormEvent<FormElement>) => {
        event.preventDefault();
        
        if (todoInput.length === 0) {
            return;
        }
        
        const sendRequest = async () => {
            const result = await addTodo({
                name: todoInput
            });
            
            const resultJson = await result.json();
            setTodoInput("");
            await fetchTodos();
        }

        sendRequest();
    }, [todoInput, fetchTodos, todos])
    
    return (
        <form className={"form w-full flex"} onSubmit={submit}>
            <input className={"flex-1"} type={"text"} value={todoInput} onChange={(e) => setTodoInput(e.currentTarget.value)} />
            <input type={"submit"}  className={"submit"} value={"Add"}></input>
        </form>
    )
}