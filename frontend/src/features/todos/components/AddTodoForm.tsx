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
    const {fetchTodos} = useTodoStore();
    
    const [todoInput, setTodoInput] = useState("");
    const [hasError, setHasError] = useState(false)
    
    const submit = async (event: FormEvent<FormElement>) => {
        event.preventDefault();
        
        if (todoInput.length === 0) {
            setHasError(true);
            return;
        }

        const response = await addTodo({
            name: todoInput
        });
        
        console.log("response", response, await response.json());

        if (response.ok) {
            await fetchTodos();
        }
    }
    
    return (
        <form onSubmit={submit} className={"w-full flex"}>
            <input className={classNames(hasError ? "inputError" : null, "flex-1")} type={"text"} value={todoInput} onChange={(e) => setTodoInput(e.currentTarget.value)} />
            <input type={"submit"} value={"Save"} />
        </form>
    )
}