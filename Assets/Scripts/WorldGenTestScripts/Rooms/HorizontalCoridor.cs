using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalCoridor : Room
{
    /*
    public int width;
    public int height;
    bool canBePlacedLeft = true;

    override public Vector2 HasPlace(Pixel[,] pixels, Vector2 startPosition, bool horizontalConnecting)
    {
        if (horizontalConnecting) {
            canBePlacedLeft = HasEmptyLine(pixels, startPosition, horizontalConnecting);
            return canBePlacedLeft || HasEmptyLine(pixels, new Vector2(startPosition.x - width, startPosition.y), horizontalConnecting)
                ? new Vector2(0, 0) : new Vector2(-1, -1);
        }
        return HasEmptyLine(pixels, startPosition, horizontalConnecting)? new Vector2(0,0) : new Vector2(-1,-1);
    }

    bool HasEmptyLine(Pixel[,] pixels, Vector2 startPosition, bool horizontalConnecting)
    {
        if (horizontalConnecting)
        {
            for (int x = (int)startPosition.x + 1; x <= (int)startPosition.x + width - 1; x++)
            {
                if (IsOccupied(pixels, new Vector2(x, startPosition.y)))
                    return false;
            }
        }
        else
        {
            for (int y = (int)startPosition.y + 1; y <= (int)startPosition.y + height - 1; y++)
            {
                if (IsOccupied(pixels, new Vector2(startPosition.x,y)))
                    return false;
            }
        }
        return true;
    }

    override public Pixel[] Place(Pixel[,] pixels, Vector2 startPosition, bool horizontalConnecting)
    {
        if (horizontalConnecting)
        {
            if (canBePlacedLeft)
                return PlaceCoridor(pixels, startPosition, horizontalConnecting);
            return PlaceCoridor(pixels, new Vector2(startPosition.x - width + 1, startPosition.y), horizontalConnecting);
        }
        else
        {
            return PlaceCoridor(pixels, startPosition, horizontalConnecting);
        }
    }

    Pixel[] PlaceCoridor(Pixel[,] pixels, Vector2 startPosition, bool horizontalConnecting)
    {
        if (horizontalConnecting)
        {
            for (int x = (int)startPosition.x; x < (int)startPosition.x + width; x++)
            {
                WorldGenerator.ChangePixelColor(new Vector2((float)x, startPosition.y), State.ROOM);
            }
            WorldGenerator.ChangePixelColor(new Vector2(startPosition.x, startPosition.y), State.CORIDOR_X);
            WorldGenerator.ChangePixelColor(new Vector2(startPosition.x + width - 1, startPosition.y), State.CORIDOR_X);
            return new Pixel[2] { pixels[(int)startPosition.x, (int)startPosition.y],
            pixels[(int)startPosition.x + width - 1, (int)startPosition.y] };
        }
        else
        {
            for (int y = (int)startPosition.y; y < (int)startPosition.y + height; y++)
            {
                WorldGenerator.ChangePixelColor(new Vector2(startPosition.x,y), State.ROOM);
            }
            WorldGenerator.ChangePixelColor(new Vector2(startPosition.x, startPosition.y), State.CORIDOR_Y);
            WorldGenerator.ChangePixelColor(new Vector2(startPosition.x, startPosition.y+ height - 1), State.CORIDOR_Y);
            return new Pixel[2] { pixels[(int)startPosition.x, (int)startPosition.y],
            pixels[(int)startPosition.x, (int)startPosition.y + height - 1] };
        }
    }*/
}
