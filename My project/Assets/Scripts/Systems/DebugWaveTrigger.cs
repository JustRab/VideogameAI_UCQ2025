using UnityEngine;

public class DebugWaveTrigger : MonoBehaviour
{
    private EnemyGenerator generator;

    void Start()
    {
        generator = GetComponent<EnemyGenerator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //generator.GenerateWave();
        }
    }
}
