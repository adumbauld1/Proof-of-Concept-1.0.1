using UnityEngine;

public class HexGridManager : MonoBehaviour
{
    public static HexGridManager Instance { get; private set; }

    [Header("Prefab & container")]
    public GameObject hexTilePrefab;    // the HexTile Prefab (RectTransform + Image + hexTileUI)
    public RectTransform tileContainer; // empty RectTransform, sibling ABOVE the RawImage
                                        // anchored/sized identically to the RawImage

    [Header("Grid size (match with map dimensions)")]
    public int columns = 10;
    public int rows = 10;

    [Header("Hex size in UI units(pixel size of one hex in the exported)")]
    [Header("image, scaled to matche RawImage's RectTransform if it isn't 1:1)")]
    public float hexWidth = 64f;
    public float hexHeight = 56f;
    public bool pointyTop = true; // check tiled map hex orientation

    private HexTileUI _selectedTile;
    private HexTileUI[,] _grid;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mapLoader.Load();
        GenerateGrid();
    }


    private void GenerateGrid()
    {
        _grid = new HexTileUI[columns, rows];

        for (int col = 0; col < columns; col++)
        {
            for (int row = 0; row < rows; row++)
            {
                Vector2 pos = pointyTop
                ? GetpointyTopPosition(col, row)
                : GetflatTopPosition(col, row);

                GameObject go = Instantiate(hexTilePrefab, tileContainer);
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchoredPosition = pos;
                rt.sizeDelta = new Vector2(hexWidth, hexHeight);

                HexTileUI tile = go.GetComponent<HexTileUI>();
                tile.col = col;
                tile.row = row;
                tile.tileType = mapLoader.GetTileType(col, row);
                tile.poiType = mapLoader.GetTileType("POI", col, row);
                tile.roadRiverType = mapLoader.GetTileType("Roads and Rivers", col, row);

                _grid[col, row] = tile;
            }
        }
    }

    // Odd-row offset, pointy-top layout
    private Vector2 GetpointyTopPosition(int col, int row)
    {
        float x = col * hexWidth + (row % 2 == 1 ? hexWidth * 0.5f : 0f);
        float y = row * hexHeight + (col % 2 == 1 ? hexHeight * 0.5f : 0f);
        return new Vector2(x, -y);  // UI y grows downward
    }

    //Odd Column offset, flat-top layout
    private Vector2 GetflatTopPosition(int col, int row)
    {
        float x = col * (hexWidth * 0.75f);
        float y = row * hexHeight + (col % 2 == 1 ? hexHeight * 0.5f : 0f);
        return new Vector2(x, -y);
    }

    public void SelectTile(HexTileUI tile)
    {
        if (_selectedTile != null)
            _selectedTile.SetSelected(false);

        _selectedTile = tile;
        tile.SetSelected(true);

        Debug.Log($"Tile has been selected: {tile}");
        // TODO: replace with actual game message/event system
    }

    public TiledMapLoader mapLoader;
}


