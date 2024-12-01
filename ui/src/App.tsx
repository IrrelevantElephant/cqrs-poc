import { useEffect, useState } from "react";
import FilterButton from "./components/FilterButton";
import Form from "./components/Form";
import Todo, { TodoProps } from "./components/Todo";
import { nanoid } from "nanoid";

const FILTER_MAP: {
  [key: string]: (task: TodoProps) => boolean | undefined;
} = {
  All: (_: TodoProps) => true,
  Active: (task: TodoProps) => !task.completed,
  Completed: (task: TodoProps) => task.completed,
};

const FILTER_NAMES = Object.keys(FILTER_MAP);

const App = () => {
  const [currentTasks, setTasks] = useState<Array<any>>([]);

  useEffect(() => {
    fetch("/api/todos")
      .then((response) => response.json())
      .then((json) => setTasks(json));
  }, []);

  const addTask = (name: string) => {
    const newTask = { id: `todo-${nanoid()}`, name, completed: false };
    setTasks([...currentTasks, newTask]);
  };

  const deleteTask = (id: string) => {
    const updatedTasks = currentTasks.filter((t) => t.id !== id);
    setTasks(updatedTasks);
  };

  const [filter, setFilter] = useState("All");

  const editTask = (id: string, newName: string) => {
    const editedTaskList = currentTasks.map((task) => {
      // if this task has the same ID as the edited task
      if (id === task.id) {
        // Copy the task and update its name
        return { ...task, name: newName };
      }
      // Return the original task if it's not the edited task
      return task;
    });
    setTasks(editedTaskList);
  };

  const toggleTaskCompleted = (id: string) => {
    console.log({ id: id, tasks: currentTasks });
    const updatedTasks = currentTasks.map((task) => {
      // if this task has the same ID as the edited task
      if (id === task.id) {
        // use object spread to make a new object
        // whose `completed` prop has been inverted
        console.log(
          `setting task ${id} as ${
            task.completed ? "not completed" : "completed"
          }`
        );
        return { ...task, completed: !task.completed };
      }
      return task;
    });
    console.log({ id: id, updatedTasks: updatedTasks });
    setTasks(updatedTasks);
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
