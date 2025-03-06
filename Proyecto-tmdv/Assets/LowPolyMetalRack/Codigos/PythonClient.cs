using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;  // Asegúrate de instalar Newtonsoft.Json en Unity

public class PythonClient : MonoBehaviour
{
    private const string SERVER_IP = "127.0.0.1";
    private const int PORT = 59056;
    public GameObject drone1, drone2;
    private List<Vector2> path1 = new List<Vector2>();
    private List<Vector2> path2 = new List<Vector2>();
    private int index1 = 0, index2 = 0;

    void Start()
    {
        StartCoroutine(ConnectToServer());
    }

    IEnumerator ConnectToServer()
    {
        using (TcpClient client = new TcpClient())
        {
            try
            {
                client.Connect(SERVER_IP, PORT);
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[2048];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (!string.IsNullOrEmpty(json))
                {
                    Dictionary<string, List<List<int>>> paths = JsonConvert.DeserializeObject<Dictionary<string, List<List<int>>>>(json);
                    
                    if (paths.ContainsKey("drone1"))
                        path1 = ConvertToVector2List(paths["drone1"]);
                    
                    if (paths.ContainsKey("drone2"))
                        path2 = ConvertToVector2List(paths["drone2"]);

                    Debug.Log("✅ Rutas cargadas correctamente.");
                }
                else
                {
                    Debug.LogError("❌ Error: JSON vacío recibido.");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error de conexión: {ex.Message}");
            }
        }

        yield return null;
    }

    List<Vector2> ConvertToVector2List(List<List<int>> path)
    {
        List<Vector2> result = new List<Vector2>();
        foreach (var point in path)
            result.Add(new Vector2(point[0], point[1]));
        return result;
    }

    void Update()
    {
        MoveDrone(drone1, path1, ref index1);
        MoveDrone(drone2, path2, ref index2);
    }

    void MoveDrone(GameObject drone, List<Vector2> path, ref int index)
    {
        if (path.Count > 0 && index < path.Count)
        {
            Vector2 target = path[index];
            drone.transform.position = Vector2.MoveTowards(drone.transform.position, target, Time.deltaTime * 5);
            
            if ((Vector2)drone.transform.position == target)
                index++;
        }
    }
}
