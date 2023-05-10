using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { EMPTY, ROOM, BORDER, CORIDOR_Y, CORIDOR_X}
public enum ConnectionType { HORIZONTAL, VERTICAL}

//class containing main world generator logic
public class WorldGenerator : MonoBehaviour
{
    // List of rooms to generate
    public Room[] rooms;
    // Number of chances that each room has to generate
    public int roomGenerationChances;
    // Size of generated world
    public int width;
    public int height;
    // Types of objects that can generate, each coresponding to one tile type
    public GameObject[] tileTypes;
    // Contain information about what is going to be created in coresponding place
    private Pixel[,] pixels;
    // List of pixels that can be connected to in specyfic direction
    private List<Pixel> connectionsX = new List<Pixel>();
    private List<Pixel> connectionsY = new List<Pixel>();
    //List of objects that contain additional content after
    //base shape of dungeon will be created (for example enemies, traps)
    private List<GameObject> extraContent = new List<GameObject>();
    // Vector encoding of lack of place available
    public static Vector2 NOWHERE = new Vector2(-1,-1);

    void Start()
    {
        Generate();
    }

    void Generate()
    {
        CreatePixels();
        AddStartingConnection();
        SetRoomArrangement();
        RemoveDeadEnds();
        ArrangeRoomBorders();
        GenerateDungeonTiles();
        ShowExtraContent();
    }

    // creates pixels wich contain data how to generate certain tile
    private void CreatePixels()
    {
        pixels = new Pixel[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                pixels[i, j] = new Pixel(i,j);
            }
        }
    }

    public void ChangePixelState(Vector2 point, TileType state)
    {
        pixels[(int)point.x, (int)point.y].SetTileType(state);
    }

    // sets position of rooms on map
    private void SetRoomArrangement()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            for (int j = 0; j < roomGenerationChances; j++)
            {
                if (TryCreateRoom(i) || !AnyConnectionsPresent(connectionsX, connectionsY))
                {
                    break;
                }
            }
        }
    }

    private void AddStartingConnection()
    {
        connectionsX.Add(pixels[width / 2, height / 2]);
    }

    // One try of seting room in certain place
    private bool TryCreateRoom(int roomID)
    {
        ConnectionType connectionOrientation = ChooseConnectionOrientation(connectionsX, connectionsY);
        Vector2 connectorPos;
        if (connectionOrientation == ConnectionType.HORIZONTAL)
        {
            connectorPos = ChooseRandomPixelPos(connectionsX);
        }
        else
        {
            connectorPos = ChooseRandomPixelPos(connectionsY);
        }
        Vector2 newRoomBottomLeftCorner = rooms[roomID].GetValidPlace(this, connectorPos, connectionOrientation);
        if (newRoomBottomLeftCorner != NOWHERE)
        {
            rooms[roomID].SetRoomOnMap(pixels, newRoomBottomLeftCorner);
            Pixel[] newConnections = rooms[roomID].GetConnectors(pixels, newRoomBottomLeftCorner);
            SortNewConnections(connectionsX, connectionsY, newConnections);
            AddNewExtraContent(newRoomBottomLeftCorner, roomID);
            return true;
        }
        return false;
    }

    // After room is layed out adds new extra content to be showed up in same place in next steps
    private void AddNewExtraContent(Vector2 roomsBottomRightCorner, int roomID)
    {
        GameObject roomContent = rooms[roomID].extraContent;
        if (roomContent != null)
        {
            var roomContentInstance = Instantiate(roomContent);
            roomContentInstance.transform.position = roomsBottomRightCorner;
            roomContentInstance.transform.parent = transform;
            roomContentInstance.SetActive(false);
            extraContent.Add(roomContentInstance);
        }
    }

    // After room is layed out adds new room connection to connection lists
    private void SortNewConnections(List<Pixel> connectionsX, List<Pixel> connectionsY, Pixel[] newConnections)
    {
        for (int k = 0; k < newConnections.Length; k++)
        {
            var connectionState = newConnections[k].GetType();
            if (connectionState == TileType.CORIDOR_X)
            {
                connectionsX.Add(newConnections[k]);
            }
            else if (connectionState == TileType.CORIDOR_Y)
            {
                connectionsY.Add(newConnections[k]);
            }
        }
    }

    // Picks one connection position from given list
    private Vector2 ChooseRandomPixelPos(List<Pixel> pixelList)
    {
        return pixelList[Random.Range(0, pixelList.Count)].GetPosition();
    }

    // Chooses connection orientation (horizontal or vertical) from existing connectors
    private ConnectionType ChooseConnectionOrientation(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        if(BothConnectionsPresent(connectionsX, connectionsY))
        {
            return (ConnectionType)Random.Range(0, 2);
        }
        else if(connectionsY.Count != 0)
        {
            return ConnectionType.VERTICAL;
        }
        return ConnectionType.HORIZONTAL;
    }

    private bool BothConnectionsPresent(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        return connectionsX.Count != 0 && connectionsY.Count != 0;
    }

    private bool AnyConnectionsPresent(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        return connectionsX.Count != 0 || connectionsY.Count != 0;
    }

    // Generates dungeon tiles on map when rooms arrangement is determined
    private void GenerateDungeonTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (pixels[i, j].GetType() != TileType.EMPTY)
                {
                    var instatnce = Instantiate(tileTypes[(int)pixels[i,j].GetType() - 1]);
                    instatnce.transform.position = pixels[i, j].GetPosition();
                    instatnce.transform.parent = transform;
                }
            }
        }
    }

    // Determines where room borders will be
    void ArrangeRoomBorders()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (IsBorderTile(i, j))
                {
                    ChangePixelState(new Vector2(i, j), TileType.BORDER);
                }
            }
        }
    }

    // Determines if tile is on border
    private bool IsBorderTile(int i, int j)
    {
        return IsAreaOccupied(new Vector2(i, j), new Vector2(i, j)) && IsEmptyTileInsideWorldBorder(i, j);
    }

    private bool IsEmptyTileInsideWorldBorder(int i, int j)
    {
        return pixels[i, j].GetType() == TileType.EMPTY && !IsOnWorldBorder(new Vector2(i, j));
    }

    // Removes doors that lead to nowhere
    void RemoveDeadEnds()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                TileType type = pixels[i, j].GetType();
                if (IsConnectorTile(type) && LeadsToNowhere(i, j))
                {
                    ChangePixelState(new Vector2((float)i, (float)j), TileType.EMPTY);
                }
            }
        }
    }

    private bool IsConnectorTile(TileType type)
    {
        return type == TileType.CORIDOR_X || type == TileType.CORIDOR_Y;
    }

    // returns if on the x,y tile will have only one way to be entered
    private bool LeadsToNowhere(int x, int y)
    {
        return !(IsOccupied(new Vector2(Mathf.Max(x - 1, 0),y)) &&
                IsOccupied(new Vector2(Mathf.Min(x + 1, width - 1), y))) &&
               !(IsOccupied(new Vector2(x,Mathf.Max(y - 1, 0))) &&
               IsOccupied(new Vector2(x,Mathf.Min(y + 1, width - 1))));
    }

    // Checks if in certain point on map something is already planned to build
    public bool IsOccupied(Vector2 point)
    {
        if (IsOutsideWorldBorder(point))
        {
            return true;
        }
        TileType state = pixels[(int)point.x, (int)point.y].GetType();
        return  state != TileType.EMPTY && state != TileType.BORDER;
    }

    // Returns true if something else is in certain area or on areas border
    private bool IsAreaOccupied(Vector2 point1, Vector2 point2)
    {
        for (int x = (int)point1.x - 1; x <= (int)point2.x + 1; x++)
        {
            for (int y = (int)point1.y - 1; y <= (int)point2.y + 1; y++)
            {
                if(IsOccupied(new Vector2(x,y)))
                    return true;
            }
        }
        return false;
    }

    public bool IsOutsideWorldBorder(Vector2 point)
    {
        return point.x < 0 || point.y < 0 || point.x >= width || point.y >= height;
    }

    private bool IsOnWorldBorder(Vector2 point)
    {
        return point.x == 0 || point.y == 0 || point.x == width - 1 || point.y == height - 1;
    }

    public Pixel[,] GetPixels()
    {
        return pixels;
    }

    // After shape of dungeon is established eneables dungeons, monseters ect.
    private void ShowExtraContent()
    {
        foreach(GameObject roomContent in extraContent){
            if(roomContent != null)
            {
                roomContent.SetActive(true);
            }
        }
    }
}
