using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public static class Utils
{
    // 네비메쉬 위에 아이템 랜덤 드랍 공용 함수
    // origin : 기준점 (이곳을 주변으로 후보 좌표 생성
    // minRadius : 최소거리
    // maxRadius : 최대거리
    // maxTries : 최대 몇번 시도할지 결정 많으면 성공률은 올라가지만 비용이 많이듬
    // sampleRadius : 후보점 주변에 네비메쉬를 찾을때 사용하는 탐색 반경 (후보 점이 조금 벗어나도 이 수치 안에 네비 메쉬가 있으면 적용)
    public static bool RandomDropPointOnNavMesh(Vector3 origin, float minRadius, float maxRadius, out Vector3 result,
        int areaMask = NavMesh.AllAreas, int maxTries = 10, float sampleRadius = 2f)
    {
        for (int i = 0; i < maxTries; i++)
        {
            float radius = Mathf.Sqrt(Random.Range(minRadius * minRadius, maxRadius * maxRadius));
            float theta = Random.Range(0f, Mathf.PI * 2f);
            Vector3 candidate = origin + new Vector3(Mathf.Cos(theta), 0f, Mathf.Sin(theta)) * radius;

            if (NavMesh.SamplePosition(candidate, out var hit, sampleRadius, areaMask))
            {
                result = hit.position;
                return true;
            }
        }

        result = origin;
        return false;
    }
    // 네비메쉬 랜덤 위치 순찰 및 플레이어 스폰 함수 (교체 가능합니다.)
    public static Vector3 GetRandomPointOnNavMesh(Vector3 center, float distance, int areaMask)
    {
        Vector3 randomPos = Random.insideUnitSphere * distance + center;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, distance, areaMask);

        return hit.position;
    }
}
