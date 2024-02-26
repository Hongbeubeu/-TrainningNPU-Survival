using Ultimate.Core.Runtime.EventManager;
using UnityEngine;

public class BoxItem : MonoBehaviour, IDamageable
{
    private TeamType _team;
    private float _timeAlive;

    private void OnEnable()
    {
        EventManager.Instance.AddListener<GameResetEvent>(OnGameReset);
    }

    private void OnDisable()
    {
        EventManager.Instance.RemoveListener<GameResetEvent>(OnGameReset);
    }

    private void OnGameReset(GameResetEvent e)
    {
        DestroyItem();
    }

    private void Update()
    {
        if (GameController.Instance.CurrentGameState == GameState.Pause)
            return;
        if (_timeAlive > GameManager.Instance.GameConfig.TimeBoxAlive)
        {
            DestroyItem();
        }

        _timeAlive += Time.deltaTime;
    }


    public void SetInfo()
    {
        _team = TeamType.None;
        _timeAlive = 0f;
        GameController.Instance.AddDamageable(this);
    }

    public void TakeDamage(IAttackable attacker)
    {
        Destroy();
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public TeamType GetTeamType()
    {
        return _team;
    }

    public void Destroy()
    {
        SpawnGiftItem();
        RemoveItem();
    }

    private void SpawnGiftItem()
    {
        var itemType = GameManager.Instance.BoxItemData.GetRandomItem();
        var item = GameManager.Instance.ObjectPooler.InstantiateCollectableItem();
        item.SetInfo(itemType);
        item.transform.position = transform.position;
    }

    private void RemoveItem()
    {
        GameController.Instance.RemoveDamageable(this);
        DestroyItem();
    }

    private void DestroyItem()
    {
        GameManager.Instance.ObjectPooler.DestroyBoxItem(gameObject);
    }
}