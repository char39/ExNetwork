using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

// 좀비 게임 오브젝트를 주기적으로 생성
public class ZombieSpawner : MonoBehaviourPun, IPunObservable
{
    public Zombie zombiePrefab; // 생성할 좀비 원본 프리팹

    public ZombieData[] zombieDatas; // 사용할 좀비 셋업 데이터들
    public Transform[] spawnPoints; // 좀비 AI를 소환할 위치들

    private List<Zombie> zombies = new List<Zombie>(); // 생성된 좀비들을 담는 리스트
    private int wave; // 현재 웨이브
    private int enemyCount;

    void Awake()
    {
        PhotonPeer.RegisterType(typeof(Color), 128, ColorSerialization.SerializeColor, ColorSerialization.DeserializeColor);
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameManager.instance != null && GameManager.instance.isGameover) return;
            if (zombies.Count <= 0)
                SpawnWave();
            UpdateUI();
        }
    }

    // 웨이브 정보를 UI로 표시
    private void UpdateUI() {
        if (PhotonNetwork.IsMasterClient)
            UIManager.instance.UpdateWaveText(wave, zombies.Count);
        else
            UIManager.instance.UpdateWaveText(wave, enemyCount);
    }

    // 현재 웨이브에 맞춰 좀비들을 생성
    private void SpawnWave() {
        // 웨이브 1 증가
        wave++;

        // 현재 웨이브 * 1.5에 반올림 한 개수 만큼 좀비를 생성
        int spawnCount = Mathf.RoundToInt(wave * 1.5f);

        // spawnCount 만큼 좀비를 생성
        for (int i = 0; i < spawnCount; i++)
        {
            // 좀비 생성 처리 실행
            CreateZombie();
        }
    }

    // 좀비를 생성하고 생성한 좀비에게 추적할 대상을 할당
    private void CreateZombie()
    {
        ZombieData zombieData = zombieDatas[Random.Range(0, zombieDatas.Length - 1)];
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject zombieObj = PhotonNetwork.Instantiate(zombiePrefab.name, spawnPoint.position, spawnPoint.rotation);
        Zombie zombie = zombieObj.GetComponent<Zombie>();

        //zombie.Setup(zombieData);
        zombie.photonView.RPC("Setup", RpcTarget.All, zombieData.health, zombieData.damage, zombieData.speed, zombieData.skinColor);                   // 모든 클라이언트에게 enemy의 hp, damage, speed, skinColor 설정

        
        zombies.Add(zombie);

        zombie.onDeath += () => zombies.Remove(zombie);
        zombie.onDeath += () => StartCoroutine(DestroyAfter(zombieObj, 3f));
        zombie.onDeath += () => GameManager.instance.AddScore(100);
    }

    private IEnumerator DestroyAfter(GameObject target, float delay)                // target을 delay 후에 제거
    {
        yield return new WaitForSeconds(delay);
        if (target != null)
            PhotonNetwork.Destroy(target);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)                       // 데이터를 보낼 때
        {
            stream.SendNext(zombies.Count);             // enemies의 개수를 전송.   1번째로 전송
            stream.SendNext(wave);                      // wave를 전송.            2번째로 전송
        }
        else if (stream.IsReading)                  // 데이터를 받을 때
        {
            enemyCount = (int)stream.ReceiveNext();     // enemyCount를 전송받은 데이터로 설정.     1번째로 전송된 데이터를 받음
            wave = (int)stream.ReceiveNext();           // wave를 전송받은 데이터로 설정.           2번째로 전송된 데이터를 받음
        }
    }
}