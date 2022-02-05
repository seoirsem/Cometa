using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    GameObject playergo;
    Player player;
    Reference reference;
    // Start is called before the first frame update
    Vector2 worldSize = new Vector2(1300 / 90, 800 / 90);

    void Start()
    {
        Reference.CreateReferences();
        playergo = Reference.playergo;
        player = new Player(playergo);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
