using TMPro;
using UnityEngine;

// A single "Label: Value" row inside the tile info panel.
// Keep this prefab dead simple (just two TMP_children) so new rows
// can be added at runtime without touching the panel's layout by hand.
public class InfoRow : MonoBehaviour
{
    public TMP_Text labelText;
    public TMP_Text valueText;

    public void SetData(string label, string value)
    {
        labelText.text = label;
        valueText.text = value;
    }
}
