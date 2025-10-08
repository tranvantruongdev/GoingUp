using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopPanel : MonoBehaviour
{
    [Header("UI References")]
    public Transform listRoot;
    public ShopItemView itemPrefab;
    public Button okButton;
    public Text scoreText;
    [Header("Optional Preview")]
    public PlayerSkinApplier previewApplier; // if assigned, selection previews skin visuals

    readonly List<ShopItemView> _items = new List<ShopItemView>();
    SkinData _pendingSelection;

    void OnEnable()
    {
        BuildList();
        SelectEquippedByDefault();
        UpdateScoreText();

        if (okButton != null)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(OnOkClicked);
        }

        if (SkinService.Instance != null)
        {
            SkinService.Instance.EquippedChanged += OnEquippedChanged;
            SkinService.Instance.UnlockedChanged += OnUnlockedChanged;
            SkinService.Instance.HighScoreChanged += OnHighScoreChanged;
        }
    }

    void OnDisable()
    {
        if (SkinService.Instance != null)
        {
            SkinService.Instance.EquippedChanged -= OnEquippedChanged;
            SkinService.Instance.UnlockedChanged -= OnUnlockedChanged;
            SkinService.Instance.HighScoreChanged -= OnHighScoreChanged;
        }
    }

    void BuildList()
    {
        // clear existing
        for (int i = listRoot.childCount - 1; i >= 0; i--)
            Destroy(listRoot.GetChild(i).gameObject);
        _items.Clear();

        if (SkinService.Instance == null || SkinService.Instance.All == null)
            return;

        foreach (var skin in SkinService.Instance.All)
        {
            if (skin == null) continue;
            var item = Instantiate(itemPrefab, listRoot);
            item.Initialize(skin, this);
            _items.Add(item);
        }
        RefreshVisuals();
    }

    void SelectEquippedByDefault()
    {
        var equipped = SkinService.Instance != null ? SkinService.Instance.Equipped : null;
        if (equipped != null)
            SetSelected(equipped);
        else
        {
            // fallback: first unlocked
            if (SkinService.Instance != null)
            {
                foreach (var s in SkinService.Instance.All)
                {
                    if (SkinService.Instance.IsUnlocked(s))
                    {
                        SetSelected(s);
                        break;
                    }
                }
            }
        }
    }

    void UpdateScoreText()
    {
        if (scoreText != null && SkinService.Instance != null)
            scoreText.text = "High Score: " + SkinService.Instance.HighScore;
    }

    void OnOkClicked()
    {
        if (_pendingSelection == null || SkinService.Instance == null)
            return;
        if (SkinService.Instance.TryEquip(_pendingSelection))
            RefreshVisuals();
    }

    void OnEquippedChanged(SkinData skin)
    {
        RefreshVisuals();
    }

    void OnUnlockedChanged(HashSet<string> unlocked)
    {
        RefreshVisuals();
    }

    void OnHighScoreChanged(int value)
    {
        UpdateScoreText();
        RefreshVisuals();
    }

    public void SetSelected(SkinData skin)
    {
        _pendingSelection = skin;
        foreach (var item in _items)
            item.SetSelected(item.Skin == skin);
        UpdateOkButtonState();

        // Preview selected skin (no persistence) if unlocked and previewApplier is set
        if (previewApplier != null && SkinService.Instance != null && skin != null)
        {
            if (SkinService.Instance.IsUnlocked(skin))
                previewApplier.ApplySkin(skin);
        }
    }

    void UpdateOkButtonState()
    {
        if (okButton == null || SkinService.Instance == null)
            return;
        bool canEquip = _pendingSelection != null && SkinService.Instance.IsUnlocked(_pendingSelection) && SkinService.Instance.Equipped != _pendingSelection;
        okButton.interactable = canEquip;
    }

    void RefreshVisuals()
    {
        foreach (var item in _items)
            item.Refresh();
        UpdateOkButtonState();
    }
}


