import "./index.css";
import {TodoList} from "@/features/todos/components/TodoList.tsx";
import {AddTodoForm} from "@/features/todos/components/AddTodoForm.tsx";

export function App() {
  return (
    <div className="flex justify-center items-center flex-col w-full h-screen">
        
        <div className="card">
            <AddTodoForm />
            <TodoList />
        </div>

    </div>
  );
}

export default App;
