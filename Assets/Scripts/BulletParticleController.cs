using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticleController : MonoBehaviour
{
    public float Speed;

    private List<Vector3> _points;
    private ParticleSystem _particleSystem;
    private float _lifeTime;
    private float _startTime;
    
    void Start()
    {
        _particleSystem = this.gameObject.GetComponent<ParticleSystem>();
        _lifeTime = _particleSystem.main.startLifetime.Evaluate(0);

    }

    private void OnEnable()
    {
        _startTime = Time.time;
    }
    private void Update()
    {
        if (_points.Count > 0)
        {
            var emission = _particleSystem.emission;
            emission.enabled = true;
            transform.position = Vector3.MoveTowards(transform.position, _points[0], 500f * Time.fixedDeltaTime);

            if (Vector3.Distance(transform.position, _points[0]) < 1)
            {
                _points.RemoveAt(0);
            }
        }
        else
        {
            var emission = _particleSystem.emission;
            emission.enabled = false;
        }


        if (Time.time - _startTime > _lifeTime)
        {
            Destroy(this.gameObject);
        }


    }
    public void FollowPoints(List<Vector3> points)
    {
        _points = points;
        transform.position = points[0];
    }
}
