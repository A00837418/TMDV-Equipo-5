using UnityEngine;
using TMPro;


public class Etiquetas : MonoBehaviour
{
    [SerializeField] 
    private TextMeshPro label;
    public Coordenadas coordenadas;

    public void SetCoordinates(int x, int y)
    {
        coordenadas = new Coordenadas(x, y);
        UpdateCordsLabel();
    }  
   private void UpdateCordsLabel()
    {
        label.text = $"({coordenadas.x}, {coordenadas.y})";
    }
}
    
