using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Android.Types;
using UnityEngine;

public class RectangeRoom : Room
{
    public int width;
    public int height;
    public List<ConnectorCords> connectorsX;
    public List<ConnectorCords> connectorsY;

    override public Vector2 HasPlace(Pixel[,] pixels, Vector2 startPosition, bool horizontalConnecting)
    {
        List<Vector2> connectorVectors = new List<Vector2>();
        List<Pixel> validConnectors = new List<Pixel>();
        connectorVectors.AddRange(GetConnectorsVectors(startPosition, horizontalConnecting));
        validConnectors.AddRange(Pixel.GetPosition(pixels, connectorVectors.ToArray()));
        foreach (var connector in connectorVectors)
        {
            ChechConnectorValidity(pixels, startPosition, validConnectors, connector);
        }
        return pickValidConnectorIfExists(startPosition, validConnectors);
    }

    private static Vector2 pickValidConnectorIfExists(Vector2 startPosition, List<Pixel> validConnectors)
    {
        if (validConnectors.Count == 0)
            return WorldGenerator.ERROR_CODE;
        Vector3 pickedConnector = validConnectors[WorldGenerator.random.Next(validConnectors.Count)].GetPosition();
        return new Vector2(2 * startPosition.x - pickedConnector.x, 2 * startPosition.y - pickedConnector.y);
    }

    private void ChechConnectorValidity(Pixel[,] pixels, Vector2 startPosition, List<Pixel> validConnectors, Vector2 connector)
    {
        int startingX = (int)(2 * startPosition.x - connector.x);
        int startingY = (int)(2 * startPosition.y - connector.y);
        for (int x = startingX; x <= startingX + width; x++)
        {
            for (int y = startingY; y <= startingY + height; y++)
            {
                if (IsOccupied(pixels, new Vector2(x, y)))
                    validConnectors.Remove(pixels[(int)connector.x, (int)connector.y]);
            }
        }
    }

    override public Pixel[] Place(Pixel[,] pixels, Vector2 startPosition)
    {
        FillRoom(pixels, startPosition);
        FillSpecialTiles(pixels, startPosition);
        return GetConnectors(pixels, startPosition);
    }

    private void FillSpecialTiles(Pixel[,] pixels, Vector2 startPosition)
    {
        SetState(pixels[(int)startPosition.x, (int)startPosition.y], State.ROOM_INIT);
        SetState(Pixel.GetPosition(pixels, GetConnectorsVectors(startPosition, connectorsY)), State.CORIDOR_Y);
        SetState(Pixel.GetPosition(pixels, GetConnectorsVectors(startPosition, connectorsX)), State.CORIDOR_X);
    }

    protected void FillRoom(Pixel[,] pixels,Vector2 startPosition)
    {
        for (int x = (int)startPosition.x; x < (int)startPosition.x + width; x++)
        {
            for (int y = (int)startPosition.y; y < (int)startPosition.y + height; y++)
            {
                SetState(pixels[x,y], State.ROOM);
            }
        }
    }

    protected Pixel[] GetConnectors(Pixel[,] pixels, Vector2 startPosition)
    {
        return Pixel.GetPosition(pixels, GetConnectorsVectors(startPosition));
    }

    protected Vector2[] GetConnectorsVectors(Vector2 startPosition,List<ConnectorCords> cords)
    {
        Vector2[] connectors = new Vector2[cords.Count];
        for(int i = 0; i < cords.Count; i++)
        {
            connectors[i] = new Vector2((int)startPosition.x + cords[i].x + (cords[i].addWidth ? width : 0),
                (int)startPosition.y + cords[i].y + (cords[i].addHeight ? height : 0));
        }
        return connectors;
    }

    protected Vector2[] GetConnectorsVectors(Vector2 startPosition)
    {
        return GetConnectorsVectors(startPosition, connectorsY).Concat(GetConnectorsVectors(startPosition,connectorsX)).ToArray();
    }

    protected Vector2[] GetConnectorsVectors(Vector2 startPosition, bool horizontalConnecting)
    {
        return horizontalConnecting ? 
            GetConnectorsVectors(startPosition, connectorsX) :
            GetConnectorsVectors(startPosition, connectorsY);
    }
}
