using System.Collections;
using Ultimate.Core.Runtime.EventManager;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField] protected EffectType _type;
    [SerializeField] protected ParticleSystem _particle;
    [SerializeField] private bool _isSelfDestroy;
    protected float _lifeTime;
    private bool _isCalculate;
    [SerializeField] private float _maxLifeTime;

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
        Destroy();
    }

    public virtual void SetInfo()
    {
        _lifeTime = 0f;
        if (_particle)
        {
            _particle.Play();
        }

        _isCalculate = true;
    }

    public void SetInfo(float maxLifeTime, float radius)
    {
        SetInfo();
        _maxLifeTime = maxLifeTime;
        _isSelfDestroy = true;
        SetSize(radius);
    }

    public virtual void SetSize(float radius)
    {
        var sizeOverLifeTime = _particle.sizeOverLifetime;
        if (!sizeOverLifeTime.enabled)
            return;
        sizeOverLifeTime.sizeMultiplier = radius;
    }

    private void Update()
    {
        if (!_isCalculate) return;
        if (!_isSelfDestroy) return;
        if (_lifeTime > _maxLifeTime)
        {
            _isCalculate = false;
            if (_particle)
            {
                DestroyParticle();
                return;
            }

            Destroy();
            return;
        }

        _lifeTime += Time.deltaTime;
    }

    private void DestroyParticle()
    {
        _isCalculate = false;
        _particle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        StartCoroutine(DestroyEffectCoroutine());
    }

    private IEnumerator DestroyEffectCoroutine()
    {
        yield return new WaitForSeconds(_particle.main.duration);
        Destroy();
    }

    public void Destroy()
    {
        GameManager.Instance.ObjectPooler.DestroyEffect(_type, gameObject);
    }
}