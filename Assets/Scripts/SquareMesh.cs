﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareMesh
{
    public int size;
    public float edgeLength;
    public Square[,] squares;
    public bool[,] edgeSquares;

    public List<Vector2> perimeterVertices; // this one the scale edge length
    public List<Square> perimeterSquares;
    public int[] perimeterIndices;

    int topLeftCount;
    int topRightCount;
    int botLeftCount;
    int botRightCount;

    public int leftmostSplitCoord;
    public int bottomSplitCoord;

    public Vector2 centreOfMass;
    public float mass;

    List<Crack> cracks;

    public Asteroid asteroid;

    public void SetAsteroid(Asteroid asteroid)
    {
        this.asteroid = asteroid;
    }

    public void ResetColliderMesh()
    {
        Vector2[] xyPoints = perimeterVertices.ToArray();
        asteroid.gameObject.GetComponent<PolygonCollider2D>().points = xyPoints;         
    }

    public void RedrawMesh()
    {
        ResetMesh();
        ResetColliderMesh();
    }

    public void FindCentreOfMass()
    {
        float count = 0;
        float xTotal = 0;
        float yTotal = 0;
        for (int x = 0; x < squares.GetLength(0); x++)
        {
            for (int y = 0; y < squares.GetLength(1); y++)
            {
                if(squares[x,y] != null)
                {
                    count += 1f;
                    xTotal += (float)x + 0.5f;
                    yTotal += (float)y + 0.5f;
                }
            }
        }
        centreOfMass = new Vector2(xTotal*edgeLength/count,yTotal*edgeLength/count);
        mass = count*edgeLength*edgeLength*9f;
    }

    public List<SquareMesh> OnSplit()
    {
        // Create a master list of squares that need to be sorted into asteroid chunks
        HashSet<Square> allSquares = new HashSet<Square>();
        for ( int i = 0; i < this.squares.GetLength(0); i++ ) 
        { 
            for ( int j = 0; j < this.squares.GetLength(1); j++ )
            {
                if ( this.squares[i,j] != null ) { allSquares.Add(this.squares[i,j]); }
            }
        }
        // Initialize a container for the asteroid chunks
        List<SquareMesh> chunks = new List<SquareMesh>();

        // If there were no squares in the mesh that has 'split', pass null for SquareMesh
        // This means the entire mesh (split or not) has been destroyed
        if ( allSquares.Count == 0 ) { chunks.Add(null); return chunks; }

        // Useful debugging syntax:
        // System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        // stopwatch.Start();
        // stopwatch.Stop();
        // Debug.Log ("Time taken: "+(stopwatch.Elapsed));
        // stopwatch.Reset();

        int safety = 0;
        while ( allSquares.Count > 0 )
        {
            Square startingSquare = null;
            // This foreach is a hack to get an element out of a HashSet (which is not indexed so can't go hashSet[i])
            foreach ( Square s in allSquares )
            {
                startingSquare = s;
                break;
            }
            // Traverse squares via adjacent neighbours until all neighbours visited; stash traversed squares
            HashSet<Square> asteroidChunkList = CrawlThroughNeighbours(startingSquare); 
            // Remove stashed quares from the master list
            allSquares = SubtractChunk(allSquares, asteroidChunkList);

            
            // if ( asteroidChunkList.Count == 0 ) 
            // {
            //     // If the chunk contains no squares (i.e. is destroyed), pass null for SquareMesh
            //     chunks.Add(null);
            // }
            // else 
            if( allSquares.Count == 0 && chunks.Count == 0)
            {
                // If depleted allSquares while traversing first chunk, this means there's only one chunk
                // This does not need to return anything -> handled below for chunks.Count <= 1 case
                
            }
            else
            {
                // Else, this either is the first chunk with some squares still to be sorted into other chunks,
                // so it should be added, or this is the second chunk and should also be added
                chunks.Add( MakeNewAsteroidFromChunk(asteroidChunkList));
            }

            safety += 1;
            if (safety > 4) { Debug.Log("Couldn't resolve all squares into chunks - tripped anti-infinity-loop safety"); break; }
        }

        if ( chunks.Count <= 1 ) 
        { 
            // There was no split
            if ( VoidThresholdExceeded() == true )
            {
                Debug.Log("Splitting to quarters");
                chunks = SplitIntoQuarters(this);
                for ( int m = 0; m < chunks.Count; m++ )
                {
                    if ( chunks[m] == null ) { continue; }
                    chunks[m] = UpdateNeighboursAndEdges(chunks[m]);
                }
            }
            else
            {
                this.asteroid.ReDrawAsteroid();
                chunks = null;
            }
        }
        else 
        { 
            // There was a split
            Debug.Log("Split! Need to make some new asteroids and pass chunks out."); 
            for ( int m = 0; m < chunks.Count; m++ )
            {
                if ( chunks[m] == null ) { continue; }
                chunks[m] = UpdateNeighboursAndEdges(chunks[m]);
                chunks[m].UpdateSquaresInQuartersCount();
            }
        }
        return chunks;
    }

    List<SquareMesh> SplitIntoQuarters(SquareMesh sm)
    {
        SquareMesh topLeft = new SquareMesh();
        Square[,] tL = new Square[(int)Mathf.Ceil(sm.squares.GetLength(0)/2f), (int)(sm.squares.GetLength(1)/2f)];
        bool isEmptyTL = true;
        SquareMesh topRight = new SquareMesh();
        Square[,] tR = new Square[(int)(sm.squares.GetLength(0)/2f), (int)(sm.squares.GetLength(1)/2f)];
        bool isEmptyTR = true;
        SquareMesh botLeft = new SquareMesh();
        Square[,] bL = new Square[(int)Mathf.Ceil(sm.squares.GetLength(0)/2f), (int)Mathf.Ceil(sm.squares.GetLength(1)/2f)];
        bool isEmptyBL = true;
        SquareMesh botRight = new SquareMesh();
        Square[,] bR = new Square[(int)(sm.squares.GetLength(0)/2f), (int)Mathf.Ceil(sm.squares.GetLength(1)/2f)];
        bool isEmptyBR = true;

        for ( int x = 0; x < sm.squares.GetLength(0); x++ )
        {
            for ( int y = 0; y < sm.squares.GetLength(1); y++ )
            {
                Square s = sm.squares[x, y];
                if ( s == null ) { continue; }
                if ( s.x < Mathf.Ceil(sm.squares.GetLength(0)/2f) && s.y < Mathf.Ceil(sm.squares.GetLength(1)/2f) ) 
                { 
                    if ( s.y == Mathf.Ceil(sm.squares.GetLength(1)/2f)  -1 || s.x == Mathf.Ceil(sm.squares.GetLength(0)/2f) - 1 ) { if( Random.Range(0,1f) > 0.5f) { continue; }; }
                    bL[x, y] = new Square(x, y); 
                    isEmptyBL = false;
                }

                if ( s.x < Mathf.Ceil(sm.squares.GetLength(0)/2f) && s.y >= Mathf.Ceil(sm.squares.GetLength(1)/2f) ) 
                { 
                    if ( s.y == Mathf.Ceil(sm.squares.GetLength(1)/2f) || s.x == Mathf.Ceil(sm.squares.GetLength(0)/2f) - 1 ) { if( Random.Range(0,1f) > 0.5f) { continue; }; }
                    tL[x, y - (int)Mathf.Ceil(sm.squares.GetLength(1)/2f)] = new Square(x, y - (int)Mathf.Ceil(sm.squares.GetLength(1)/2f)); 
                    isEmptyTL = false;
                }

                if ( s.x >= Mathf.Ceil(sm.squares.GetLength(0)/2f) && s.y < Mathf.Ceil(sm.squares.GetLength(1)/2f) ) 
                { 
                    if ( s.y == Mathf.Ceil(sm.squares.GetLength(1)/2f) - 1 || s.x == Mathf.Ceil(sm.squares.GetLength(0)/2f) ) { if( Random.Range(0,1f) > 0.5f) { continue; }; }
                    bR[x - (int)Mathf.Ceil(sm.squares.GetLength(0)/2f), y] = new Square(x - (int)Mathf.Ceil(sm.squares.GetLength(0)/2f), y); 
                    isEmptyBR = false;
                }
                
                if ( s.x >= Mathf.Ceil(sm.squares.GetLength(0)/2f) && s.y >= Mathf.Ceil(sm.squares.GetLength(1)/2f) ) 
                { 
                    if ( s.y == Mathf.Ceil(sm.squares.GetLength(1)/2f) || s.x == Mathf.Ceil(sm.squares.GetLength(0)/2f) ) { if( Random.Range(0,1f) > 0.5f) { continue; }; }
                    tR[x - (int)Mathf.Ceil(sm.squares.GetLength(0)/2f), y - (int)Mathf.Ceil(sm.squares.GetLength(1)/2f)] = 
                                new Square(x - (int)Mathf.Ceil(sm.squares.GetLength(0)/2f), y - (int)Mathf.Ceil(sm.squares.GetLength(1)/2f)); 
                    isEmptyTR = false;
                }
            }
        }
        botLeft.squares = bL; topLeft.squares = tL; botRight.squares = bR; topRight.squares = tR;

        botLeft.leftmostSplitCoord = 0;
        botLeft.bottomSplitCoord = 0;

        botLeft.UpdateSquaresInQuartersCount();
        Debug.Log(botLeft.topLeftCount);

        topLeft.leftmostSplitCoord = 0;
        topLeft.bottomSplitCoord = (int)Mathf.Ceil(sm.squares.GetLength(1)/2f);
        topLeft.UpdateSquaresInQuartersCount();

        botRight.leftmostSplitCoord = (int)Mathf.Ceil(sm.squares.GetLength(0)/2f);
        botRight.bottomSplitCoord = 0;
        botRight.UpdateSquaresInQuartersCount();

        topRight.leftmostSplitCoord = (int)Mathf.Ceil(sm.squares.GetLength(0)/2f);
        topRight.bottomSplitCoord = (int)Mathf.Ceil(sm.squares.GetLength(1)/2f);
        topRight.UpdateSquaresInQuartersCount();

        if ( isEmptyBL ) { botLeft = null; }
        if ( isEmptyTL ) { topLeft = null; }
        if ( isEmptyBR ) { botRight = null; }
        if ( isEmptyTR ) { topRight = null; }

        List<SquareMesh> chunks = new List<SquareMesh>();
        chunks.Add(botLeft);
        chunks.Add(topLeft);
        chunks.Add(botRight);
        chunks.Add(topRight);
        foreach(SquareMesh newMesh in chunks)
        {
            if ( newMesh == null ){ continue; }
            newMesh.size = this.size;
            newMesh.edgeLength = this.edgeLength;
        }
        if ( botLeft == null && topLeft == null && botRight == null && topRight == null ) { chunks = null; }
        return chunks;

    }

    bool VoidThresholdExceeded()
    {
        bool voidThresholdExceeded = false;
        float threshold = 0.33f;
        float maxCountInQuarter = (float)this.squares.GetLength(0) * (float)this.squares.GetLength(1) / 4f;

        if( topLeftCount/maxCountInQuarter < threshold  || botLeftCount/maxCountInQuarter < threshold || 
            topRightCount/maxCountInQuarter < threshold || botRightCount/maxCountInQuarter < threshold)
        {
            voidThresholdExceeded = true;
        }
        return voidThresholdExceeded;
    }

    public SquareMesh MakeNewAsteroidFromChunk(HashSet<Square> chunkSquaresList)
    {
        SquareMesh newSquareMesh = new SquareMesh();
        int leftmostCoord = 999; int rightmostCoord = -1; int topCoord = -1; int bottomCoord = 999;
        foreach ( Square s in chunkSquaresList )
        {
            if ( s.x < leftmostCoord ) { leftmostCoord = s.x; }
            if ( s.x > rightmostCoord ) { rightmostCoord = s.x; }
            if ( s.y < bottomCoord ) { bottomCoord = s.y; }
            if ( s.y > topCoord ) { topCoord = s.y; }
        }
        newSquareMesh.leftmostSplitCoord = leftmostCoord;
        newSquareMesh.bottomSplitCoord = bottomCoord;

        Square[,] chunkSquaresArray = new Square[rightmostCoord - leftmostCoord + 1, topCoord - bottomCoord + 1];
        foreach ( Square s in chunkSquaresList )
        {
            Square newSquare = new Square(s.x - leftmostCoord, s.y - bottomCoord);
            chunkSquaresArray[newSquare.x, newSquare.y] = newSquare;
        }
        newSquareMesh.squares = chunkSquaresArray;
        newSquareMesh.size = this.size;
        newSquareMesh.edgeLength = this.edgeLength;
        return newSquareMesh;
    }

    public HashSet<Square> SubtractChunk(HashSet<Square> allSquares, HashSet<Square> asteroidChunkList)
    {
        foreach ( Square s in asteroidChunkList )
        {
            if ( allSquares.Contains(s) ) 
            { 
                allSquares.Remove(s); 
            }
        }
        return allSquares;
    }

    public HashSet<Square> CrawlThroughNeighbours(Square startingSquare)
    {
        HashSet<Square> gotNeighboursOf = new HashSet<Square>(); 
        HashSet<Square> chunkSquaresList = new HashSet<Square>(); 
        HashSet<Square> searchForNeighbours = new HashSet<Square>();

        Square currentSquare = null;
        searchForNeighbours.Add(startingSquare);
        chunkSquaresList.Add(startingSquare);
        int safety = 0;
        while ( searchForNeighbours.Count > 0 )
        {
            // This foreach is a hack to get an element out of a HashSet (which is not indexed so can't go hashSet[i])
            foreach ( Square s in searchForNeighbours )
            {
                currentSquare = s;
                break;
            }

            foreach (Square s in currentSquare.neighbourSquares)
            {
                if ( s != null )
                {
                    if ( gotNeighboursOf.Contains(s) == false && searchForNeighbours.Contains(s) == false ) { searchForNeighbours.Add(s); }
                    if ( chunkSquaresList.Contains(s) == false ) { chunkSquaresList.Add(s); }
                }
            }
            gotNeighboursOf.Add(currentSquare);
            searchForNeighbours.Remove(currentSquare);
            safety += 1;
            if (safety > 10000){ Debug.Log("Safety break in Crawl"); break;}
        }
        return chunkSquaresList;
    }

    public List<SquareMesh> RemoveSquaresInRadius(Vector2 centre, float radius)
    {
        List<Square> squaresToRemove = SquaresInRadius(centre, radius);
        List<SquareMesh> chunks = new List<SquareMesh>();
        chunks = null;
        
        if ( squaresToRemove.Count > 0 ) 
        {
            RemoveSquares(squaresToRemove);
            chunks = OnSplit();
        }
        return chunks;
    }
    
    // public void RemoveSquareAtWorldPosition(Vector2 worldPosition)
    // {
    //     Square squareToRemove = SquareAtWorldPoint(worldPosition);
        
    //     if ( squareToRemove != null ) 
    //     {
    //         List<Square> squaresToRemove = new List<Square>();
    //         squaresToRemove.Add(squareToRemove);
    //         RemoveSquares(squaresToRemove);
    //         // Check for a split here?
    //         // Debug.Log("Hitting 'OnSplit'");
    //         OnSplit();
            
    //     }
    // }

    public void ScaleEdgeLength()
    {
        List<Vector2> tmp = new List<Vector2>();
        foreach ( Vector2 v in perimeterVertices )
        {
            tmp.Add(v * this.edgeLength);
        }
        this.perimeterVertices = tmp;
    }

    public void ResetMesh()
    {
  //      Debug.Log(asteroid);
//        Debug.Log(asteroid.gameObject);
        MeshFilter meshFilter = asteroid.gameObject.GetComponent<MeshFilter>();
        Mesh mesh2 = new Mesh();
        meshFilter.mesh = mesh2;
        
        // Mesh only accepts Vector3's, so got to convert...
        List<Vector3> meshVertices = new List<Vector3>();
        Vector3 temp;
        foreach (Vector2 v2 in perimeterVertices)
        {
            temp = new Vector3((float)v2.x, (float)v2.y, 0f);
            meshVertices.Add( temp );
        }

        mesh2.vertices = meshVertices.ToArray();
        meshFilter.mesh = mesh2;

        perimeterIndices = new int[2*meshVertices.Count];

        for (int i = 0; i < meshVertices.Count; i++)
        {
            perimeterIndices[2*i] = i;    
            perimeterIndices[2*i+1] = i+1;
        }

        perimeterIndices[(meshVertices.Count)*2-1] = 0;

        mesh2.SetIndices(perimeterIndices, MeshTopology.Lines, 0);
    }

    public List<Square> SquaresInRadius(Vector2 collisionPoint, float radius)
    {

        Vector2 asteroidCorner = (Vector2)asteroid.gameObject.transform.position;
        Vector2 offsetCollisionPoint = collisionPoint - asteroidCorner;
        float asteroidRotationAngle = asteroid.gameObject.transform.rotation.eulerAngles.z * Mathf.PI/180f;
        float rotatedX = offsetCollisionPoint.x * Mathf.Cos(-asteroidRotationAngle) - offsetCollisionPoint.y * Mathf.Sin(-asteroidRotationAngle);
        float rotatedY = offsetCollisionPoint.x * Mathf.Sin(-asteroidRotationAngle) + offsetCollisionPoint.y * Mathf.Cos(-asteroidRotationAngle);

        float asteroidFrameX = rotatedX / edgeLength;
        float asteroidFrameY = rotatedY / edgeLength;

        Vector2 collisionPointInAsteroidCoords = new Vector2(asteroidFrameX, asteroidFrameY);
        List<Square> affectedSquares = new List<Square>();

        for (int i = (int)asteroidFrameX - ( (int)radius + 1 ); i < (int)asteroidFrameX + ( (int)radius + 1 ); i++)
        {
            for (int j = (int)asteroidFrameY - ( (int)radius + 1 ); j < (int)asteroidFrameY + ( (int)radius + 1 ); j++)
            {
                if ( i < 0 || j < 0 || i > squares.GetLength(0) - 1 || j > squares.GetLength(1) - 1 ) { continue; }
                if ( squares[i, j] == null ) { continue; }
                // If one coordinate alone is > radius, no need to compute expensive Pythagoras
                if ( Mathf.Abs( ((float)squares[i, j].x + 0.5f) - asteroidFrameX ) > radius || Mathf.Abs( ((float)squares[i, j].y + 0.5f) - asteroidFrameY) > radius  ) { continue; }
                if ( Mathf.Sqrt( Mathf.Pow( ((float)squares[i, j].x + 0.5f) - asteroidFrameX, 2) + Mathf.Pow( ((float)squares[i, j].y + 0.5f) - asteroidFrameY, 2) ) < radius)
                {
                    affectedSquares.Add(squares[i, j]);
                }
            }
        }
        return affectedSquares;
    }

    public Square SquareAtWorldPoint(Vector3 worldPoint)
    {
        Square closestSquare = null;
        Vector2 point = (Vector2)((worldPoint - asteroid.gameObject.transform.position) / this.edgeLength);

        point -= new Vector2(0.5f, 0.5f);

        if ( (int)Mathf.Round(point.x) > this.size - 1 || (int)Mathf.Round(point.y) > size - 1 ) { return closestSquare; }
        if ( (int)Mathf.Round(point.x) < 0 || (int)Mathf.Round(point.y) < 0 ) { return closestSquare; }
        closestSquare = this.squares[(int)Mathf.Round(point.x), (int)Mathf.Round(point.y)];

        return closestSquare;
    }

    public void GenerateCircularMesh(int size, float celSize)
    {
        // size determines the outer extents of the bounding square
        this.size = size;
        this.edgeLength = celSize;
        this.squares = new Square[size, size];
        // generate shape
        Vector2 centre = new Vector2(size/2,size/2);
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2 squareCentre = new Vector2(x,y);
                float distance = Vector2.Distance(squareCentre,centre);
                if(distance < size/2)
                {
                    squares[x,y] = new Square(x, y);
                }
                else
                {
                    squares[x,y] = null;
                }
            }
        }
        // UpdateNeighboursAndEdges();
    }

    

    public SquareMesh UpdateNeighboursAndEdges(SquareMesh sm)
    {
        for (int x = 0; x < sm.squares.GetLength(0); x++)
        {
            for (int y = 0; y < sm.squares.GetLength(1); y++)
            {
                if(sm.squares[x,y] == null)
                {
                    continue;
                }
                if(y != 0)
                {
                    if(sm.squares[x,y-1] == null)
                    {
                        sm.squares[x,y].AddEdge(2);
                    }
                    else
                    {
                        sm.squares[x,y].neighbourSquares[2] = sm.squares[x,y-1];
                    }
                }
                else
                {
                    sm.squares[x,0].AddEdge(2);
                }
                if(y != sm.squares.GetLength(1) - 1)
                {
                    if(sm.squares[x,y+1] == null)
                    {
                        sm.squares[x,y].AddEdge(0);
                    }
                    else
                    {
                        sm.squares[x,y].neighbourSquares[0] = sm.squares[x,y+1];
                    }
                }
                else
                {
                    sm.squares[x, sm.squares.GetLength(1) - 1].AddEdge(0);
                }
                if(x != 0)
                {
                    if(sm.squares[x-1,y] == null)
                    {
                        sm.squares[x,y].AddEdge(3);
                    }
                    else
                    {
                        sm.squares[x,y].neighbourSquares[3] = sm.squares[x-1,y];
                    }
                }
                else
                {
                    sm.squares[0,y].AddEdge(3);
                }
                if(x != sm.squares.GetLength(0) - 1)
                {
                    if(sm.squares[x+1,y] == null)
                    {
                        sm.squares[x,y].AddEdge(1);
                    }
                    else
                    {
                        sm.squares[x,y].neighbourSquares[1] = sm.squares[x+1,y];
                    }
                }
                else
                {
                    sm.squares[sm.squares.GetLength(0) - 1, y].AddEdge(1);
                }
                
            }
        }
        FindCentreOfMass();
        //FindMass();
        return sm;
    }

    void UpdateSquaresInQuartersCount()
    {
        for (int x = 0; x < this.squares.GetLength(0); x++)
        {
            for (int y = 0; y < this.squares.GetLength(1); y++)
            {
                if ( squares[x, y] == null ){ continue; }
                if ( x < Mathf.Ceil(squares.GetLength(0)/2f) && y < Mathf.Ceil(squares.GetLength(1)/2f) ) { this.botLeftCount += 1; }
                if ( x < Mathf.Ceil(squares.GetLength(0)/2f) && y >= Mathf.Ceil(squares.GetLength(1)/2f) ) { this.topLeftCount += 1; }
                if ( x >= Mathf.Ceil(squares.GetLength(0)/2f) && y < Mathf.Ceil(squares.GetLength(1)/2f) ) { this.botRightCount += 1; }
                if ( x >= Mathf.Ceil(squares.GetLength(0)/2f) && y >= Mathf.Ceil(squares.GetLength(1)/2f) ) { this.topRightCount += 1; }
            }
        }
    }

    public void GenerateSquareMesh(int size, float celSize)
    {
        this.size = size;
        this.edgeLength = celSize;
        this.squares = new Square[size, size];
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Square newSquare = new Square(x, y);
                // Check if a square is an edge square, and if so update externalEdges
                if ( y == size - 1 ){ newSquare.AddEdge(0); }
                else {newSquare.externalEdges[0] = null;}

                if ( x == size - 1 ){ newSquare.AddEdge(1); }
                else {newSquare.externalEdges[1] = null;}

                if ( y == 0 ){ newSquare.AddEdge(2);}
                else {newSquare.externalEdges[2] = null;}
                
                if ( x == 0 ){ newSquare.AddEdge(3); }
                else {newSquare.externalEdges[3] = null;}
                
                this.squares[x, y] = newSquare;
            }
        }
        UpdateSquaresInQuartersCount();

        // Now that squares are created, we can populate neighbours
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // Update neighbourSquares; intialise to solid SquareMesh, so 
                // only edgeSquares have non-complete neighbour list
                // if ( (x > 0 && x < size - 1) && (y > 0 && y < size - 1) )
                // {
                if ( y+1 <= size-1 ) { squares[x,y].neighbourSquares[0] = this.squares[x, y+1]; }
                else { squares[x,y].neighbourSquares[0] = null; }

                if ( x+1 <= size-1 ) { squares[x,y].neighbourSquares[1] = this.squares[x+1, y]; }
                else { squares[x,y].neighbourSquares[1] = null; }

                if ( y-1 >= 0 ) { squares[x,y].neighbourSquares[2] = this.squares[x, y-1]; }
                else { squares[x,y].neighbourSquares[2] = null; }

                if ( x-1 >= 0 ) { squares[x,y].neighbourSquares[3] = this.squares[x-1, y]; }
                else { squares[x,y].neighbourSquares[3] = null; }
                // }
            }
        }        

        // Finally, we set up the chain of edges to draw the shape outline
        // FindOutline();
        FindCentreOfMass();
    }

    public bool AllEdgesAccountedFor(Square square, List<Vector2> vertices)
    {
        foreach(SquareEdge edge in square.externalEdges)
        {
            if (edge == null) { continue; }
            if (vertices.Contains(edge.end) == false) { return false; }
        }
        return true;
    }

    public void FindOutline()
    {
        List<Vector2> vertices = new List<Vector2>();
        Square startingSquare = null;
        for (int i = 0; i < squares.GetLength(0); i++)
        {
            for (int j = 0; j < squares.GetLength(1); j++)
            {
                if (squares[i,j] != null){ startingSquare = squares[i,j]; } // Retain starting square to use in halting condition
                if ( startingSquare != null) { break; };
            }
            if ( startingSquare != null) { break; };
        }

        if ( startingSquare == null ) { Debug.Log("Could not find a starting square!"); }
        Square currentSquare = startingSquare; // Start with any square

        // Get things started by explicitly calling ExtendVerticesList on the first square
        vertices = ExtendVerticesList(currentSquare, vertices);
        // Debug.Log(vertices.Count);

        // At first iteration currentSquare happens to be starting square without having looped all 
        // around the perimeter, so I need this flag to make while not bail at first iteration
        bool firstLoopCompleted = false; 
        // Now loop around the perimeter until we return to the starting square.
        // Initialise a list that will hold all squares that have an edge on the permieter
        List<Square> outerSquares = new List<Square>();
        while ( ( currentSquare != startingSquare || AllEdgesAccountedFor(currentSquare, vertices) == false ) || firstLoopCompleted == false )
        {
            firstLoopCompleted = true;
            
            
            // Check which of the currentSquare's neighbours contains the edge that starts where cS's latest edge 
            // ended and which is also active
            // Debug.Log("Searching for candidates");
            Square candidate = null;
            int startFromEdgeIndex = 0;
            for (int dx = -1; dx < 2; dx++)
            {
                for (int dy = -1; dy < 2; dy++)
                {
                    // startFromEdgeIndex keeps track which edge (top, right...) is the direct continuation
                    // of the perimeter and passes that information to ExtendVerticesList to make sure
                    // we go through the outline in sequence
                    startFromEdgeIndex = -1;
                    // Debug.Log(currentSquare.neighbourSquares[k]);
                    // Debug.Log("Current square is ");
                    // Debug.Log(currentSquare.x);
                    // Debug.Log(currentSquare.y);
                    if ( currentSquare.x+dx >= 0 && currentSquare.x+dx <= squares.GetLength(0) - 1 && currentSquare.y+dy >= 0 && currentSquare.y+dy <= squares.GetLength(1) - 1 )
                    {
                        if ( dx == 0 && dy == 0) { continue; }
                        // Debug.Log("Candidate square is:");
                        // Debug.Log(currentSquare.x + dx);
                        // Debug.Log(currentSquare.y + dy);
                        candidate = squares[currentSquare.x+dx, currentSquare.y+dy];
                        // if ( outerSquares.Contains(candidate) ){ continue; } // Prevents looping back to where we came from
                        if ( candidate == null ){ continue; }
                        for (int l = 0; l < 4; l++)
                        {
                            if ( candidate.externalEdges[l] != null )
                            {
                                if ( candidate.externalEdges[l].start == vertices[vertices.Count-1] )
                                {
                                    currentSquare = candidate;
                                    startFromEdgeIndex = l;
                                    // Debug.Log("Continuation edge is:");
                                    // Debug.Log(startFromEdgeIndex);
                                    break;
                                }
                            }
                        }
                    }
                    // Only significance of this is that we have gone into the innermost 'if' in above block
                    // (we know that since startFromEdgeIndex is pre-set to -1 by default) and we can stop looking
                    // as we already found the square and the edge that continue our outline
                    if ( startFromEdgeIndex >= 0 ) { break; } 
                }
                if ( startFromEdgeIndex >= 0 ) { break; }
            }
            // Add the square to the perimeter squares list; thanks to this, functions that interact
            // with the outer squares don't have to loop the whole nxn array
            outerSquares.Add(currentSquare);
            // Stop the loop if there is no neighbours with an edge continuing the perimeter
            // or if we have returned to the starting square
            // Debug.Log("Halting or continuing on the perimeter");
            if ( startFromEdgeIndex < 0 ) {  break;}
            if ( candidate == null ) { break; }
            if ( candidate == startingSquare && AllEdgesAccountedFor(currentSquare, vertices) ) { break; }
            vertices = ExtendVerticesList(currentSquare, vertices, startFromEdgeIndex);
            // Debug.Log("Here");
            // Debug.Log(( ( currentSquare != startingSquare && AllEdgesAccountedFor(currentSquare, vertices) ) || firstLoopCompleted == false ));
            // Debug.Log(( ( currentSquare != startingSquare && AllEdgesAccountedFor(currentSquare, vertices) ) ));
            // Debug.Log(currentSquare != startingSquare);
            // Debug.Log(AllEdgesAccountedFor(currentSquare, vertices));
            // Debug.Log(firstLoopCompleted);
        }
        this.perimeterSquares = outerSquares;
        this.perimeterVertices = vertices;
    }

    public List<Vector2> ExtendVerticesList(Square currentSquare, List<Vector2> vertices, int startFromEdgeIndex = 0)
    {
        // startFromEdgeIndex ensures that, if the square has more than one active edge,
        // we start adding from that edge (to avoid zig-zags in the permiter outline by
        // going further ahead and then having to come back)

        // Find first edge of the square that is on the shape perimeter
        // count keeps track of how many edges we have investigated
        // Debug.Log("Extending vertices on square: ");
        // Debug.Log(new Vector2(currentSquare.x, currentSquare.y));
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            count += 1;
            // Debug.Log(i);
            // Debug.Log(count);
            // Debug.Log(startFromEdgeIndex);
            // Debug.Log((startFromEdgeIndex + i) % 4);
            // Debug.Log( currentSquare.externalEdges[(startFromEdgeIndex + i) % 4] );
            if ( currentSquare.externalEdges[(startFromEdgeIndex + i) % 4] != null )
            {
                // Debug.Log("Adding a vertex: ");
                // Debug.Log((startFromEdgeIndex + i) % 4);
                // vertices.Add(currentSquare.externalEdges[i].start);
                vertices.Add(currentSquare.externalEdges[(startFromEdgeIndex + i) % 4].end);
                break;
            }
            
        }
        // Find if the same square holds the next clockwise edge on perimeter
        // if investigated more than 4 edges (count > 4) then stop as each square
        // has at most 4 edges
        // Debug.Log("Investigating subsequent edges on the same quare");
        while (count < 4)
        {
            for (int j = count; j < 4; j++)
            {
                // Debug.Log(count);
                // Debug.Log(startFromEdgeIndex);
                // Debug.Log((startFromEdgeIndex + j) % 4);
                // Debug.Log(currentSquare.externalEdges[(startFromEdgeIndex + j) % 4]);
                // If active edge is followed by inactive, next clockwise active edge belongs to different square so break both loops
                if ( currentSquare.externalEdges[(startFromEdgeIndex + j) % 4] == null ) 
                { 
                    // Debug.Log("Active edge followed by a dead edge - breaking");
                    count = 4; 
                    break; 
                }
                else
                {
                    // vertices.Add(currentSquare.externalEdges[j].start);
                    // Debug.Log("Next edge on the same square is:");
                    // Debug.Log((startFromEdgeIndex + j) % 4);
                    vertices.Add(currentSquare.externalEdges[(startFromEdgeIndex + j) % 4].end);
                }
                count += 1;
            }
        }
        return vertices;
    }

    public void SparsifyMesh(int deletionCount)
    {
        List<Square> squaresToRemove = new List<Square>();
        for (int i = 0; i < deletionCount; i++)
        {
            int toRemoveIndex = Random.Range(0, perimeterSquares.Count - 1);
            squaresToRemove.Add(perimeterSquares[toRemoveIndex]);
        }
        RemoveSquares(squaresToRemove);
    }

    public void RemoveSquares(List<Square> squaresToRemove)
    {
        foreach ( Square s in squaresToRemove )
        {
            if (s == null) { continue; }

            // Update neighbours and edges of neighbours
            if ( s.neighbourSquares[0] != null )
            {
                s.neighbourSquares[0].neighbourSquares[2] = null;
                s.neighbourSquares[0].AddEdge(2);
                if ( perimeterSquares.Contains(s) == true ) { perimeterSquares.Remove(s); }
                if ( perimeterSquares.Contains(s.neighbourSquares[0]) == false ){ perimeterSquares.Add(s.neighbourSquares[0]); }
            }

            if ( s.neighbourSquares[1] != null )
            {
                s.neighbourSquares[1].neighbourSquares[3] = null;
                s.neighbourSquares[1].AddEdge(3);
                if ( perimeterSquares.Contains(s) == true ) { perimeterSquares.Remove(s); }
                if ( perimeterSquares.Contains(s.neighbourSquares[1]) == false ){ perimeterSquares.Add(s.neighbourSquares[1]); }
            }

            if ( s.neighbourSquares[2] != null )
            {
                s.neighbourSquares[2].neighbourSquares[0] = null;
                s.neighbourSquares[2].AddEdge(0);
                if ( perimeterSquares.Contains(s) == true ) { perimeterSquares.Remove(s); }
                if ( perimeterSquares.Contains(s.neighbourSquares[2]) == false ){ perimeterSquares.Add(s.neighbourSquares[2]); }
            }

            if ( s.neighbourSquares[3] != null )
            {
                s.neighbourSquares[3].neighbourSquares[1] = null;
                s.neighbourSquares[3].AddEdge(1);
                if ( perimeterSquares.Contains(s) == true ) { perimeterSquares.Remove(s); }
                if ( perimeterSquares.Contains(s.neighbourSquares[3]) == false ){ perimeterSquares.Add(s.neighbourSquares[3]); }
            }

            if ( s.x < Mathf.Ceil(squares.GetLength(0)/2f) && s.y  < Mathf.Ceil(squares.GetLength(1)/2f) ) { botLeftCount -= 1; }
            if ( s.x < Mathf.Ceil(squares.GetLength(0)/2f) && s.y  >= Mathf.Ceil(squares.GetLength(1)/2f) ) { topLeftCount -= 1; }
            if ( s.x >= Mathf.Ceil(squares.GetLength(0)/2f) && s.y < Mathf.Ceil(squares.GetLength(1)/2f) ) { botRightCount -= 1; }
            if ( s.x >= Mathf.Ceil(squares.GetLength(0)/2f) && s.y >= Mathf.Ceil(squares.GetLength(1)/2f) ) { topRightCount -= 1; }

            this.squares[s.x, s.y] = null;
        }
    }


}
