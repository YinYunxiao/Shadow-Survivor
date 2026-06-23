using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : SingleBaseMono<MapGenerator>
{
    [SerializeField] private string grassSpritePath = "Sprites/Map/Grass/ISO_Tile_Dirt_01_Grass_01";
    [SerializeField] private float xOffset = 0;
    [SerializeField] private float yOffset = -0.4f;

    private const int ChunkSize = 10;
    private const int ViewDistance = 2;

    private Dictionary<Vector2Int, GameObject> activeChunks = new Dictionary<Vector2Int, GameObject>();
    private Queue<GameObject> chunkPool = new Queue<GameObject>();
    private Vector2Int lastPlayerChunk = new Vector2Int(int.MinValue, int.MinValue);
    private Sprite grassSprite;
    private float tileWidth;
    private float tileHeight;
    private bool isActive;

    public void StartMap()
    {
        grassSprite = Resources.Load<Sprite>(grassSpritePath);
        if (grassSprite == null)
        {
            Debug.LogError("MapGenerator: sprite not found");
            return;
        }

        tileWidth = grassSprite.bounds.size.x;
        tileHeight = grassSprite.bounds.size.y;
        isActive = true;
        lastPlayerChunk = new Vector2Int(int.MinValue, int.MinValue);
    }

    public void StopMap()
    {
        isActive = false;
        foreach (var chunk in activeChunks.Values)
        {
            chunk.SetActive(false);
            chunkPool.Enqueue(chunk);
        }
        activeChunks.Clear();
    }

    private void Update()
    {
        if (!isActive || grassSprite == null) return;

        Transform player = GameManager.Instance.PlayerTransform;
        if (player == null) return;

        Vector2Int playerChunk = WorldToChunk(player.position);

        if (playerChunk != lastPlayerChunk)
        {
            lastPlayerChunk = playerChunk;
            UpdateChunks(playerChunk);
        }
    }

    private void UpdateChunks(Vector2Int center)
    {
        HashSet<Vector2Int> needed = new HashSet<Vector2Int>();

        for (int dx = -ViewDistance; dx <= ViewDistance; dx++)
        {
            for (int dy = -ViewDistance; dy <= ViewDistance; dy++)
            {
                Vector2Int coord = new Vector2Int(center.x + dx, center.y + dy);
                needed.Add(coord);

                if (!activeChunks.ContainsKey(coord))
                {
                    var chunk = GetChunk();
                    FillChunk(chunk, coord);
                    activeChunks[coord] = chunk;
                }
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in activeChunks)
        {
            if (!needed.Contains(kvp.Key))
                toRemove.Add(kvp.Key);
        }

        foreach (var coord in toRemove)
        {
            var chunk = activeChunks[coord];
            chunk.SetActive(false);
            chunkPool.Enqueue(chunk);
            activeChunks.Remove(coord);
        }
    }

    private GameObject GetChunk()
    {
        if (chunkPool.Count > 0)
        {
            var chunk = chunkPool.Dequeue();
            chunk.SetActive(true);
            return chunk;
        }

        var go = new GameObject("GrassChunk");
        go.transform.SetParent(transform);
        return go;
    }

    private void FillChunk(GameObject chunk, Vector2Int coord)
    {
        for (int i = chunk.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(chunk.transform.GetChild(i).gameObject);
        }

        int startX = coord.x * ChunkSize;
        int startY = coord.y * ChunkSize;

        for (int gx = 0; gx < ChunkSize; gx++)
        {
            for (int gy = 0; gy < ChunkSize; gy++)
            {
                int isoX = startX + gx;
                int isoY = startY + gy;

                Vector3 worldPos = IsoToWorld(isoX, isoY);

                GameObject tile = new GameObject();
                tile.transform.SetParent(chunk.transform);
                tile.transform.position = worldPos;

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = grassSprite;
                sr.sortingOrder = -(isoX + isoY);
            }
        }
    }

    private Vector3 IsoToWorld(int gx, int gy)
    {
        float wx = (gx - gy) * tileWidth * 0.5f + (gx + gy) * xOffset;
        float wy = (gx + gy) * tileHeight * 0.5f + (gx + gy) * yOffset;
        return new Vector3(wx, wy, 0);
    }

    private Vector2Int WorldToChunk(Vector3 pos)
    {
        float halfW = tileWidth * 0.5f;
        float halfH = tileHeight * 0.5f;

        float s = pos.y / (halfH + yOffset);
        float d = (pos.x - s * xOffset) / halfW;

        int gx = Mathf.FloorToInt((s + d) * 0.5f);
        int gy = Mathf.FloorToInt((s - d) * 0.5f);

        return new Vector2Int(
            Mathf.FloorToInt((float)gx / ChunkSize),
            Mathf.FloorToInt((float)gy / ChunkSize)
        );
    }
}
