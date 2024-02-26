using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    private float _coolDown;

    public void SetInfo()
    {
        _coolDown = GameManager.Instance.GameConfig.TimeBoxSpawn;
    }

    private void Update()
    {
        // if (GameController.Instance.CurrentGameState == GameState.Pause)
        //     return;
        // if (_coolDown <= 0f)
        // {
        //     var boxItem = GameManager.Instance.ObjectPooler.InstantiateBoxItem();
        //     boxItem.SetInfo();
        //     var pos = GameController.Instance.Player.transform.position;
        //     pos.x += Random.Range(-30f, 30f);
        //     pos.y += Random.Range(-30f, 30f);
        //     boxItem.transform.position = pos;
        //     _coolDown = GameManager.Instance.GameConfig.TimeBoxSpawn;
        // }
        //
        // _coolDown -= Time.deltaTime;
    }
}