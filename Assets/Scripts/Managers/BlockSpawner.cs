using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour {

    public static BlockSpawner instance;

    public GameObject spawnLocation;
    public GameObject blockPrefab;

    void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    void Start () {
        if (spawnLocation == null) {
            spawnLocation = gameObject;
        }
        if (GameManager.activeBlock == null) {
            GameObject instantiatedBlock = Instantiate(blockPrefab);
            instantiatedBlock.transform.position = spawnLocation.transform.position;
            GameManager.UpdateActiveBlock(instantiatedBlock);
        }
    }

    void Update () {
        if (GameManager.activeBlockComponent.team != null) {
            GameObject instantiatedBlock = Instantiate(blockPrefab);
            instantiatedBlock.transform.position = spawnLocation.transform.position;
            GameManager.UpdateActiveBlock(instantiatedBlock);
        }
    }
}
