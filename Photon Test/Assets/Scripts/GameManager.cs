using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        // 생성할 랜덤 위치 지정
        Vector3 randomSpawnPos;
        Vector2 randomPos = Random.insideUnitCircle * 5f;

        randomSpawnPos.x = randomPos.x;
        randomSpawnPos.z = randomPos.y;
        // 위치 y값은 0으로 변경
        randomSpawnPos.y = 0f;

        // 네트워크 상의 모든 클라이언트들에서 생성 실행
        // 단, 해당 게임 오브젝트의 주도권은, 생성 메서드를 직접 실행한 클라이언트에게 있음
        PhotonNetwork.Instantiate(playerPrefab.name, randomSpawnPos, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
