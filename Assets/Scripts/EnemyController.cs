using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class EnemyController : MonoBehaviour, IKillable
{

    private Rigidbody2D _rigidbody;
    private AudioSource _audioSource;   

    [SerializeField] private float _movementSpeed;
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();        
    }

    private void FixedUpdate()
    {
        if (PlayerController.Instance != null)
        {
            _rigidbody.MovePosition(Vector3.MoveTowards(transform.position, PlayerController.Instance.transform.position, _movementSpeed * Time.deltaTime));
            float angle = Mathf.Atan2(PlayerController.Instance.transform.position.y - transform.position.y, PlayerController.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg;

            _rigidbody.rotation = angle;
        }
    }
    public void Kill()
    {
        Instantiate(GameController.Instance.DeathParticles, transform.position, Quaternion.identity);
        gameObject.SetActive(false);

    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            IKillable player = collision.gameObject.GetComponent<IKillable>();
            player.Kill();
            GameController.Instance.EndText.text = "Game Over! Your score: " + GameController.Instance.ScoreForKillEnemy.ToString(); 
            GameController.Instance.InfoText.text = "Press [SPACE] to restart";
            GameController.Instance.GameEnded = true;
        }
    }
}
