using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Goes on the HexTile Prefab. Sits as an invisible (or semi-transparent)
// clickable region positioned over one hex of the map image
[RequireComponent(typeof(Image))]
public class HexTileUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Grid position (offset coords, matches Tiled)")]
    public int col;
    public int row;

    [Header("Tile data")]
    public HexTileType tileType;          // base terrain, always set
    public HexTileType? poiType;          // village, city, checkpoint, etc. — may be empty
    public HexTileType? roadRiverType;    // road or river — may be empty

    [Header("Visual feedback")]
    public Color normalColor = new Color(1f, 1f, 1f, 0f);       // fully transparent
    public Color highlightColor = new Color(1f, 1f, 0f, 0.35f); // translucent yellow

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.color = normalColor;

        // Requires the sprite's texture to have "Read/Write Enabled" checked.
        // Makes clicks only register on opaque hex pixels, not the bounding box.
        _image.alphaHitTestMinimumThreshold = 0.1f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        HexGridManager.Instance.SelectTile(this);
    }

    public void SetSelected(bool selected)
    {
        _image.color = selected ? highlightColor : normalColor;
    }

    public override string ToString()
    {
        string msg = $"Hex({col},{row}) [{tileType}]";
        if (poiType.HasValue) msg += $", POI: {poiType.Value}";
        if (roadRiverType.HasValue) msg += $", Feature: {roadRiverType.Value}";
        return msg;
    }
}