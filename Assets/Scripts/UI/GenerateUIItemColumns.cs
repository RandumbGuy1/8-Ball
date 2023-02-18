using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateUIItemColumns : MonoBehaviour
{
    [SerializeField] private List<RectTransform> buttons = new List<RectTransform>();
    [SerializeField] private RectTransform itemSlot;
    [SerializeField] private Button itemSlotButton;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private float buttonSpacing = 10;

    public void Generate(GameObject[] prefabs, SpectatorPlayer player)
    {
        if (prefabs.Length == 0) return;

        foreach (RectTransform button in buttons) Destroy(button.gameObject);
        buttons.Clear();

        GameObject prefabSelect1 = prefabs[0];
        itemText.text = prefabs[0].name;
        itemSlotButton.onClick.AddListener(() => player.SelectItem(prefabSelect1));

        Vector3 offset = Vector3.zero;
        for (int i = 1; i < prefabs.Length; i++)
        {
            RectTransform genSlot = Instantiate(itemSlot, itemSlot.position, itemSlot.rotation);
            offset += Vector3.right * (buttonSpacing + prefabs[i].name.Length * 20);

            genSlot.position += offset;
            genSlot.SetParent(transform);
            genSlot.localScale = Vector3.one;

            TextMeshProUGUI genText = genSlot.GetComponentInChildren<TextMeshProUGUI>();
            if (genText == null) continue;

            genText.text = prefabs[i].name;

            Button genButton = genSlot.GetComponent<Button>();
            if (genButton == null) continue;

            GameObject prefabSelect2 = prefabs[i];
            genButton.onClick.AddListener(() => player.SelectItem(prefabSelect2));

            buttons.Add(genSlot);
        }
    }
}
