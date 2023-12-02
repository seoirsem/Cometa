using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexComparer : IEqualityComparer<Vector2>{
    
    float threshold = 0.0001f;

    public bool Equals(Vector2 vec1, Vector2 vec2) {
        return Mathf.Abs(vec1.x - vec2.x) < threshold && Mathf.Abs(vec1.y - vec2.y) < threshold;
    }

    public int GetHashCode (Vector2 vec){
        return Mathf.FloorToInt (vec.x) ^ Mathf.FloorToInt (vec.y) << 2 ^ Mathf.FloorToInt (vec.y) >> 2;
    }
}
