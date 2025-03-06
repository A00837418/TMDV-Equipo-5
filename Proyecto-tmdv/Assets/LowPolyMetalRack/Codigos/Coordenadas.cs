using System;

[Serializable]

public struct Coordenadas
{
    public int x;
    public int y;

    public Coordenadas(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}
