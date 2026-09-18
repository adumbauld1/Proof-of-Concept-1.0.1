using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Shows a panel with details about whichever hex tile was last selected.
// Info is built as a dynamic list of "InfoRow" entries rather than fixed fields,
// so adding a new stat later (yield, owner, building status, etc.) is one line
// in Show() below - no new UI elements to wire up by hand each time.
public class TileInfoPanel : MonoBehaviour
{
    public static TileInfoPanel Instance { get; private set; }

    [Header("Root & layout")]
    [Tooltip("The whole panel GameObject - toggled active/inactive to show/hide")]
    public GameObject panelRoot;
    [Tooltip("Parent with a Vertical Layout Group - InfoRow prefabs get spawned as children here")]
    public Transform infoRowContainer;
    public GameObject infoRowPrefab;

    [Header("Header")]
    public TMP_Text headerText;
    public string headerFormat = "Hex ({0}, {1})";

    [Header("Future: action buttons (buildable POIs, interactables, etc.)")]
    [Tooltip("Parent for future action buttons - not populated yet, just wired up for later")]
    public Transform actionButtonContainer;
    public GameObject actionButtonPrefab;

    [Header("Close button (optional)")]
    public Button closeButton;

    private void Awake()
    {
        Instance = this;
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        Hide();
    }

    public void Show(HexTileUI tile)
    {
        ClearRows();
        ClearActionButtons();

        headerText.text = string.Format(headerFormat, tile.col, tile.row);

        AddInfoRow("Terrain", tile.tileType.ToString());
        AddInfoRow("Difficulty", TerrainDifficulty.GetDisplayString(tile.tileType));

        if (tile.poiType.HasValue)
            AddInfoRow("POI", tile.poiType.Value.ToString());

        if (tile.roadRiverType.HasValue)
            AddInfoRow("Feature", tile.roadRiverType.Value.ToString());

        // TODO: as buildable/interactable POIs are added, call AddActionButton(...)
        // here for whichever actions are valid on this tile.

        panelRoot.SetActive(true);
    }

    public void Hide()
    {
        panelRoot.SetActive(false);
    }

    // Call this anywhere else new tile data needs to show up - e.g.
    // AddInfoRow("Owner", tile.owner.ToString());
    // AddInfoRow("Resource Yield", tile.resourceYield.ToString());
    public void AddInfoRow(string label, string value)
    {
        GameObject rowGO = Instantiate(infoRowPrefab, infoRowContainer);
        InfoRow row = rowGO.GetComponent<InfoRow>();
        row.SetData(label, value);
    }

    // Stubbed out for when buildable/interactable POIs are added -
    // e.g. AddActionButton("Build Outpost", () => BuildingManager.Instance.Build(tile));
    public void AddActionButton(string label, System.Action onClick)
    {
        GameObject btnGO = Instantiate(actionButtonPrefab, actionButtonContainer);
        Button button = btnGO.GetComponentInChildren<Button>();
        TMP_Text label_text = btnGO.GetComponentInChildren<TMP_Text>();
        label_text.text = label;
        button.onClick.AddListener(() => onClick());
    }

    private void ClearRows()
    {
        foreach (Transform child in infoRowContainer)
            Destroy(child.gameObject);
    }

    private void ClearActionButtons()
    {
        foreach (Transform child in actionButtonContainer)
            Destroy(child.gameObject);
    }
}