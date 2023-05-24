using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// data container that has information what type of block to create in certain place
public class Pixel
{
    private int x , y;
    private TileType tileType;

    public Pixel(int x, int y)
    {
        this.x = x;
        this.y = y;
        tileType = TileType.EMPTY;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(x, y);
    }

    public void SetTileType(TileType tileType)
    {
        this.tileType = tileType;
    }

    public new TileType GetType()
    {
        return tileType;
    }
}
