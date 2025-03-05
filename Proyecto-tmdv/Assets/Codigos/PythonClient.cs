using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Newtonsoft.Json; // ✅ Importar Newtonsoft.Json

public class PythonClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    public GameObject player; // Objeto que se moverá
    private Queue<Vector3> movementQueue = new Queue<Vector3>(); // ✅ Cola para movimientos
    public float speed = 2.0f; // ✅ Variable para controlar la velocidad

    void Start()
    {
        try
        {
            client = new TcpClient("127.0.0.1", 59056);
            stream = client.GetStream();
            Debug.Log("Conectado a Python");
        }
        catch (Exception e)
        {
            Debug.LogError("No se pudo conectar a Python: " + e.Message);
        }
    }

    void Update()
    {
        if (stream != null && stream.DataAvailable)
        {
            byte[] data = new byte[4096]; // ✅ Aumentar tamaño del buffer
            int bytesRead = stream.Read(data, 0, data.Length);
            string message = Encoding.UTF8.GetString(data, 0, bytesRead).Trim(); 

            Debug.Log("Datos recibidos: " + message); // ✅ Depurar JSON recibido

            try
            {
                // ✅ Deserializar el JSON en una estructura de datos
                PathData pathData = JsonConvert.DeserializeObject<PathData>(message);

                if (pathData != null && pathData.path.Count > 0)
                {
                    movementQueue.Clear(); // Limpiar movimientos previos

                    foreach (var coord in pathData.path)
                    {
                        movementQueue.Enqueue(new Vector3(coord[0], 6.5f, coord[1])); // ✅ Ajustar Y según necesidad
                    }
                }
                else
                {
                    Debug.LogError("JSON recibido no tiene datos válidos.");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error al deserializar JSON: " + e.Message);
            }
        }

        if (movementQueue.Count > 0)
        {
            MovePlayer();
        }
    }

    void MovePlayer()
    {
        if (movementQueue.Count > 0)
        {
            Vector3 nextPosition = movementQueue.Peek(); // Obtenemos el siguiente destino sin eliminarlo
            player.transform.position = Vector3.MoveTowards(player.transform.position, nextPosition, speed * Time.deltaTime);

            if (Vector3.Distance(player.transform.position, nextPosition) < 0.1f) // Si ya llegó al punto
            {
                movementQueue.Dequeue(); // Eliminamos la posición alcanzada
            }
        }
    }

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}

// ✅ Estructura para deserializar JSON
[Serializable]
public class PathData
{
    public List<List<int>> path;
}
