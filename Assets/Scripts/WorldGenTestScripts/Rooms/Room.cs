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

    // Checks if room has any valid connector (in given orientation) to startPosition
    public Vector2 GetValidPlace(WorldGenerator worldGenerator, Vector2 startPosition, bool horizontalConnecting)
    {
        List<Vector2> connectorVectors = new List<Vector2>();
        List<Pixel> validConnectors = new List<Pixel>();
        connectorVectors.AddRange(GetConnectorsVectors(startPosition, horizontalConnecting));
        validConnectors.AddRange(VectorsToPixels(worldGenerator.GetPixels(), connectorVectors.ToArray()));
        foreach (var connector in connectorVectors)
        {
            CheckConnectorValidity(worldGenerator, startPosition, validConnectors, connector);
        }
        return pickValidConnectorIfExists(startPosition, validConnectors);
    }

    /* 
     * Picks random connector from valid ones and returns position where room can be placed,
     * bottom left angle of rooms area when connected to picked connector
     */
    private Vector2 pickValidConnectorIfExists(Vector2 startPosition, List<Pixel> validConnectors)
    {
        if (validConnectors.Count == 0)
        {
            return WorldGenerator.NOWHERE;
        }
        Vector3 pickedConnector = validConnectors[Random.Range(0,validConnectors.Count)].GetPosition();
        return new Vector2(2 * startPosition.x - pickedConnector.x, 2 * startPosition.y - pickedConnector.y);
    }

    // Checks if connector places room in valid position
    private void CheckConnectorValidity(WorldGenerator worldGenerator, Vector2 startPosition, List<Pixel> validConnectors, Vector2 connectorPos)
    {
        int startingX = (int)(2 * startPosition.x - connectorPos.x);
        int startingY = (int)(2 * startPosition.y - connectorPos.y);
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

    // Sets area where room will be generated
    public Pixel[] SetPlace(Pixel[,] pixels, Vector2 startPosition)
    {
        FillRoom(pixels, startPosition);
        FillSpecialTiles(pixels, startPosition);
        return GetConnectors(pixels, startPosition);
    }

    // Sets connectors and content tiles (ressponsible for generating stuff inside) to room
    private void FillSpecialTiles(Pixel[,] pixels, Vector2 startPosition)
    {
        pixels[(int)startPosition.x, (int)startPosition.y].SetState(TileType.ROOM_INIT);
        SetState(VectorsToPixels(pixels, GetConnectorsVectors(startPosition, connectorsY)), TileType.CORIDOR_Y);
        SetState(VectorsToPixels(pixels, GetConnectorsVectors(startPosition, connectorsX)), TileType.CORIDOR_X);
    }

    // Sets room space
    private void FillRoom(Pixel[,] pixels,Vector2 startPosition)
    {
        for (int x = (int)startPosition.x; x < (int)startPosition.x + width; x++)
        {
            for (int y = (int)startPosition.y; y < (int)startPosition.y + height; y++)
            {
                pixels[x, y].SetState(TileType.ROOM);
            }
        }
    }

    private Pixel[] GetConnectors(Pixel[,] pixels, Vector2 startPosition)
    {
        return VectorsToPixels(pixels, GetConnectorsVectors(startPosition));
    }

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

    private Vector2[] GetConnectorsVectors(Vector2 startPosition)
    {
        return GetConnectorsVectors(startPosition, connectorsY)
                .Concat(GetConnectorsVectors(startPosition,connectorsX)).ToArray();
    }

    private Vector2[] GetConnectorsVectors(Vector2 startPosition, bool horizontalConnecting)
    {
        return horizontalConnecting ? 
            GetConnectorsVectors(startPosition, connectorsX) :
            GetConnectorsVectors(startPosition, connectorsY);
    }

    private Pixel[] VectorsToPixels(Pixel[,] pixelArea, Vector2[] vectors)
    {
        Pixel[] vectorsPixels = new Pixel[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            vectorsPixels[i] = pixelArea[(int)vectors[i].x, (int)vectors[i].y];
        }
        return vectorsPixels;
    }

    // Chceks if point cant be overriten by room
    private bool IsOccupied(WorldGenerator worldGenerator, Vector2 point)
    {
        if (worldGenerator.OutsideWorldBorder(point))
            return true;
        var state = worldGenerator.GetPixels()[(int)point.x, (int)point.y].GetType();
        return state != TileType.EMPTY &&
               state != TileType.CORIDOR_Y &&
               state != TileType.CORIDOR_Y;
    }

    private void SetState(Pixel[] pixels, TileType state)
    {
        foreach (Pixel pixel in pixels)
        {
            pixel.SetState(state);
        }
    }
}
