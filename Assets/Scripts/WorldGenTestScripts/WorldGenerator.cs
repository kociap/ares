using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WorldGenerator : MonoBehaviour
{
    System.Random random;
    public int nrOfBigRooms;
    public int nrOfSmallRooms;
    public int width;
    public int height;
    public Pixel template; 
    Pixel[,] pixels;

    void Start()
    {
        random = new System.Random();
        pixels = new Pixel[width, height];
        for(int i = 0;i< width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixels[i,j] = Instantiate(template, new Vector3(i,j,0), Quaternion.identity);
            }
        }
        Generate();
    }

    void ChangePixelColor(Vector2 point, int color)
    {
        pixels[(int)point.x, (int)point.y].SetColor(color);
    }

    void Generate()
    {
        for(int i = 0; i< nrOfBigRooms; i++)
        {
            GenerateRoom(20,35,12,20);
        }
        for (int i = 0; i < nrOfSmallRooms; i++)
        {
            GenerateRoom(12, 15, 8, 12);
        }
    }

    void GenerateRoom(int xMin, int xMax, int yMin, int yMax)
    {
        Vector2 point1 = new Vector2(random.Next(width - xMax), random.Next(height - yMax));
        Vector2 point2 = new Vector2((point1.x + random.Next(xMin, xMax)),
                                     (point1.y + random.Next(yMin, yMax)));
        if (IsAreaOccupied(point1, point2))
            return;
        for (int x = (int)point1.x; x < (int)point2.x; x++)
        {
            for (int y = (int)point1.y; y < (int)point2.y; y++)
            {
                ChangePixelColor(new Vector2((float)x, (float)y), 2);
            }
        }
        Debug.Log("location: " + point1.x + "," + point1.y + ";" + point2.x + "," + point2.y);
    }

    bool IsOccupied(Vector2 point)
    {
        return ( point.x<0 || point.y<0 || point.x >= width || point.y >= height || pixels[(int)point.x, (int)point.y].colorId == 2);
    }

    bool IsAreaOccupied(Vector2 point1, Vector2 point2)
    {
        for (int x = (int)point1.x-5; x <= (int)point2.x+5; x++)
        {
            for (int y = (int)point1.y-5; y <= (int)point2.y+5; y++)
            {
                if(IsOccupied(new Vector2(x,y)))
                    return true;
            }
        }
        return false;
    }

}
