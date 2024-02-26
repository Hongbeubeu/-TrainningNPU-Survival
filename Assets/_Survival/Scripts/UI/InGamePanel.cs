using TMPro;
using Ultimate.Core.Runtime.EventManager;
using UnityEngine;

public class InGamePanel : BaseUI
{
	[SerializeField] private TextMeshProUGUI _goldText;
	public EndGamePanel EndGamePanel;
	public SelectWeaponSkillPanel SelectWeaponSkillPanel;
	public BossHPBarPanel BossHpBarPanel;

	public override void Start()
	{
		base.Start();
		_goldText.SetText($"{GameManager.Instance.PlayerData.Gold}");
	}

	private void OnEnable()
	{
		EventManager.Instance.AddListener<GoldChangeEvent>(OnGoldChange);
	}

	private void OnDisable()
	{
		EventManager.Instance.RemoveListener<GoldChangeEvent>(OnGoldChange);
	}

	private void OnGoldChange(GoldChangeEvent e)
	{
		_goldText.SetText($"{e.CurrentGold}");
	}
}