
using UnityEngine;

// Data Connector that determines where room has connectors
[System.Serializable]
public struct ConnectorCords
{
    // x cordinate of connector (relative to left)
    public int x;
    // adds width to x (makes x relative to rigth)
    public bool addWidth;
    //same for y
    public int y;
    public bool addHeight;
}
