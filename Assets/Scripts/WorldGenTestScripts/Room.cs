using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// class contating logic of room from world generation
public class Room : MonoBehaviour
{
    // Rooms size
    public int width;
    public int height;
    // Contain information where connecotrs are be palced in rooms wall
    public List<ConnectorCords> connectorsX;
    public List<ConnectorCords> connectorsY;
    // Prefab wich contains objects like enemies, chests ect.
    public GameObject extraContent;

    // Represents rooms bottom left corner when position is estabished
    private Vector2 bottomLeftCorner;

    // Returns place where room can be placed, given one connector that is already on map. 
    public Vector2 GetValidPlace(WorldGenerator worldGenerator, Vector2 connectorFromOtherRoom, ConnectionType connecting)
    {
        List<Vector2> roomsConnectorVectors = new List<Vector2>();
        List<Pixel> roomsValidConnectors = new List<Pixel>();
        if (connecting == ConnectionType.HORIZONTAL)
        {
            roomsConnectorVectors.AddRange(GetConnectorsVectors(connectorFromOtherRoom, connectorsX));
        }
        else
        {
            roomsConnectorVectors.AddRange(GetConnectorsVectors(connectorFromOtherRoom, connectorsY));
        }
        roomsValidConnectors.AddRange(GetPixelsSubset(worldGenerator.GetPixels(), roomsConnectorVectors.ToArray()));
        foreach (var connectorVector in roomsConnectorVectors)
        {
            removeIfNotValid(worldGenerator, connectorFromOtherRoom, roomsValidConnectors, connectorVector);
        }
        return pickValidConnectorIfExists(connectorFromOtherRoom, roomsValidConnectors);
    }

    // Returns random valid connector from this room,
    // wich combined with connector from other room
    // will not cause this room to colide with another already existing
    private Vector2 pickValidConnectorIfExists(Vector2 connectorFromOtherRoom, List<Pixel> validConnectors)
    {
        if (validConnectors.Count == 0)
        {
            return WorldGenerator.NOWHERE;
        }
        Vector3 pickedConnector = validConnectors[Random.Range(0,validConnectors.Count)].GetPosition();
        return new Vector2(2 * connectorFromOtherRoom.x - pickedConnector.x, 2 * connectorFromOtherRoom.y - pickedConnector.y);
    }

    // removes connector from list if connector is not valid
    private void removeIfNotValid(WorldGenerator worldGenerator, Vector2 connectorFromOtherRoom,
                                        List<Pixel> validConnectors, Vector2 connectorPos)
    {
        int startingX = (int)(2 * connectorFromOtherRoom.x - connectorPos.x);
        int startingY = (int)(2 * connectorFromOtherRoom.y - connectorPos.y);
        for (int x = startingX; x <= startingX + width; x++)
        {
            for (int y = startingY; y <= startingY + height; y++)
            {
                if (IsOccupied(worldGenerator, new Vector2(x, y)))
                {
                    validConnectors.Remove(worldGenerator.GetPixels()[(int)connectorPos.x, (int)connectorPos.y]);
                }
            }
        }
    }

    // Sets room tiles on map
    public void SetRoomOnMap(Pixel[,] pixels, Vector2 bottomLefttCorner)
    {
        this.bottomLeftCorner = bottomLefttCorner;
        SetRoomFloorOnMap(pixels);
        SetSpecialTilesOnMap(pixels);
    }

    // Sets special tiles (like connectors)
    private void SetSpecialTilesOnMap(Pixel[,] pixels)
    {
        SetTileType(GetPixelsSubset(pixels, GetConnectorsVectors(bottomLeftCorner, connectorsY)), TileType.CORIDOR_Y);
        SetTileType(GetPixelsSubset(pixels, GetConnectorsVectors(bottomLeftCorner, connectorsX)), TileType.CORIDOR_X);
    }

    // Sets rooms floor
    private void SetRoomFloorOnMap(Pixel[,] pixels)
    {
        for (int x = (int)bottomLeftCorner.x; x < (int)bottomLeftCorner.x + width; x++)
        {
            for (int y = (int)bottomLeftCorner.y; y < (int)bottomLeftCorner.y + height; y++)
            {
                pixels[x, y].SetTileType(TileType.ROOM);
            }
        }
    }

    public Pixel[] GetConnectors(Pixel[,] pixels, Vector2 givenBottomLeftCorner)
    {
        return GetPixelsSubset(pixels, GetConnectorsVectors(givenBottomLeftCorner));
    }

    // Returns connectors placement relative to given startPos
    private Vector2[] GetConnectorsVectors(Vector2 startPosition,List<ConnectorCords> cords)
    {
        Vector2[] connectors = new Vector2[cords.Count];
        for(int i = 0; i < cords.Count; i++)
        {
            connectors[i] = new Vector2((int)startPosition.x + cords[i].x + (cords[i].addWidth ? width : 0),
                (int)startPosition.y + cords[i].y + (cords[i].addHeight ? height : 0));
        }
        return connectors;
    }

    // Returns connectors placement relative to given startPos
    private Vector2[] GetConnectorsVectors(Vector2 startPosition)
    {
        return GetConnectorsVectors(startPosition, connectorsY)
                .Concat(GetConnectorsVectors(startPosition,connectorsX)).ToArray();
    }

    // Returns pixel subset specified by vectors
    private Pixel[] GetPixelsSubset(Pixel[,] pixels, Vector2[] vectors)
    {
        Pixel[] vectorsPixels = new Pixel[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            vectorsPixels[i] = pixels[(int)vectors[i].x, (int)vectors[i].y];
        }
        return vectorsPixels;
    }

    // Chceks if point cant be overriten by room
    private bool IsOccupied(WorldGenerator worldGenerator, Vector2 point)
    {
        if (worldGenerator.IsOutsideWorldBorder(point))
        {
            return true;
        }
        var state = worldGenerator.GetPixels()[(int)point.x, (int)point.y].GetType();
        return state != TileType.EMPTY &&
               state != TileType.CORIDOR_Y &&
               state != TileType.CORIDOR_Y;
    }

    private void SetTileType(Pixel[] pixels, TileType tileType)
    {
        foreach (Pixel pixel in pixels)
        {
            pixel.SetTileType(tileType);
        }
    }
}
