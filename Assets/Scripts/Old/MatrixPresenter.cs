using UnityEngine;
using UnityEngine.UI;

public class MatrixPresenter : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTrnansform;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("========================================");
        Debug.Log($"Rect: {_rectTrnansform.rect}");
        Debug.Log($"Size delta: {_rectTrnansform.sizeDelta}");
        Debug.Log("========================================");
        Debug.Log($"Anchor Min: {_rectTrnansform.anchorMin}");
        Debug.Log($"Anchor Max: {_rectTrnansform.anchorMax}");
        Debug.Log("========================================");
        Debug.Log($"Anchor Position: {_rectTrnansform.anchoredPosition}");
        Debug.Log("========================================");
        Debug.Log($"Offset Min: {_rectTrnansform.offsetMin}");
        Debug.Log($"Offset Max: {_rectTrnansform.offsetMax}");
        Debug.Log("========================================");
        
        var go = new GameObject();
        go.name = "Yellow";
        
        var rectTransform = go.AddComponent<RectTransform>();
        rectTransform.SetParent(this._rectTrnansform);
        rectTransform.localScale = Vector3.one;
        rectTransform.anchoredPosition3D = Vector3.zero;
        rectTransform.sizeDelta = new Vector2(0, 0);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        
        var image = go.AddComponent<Image>();
        image.color = new Color(1, 1, 1, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
