using UnityEngine;

namespace Animalis.Stage
{
    [CreateAssetMenu(menuName = "Animalis/Stage/Map Visual Definition", fileName = "NewMapVisualDefinition")]
    public sealed class MapVisualDefinition : ScriptableObject
    {
        [Header("Plain Background")]
        [Tooltip("Optional single sprite for the flat background. Leave empty to use a generated square.")]
        [SerializeField] private Sprite backgroundSprite;
        [SerializeField] private Color backgroundColor = new(0.31f, 0.50f, 0.23f, 1f);

        [Header("Detail Sprites")]
        [SerializeField] private Sprite[] detailSprites;
        [SerializeField] private Sprite[] treeSprites;
        [SerializeField] private Sprite[] rockSprites;

        [Header("Detail Placement")]
        [Min(0.5f)]
        [SerializeField] private float detailCellSize = 1f;
        [Range(0f, 0.4f)]
        [SerializeField] private float detailChancePerCell = 0.075f;
        [Tooltip("Random scale range for each small detail sprite.")]
        [SerializeField] private Vector2 detailScaleRange = new(2.55f, 3.54f);
        [Range(0f, 0.5f)]
        [SerializeField] private float detailCellJitter = 0.28f;

        [Header("Prop Placement")]
        [Range(0f, 1f)]
        [SerializeField] private float treeChancePerChunk = 0.12f;
        [Range(0f, 1f)]
        [SerializeField] private float rockChancePerChunk = 0.28f;
        [Range(0f, 1f)]
        [SerializeField] private float extraRockChanceMultiplier = 0.35f;
        [Range(0.1f, 0.5f)]
        [SerializeField] private float propPlacementRange = 0.42f;
        [Tooltip("Random scale range for each tree sprite.")]
        [SerializeField] private Vector2 treeScaleRange = new(4.62f, 5.88f);
        [Tooltip("Random scale range for each rock sprite.")]
        [SerializeField] private Vector2 rockScaleRange = new(2.42f, 3.08f);
        [Min(0f)]
        [SerializeField] private float startingAreaPropClearRadius = 5f;

        [Header("Sorting")]
        [SerializeField] private int backgroundSortingOrder = -30;
        [SerializeField] private int detailSortingOrder = -25;
        [SerializeField] private int propSortingOrder = -6;

        public Sprite BackgroundSprite => backgroundSprite;
        public Color BackgroundColor => backgroundColor;
        public Sprite[] DetailSprites => detailSprites;
        public Sprite[] TreeSprites => treeSprites;
        public Sprite[] RockSprites => rockSprites;
        public float DetailCellSize => detailCellSize;
        public float DetailChancePerCell => detailChancePerCell;
        public Vector2 DetailScaleRange => detailScaleRange;
        public float DetailCellJitter => detailCellJitter;
        public float TreeChancePerChunk => treeChancePerChunk;
        public float RockChancePerChunk => rockChancePerChunk;
        public float ExtraRockChanceMultiplier => extraRockChanceMultiplier;
        public float PropPlacementRange => propPlacementRange;
        public Vector2 TreeScaleRange => treeScaleRange;
        public Vector2 RockScaleRange => rockScaleRange;
        public float StartingAreaPropClearRadius => startingAreaPropClearRadius;
        public int BackgroundSortingOrder => backgroundSortingOrder;
        public int DetailSortingOrder => detailSortingOrder;
        public int PropSortingOrder => propSortingOrder;

        private void OnValidate()
        {
            detailCellSize = Mathf.Max(0.5f, detailCellSize);
            detailScaleRange = ValidateRange(detailScaleRange, 0.1f);
            treeScaleRange = ValidateRange(treeScaleRange, 0.1f);
            rockScaleRange = ValidateRange(rockScaleRange, 0.1f);
            startingAreaPropClearRadius = Mathf.Max(0f, startingAreaPropClearRadius);
        }

        private static Vector2 ValidateRange(Vector2 range, float minimum)
        {
            range.x = Mathf.Max(minimum, range.x);
            range.y = Mathf.Max(range.x, range.y);
            return range;
        }
    }
}
