using System;
using Npu;
using TMPro;
using Ultimate.Core.Runtime.EventManager;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerUpgradePanel : BaseUI, IDataProvider
{
    // [SerializeField] private TextMeshProUGUI _goldText;
    [SerializeField] private UpgradeItem[] _upgradeItems;

    public int Gold => GameManager.Instance.PlayerData?.Gold ?? 0;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<GoldChangeEvent>(OnGoldChanged);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GoldChangeEvent>(OnGoldChanged);
    }

    public override void Show()
    {
        base.Show();
        SetInfo();
    }

    public void SetInfo()
    {
        TriggerChangedAll();
        // _goldText.SetText(GameManager.Instance.PlayerData.Gold.ToString());
        for (var i = 0; i < _upgradeItems.Length; i++)
        {
            _upgradeItems[i]?.Init();
        }
    }

    void OnGoldChanged(GoldChangeEvent e)
    {
        TriggerChangedDynamic();
    }

    #region IDataProvider

    public event System.Action<IDataProvider, int> DataChanged;
    public bool Ready => true;
    public System.Type GetDataType() => GetType();
    public object GetData() => this;
    private static string[] _bindingFilters = { "default", "dynamic" };
    public string[] BindingFilters => _bindingFilters;

    protected void TriggerChanged(int mask) => DataChanged?.Invoke(this, mask);
    protected void TriggerChanged(string mask) => DataChanged?.Invoke(this, this.GetFilters(mask));

    [ContextMenu("Trigger Change All")]
    private void TriggerChangedAll() => TriggerChanged(~0);

    void TriggerChangedDynamic() => TriggerChanged(1 << 1);

    #endregion
}