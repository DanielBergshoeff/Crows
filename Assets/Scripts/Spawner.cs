using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> ObjectPrefabs;
    public List<GameObject> EnemyPrefabs;
    public GameObject GlimmerPrefab;

    public int AmountOfObjects;
    public int AmountOfEnemies;

    private List<GameObject> ObjectPositions;
    private List<GameObject> EnemyPositions;

    // Start is called before the first frame update
    void Start()
    {
        ObjectPositions = new List<GameObject>(GameObject.FindGameObjectsWithTag("ObjectSpawn"));
        EnemyPositions = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemySpawn"));

        //SpawnFromPositions(ObjectPrefabs, ObjectPositions, AmountOfObjects);
        SpawnGlimmers();
        SpawnFromPositions(EnemyPrefabs, EnemyPositions, AmountOfEnemies);
    }

    private void SpawnFromPositions(List<GameObject> prefabs, List<GameObject> positions, int amount) {
        if (positions.Count < amount)
            return;

        for (int i = 0; i < amount; i++) {
            GameObject pos = positions[Random.Range(0, positions.Count)];
            GameObject go = Instantiate(prefabs[Random.Range(0, prefabs.Count)]);
            go.transform.position = pos.transform.position;
            positions.Remove(pos);
        }
    }

    private void SpawnGlimmers() {
        if (ObjectPositions.Count < AmountOfObjects)
            return;

        for (int i = 0; i < AmountOfObjects; i++) {
            GameObject pos = ObjectPositions[Random.Range(0, ObjectPositions.Count)];
            GameObject go = Instantiate(GlimmerPrefab);
            go.transform.position = pos.transform.position;
            go.GetComponent<Glimmer>().MyShinyObjectPrefab = ObjectPrefabs[Random.Range(0, ObjectPrefabs.Count)];
            ObjectPositions.Remove(pos);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
