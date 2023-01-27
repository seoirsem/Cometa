using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Square
{
    public int x;
    public int y;

    public Square[] neighbourSquares; // only full face neighbbours, i.e. no diagonals
    // public bool[] externalEdges; 
    public SquareEdge[] externalEdges; // 0 is top, then clockwise: 1 is right etc.

    public Square(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.externalEdges = new SquareEdge[4]; 
        this.neighbourSquares = new Square[4];
    }

    public void AddEdge(int index)
    {
        Vector2 baseVector = new Vector2(this.x, this.y);
        switch( index )
        {
            case 0:
                this.externalEdges[0] = new SquareEdge(baseVector + new Vector2(0, 1), baseVector + new Vector2(1, 1));
                break;

            case 1:
                this.externalEdges[1] = new SquareEdge(baseVector + new Vector2(1, 1), baseVector + new Vector2(1, 0));
                break;

            case 2:
                this.externalEdges[2] = new SquareEdge(baseVector + new Vector2(1, 0), baseVector);
                break;

            case 3:
                this.externalEdges[3] = new SquareEdge(baseVector, baseVector + new Vector2(0, 1));
                break;
        }
    }


}
