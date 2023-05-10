using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    private State state = State.EMPTY;
    public GameObject[] states;

    public Vector2 GetPosition()
    {
        return new Vector2((transform.position.x), (transform.position.y));
    }

    public static Pixel[] GetPosition(Pixel[,] pixelArea, Vector2[] vectors)
    {
        Pixel[] vectorsPixels = new Pixel[vectors.Length];
        for (int i = 0; i < vectors.Length; i++)
        {
            vectorsPixels[i] = pixelArea[(int)vectors[i].x, (int)vectors[i].y];
        }
        return vectorsPixels;
    }

    public void SetState(State state)
    {
        this.state = state;
    }

    public State GetState()
    {
        return state;
    }

    public void GenerateTile(Transform parent)
    {
        if(state != State.EMPTY)
            Instantiate(states[(int)state-1],
                new Vector3(transform.position.x,transform.position.y,0),Quaternion.identity,parent);
    }
}
