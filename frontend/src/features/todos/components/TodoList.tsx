import {useEffect, useMemo, useState} from "react";
import {getTodos} from "@/features/todos/services/getTodos.ts";
import {TodoItem} from "@/features/todos/components/TodoItem.tsx";
import {useTodoStore} from "@/features/todos/stores/useTodoStore.ts";
import classNames from "classnames";

const modes = ["All", "Done", "Todo"] as const;

type Mode = typeof modes[number];

export const TodoList = () => {
    const {todos, fetchTodos} = useTodoStore();
    
    const [mode, setMode] = useState<Mode>("All");
    
    const doneTodos = useMemo(() => todos.filter(t => t.done), [todos]);
    const todoTodos = useMemo(() => todos.filter(t => !t.done), [todos]);
    
    useEffect(() => {
        fetchTodos();
    }, [fetchTodos])
    
    return (
        <div className={"todoList"}>
            <div className={"todoListHeader"}>
                {modes.map((m) => (<span className={classNames("modeTag", mode === m ? "active" : null)} onClick={() => setMode(m)} key={m}>{m}</span>))}
            </div>
            <div className={"todoListContent"}>
                {mode === "All" && todos.map((todo, index) => <TodoItem key={index} todo={todo} />)}
                {mode === "Done" && doneTodos.map((todo, index) => <TodoItem key={index} todo={todo} />)}
                {mode === "Todo" && todoTodos.map((todo, index) => <TodoItem key={index} todo={todo} />)}
            </div>
        </div>
    )
}