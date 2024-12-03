import { createServer } from "miragejs";

const createMockApi = () => {
  const server = createServer({
    routes() {
      this.get("/api/todos", (schema, _) => schema.db.todos);
      this.post("/api/todos", (schema, request) => {
        const newTask = JSON.parse(request.requestBody);
        schema.db.todos.insert(newTask);
        return {};
      });
      this.put("/api/todos/:id", (schema, request) => {
        const id = request.params.id;
        const taskToUpdate = JSON.parse(request.requestBody);
        schema.db.todos.update({ id: id }, taskToUpdate);
        return {};
      });
      this.delete("/api/todos/:id", (schema, request) => {
        const id = request.params.id;
        schema.db.todos.remove({ id: id });
        return {};
      });
    },
    seeds(server) {
      server.db.loadData({
        todos: [
          { id: "todo-0", name: "Eat", completed: true },
          { id: "todo-1", name: "Sleep", completed: false },
          { id: "todo-2", name: "Repeat", completed: false },
        ],
      });
    },
  });

  return server;
};

export default createMockApi;
