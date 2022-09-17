using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        var playerPrefabRequest = Resources.LoadAsync<GameObject>("Prefabs/Character/ModularCharacterPBR_Male");
        yield return playerPrefabRequest;
        if (playerPrefabRequest.asset != null)
        {
            var playerObj = GameObject.Instantiate((GameObject)playerPrefabRequest.asset, Vector3.zero, Quaternion.identity);
            DontDestroyOnLoad(playerObj);
        }

        if (Camera.main != null)
        {
            DontDestroyOnLoad(Camera.main);
        }

        SceneChanger.Instance.ChangeScene("SampleScene");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
