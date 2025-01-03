import { useEffect, useState } from "react";
import FilterButton from "./components/FilterButton";
import Form from "./components/Form";
import Todo, { TodoProps } from "./components/Todo";
import { nanoid } from "nanoid";
import Connector from "./signalr-connection";

const FILTER_MAP: {
  [key: string]: (task: TodoProps) => boolean | undefined;
} = {
  All: (_: TodoProps) => true,
  Active: (task: TodoProps) => !task.completed,
  Completed: (task: TodoProps) => task.completed,
};

const FILTER_NAMES = Object.keys(FILTER_MAP);

const getApiUrl = (): string => {
  if (process.env.NODE_ENV === "development") {
    return "/api";
  } else {
    return import.meta.env.VITE_API_URL;
  }
};

const baseApi = getApiUrl();

const App = () => {
  const [currentTasks, setTasks] = useState<Array<any>>([]);
  const [loadingTasks, setLoadingTasks] = useState(false);

  const { events } = Connector();

  useEffect(() => {
    events((_) => retrieveClickHandler());
  });

  const retrieveClickHandler = () => {
    setLoadingTasks(true);
    fetch(`${baseApi}/todos`)
      .then((response) => response.json())
      .then((json) => setTasks(json))
      .finally(() => setLoadingTasks(false));
  };

  const addTask = async (name: string) => {
    const newTask = { id: `todo-${nanoid()}`, name, completed: false };
    const body = JSON.stringify(newTask);
    await fetch(`${baseApi}/todos`, {
      method: "POST",
      body: body,
      headers: {
        "Content-Type": "application/json",
      },
    });
  };

  const deleteTask = async (id: string) => {
    await fetch(`${baseApi}/todos/${id}`, { method: "DELETE" });
  };

  const [filter, setFilter] = useState("All");

  const editTask = async (id: string, newName: string) => {
    const taskToUpdate = currentTasks.find((t) => t.id === id);
    const body = JSON.stringify({ ...taskToUpdate, name: newName });
    await fetch(`${baseApi}/todos/${id}`, {
      method: "PUT",
      body: body,
      headers: {
        "Content-Type": "application/json",
      },
    });
  };

  const toggleTaskCompleted = async (id: string) => {
    const taskToUpdate = currentTasks.find((t) => t.id === id);
    const body = JSON.stringify({
      ...taskToUpdate,
      completed: !taskToUpdate.completed,
    });
    await fetch(`${baseApi}/todos/${id}`, {
      method: "PUT",
      body: body,
      headers: {
        "Content-Type": "application/json",
      },
    });
  };

  const taskList = currentTasks
    .filter(FILTER_MAP[filter])
    .map((task) => (
      <Todo
        id={task.id}
        name={task.name}
        completed={task.completed}
        key={task.id}
        toggleTaskCompleted={toggleTaskCompleted}
        deleteTask={deleteTask}
        editTask={editTask}
      />
    ));

  const filterList = FILTER_NAMES.map((name) => (
    <FilterButton
      key={name}
      name={name}
      isPressed={name === filter}
      setFilter={setFilter}
    />
  ));

  const tasksNoun = taskList.length !== 1 ? "tasks" : "task";
  const headingText = `${taskList.length} ${tasksNoun} remaining`;

  return (
    <div className="todoapp stack-large">
      <h1>TodoMatic</h1>
      <div>
        <button
          className="btn btn__primary btn__lg"
          onClick={() => retrieveClickHandler()}
          disabled={loadingTasks}
        >
          {loadingTasks ? "Loading tasks..." : "Retrieve tasks"}
        </button>
      </div>
      <Form onSubmit={addTask} />
      <div className="filters btn-group stack-exception">{filterList}</div>
      <h2 id="list-heading">{headingText}</h2>
      <ul
        role="list"
        className="todo-list stack-large stack-exception"
        aria-labelledby="list-heading"
      >
        {taskList}
      </ul>
    </div>
  );
};

export default App;
