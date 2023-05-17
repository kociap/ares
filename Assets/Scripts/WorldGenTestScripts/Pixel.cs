using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// data container that has information what type of block to create in certain place
public class Pixel
{
    private int x , y;
    private TileType state;

    public Pixel(int x, int y)
    {
        this.x = x;
        this.y = y;
        state = TileType.EMPTY;
    }

    public Vector2 GetPosition()
    {
        return new Vector2(x, y);
    }

    public void SetState(TileType state)
    {
        this.state = state;
    }

    public new TileType GetType()
    {
        return state;
    }
}
