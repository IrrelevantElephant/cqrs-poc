import { createServer } from "miragejs";

const mockApi = () => {
  createServer({
    routes() {
      this.get("/api/todos", () => [
        { id: "todo-0", name: "Eat", completed: true },
        { id: "todo-1", name: "Sleep", completed: false },
        { id: "todo-2", name: "Repeat", completed: false },
      ]);
    },
  });
};

export default mockApi;
