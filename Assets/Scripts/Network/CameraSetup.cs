using System.Collections;
using System.Collections.Generic;
using UnityEngine;          // 유니티 엔진 관련 라이브러리
using Photon.Pun;           // 유니티용 포톤 라이브러리
using Cinemachine;          // 시네머신 라이브러리

public class CameraSetup : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine)              // 내 캐릭터일 경우. photonView.IsMine은 현재 로컬 플레이어가 이 객체를 컨트롤하는지 여부를 나타내는 bool 값
        {
            CinemachineVirtualCamera followCam = FindObjectOfType<CinemachineVirtualCamera>();
            followCam.Follow = transform;
            followCam.LookAt = transform;
        }
    }

    void Update()
    {

    }
}
