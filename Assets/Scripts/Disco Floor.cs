#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using UnityEngine;

[ExecuteAlways]
public class DiscoFloor : MonoBehaviour
{
    [Header("Disco Floor Parameters")]
    [SerializeField] private float tileSize = 1f;
    [SerializeField] private float tileDistance = 0.2f;
    [SerializeField] private Vector2Int numberOfTiles = Vector2Int.one * 5;

    [Header("Prefabs")]
    [SerializeField] private GameObject tilePrefab;

    private GameObject[,] _tiles;
    private AudioClip _song;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            // Defer generation until after validation is done
            EditorApplication.delayCall += () =>
            {
                if (this != null) // make sure object still exists
                    GenerateTiles();
            };
        }
#endif
    }

    private void GenerateTiles()
    {
        // Destroy old tiles
        if (_tiles != null)
        {
            foreach (var tile in _tiles)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    DestroyImmediate(tile);
                else
                    Destroy(tile);
#else
                Destroy(tile);
#endif
            }
        }

        if (tilePrefab == null) return;

        _tiles = new GameObject[numberOfTiles.x, numberOfTiles.y];

        float width = numberOfTiles.x * tileSize + (tileDistance * (numberOfTiles.x - 1));
        float height = numberOfTiles.y * tileSize + (tileDistance * (numberOfTiles.y - 1));

        int i = 0;
        for (int x = 0; x < numberOfTiles.x; x++)
        {
            for (int z = 0; z < numberOfTiles.y; z++)
            {
                float posX = -width / 2 + x * (tileSize + tileDistance) + tileSize / 2;
                float posZ = -height / 2 + z * (tileSize + tileDistance) + tileSize / 2;

                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.transform.localPosition = new Vector3(posX, 0, posZ);
                newTile.transform.localScale = new Vector3(tileSize, newTile.transform.localScale.y, tileSize);
                _tiles[x,z] = newTile;
            }
        }
    }

    private void Update()
    {
        #if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
        #endif
        
        _song = Camera.main.gameObject.GetComponent<AudioSource>().clip;
        if (_song == null) return;
    }
}