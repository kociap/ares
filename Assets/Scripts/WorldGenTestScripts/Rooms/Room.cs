using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public virtual Vector2 HasPlace(Pixel[,] pixels, Vector2 startPosition, bool horizontalConnecting)
    {
        return new Vector2(-1,-1);
    }

    public virtual Pixel[] Place(Pixel[,] pixels, Vector2 startPosition)
    { 
        return null;
    }

    public bool IsOccupied(Pixel[,] pixels, Vector2 point)
    {
        return (WorldGenerator.OutsideWorldBorder(point) || 
            (pixels[(int)point.x, (int)point.y].GetState() != State.EMPTY &&
            pixels[(int)point.x, (int)point.y].GetState() != State.CORIDOR_Y &&
            pixels[(int)point.x, (int)point.y].GetState() != State.CORIDOR_Y));
    }

    protected void SetState(Pixel pixel, State color)
    {
        pixel.SetState(color);
    }

    protected void SetState(Pixel[] pointsToPait, State color)
    {
        foreach(Pixel pixel in pointsToPait)
        {
            pixel.SetState(color);
        }
    }
}
