import {type FormEvent, memo, useCallback, useState} from "react";
import {addTodo} from "@/features/todos/services/addTodo.ts";
import {useTodoStore} from "@/features/todos/stores/useTodoStore.ts";

interface FormElements extends HTMLFormControlsCollection {
    todoName: HTMLInputElement
}

interface FormElement extends HTMLFormElement {
    readonly elements: FormElements
}

export const AddTodoForm = () => {
    const {fetchTodos} = useTodoStore();
    
    const [todoInput, setTodoInput] = useState("");
    
    const submit = async (event: FormEvent<FormElement>) => {
        event.preventDefault();

        const response = await addTodo({
            name: todoInput
        });
        
        console.log("response", response, await response.json());

        if (response.ok) {
            await fetchTodos();
        }
    }
    
    return (
        <form onSubmit={submit}>
            <input type={"text"} value={todoInput} onChange={(e) => setTodoInput(e.currentTarget.value)} />
            <input type={"submit"} value={"Save"} />
        </form>
    )
}