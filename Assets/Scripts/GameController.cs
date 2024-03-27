using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public bool GameEnded { get; set; }

    public GameObject DeathParticles;
    public GameObject ShootParticles;
    public CameraShake ShakeScript;
    public float SpawnRadius;
    public TMP_Text EndText;
    public TMP_Text ScoreText;
    public TMP_Text InfoText;

    internal int ScoreForKillEnemy = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        if (ObjectPooler.SharedInstance?.GetPooledObject("Reflector") != null)
        {
            GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Reflector");
            obj.transform.rotation = Quaternion.Euler(0, 0, GetRandomAngle());
            obj.transform.position = GetRandomPosition();
            obj.SetActive(true);
        }
        ScoreText.text = "Kills: " + ScoreForKillEnemy.ToString();
    }

    private void Update()
    {
        if (GameEnded)
        {
            Destroy(ScoreText);
        }
        ScoreText.text = "Kills: " + ScoreForKillEnemy.ToString();
        if (Random.value <= 0.01)
        {
            if (ObjectPooler.SharedInstance.GetPooledObject("Reflector") != null)
            {
                GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Reflector");
                obj.transform.rotation = Quaternion.Euler(0, 0, GetRandomAngle());


                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
            }
        }

        if (Random.value <= 0.01)
        {
            if (ObjectPooler.SharedInstance.GetPooledObject("Enemy") != null)
            {
                GameObject obj = ObjectPooler.SharedInstance.GetPooledObject("Enemy");
                obj.transform.position = GetRandomPosition();
                obj.SetActive(true);
            }
        }

        RestartGame();
    }

    private float GetRandomAngle()
    {
        float output = ((int)(Random.value * 360) / 45) * 45;
        return output;
    }

    private Vector3 GetRandomPosition()
    {

        Vector3 output = new Vector3(Random.value * 20 - 10, Random.value * 16 - 8, 0);

        while (true)
        {
            bool ValidPosition = true;
            output = new Vector3(Random.value * 20 - 10, Random.value * 16 - 8, 0);


            for (int i = 0; i < ObjectPooler.SharedInstance.GetTotalPoolSize(); i++)
            {
                if (!DistanceCheck(output, ObjectPooler.SharedInstance.GetPooledObject(i).transform.position))
                {
                    ValidPosition = false;
                }
            }

            if (ValidPosition)
            {
                break;
            }
        }
        return output;
    }

    private bool DistanceCheck(Vector3 currentPos, Vector3 targetPos)
    {
        if (Vector3.Distance(currentPos, targetPos) < SpawnRadius)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    private void RestartGame()
    {
        if (GameEnded && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }
}
