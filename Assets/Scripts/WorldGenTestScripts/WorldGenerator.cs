using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.MemoryProfiler;

public enum State { EMPTY, ROOM, BORDER, CORIDOR_Y, CORIDOR_X, ROOM_INIT }

public class WorldGenerator : MonoBehaviour
{
    static WorldGenerator worldGenerator;
    public static System.Random random = new System.Random();
    public Room[] rooms;
    public int roomGenerationChances;
    public int width;
    public int height;
    public Pixel template; 
    Pixel[,] pixels;
    List<Pixel> connectionsX = new List<Pixel>();
    List<Pixel> connectionsY = new List<Pixel>();

    public static Vector2 ERROR_CODE = new Vector2(-1,-1);

    void Start()
    {
        if (worldGenerator == null)
            worldGenerator = this;
        Generate();
    }

    private void CreatePixels()
    {
        pixels = new Pixel[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixels[i, j] = Instantiate(template, new Vector3(i, j, 0), Quaternion.identity, transform);
            }
        }
    }

    public static void ChangePixelColor(Vector2 point, State state)
    {
        worldGenerator.pixels[(int)point.x, (int)point.y].SetState(state);
    }

    void Generate()
    {
        CreatePixels();
        GenerateRooms();
        RemoveDeadEnds();
        CreateRoomBorders();
        GenerateTiles();
        RemovePixels();
    }

    private void GenerateRooms()
    {
        connectionsX.Add(pixels[width / 2, height / 2]);
        for (int i = 0; i < rooms.Length && anyConnections(connectionsX, connectionsY); i++)
        {
            for (int j = 0; j < roomGenerationChances && anyConnections(connectionsX, connectionsY); j++)
            {
                if (TryCreateRoom(i))
                    break;
            }
        }
    }

    private void RemovePixels()
    {
        foreach(Pixel pixel in pixels)
        {
            Destroy(pixel.gameObject);
        }
    }

    private bool TryCreateRoom(int roomID)
    {
        bool connectionOrientation = chooseConnectionOrientation(connectionsX, connectionsY);
        Vector2 connector = ChooseConnection(connectionsX, connectionsY, connectionOrientation).GetPosition();
        Vector2 startPoint = rooms[roomID].HasPlace(pixels, connector, connectionOrientation);
        if (startPoint != ERROR_CODE)
        {
            Pixel[] newConnections = rooms[roomID].Place(pixels, startPoint);
            ManageNewConnections(connectionsX, connectionsY, newConnections);
            return true;
        }
        return false;
    }

    private static void ManageNewConnections(List<Pixel> connectionsX, List<Pixel> connectionsY, Pixel[] newConnections)
    {
        for (int k = 0; k < newConnections.Length; k++)
        {
            if (newConnections[k].GetState() == State.CORIDOR_X)
                connectionsX.Add(newConnections[k]);
            if (newConnections[k].GetState() == State.CORIDOR_Y)
                connectionsY.Add(newConnections[k]);
        }
    }

    private static Pixel ChooseConnection(List<Pixel> connectionsX, List<Pixel> connectionsY, bool horizontalConnecting)
    {
        return horizontalConnecting ? connectionsX[random.Next(connectionsX.Count)] :
            connectionsY[random.Next(connectionsY.Count)];
    }

    private static bool chooseConnectionOrientation(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        return connectionsX.Count * connectionsY.Count != 0 ? (random.Next(2) % 2 == 1) : connectionsY.Count != 0 ? false : true;
    }

    private static bool anyConnections(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        return connectionsX.Count != 0 || connectionsY.Count != 0;
    }

    private void GenerateTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixels[i, j].GenerateTile(transform);
            }
        }
    }

    void CreateRoomBorders() 
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (IsAreaOccupied(new Vector2(i, j), new Vector2(i, j), 1) &&
                    pixels[i, j].GetState() == State.EMPTY &&
                    !OnWorldBorder(new Vector2(i,j)))
                {
                    ChangePixelColor(new Vector2(i,j), State.BORDER);
                }
            }
        }
    }

    void RemoveDeadEnds()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if ((pixels[i, j].GetState() == State.CORIDOR_X || pixels[i, j].GetState() == State.CORIDOR_Y)
                    && IsEntranceToNowhere(i,j))
                {
                    ChangePixelColor(new Vector2((float)i, (float)j), State.EMPTY);
                }
            }
        }
    }

    private bool IsEntranceToNowhere(int x, int y)
    {
        return !(IsOccupied(new Vector2(Mathf.Max(x - 1, 0),y)) && IsOccupied(new Vector2(Mathf.Min(x + 1, width - 1), y))) &&
               !(IsOccupied(new Vector2(x,Mathf.Max(y - 1, 0))) && IsOccupied(new Vector2(x,Mathf.Min(y + 1, width - 1))));
    }

    public bool IsOccupied(Vector2 point)
    {
        return ( OutsideWorldBorder(point) ||
            (pixels[(int)point.x, (int)point.y].GetState() != State.EMPTY &&
            pixels[(int)point.x, (int)point.y].GetState() != State.BORDER));
    }

    bool IsAreaOccupied(Vector2 point1, Vector2 point2,int precision)
    {
        for (int x = (int)point1.x - precision; x <= (int)point2.x+ precision; x++)
        {
            for (int y = (int)point1.y - precision; y <= (int)point2.y+ precision; y++)
            {
                if(IsOccupied(new Vector2(x,y)))
                    return true;
            }
        }
        return false;
    }

    public static bool OutsideWorldBorder(Vector2 point)
    {
        return point.x < 0 || point.y < 0 || point.x >= worldGenerator.width || point.y >= worldGenerator.height;
    }

    public static bool OnWorldBorder(Vector2 point)
    {
        return point.x == 0 || point.y == 0 || point.x == worldGenerator.width - 1 || point.y == worldGenerator.height - 1;
    }
}
