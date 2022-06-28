using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public bool dontDestory;

    [SerializeField]
    private GameObject waterPrefab;

    [SerializeField]
    private Vector3 scale;

    [SerializeField]
    private Vector2 neighborsDistance;

    [Space(10)]
    [SerializeField]
    private SpriteRenderer waterImage;

    [SerializeField]
    private Color[] waterColors;

    [Space(10)]    
    [SerializeField] 
    private float distanceToDestoryWater;

    [SerializeField] 
    private float spawnRadius;


    [SerializeField]
    private int objectCount;

    [SerializeField]
    private int bombsCount;

    [SerializeField]
    private int bombsDepthIncreaseDelta;

    [SerializeField]
    private WaterObject[] scanableObjectsPrefabs;

    [SerializeField]
    private WaterObject[] bombsObjectsPrefabs;

    private Bathyscaphe bathyscaphe;

    [SerializeField]
    private float distanceToBathyscaphe;


    private bool bathyscapheIn;
    private List<WaterObject> waterObjects = new List<WaterObject>();

    private void Awake()
    {
        transform.localScale = scale;
        waterImage.color = waterColors[UnityEngine.Random.Range(0, waterColors.Length)];

        StartCoroutine(DestroyFar());
    }

    private void OnEnable()
    {
        StartCoroutine(OnEnableAsync());
    }

    private IEnumerator OnEnableAsync()
    {
        yield return new WaitForEndOfFrame();
        Setup();
    }

    public void Setup()
    {
        DestoryWaterObjects();
        bathyscaphe = FindObjectOfType<Bathyscaphe>();

        SpawnWaterObjects(objectCount, spawnRadius, scanableObjectsPrefabs);

        float depthAddBombs = bathyscaphe.data.depth < 0 ? (-bathyscaphe.data.depth / bombsDepthIncreaseDelta) : 0;
        SpawnWaterObjects(bombsCount + (int)depthAddBombs, spawnRadius, bombsObjectsPrefabs);

        bathyscapheIn = false;
    }

    private void Update()
    {
        if (bathyscaphe == null)
            return;

        distanceToBathyscaphe = Vector3.Distance(bathyscaphe.transform.position, transform.position);

        if (LevelManager.Instance.playEnds)
            return;

        if (distanceToBathyscaphe < 10 && bathyscapheIn == false)
        {
            bathyscapheIn = true;
            SpawnNeighbors(new Vector2Int(0, 1), neighborsDistance * Vector2.down);         // down
            SpawnNeighbors(new Vector2Int(2, 1), neighborsDistance * new Vector2(-1, -1));  // left    
            SpawnNeighbors(new Vector2Int(2, 1), neighborsDistance * new Vector2(1, -1));   // right                    
        }
    }

    private void DestoryWaterObjects()
    {
        foreach (WaterObject item in waterObjects)
        {
            if (item != null)
                item.DestroyDestoryObject(0.01f, false);
        }
    }

    private IEnumerator DestroyFar()
    {
        while (true)
        {
            if (distanceToBathyscaphe > distanceToDestoryWater)
                DestoryWater();

            yield return new WaitForSecondsRealtime(0.2f);
        }
    }

    private void InstantiateWater(Vector3 pos)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, -Vector2.up);

        if (hit.collider != null)
            return;

        GameObject obj = Instantiate(waterPrefab, pos, Quaternion.identity, this.transform.parent);
        Water waterObj = obj.GetComponent<Water>();
        waterObj.dontDestory = false;
        obj.transform.SetAsFirstSibling();
    }

    private void SpawnNeighbors(Vector2Int count, Vector2 distance)
    {
        for (int i = 1; i < count.y + 1; i++)
        {
            if (count.x == 0)
            {
                Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + (distance.y * i), transform.position.z);
                InstantiateWater(newPosition);
            }

            for (int j = 1; j < count.x + 1; j++)
            {
                Vector3 newPosition = new Vector3(transform.position.x + (distance.x * j), transform.position.y + (distance.y * i), transform.position.z);
                InstantiateWater(newPosition);
            }
        }
    }

    private void SpawnWaterObjects(int count, float radius, WaterObject[] waterObjectsPrefabs)
    {
        int spawned = 0;
        int trySpawn = 0;

        while (spawned != count)
        {
            WaterObject objectsToInstantiate = waterObjectsPrefabs[UnityEngine.Random.Range(0, waterObjectsPrefabs.Length)];
            trySpawn++;

            if (trySpawn > count * 10)
                break;

            if (-bathyscaphe.data.depth < objectsToInstantiate.fromDepth)
                continue;

            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 newPosition = transform.position + new Vector3(randomPosition.x, randomPosition.y, 0);

            waterObjects.Add(Instantiate(objectsToInstantiate, newPosition, Quaternion.identity, transform.parent));
            spawned++;
        }
    }

    public void DestoryWater()
    {
        DestoryWaterObjects();

        if (dontDestory == false)
            Destroy(gameObject);
    }

}
