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
    private float neighborsDistance;

    [Space(10)]
    [SerializeField]
    private SpriteRenderer waterImage;

    [SerializeField]
    private Color[] waterColors;

    [Space(10)]
    [SerializeField]
    private float spawnRadius;

    [SerializeField]
    private int objectCount;

    [SerializeField]
    private int bombsCount;

    [SerializeField]
    private int bombsDepthIncreaseDelta;

    [SerializeField]
    private WaterObject[] waterObjectsPrefabs;

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
        SpawnWaterObjects(objectCount, spawnRadius);
        float depthAddBombs = bathyscaphe.data.depth < 0 ? (-bathyscaphe.data.depth / bombsDepthIncreaseDelta) : 0;
        SpawnBombs(bombsCount + (int)depthAddBombs, spawnRadius);
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
            SpawnNeighbors();
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

    public void DestoryWater()
    {
        DestoryWaterObjects();

        if (dontDestory == false)
            Destroy(gameObject);
    }

    private IEnumerator DestroyFar()
    {
        while (true)
        {
            if (distanceToBathyscaphe > 80.0f)
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

    private void SpawnNeighbors()
    {
        Vector3 newPositionDown = new Vector3(transform.position.x, transform.position.y - neighborsDistance, transform.position.z);
        InstantiateWater(newPositionDown);

        Vector3 newPositionLeftDown = new Vector3(transform.position.x - neighborsDistance, transform.position.y - neighborsDistance, transform.position.z);
        InstantiateWater(newPositionLeftDown);

        Vector3 newPositionLeftDown2 = new Vector3(transform.position.x - (neighborsDistance * 2), transform.position.y - neighborsDistance, transform.position.z);
        InstantiateWater(newPositionLeftDown2);

        Vector3 newPositionRigthDown = new Vector3(transform.position.x + neighborsDistance, transform.position.y - neighborsDistance, transform.position.z);
        InstantiateWater(newPositionRigthDown);

        Vector3 newPositionRigthDown2 = new Vector3(transform.position.x + (neighborsDistance * 2), transform.position.y - neighborsDistance, transform.position.z);
        InstantiateWater(newPositionRigthDown2);
    }

    private void SpawnWaterObjects(int count, float radius)
    {
        int spawned = 0;

        while (spawned != count)
        {

            WaterObject objectsToInstantiate = waterObjectsPrefabs[UnityEngine.Random.Range(0, waterObjectsPrefabs.Length)];

            if (-bathyscaphe.data.depth < objectsToInstantiate.fromDepth)
                continue;

            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 newPosition = transform.position + new Vector3(randomPosition.x, randomPosition.y, 0);

            waterObjects.Add(Instantiate(objectsToInstantiate, newPosition, Quaternion.identity, this.transform.parent));
            spawned++;
        }
    }

    private void SpawnBombs(int count, float radius)
    {
        int spawned = 0;
        int trySpawn = 0;

        while (spawned != count)
        {

            WaterObject objectsToInstantiate = bombsObjectsPrefabs[UnityEngine.Random.Range(0, bombsObjectsPrefabs.Length)];
            trySpawn++;

            if (trySpawn > count * 10)
                break;

            if (-bathyscaphe.data.depth < objectsToInstantiate.fromDepth)
                continue;

            Vector2 randomPosition = UnityEngine.Random.insideUnitCircle * radius;
            Vector3 newPosition = transform.position + new Vector3(randomPosition.x, randomPosition.y, 0);

            waterObjects.Add(Instantiate(objectsToInstantiate, newPosition, Quaternion.identity, this.transform.parent));
            spawned++;
        }
    }

}
