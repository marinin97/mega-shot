using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour, IKillable, IDamageable
{
    public int BaseHealth;
    public List<Sprite> Sprites;

    private int _health;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _health = BaseHealth;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        _health = BaseHealth;
    }

    private void Update()
    {
        if (_health <= 0)
        {
            Kill();

        }
    }

    public void Kill()
    {
        this.gameObject.SetActive(false);
        _health = BaseHealth;
        _spriteRenderer.sprite = Sprites[_health];
    }

    public void Damage(int damage)
    {
        _health -= damage;
        if (_health > 0)
        {
            _spriteRenderer.sprite = Sprites[_health];
        }

    }
}
