using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GenerateUIItemColumns : MonoBehaviour
{
    [SerializeField] private List<RectTransform> buttons = new List<RectTransform>();
    [SerializeField] private RectTransform itemSlot;
    [SerializeField] private TextMeshProUGUI itemText;
    [SerializeField] private float buttonSpacing = 10;

    public void Generate(GameObject[] prefabs)
    {
        if (prefabs.Length == 0) return;

        foreach (RectTransform button in buttons) Destroy(button.gameObject);
        buttons.Clear();

        itemText.text = prefabs[0].name;

        Vector3 offset = Vector3.zero;
        for (int i = 1; i < prefabs.Length; i++)
        {
            RectTransform genSlot = Instantiate(itemSlot, itemSlot.position, itemSlot.rotation);
            offset += Vector3.right * buttonSpacing;

            genSlot.position += offset;
            genSlot.SetParent(transform);
            genSlot.localScale = Vector3.one;

            TextMeshProUGUI genText = genSlot.GetComponentInChildren<TextMeshProUGUI>();

            if (genText == null) continue;
            genText.text = prefabs[i].name;

            buttons.Add(genSlot);
        }
    }
}
