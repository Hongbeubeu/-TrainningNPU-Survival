using UnityEngine;

public class DamageableComponent : MonoBehaviour, IDamageable
{
    private Character _character;

    public void SetInfo(Character character)
    {
        _character = character;
    }

    public void TakeDamage(IAttackable attacker)
    {
        var damage = attacker.GetDamage();

        if (attacker.GetAttacker()?.CurrentData.CriticalChance > 0)
        {
            var rand = Random.Range(0f, 1f);
            if (rand <= attacker.GetAttacker().CurrentData.CriticalChance)
            {
                damage += damage * attacker.GetAttacker().CurrentData.CriticalDamageMultiplier;
            }
        }

        if (_character.CurrentData.DodgerChance > 0)
        {
            var rand = Random.Range(0f, 1f);
            if (rand <= _character.CurrentData.DodgerChance)
            {
                damage = 0f;
            }
        }

        var hp = _character.CurrentHP - damage;
        var hitEffect = GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.Hit);
        hitEffect.SetInfo();
        hitEffect.transform.position = transform.position;
        var effectText =
            GameManager.Instance.ObjectPooler.InstantiateEffect(EffectType.DamgeEffect);
        effectText.transform.position = transform.position;
        var textDamageEffect = effectText as DamageEffect;
        textDamageEffect?.SetInfo();
        textDamageEffect?.SetDamage(damage);
        hp = Mathf.Max(0, hp);
        _character.SetHP(hp);
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public TeamType GetTeamType()
    {
        return _character.TeamType;
    }

    public void Destroy()
    {
        var e = _character as Enemy;
        if (e == null)
            return;
        e.DestroyEnemy();
    }
}