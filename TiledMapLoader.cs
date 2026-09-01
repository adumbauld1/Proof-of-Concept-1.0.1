using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TiledProperty
{
    public string name;
    public string type;
    public string value;
}

[System.Serializable]
public class TiledTileDef
{
    public int id;
    public TiledProperty[] properties;
}

[System.Serializable]
public class TiledTileset
{
    public int firstgid;
    public string source;        // present on external tilesets (no "tiles" data)
    public string name;
    public int tilecount;
    public int columns;
    public TiledTileDef[] tiles; // only populated for embedded tilesets with properties
}

[System.Serializable]
public class TiledLayer
{
    public int[] data;
    public int height;
    public int id;
    public string name;
    public string type;
    public bool visible;
    public int width;
}

[System.Serializable]
public class TiledMap
{
    public int width;
    public int height;
    public string orientation;
    public string staggeraxis;
    public string staggerindex;
    public TiledLayer[] layers;
    public TiledTileset[] tilesets;
}

// Loads a Tiled JSON export and resolves each cell's gid to a HexTileType,
// per layer, using each tile's Tiled "property name" (e.g. "Grass", "Village") as the type.
public class TiledMapLoader : MonoBehaviour
{
    [Tooltip("Drag your exported .json map file in here")]
    public TextAsset mapJson;

    [Tooltip("The base terrain layer (always assumed to have a value; falls back to Grass)")]
    public string terrainLayerName = "Terrain";

    public int MapWidth { get; private set; }
    public int MapHeight { get; private set; }

    // layerName -> [col, row] -> tile type, or null if that layer has nothing on that cell
    private Dictionary<string, HexTileType?[,]> _layerGrids;
    private Dictionary<int, HexTileType> _gidToType;

    public void Load()
    {
        TiledMap map = JsonUtility.FromJson<TiledMap>(mapJson.text);
        MapWidth = map.width;
        MapHeight = map.height;

        BuildGidLookup(map);

        _layerGrids = new Dictionary<string, HexTileType?[,]>();

        foreach (TiledLayer layer in map.layers)
        {
            if (layer.data == null) continue; // skip non-tile layers (object layers, etc.)

            var grid = new HexTileType?[MapWidth, MapHeight];
            for (int row = 0; row < MapHeight; row++)
            {
                for (int col = 0; col < MapWidth; col++)
                {
                    int gid = layer.data[row * MapWidth + col];
                    grid[col, row] = ResolveType(gid); // null if gid is 0 or unmapped
                }
            }
            _layerGrids[layer.name] = grid;
        }
    }

    private void BuildGidLookup(TiledMap map)
    {
        _gidToType = new Dictionary<int, HexTileType>();

        foreach (TiledTileset ts in map.tilesets)
        {
            if (ts.tiles == null) continue; // external tileset, no embedded property data

            foreach (TiledTileDef tile in ts.tiles)
            {
                if (tile.properties == null || tile.properties.Length == 0) continue;

                string typeName = tile.properties[0].name; // e.g. "Grass", "Village", "Road"
                if (System.Enum.TryParse(typeName, true, out HexTileType parsed))
                {
                    int gid = ts.firstgid + tile.id;
                    _gidToType[gid] = parsed;
                }
                else
                {
                    Debug.LogWarning($"Tiled property '{typeName}' has no matching HexTileType entry.");
                }
            }
        }
    }

    private HexTileType? ResolveType(int gid)
    {
        if (gid == 0) return null; // empty cell on this layer
        return _gidToType.TryGetValue(gid, out HexTileType type) ? type : (HexTileType?)null;
    }

    // Generic lookup for any layer by name. Returns null if that layer has nothing on this cell.
    public HexTileType? GetTileType(string layerName, int col, int row)
    {
        if (_layerGrids != null && _layerGrids.TryGetValue(layerName, out var grid))
            return grid[col, row];
        return null;
    }

    // Convenience overload for the terrain layer specifically — always returns a value,
    // falling back to Grass for empty cells.
    public HexTileType GetTileType(int col, int row)
    {
        return GetTileType(terrainLayerName, col, row) ?? HexTileType.Grass;
    }
}