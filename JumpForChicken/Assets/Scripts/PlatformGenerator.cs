using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlatformGenerator : MonoBehaviour
{
    public TileLoader tileLoader; // TileLoader의 참조

    public int minTile = 4; // 최소 타일 개수
    public float minMove = 16f; // 최소 이동 거리
    public float declineArea = 3f; // 제외 범위
    public float sideDecline = 2.0f; // 벽에서부터 생성 불가능 범위
    public float minYSelect = 2.0f;

    private List<Vector3> tileList = new List<Vector3>(); // 타일 리스트
    private List<float> tileYList = new List<float>(); // 타일의 y좌표 리스트

    public Grid grid; // 그리드 설정

    void Start()
    {
        Sync();
        GenerateInitialTiles();
    }

    void Sync(){
        declineArea = PlayerPrefs.GetFloat("declineArea");
        sideDecline = PlayerPrefs.GetFloat("sideDecline");
        minMove = PlayerPrefs.GetFloat("minMove");
        minTile = PlayerPrefs.GetInt("minTile");
        minYSelect = PlayerPrefs.GetFloat("minYSelect");
    }

    void GenerateInitialTiles()
    {
        if (tileLoader != null)
        {
            tileLoader.LoadRandomTiles();
        }

        if (tileLoader.RandomTiles != null && tileLoader.RandomTiles.Length > 0)
        {
            // 타일을 활용하여 프리팹 생성하기
            foreach (GameObject prefab in tileLoader.RandomTiles)
            {
                nextTile(prefab);
            }
        }

        /*for (int i = 0; i < tileCount; i++)
        {
            nextTile();
        }*/

        Debug.Log($"Spawn Rule: {declineArea} {minTile} {minMove} {sideDecline} {minYSelect}");
    }

    public void nextTile(GameObject prefab)
    {
        float minYSelectP = minYSelect;
        float x = 0f;
        float y = 0f;
        float pointX = 0f;
        float rMin = 0f;
        float rMax = 8f;
        float yTest = 0f;
        float yLeftMin = 0f;
        float yRightMin = 0f;

        // 다음 타일의 높이 범위 설정
        float nextTileHeight = Random.Range(3, 5);

        if (tileYList.Count >= minTile)
        {
            float lastTileSum = 0f;
            for (int i = Mathf.Max(0, tileYList.Count - (minTile - 2)); i < tileYList.Count; i++)
            {
                lastTileSum += tileYList[i];
            }
            float sumDifference = minMove - lastTileSum;

            if (sumDifference > 2f && sumDifference <= 8f)
            {
                minYSelectP = sumDifference;
            }
            else if (sumDifference > 8f)
            {
                minYSelectP = 8f;
            }
        }

        y = minYSelectP == 8f ? 8f : Mathf.Round(Random.Range(minYSelectP, 8f) * 10f) / 10f;

        if (y >= 0 && y < declineArea * 7 / 8)
        {
            yTest = -(4f / 7f) * y + 6f * declineArea / 8f;
        }
        else
        {
            yTest = -(1f / 4f) * y + 3f * declineArea / 8f;
        }

        float lastTileX = tileList.Count > 0 ? tileList[tileList.Count - 1].x : 0f;
        float lastTileSize = tileList.Count > 0 ? tileList[tileList.Count - 1].z : 0f;

        float leftIn = lastTileX - yTest - lastTileSize / 2;
        float rightIn = lastTileX + yTest + lastTileSize / 2;

        if (leftIn < -4.5f + sideDecline && rightIn > 4.5f - sideDecline)
        {
            yLeftMin = -(7f / 4f) * (lastTileX - lastTileSize / 2 + 4.5f - sideDecline) + 21f * declineArea / 16f;
            yRightMin = -(7f / 4f) * (-lastTileX - lastTileSize / 2 + 4.5f - sideDecline) + 21f * declineArea / 16f;
            y = Mathf.Min(yLeftMin, yRightMin);
        }

        if (y >= declineArea)
        {
            rMin = 0f;
            rMax = y >= 0 && y < 7f ? -(4f / 7f) * y + 6f : -(1f / 4f) * y + 3f;

            float left = lastTileX - rMax - lastTileSize / 2;
            float right = lastTileX + rMax + lastTileSize / 2;

            if (left < -4.5f + sideDecline)
            {
                left = -4.5f + sideDecline;
            }
            if (right > 4.5f - sideDecline)
            {
                right = 4.5f - sideDecline;
            }

            pointX = Random.Range(left, right);
        }
        else
        {
            rMin = y >= 0 && y < declineArea * 7 / 8 ? -(4f / 7f) * y + 6f * declineArea / 8f : -(1f / 4f) * y + 3f * declineArea / 8f;
            rMax = -(4f / 7f) * y + 6f;

            float leftOut = lastTileX - rMax - lastTileSize / 2;
            float rightOut = lastTileX + rMax + lastTileSize / 2;
            leftIn = lastTileX - rMin - lastTileSize / 2;
            rightIn = lastTileX + rMin + lastTileSize / 2;

            int side = 0;
            if (leftIn < -4.5f + sideDecline) side = 2;
            if (rightIn > 4.5f - sideDecline) side = 1;

            if (leftOut < -4.5f + sideDecline) leftOut = -4.5f + sideDecline;
            if (rightOut > 4.5f - sideDecline) rightOut = 4.5f - sideDecline;

            if (side == 1)
            {
                pointX = Random.Range(leftOut, leftIn);
            }
            else if (side == 2)
            {
                pointX = Random.Range(rightIn, rightOut);
            }
            else
            {
                float rand = Random.Range(0f, leftIn - leftOut + rightIn - rightOut);
                if (rand > leftIn - leftOut)
                {
                    pointX = rightIn + rand - (leftIn - leftOut);
                }
                else
                {
                    pointX = leftOut + rand;
                }
            }
        }

        x = pointX > lastTileX ? pointX + nextTileHeight / 2 : pointX - nextTileHeight / 2;

        // tileYList와 tileList 업데이트
        tileYList.Add(y);
        tileList.Add(new Vector3(x, tileList.Count > 0 ? tileList[tileList.Count - 1].y + y : y, nextTileHeight));

        // 새 타일 Instantiate하고 높이, 위치 설정 및 TileLoader에서 프리팹 불러오기
        GameObject newTile = Instantiate(prefab, new Vector3(x, tileList[tileList.Count - 1].y, 0), Quaternion.identity);

        // 발판의 현재 월드 좌표를 저장
        Vector3 worldPosition = newTile.transform.position;

        // 새 타일을 Grid의 자식으로 설정
        newTile.transform.SetParent(grid.transform);

        // 발판의 위치를 다시 설정 (로컬 좌표계를 유지)
        newTile.transform.position = worldPosition;
    }

    public void DAUp(){PlayerPrefs.SetFloat("declineArea", PlayerPrefs.GetFloat("declineArea") + .1f); Sync();}
    public void DADown(){PlayerPrefs.SetFloat("declineArea", PlayerPrefs.GetFloat("declineArea") - .1f); Sync();}
    public void MTUp(){PlayerPrefs.SetInt("minTile", PlayerPrefs.GetInt("minTile") + 1); Sync();}
    public void MTDown(){PlayerPrefs.SetInt("minTile", PlayerPrefs.GetInt("minTile") - 1); Sync();}
    public void MMUp(){PlayerPrefs.SetFloat("minMove", PlayerPrefs.GetFloat("minMove") + .5f); Sync();}
    public void MMDown(){PlayerPrefs.SetFloat("minMove", PlayerPrefs.GetFloat("minMove") - .5f); Sync();}
    public void SDUp(){PlayerPrefs.SetFloat("sideDecline", PlayerPrefs.GetFloat("sideDecline") + .05f); Sync();}
    public void SDDown(){PlayerPrefs.SetFloat("sideDecline", PlayerPrefs.GetFloat("sideDecline") - .05f); Sync();}
    public void MYUp(){PlayerPrefs.SetFloat("minYSelect", PlayerPrefs.GetFloat("minYSelect") + .1f); Sync();}
    public void MYDown(){PlayerPrefs.SetFloat("minYSelect", PlayerPrefs.GetFloat("minYSelect") - .1f); Sync();}
}
