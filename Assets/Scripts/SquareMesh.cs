using System.Collections;
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


    Asteroid asteroid;

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
        //ScaleEdgeLength();
        ResetMesh();
        ResetColliderMesh();
    }

    public void OnSplit()
    {
        Debug.Log("In OnSplit");
        List<Square> allSquares = new List<Square>();
        for ( int i = 0; i < this.squares.GetLength(0); i++ ) 
        { 
            for ( int j = 0; j < this.squares.GetLength(1); j++ )
            {
                if ( this.squares[i,j] != null ) { allSquares.Add(this.squares[i,j]); }
            }
        }
        if ( allSquares.Count == 0 ) { return; }

        List<SquareMesh> chunks = new List<SquareMesh>();

        int safety = 0;
        while ( allSquares.Count > 0 )
        {
            Debug.Log(allSquares.Count);
            Square startingSquare = allSquares[0];        
            List<Square> asteroidChunkList = CrawlThroughNeighbours(startingSquare); 
            Debug.Log(asteroidChunkList.Count);
            allSquares = SubtractChunk(allSquares, asteroidChunkList);
            // SquareMesh newAsteroidChunk = MakeNewAsteroidFromChunk(asteroidChunkList);
            chunks.Add( MakeNewAsteroidFromChunk(asteroidChunkList) );
            Debug.Log("Made a chunk");
            safety += 1;
            if (safety > 4) { break; }
        }
        if ( chunks.Count == 1 ) 
        { 
            Debug.Log("There was no split"); 
            FindOutline();
            ScaleEdgeLength();
            ResetMesh();
            ResetColliderMesh();
        }
        else 
        { 
            Debug.Log("Split! Need to make some new asteroids and pass chunks out."); 
            FindOutline();
            ScaleEdgeLength();
            ResetMesh();
            ResetColliderMesh();
        }
    }

    public SquareMesh MakeNewAsteroidFromChunk(List<Square> chunkSquaresList)
    {
        SquareMesh newSquareMesh = new SquareMesh();
        int leftmostCoord = -1; int rightmostCoord = -1; int topCoord = -1; int bottomCoord = -1;
        foreach ( Square s in chunkSquaresList )
        {
            if ( s.x < leftmostCoord ) { leftmostCoord = s.x; }
            if ( s.x > rightmostCoord ) { rightmostCoord = s.x; }
            if ( s.y < bottomCoord ) { bottomCoord = s.y; }
            if ( s.y > topCoord ) { topCoord = s.y; }
        }
        Square[,] chunkSquaresArray = new Square[rightmostCoord - leftmostCoord, topCoord - bottomCoord];
        foreach ( Square s in chunkSquaresList )
        {
            chunkSquaresArray[s.x, s.y] = s;
        }
        newSquareMesh.squares = chunkSquaresArray;
        newSquareMesh.edgeLength = this.edgeLength;
        return newSquareMesh;
    }

    public List<Square> SubtractChunk(List<Square> allSquares, List<Square> asteroidChunkList)
    {
        Debug.Log("Subtracting chunk");
        Debug.Log(asteroidChunkList.Count);
        foreach ( Square s in asteroidChunkList )
        {
            if ( allSquares.Contains(s) ) 
            { 
                allSquares.Remove(s); 
            }
            else { Debug.Log("allSquares did not contain the chunk's element, and I think it should?"); }
        }
        return allSquares;
    }

    public List<Square> CrawlThroughNeighbours(Square startingSquare)
    {
        Debug.Log("Crawling through  chunk");
        List<Square> gotNeighboursOf = new List<Square>(); List<Square> chunkSquaresList = new List<Square>(); List<Square> searchForNeighbours = new List<Square>();
        Square currentSquare;
        searchForNeighbours.Add(startingSquare);
        int safety = 0;
        while ( searchForNeighbours.Count > 0 )
        {
            currentSquare = searchForNeighbours[0];
            // Debug.Log("Currently at square:");
            // Debug.Log(new Vector2( currentSquare.x, currentSquare.y ));

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
            if (safety > 100){break;}
        }
        // Square[,] chunkSquaresArray = Square[rightmostCoord - leftmostCoord + 1, topCoord - bottomCoord + 1];
        // foreach ( Square s in chunkSquaresList )
        // {
        //     chunkSquaresArray[s.x, s.y] = s;
        // }
        Debug.Log("Finished crawling through a chunk");
        return chunkSquaresList;
    }

    public void RemoveSquaresInRadius(Vector2 centre, float radius)
    {
        List<Square> squaresToRemove = SquaresInRadius(centre, radius);
        
        if ( squaresToRemove.Count > 0 ) 
        {
            RemoveSquares(squaresToRemove);
            // Check for a split here?
            Debug.Log("Hitting 'OnSplit'");
            OnSplit();
        }
    }
    
    public void RemoveSquareAtWorldPosition(Vector2 worldPosition)
    {
        Square squareToRemove = SquareAtWorldPoint(worldPosition);
        
        if ( squareToRemove != null ) 
        {
            List<Square> squaresToRemove = new List<Square>();
            squaresToRemove.Add(squareToRemove);
            RemoveSquares(squaresToRemove);
            // Check for a split here?
            Debug.Log("Hitting 'OnSplit'");
            OnSplit();
            
        }
    }

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
        
        Debug.Log("Coordinates in asteroid frame are:");
        Debug.Log(collisionPointInAsteroidCoords);

        List<Square> affectedSquares = new List<Square>();
        for (int i = 0; i < squares.GetLength(0); i++)
        {
            for (int j = 0; j < squares.GetLength(1); j++)
            {
                if ( squares[i, j] == null ) { continue; }
                Debug.Log("Evaluating square");
                Debug.Log(new Vector2(i,j));
                Debug.Log("Against explosion centre:");
                Debug.Log(new Vector2(rotatedX, rotatedY));
                Debug.Log("And radius:");
                Debug.Log(radius);
                if ( Mathf.Sqrt( Mathf.Pow( (squares[i, j].x + 0.5f) - asteroidFrameX, 2) + Mathf.Pow((squares[i, j].y + 0.5f) - asteroidFrameY, 2) ) < radius)
                {
                    Debug.Log("Affected square added:");
                    Debug.Log(new Vector2(i,j));
                    affectedSquares.Add(squares[i, j]);
                }
            }
        }
        return affectedSquares;
    }

    public Square SquareAtWorldPoint(Vector3 worldPoint)
    {
        // Debug.Log("World point: ");
        // Debug.Log(worldPoint);
        Square closestSquare = null;
        Vector2 point = (Vector2)((worldPoint - asteroid.gameObject.transform.position) / this.edgeLength);
        // Debug.Log("Point in square reference frame: ");
        // Debug.Log(point);
        point -= new Vector2(0.5f, 0.5f);
        // Debug.Log("And now shifted by half: ");
        // Debug.Log(point);
        if ( (int)Mathf.Round(point.x) > this.size - 1 || (int)Mathf.Round(point.y) > size - 1 ) { return closestSquare; }
        if ( (int)Mathf.Round(point.x) < 0 || (int)Mathf.Round(point.y) < 0 ) { return closestSquare; }
        closestSquare = this.squares[(int)Mathf.Round(point.x), (int)Mathf.Round(point.y)];
        // if (closestSquare != null)
        // {
        //     Debug.Log(new Vector2(closestSquare.x, closestSquare.y));
        // }
        
        return closestSquare;
    }

    public void GenerateMesh(int size)
    {
        this.size = size;
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
        FindOutline();
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
            // Debug.Log(i);
            int toRemoveIndex = Random.Range(0, perimeterSquares.Count - 1);
            // Debug.Log(toRemoveIndex);
            // Debug.Log(perimeterSquares[toRemoveIndex].x);
            // Debug.Log(perimeterSquares[toRemoveIndex].y);
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
            this.squares[s.x, s.y] = null;
        }
    }


}
