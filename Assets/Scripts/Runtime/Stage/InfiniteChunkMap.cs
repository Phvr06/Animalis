using System.Collections.Generic;
using Animalis.Core;
using UnityEngine;

namespace Animalis.Stage
{
    public sealed class InfiniteChunkMap : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Transform chunkParent;
        [Min(4f)]
        [SerializeField] private float chunkSize = 12f;
        [Range(1, 3)]
        [SerializeField] private int visibleRadius = 2;
        [SerializeField] private Color evenChunkColor = new(0.30f, 0.47f, 0.24f, 1f);
        [SerializeField] private Color oddChunkColor = new(0.25f, 0.42f, 0.20f, 1f);

        private readonly Dictionary<Vector2Int, GameObject> _chunks = new();
        private Vector2Int _lastCenter = new(int.MinValue, int.MinValue);

        private void Start()
        {
            ResolveReferences();
            RefreshChunks(force: true);
        }

        private void Update()
        {
            ResolveTarget();
            RefreshChunks(force: false);
        }

        private void RefreshChunks(bool force)
        {
            if (target == null)
            {
                return;
            }

            Vector2Int center = WorldToChunk(target.position);
            if (!force && center == _lastCenter)
            {
                return;
            }

            _lastCenter = center;
            HashSet<Vector2Int> needed = new();

            for (int y = -visibleRadius; y <= visibleRadius; y++)
            {
                for (int x = -visibleRadius; x <= visibleRadius; x++)
                {
                    Vector2Int coordinate = new(center.x + x, center.y + y);
                    needed.Add(coordinate);

                    if (!_chunks.ContainsKey(coordinate))
                    {
                        _chunks.Add(coordinate, CreateChunk(coordinate));
                    }
                }
            }

            List<Vector2Int> stale = new();
            foreach (Vector2Int coordinate in _chunks.Keys)
            {
                if (!needed.Contains(coordinate))
                {
                    stale.Add(coordinate);
                }
            }

            foreach (Vector2Int coordinate in stale)
            {
                Destroy(_chunks[coordinate]);
                _chunks.Remove(coordinate);
            }
        }

        private GameObject CreateChunk(Vector2Int coordinate)
        {
            GameObject chunk = new($"FarmChunk_{coordinate.x}_{coordinate.y}");
            chunk.transform.SetParent(chunkParent != null ? chunkParent : transform, false);
            chunk.transform.position = new Vector3(coordinate.x * chunkSize, coordinate.y * chunkSize, 1f);
            chunk.transform.localScale = new Vector3(chunkSize, chunkSize, 1f);

            SpriteRenderer ground = chunk.AddComponent<SpriteRenderer>();
            ground.sprite = PlaceholderVisualFactory.GetSquareSprite();
            ground.color = (coordinate.x + coordinate.y) % 2 == 0 ? evenChunkColor : oddChunkColor;
            ground.sortingOrder = -20;

            CreateDecoration(chunk.transform, coordinate, new Vector2(-0.3f, 0.34f), new Color(0.42f, 0.28f, 0.12f, 1f), "FencePost");
            CreateDecoration(chunk.transform, coordinate, new Vector2(0.32f, -0.28f), new Color(0.45f, 0.45f, 0.38f, 1f), "Stone");
            return chunk;
        }

        private void CreateDecoration(Transform parent, Vector2Int coordinate, Vector2 normalizedOffset, Color color, string label)
        {
            int hash = Mathf.Abs(coordinate.x * 73856093 ^ coordinate.y * 19349663 ^ label.GetHashCode());
            if (hash % 3 == 0)
            {
                return;
            }

            GameObject decoration = new(label);
            decoration.transform.SetParent(parent, false);
            decoration.transform.localPosition = new Vector3(normalizedOffset.x, normalizedOffset.y, -0.02f);
            decoration.transform.localScale = new Vector3(0.06f, 0.18f, 1f);

            SpriteRenderer renderer = decoration.AddComponent<SpriteRenderer>();
            renderer.sprite = PlaceholderVisualFactory.GetSquareSprite();
            renderer.color = color;
            renderer.sortingOrder = -10;
        }

        private Vector2Int WorldToChunk(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt((position.x + chunkSize * 0.5f) / chunkSize),
                Mathf.FloorToInt((position.y + chunkSize * 0.5f) / chunkSize));
        }

        private void ResolveReferences()
        {
            if (chunkParent == null)
            {
                GameObject environment = GameObject.Find("Environment");
                chunkParent = environment != null ? environment.transform : transform;
            }

            ResolveTarget();
        }

        private void ResolveTarget()
        {
            if (target != null)
            {
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            target = player != null ? player.transform : null;
        }

        private void Reset()
        {
            ResolveReferences();
        }
    }
}
