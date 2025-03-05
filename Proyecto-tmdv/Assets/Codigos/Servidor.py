import socket
import json
import heapq

# Definir tamaño de la matriz
GRID_WIDTH = 33  # Ancho de la matriz
GRID_HEIGHT = 22  # Alto de la matriz

# Definir obstáculos dentro del nuevo tamaño de la matriz
OBSTACLES = {(2,2),(3,2),(4,2),(5,2),(6,2),(7,2),(8,2),(9,2),(10,2),(11,2),(12,2),}

# Algoritmo A* para encontrar la mejor ruta
def astar(start, goal):
    open_list = []
    heapq.heappush(open_list, (0, start))
    came_from = {}
    cost_so_far = {start: 0}
    
    while open_list:
        _, current = heapq.heappop(open_list)

        if current == goal:
            break  # Ruta encontrada

        for dx, dy in [(0, 1), (1, 0), (0, -1), (-1, 0)]:  # Movimientos arriba, derecha, abajo, izquierda
            next_node = (current[0] + dx, current[1] + dy)
            if 0 <= next_node[0] < GRID_WIDTH and 0 <= next_node[1] < GRID_HEIGHT and next_node not in OBSTACLES:
                new_cost = cost_so_far[current] + 1
                if next_node not in cost_so_far or new_cost < cost_so_far[next_node]:
                    cost_so_far[next_node] = new_cost
                    priority = new_cost + abs(goal[0] - next_node[0]) + abs(goal[1] - next_node[1])  # Heurística
                    heapq.heappush(open_list, (priority, next_node))
                    came_from[next_node] = current

    # Reconstrucción del camino
    path = []
    node = goal
    while node in came_from:
        path.append(node)
        node = came_from[node]
    path.append(start)
    path.reverse()
    
    return path if len(path) > 1 else None  # Devuelve None si no se encuentra camino

# Crear conexión con Unity
def start_server():
    server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server.bind(("127.0.0.1", 59056))  # Puerto que debe coincidir con Unity
    server.listen(1)
    print("Esperando conexión con Unity...")

    while True:
        conn, addr = server.accept()
        print("Conectado con Unity:", addr)

        # Generar rutas en la nueva matriz
        path1 = astar((0, 0), (32, 21))
        path2 = astar((32, 21), (16, 0))
        path3 = astar((16, 0), (0, 0))

        if not path1 or not path2 or not path3:
            print("⚠️ No se pudo encontrar una ruta válida")
            conn.close()
            continue

        full_path = path1 + path2[1:] + path3[1:]  # Evitar duplicar nodos

        # Crear JSON con la ruta
        json_data = json.dumps({"path": full_path})
        print("JSON enviado:", json_data)  # ✅ Verificar JSON antes de enviar

        # Enviar JSON a Unity
        conn.sendall(json_data.encode("utf-8"))
        conn.close()
        print("Ruta enviada a Unity.")

if __name__ == "__main__":
    start_server()
