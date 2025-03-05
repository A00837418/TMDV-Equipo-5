using UnityEngine;
using System.Collections;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    [SerializeField] public Coordenadas gridSize;
    [field:SerializeField] public int TileSize { get; private set; }
    [SerializeField] private GameObject tilePrefab;

    private void Awake()
     {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
     }

    private void Start(){
    
        ClearGrid();
        SpawnGrid();
    }

    [ContextMenu("Spawn Grid")]
    private void SpawnGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 tilePosition = new Vector3(x * TileSize,0, y * TileSize);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = $"Tile ({x}, {y})";
                Etiquetas etiquetas = tile.GetComponent<Etiquetas>();
                etiquetas.SetCoordinates(x, y);
            }
        }
    }

    [ContextMenu("Clear Grid")]
    private void ClearGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    public Vector3 GetWorldPosition(Coordenadas coordenadas)
    {
        return new Vector3(coordenadas.x * TileSize,0, coordenadas.y * TileSize);
        
    }
}





       
    

    

