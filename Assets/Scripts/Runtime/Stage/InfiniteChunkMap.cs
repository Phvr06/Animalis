using System.Collections.Generic;
using Animalis.Core;
using UnityEngine;

namespace Animalis.Stage
{
    public sealed class InfiniteChunkMap : MonoBehaviour
    {
        private static readonly Color DefaultBackgroundColor = new(0.31f, 0.50f, 0.23f, 1f);

        [SerializeField, HideInInspector] private Transform target;
        [SerializeField] private Transform chunkParent;
        [Min(4f)]
        [SerializeField] private float chunkSize = 12f;
        [Range(1, 3)]
        [SerializeField] private int visibleRadius = 2;
        [Tooltip("Editable visual profile for the infinite map. Artists can change colors, sprites, density, and scale here.")]
        [SerializeField] private MapVisualDefinition visualDefinition;

        private readonly Dictionary<Vector2Int, GameObject> _chunks = new();
        private Vector2Int _lastCenter = new(int.MinValue, int.MinValue);
        private Sprite[] _resolvedDetailSprites;
        private Sprite[] _resolvedTreeSprites;
        private Sprite[] _resolvedRockSprites;

        public void SetTarget(Transform runtimeTarget)
        {
            target = runtimeTarget;
            ResolveVisualSprites();
            RefreshChunks(force: true);
        }

        private void Start()
        {
            if (chunkParent == null)
            {
                Debug.LogWarning("Infinite chunk map requires an explicit chunk parent reference.", this);
                enabled = false;
                return;
            }

            ResolveVisualSprites();
            RefreshChunks(force: true);
        }

        private void Update()
        {
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

            System.Random random = CreateRandom(coordinate, 1301);

            CreatePlainBackground(chunk.transform);
            CreateDetails(chunk.transform, coordinate, random);
            CreatePastureProps(chunk.transform, coordinate, random);
            return chunk;
        }

        private void CreatePlainBackground(Transform parent)
        {
            Sprite sprite = visualDefinition != null && visualDefinition.BackgroundSprite != null
                ? visualDefinition.BackgroundSprite
                : PlaceholderVisualFactory.GetSquareSprite();
            GameObject background = CreateSpriteObject(parent, "PlainGrassBackground", sprite, Vector3.zero, BackgroundSortingOrder);

            SpriteRenderer renderer = background.GetComponent<SpriteRenderer>();
            renderer.color = visualDefinition != null ? visualDefinition.BackgroundColor : DefaultBackgroundColor;
            FitSpriteToWorldWidth(background.transform, sprite, chunkSize);
        }

        private Vector2Int WorldToChunk(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt((position.x + chunkSize * 0.5f) / chunkSize),
                Mathf.FloorToInt((position.y + chunkSize * 0.5f) / chunkSize));
        }

        private void CreateDetails(Transform parent, Vector2Int coordinate, System.Random random)
        {
            if (_resolvedDetailSprites == null || _resolvedDetailSprites.Length == 0)
            {
                return;
            }

            int cellsPerSide = Mathf.CeilToInt(chunkSize / DetailCellSize);
            float halfChunk = chunkSize * 0.5f;
            float firstCellCenter = -halfChunk + DetailCellSize * 0.5f;

            for (int y = 0; y < cellsPerSide; y++)
            {
                for (int x = 0; x < cellsPerSide; x++)
                {
                    if (random.NextDouble() > DetailChancePerCell)
                    {
                        continue;
                    }

                    Sprite sprite = _resolvedDetailSprites[random.Next(_resolvedDetailSprites.Length)];
                    float maxOffset = DetailCellSize * DetailCellJitter;
                    float offsetX = RandomRange(random, -maxOffset, maxOffset);
                    float offsetY = RandomRange(random, -maxOffset, maxOffset);
                    Vector3 localPosition = new(
                        firstCellCenter + x * DetailCellSize + offsetX,
                        firstCellCenter + y * DetailCellSize + offsetY,
                        0f);

                    GameObject detail = CreateSpriteObject(parent, $"Detail_{coordinate.x}_{coordinate.y}_{x}_{y}", sprite, localPosition, DetailSortingOrder);
                    float scale = RandomRange(random, DetailScaleRange);
                    detail.transform.localScale = new Vector3(scale, scale, 1f);

                    SpriteRenderer renderer = detail.GetComponent<SpriteRenderer>();
                    renderer.flipX = random.NextDouble() < 0.5;
                }
            }
        }

        private void CreatePastureProps(Transform parent, Vector2Int coordinate, System.Random random)
        {
            TryCreateProp(parent, coordinate, random, _resolvedTreeSprites, TreeChancePerChunk, "Tree", TreeScaleRange);
            TryCreateProp(parent, coordinate, random, _resolvedRockSprites, RockChancePerChunk, "Rock", RockScaleRange);

            if (random.NextDouble() < RockChancePerChunk * ExtraRockChanceMultiplier)
            {
                TryCreateProp(parent, coordinate, random, _resolvedRockSprites, 1f, "Rock", RockScaleRange * 0.82f);
            }
        }

        private void TryCreateProp(
            Transform parent,
            Vector2Int coordinate,
            System.Random random,
            Sprite[] sprites,
            float chance,
            string label,
            Vector2 scaleRange)
        {
            if (sprites == null || sprites.Length == 0 || random.NextDouble() > chance)
            {
                return;
            }

            Vector3 localPosition = new(
                RandomRange(random, -chunkSize * PropPlacementRange, chunkSize * PropPlacementRange),
                RandomRange(random, -chunkSize * PropPlacementRange, chunkSize * PropPlacementRange),
                0f);
            Vector3 worldPosition = parent.TransformPoint(localPosition);

            if (worldPosition.sqrMagnitude < StartingAreaPropClearRadius * StartingAreaPropClearRadius)
            {
                return;
            }

            Sprite sprite = sprites[random.Next(sprites.Length)];
            GameObject prop = CreateSpriteObject(parent, $"{label}_{coordinate.x}_{coordinate.y}", sprite, localPosition, PropSortingOrder);
            float scale = RandomRange(random, scaleRange);
            prop.transform.localScale = new Vector3(scale, scale, 1f);

            SpriteRenderer renderer = prop.GetComponent<SpriteRenderer>();
            renderer.flipX = random.NextDouble() < 0.5;
        }

        private static GameObject CreateSpriteObject(Transform parent, string label, Sprite sprite, Vector3 localPosition, int sortingOrder)
        {
            GameObject spriteObject = new(label);
            spriteObject.transform.SetParent(parent, false);
            spriteObject.transform.localPosition = localPosition;

            SpriteRenderer renderer = spriteObject.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = Color.white;
            renderer.sortingOrder = sortingOrder;
            return spriteObject;
        }

        private static void FitSpriteToWorldWidth(Transform spriteTransform, Sprite sprite, float worldWidth)
        {
            if (sprite == null || sprite.bounds.size.x <= 0f)
            {
                return;
            }

            float scale = worldWidth / sprite.bounds.size.x;
            spriteTransform.localScale = new Vector3(scale, scale, 1f);
        }

        private void ResolveVisualSprites()
        {
            _resolvedDetailSprites = RemoveNullSprites(visualDefinition != null ? visualDefinition.DetailSprites : null);
            _resolvedTreeSprites = RemoveNullSprites(visualDefinition != null ? visualDefinition.TreeSprites : null);
            _resolvedRockSprites = RemoveNullSprites(visualDefinition != null ? visualDefinition.RockSprites : null);
        }

        private static Sprite[] RemoveNullSprites(Sprite[] sprites)
        {
            if (sprites == null || sprites.Length == 0)
            {
                return System.Array.Empty<Sprite>();
            }

            List<Sprite> usableSprites = new(sprites.Length);
            for (int i = 0; i < sprites.Length; i++)
            {
                if (sprites[i] != null)
                {
                    usableSprites.Add(sprites[i]);
                }
            }

            return usableSprites.ToArray();
        }

        private static System.Random CreateRandom(Vector2Int coordinate, int salt)
        {
            int seed = BuildSeed(coordinate, salt);
            return new System.Random(seed);
        }

        private static int BuildSeed(Vector2Int coordinate, int salt)
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + coordinate.x;
                hash = hash * 31 + coordinate.y;
                hash = hash * 31 + salt;
                return hash & 0x7fffffff;
            }
        }

        private static float RandomRange(System.Random random, float min, float max)
        {
            return min + (max - min) * (float)random.NextDouble();
        }

        private static float RandomRange(System.Random random, Vector2 range)
        {
            return RandomRange(random, range.x, range.y);
        }

        private float DetailCellSize => visualDefinition != null ? visualDefinition.DetailCellSize : 1f;
        private float DetailChancePerCell => visualDefinition != null ? visualDefinition.DetailChancePerCell : 0f;
        private Vector2 DetailScaleRange => visualDefinition != null ? visualDefinition.DetailScaleRange : Vector2.one;
        private float DetailCellJitter => visualDefinition != null ? visualDefinition.DetailCellJitter : 0f;
        private float TreeChancePerChunk => visualDefinition != null ? visualDefinition.TreeChancePerChunk : 0f;
        private float RockChancePerChunk => visualDefinition != null ? visualDefinition.RockChancePerChunk : 0f;
        private float ExtraRockChanceMultiplier => visualDefinition != null ? visualDefinition.ExtraRockChanceMultiplier : 0f;
        private float PropPlacementRange => visualDefinition != null ? visualDefinition.PropPlacementRange : 0.42f;
        private Vector2 TreeScaleRange => visualDefinition != null ? visualDefinition.TreeScaleRange : Vector2.one;
        private Vector2 RockScaleRange => visualDefinition != null ? visualDefinition.RockScaleRange : Vector2.one;
        private float StartingAreaPropClearRadius => visualDefinition != null ? visualDefinition.StartingAreaPropClearRadius : 0f;
        private int BackgroundSortingOrder => visualDefinition != null ? visualDefinition.BackgroundSortingOrder : -30;
        private int DetailSortingOrder => visualDefinition != null ? visualDefinition.DetailSortingOrder : -25;
        private int PropSortingOrder => visualDefinition != null ? visualDefinition.PropSortingOrder : -6;
    }
}
