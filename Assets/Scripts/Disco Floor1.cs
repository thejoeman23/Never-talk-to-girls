using UnityEngine;

public class DiscoFloor : MonoBehaviour
{
    [Header("Disco Floor Parameters")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float tileDistance = 0.2f;
    [SerializeField] private Vector2Int numberOfTiles = Vector2Int.one * 5;

    [Header("Prefabs")]
    [SerializeField] private GameObject tilePrefab;

    [Header("Shader Settings")]
    [SerializeField] private Shader tileShader;               // Shader picked in inspector
    [SerializeField] private float colorChangeInterval = 1f;  // Time in seconds between color changes

    private GameObject[,] _tiles;
    private Material[,] _materials;
    private float _timer;

    private void Start()
    {
        GenerateTiles(); // Only generate when game starts
    }

    private void GenerateTiles()
    {
        // Destroy old tiles if any
        if (_tiles != null)
        {
            foreach (var tile in _tiles)
            {
                if (tile != null) Destroy(tile);
            }
        }

        if (tilePrefab == null) return;

        _tiles = new GameObject[numberOfTiles.x, numberOfTiles.y];
        _materials = new Material[numberOfTiles.x, numberOfTiles.y];

        float width = numberOfTiles.x * tileSize + (tileDistance * (numberOfTiles.x - 1));
        float height = numberOfTiles.y * tileSize + (tileDistance * (numberOfTiles.y - 1));

        for (int x = 0; x < numberOfTiles.x; x++)
        {
            for (int z = 0; z < numberOfTiles.y; z++)
            {
                float posX = -width / 2 + x * (tileSize + tileDistance) + tileSize / 2;
                float posZ = -height / 2 + z * (tileSize + tileDistance) + tileSize / 2;

                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.transform.localPosition = new Vector3(posX, 0, posZ);
                newTile.transform.localScale = new Vector3(tileSize, newTile.transform.localScale.y, tileSize);

                // Apply duplicated material
                Renderer renderer = newTile.GetComponent<Renderer>();
                if (renderer != null && tileShader != null)
                {
                    Material mat = new Material(tileShader);
                    renderer.material = mat; // use .material instead of .sharedMaterial so we don’t overwrite prefab
                    _materials[x, z] = mat;
                }

                _tiles[x, z] = newTile;
            }
        }
        
        // Light up floor so that its lit instantly before the player loads in
        RandomizeColors();
    }

    private void Update()
    {
        if (_materials == null) return;
        
        _timer += Time.deltaTime;
        if (_timer >= colorChangeInterval)
        {
            _timer = 0f;
            RandomizeColors();
        }
    }

    private void RandomizeColors()
    {
        for (int x = 0; x < numberOfTiles.x; x++)
        {
            for (int z = 0; z < numberOfTiles.y; z++)
            {
                if (_materials[x, z] != null)
                {
                    _materials[x, z].color = Random.ColorHSV(0f, 1f, 0.7f, 1f, 0.8f, 1f);
                }
            }
        }
    }
}