import socket
import json
import heapq

# Tamaño de la matriz
GRID_WIDTH = 30
GRID_HEIGHT = 45

# Definir obstáculos
OBSTACLES = {
    (2, 2), (3, 2), (4, 2), (5, 2), (6, 2), (7, 2), (8, 2), (9, 2), (10, 2), (11, 2), (12, 2), (13, 2),
    (2, 4), (3, 4), (4, 4), (5, 4), (6, 4), (7, 4), (8, 4), (9, 4), (10, 4), (11, 4), (12, 4), (13, 4),
    (2, 6), (3, 6), (4, 6), (5, 6), (6, 6), (7, 6), (8, 6), (9, 6), (10, 6), (11, 6), (12, 6), (13, 6),
    (2, 8), (3, 8), (4, 8), (5, 8), (6, 8), (7, 8), (8, 8), (9, 8), (10, 8), (11, 8), (12, 8), (13, 8)
}

# Algoritmo A* para encontrar la mejor ruta
def astar(start, goal):
    open_list = []
    heapq.heappush(open_list, (0, start))
    came_from = {}
    cost_so_far = {start: 0}

    while open_list:
        _, current = heapq.heappop(open_list)

        if current == goal:
            break

        for dx, dy in [(0, 1), (1, 0), (0, -1), (-1, 0)]:
            next_node = (current[0] + dx, current[1] + dy)
            if 0 <= next_node[0] < GRID_WIDTH and 0 <= next_node[1] < GRID_HEIGHT and next_node not in OBSTACLES:
                new_cost = cost_so_far[current] + 1
                if next_node not in cost_so_far or new_cost < cost_so_far[next_node]:
                    cost_so_far[next_node] = new_cost
                    priority = new_cost + abs(goal[0] - next_node[0]) + abs(goal[1] - next_node[1])
                    heapq.heappush(open_list, (priority, next_node))
                    came_from[next_node] = current

    path = []
    node = goal
    while node in came_from:
        path.append(node)
        node = came_from[node]
    path.append(start)
    path.reverse()

    return path if len(path) > 1 else None

# Genera rutas seguras sin valores vacíos
def generate_paths():
    def safe_astar(start, goal):
        path = astar(start, goal)
        return path if path else []

    paths = {
        "drone1": safe_astar((0, 0), (9, 5)) + safe_astar((9, 5), (7, 9)) + safe_astar((7, 9), (0, 0)),
        "drone2": safe_astar((29, 0), (20, 5)) + safe_astar((20, 5), (22, 10)) + safe_astar((22, 10), (29, 0))
    }

    for drone, path in paths.items():
        if not path:
            print(f"⚠️ No se pudo encontrar una ruta válida para {drone}")
            return None

    return paths

# Servidor para comunicarse con Unity
def start_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("127.0.0.1", 59056))
    server.listen(1)
    print("🟢 Servidor esperando conexión con Unity...")

    while True:
        conn, addr = server.accept()
        print("🔵 Conectado con Unity:", addr)

        paths = generate_paths()
        if not paths:
            print("❌ No se encontraron rutas, cerrando conexión.")
            conn.close()
            continue

        json_data = json.dumps(paths)
        with open("debug.json", "w") as f:
            json.dump(paths, f, indent=4)

        print("📤 Enviando JSON:", json_data)
        conn.sendall(json_data.encode("utf-8"))
        conn.close()
        print("✅ Rutas enviadas a Unity.")

# Ejecutar servidor
if __name__ == "__main__":
    start_server()
