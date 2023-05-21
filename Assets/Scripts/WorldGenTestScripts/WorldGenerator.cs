using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { EMPTY, ROOM, BORDER, CORIDOR_Y, CORIDOR_X}

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
    /* 
     * List of objects that contain additional content after
     * base shape of dungeon will be created (for example enemies, traps)
     */
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
        pixels[(int)point.x, (int)point.y].SetState(state);
    }

    // sets position of rooms on map
    private void SetRoomArrangement()
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            for (int j = 0; j < roomGenerationChances; j++)
            {
                if (TryCreateRoom(i) || !AnyConnections(connectionsX, connectionsY))
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
        bool connectionOrientation = chooseConnectionOrientation(connectionsX, connectionsY);
        Vector2 connector = ChooseConnection(connectionsX, connectionsY, connectionOrientation).GetPosition();
        Vector2 startPoint = rooms[roomID].GetValidPlace(this, connector, connectionOrientation);
        if (startPoint != NOWHERE)
        {
            Pixel[] newConnections = rooms[roomID].SetPlace(pixels, startPoint);
            ManageNewConnections(connectionsX, connectionsY, newConnections);
            ManageNewExtraContent(startPoint, roomID);
            return true;
        }
        return false;
    }

    // After room is layed out adds new extra content to be showed up in same place in next steps
    private void ManageNewExtraContent(Vector2 startPoint, int roomID)
    {
        GameObject roomContent = rooms[roomID].extraContent;
        if (roomContent != null)
        {
            var roomContentInstance = Instantiate(roomContent);
            roomContentInstance.transform.position = startPoint;
            roomContentInstance.transform.parent = transform;
            roomContentInstance.SetActive(false);
            extraContent.Add(roomContentInstance);
        }
    }

    // After room is layed out adds new room connection to connection lists
    private void ManageNewConnections(List<Pixel> connectionsX, List<Pixel> connectionsY, Pixel[] newConnections)
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

    // Picks one connection with given orientation in order to check if new room can fit to it
    private Pixel ChooseConnection(List<Pixel> connectionsX, List<Pixel> connectionsY, bool horizontalConnecting)
    {
        return horizontalConnecting ? 
                connectionsX[Random.Range(0, connectionsX.Count)] :
                connectionsY[Random.Range(0, connectionsY.Count)];
    }

    // Chooses connection orientation (horizontal or vertical) from existing connectors
    private bool chooseConnectionOrientation(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        if(ConnectionsAvailable(connectionsX, connectionsY))
        {
            return Random.Range(0, 2) == 0;
        }
        else if(connectionsY.Count != 0)
        {
            return false;
        }   
        return true;
    }

    private bool ConnectionsAvailable(List<Pixel> connectionsX, List<Pixel> connectionsY)
    {
        return connectionsX.Count != 0 && connectionsY.Count != 0;
    }

    private bool AnyConnections(List<Pixel> connectionsX, List<Pixel> connectionsY)
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
        return IsAreaOccupied(new Vector2(i, j), new Vector2(i, j), 1) && EmptyTileInsideWorldBorder(i, j);
    }

    private bool EmptyTileInsideWorldBorder(int i, int j)
    {
        return pixels[i, j].GetType() == TileType.EMPTY && !OnWorldBorder(new Vector2(i, j));
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

    private static bool IsConnectorTile(TileType type)
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
        if (OutsideWorldBorder(point))
        {
            return true;
        }
        TileType state = pixels[(int)point.x, (int)point.y].GetType();
        return  state != TileType.EMPTY && state != TileType.BORDER;
    }

    // Checks if something alse is in certain area, size of area detemined by radious
    private bool IsAreaOccupied(Vector2 point1, Vector2 point2,int radious)
    {
        for (int x = (int)point1.x - radious; x <= (int)point2.x + radious; x++)
        {
            for (int y = (int)point1.y - radious; y <= (int)point2.y + radious; y++)
            {
                if(IsOccupied(new Vector2(x,y)))
                    return true;
            }
        }
        return false;
    }

    public bool OutsideWorldBorder(Vector2 point)
    {
        return point.x < 0 || point.y < 0 || point.x >= width || point.y >= height;
    }

    private bool OnWorldBorder(Vector2 point)
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
