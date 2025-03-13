using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrefabName;
using Unity.VisualScripting;

public class PlatformGenerator : MonoBehaviour
{
    public TileLoader tileLoader; // TileLoader의 참조

    public int tileCount = 70;
    public int minTile = 4; // 최소 타일 개수
    public float minMove = 16f; // 최소 이동 거리
    public float declineArea = 3f; // 제외 범위
    public float sideDecline = 2.0f; // 벽에서부터 생성 불가능 범위
    public float minYSelect = 2.0f;

    public int stage = 1;

    public bool isLoaded = false;
    public int runFrames = 0;

    private List<Vector3> tileList = new List<Vector3>(); // 타일 리스트
    private List<float> tileYList = new List<float>(); // 타일의 y좌표 리스트

    public Grid grid; // 그리드 설정

    private BlockStore blockStore;

    /// <summary>
    /// 스테이지 1 장애물 설치 알고리즘을 위한 변수 배열
    /// </summary>
    /// <value>0: first T3 (BoolInt), 1: T3-4 Count</value>
    private int[] stage1Conditions = {0, 0};

    /// <summary>
    /// 스테이지 2 장애물 설치 알고리즘을 위한 변수 배열
    /// </summary>
    /// <value>0: first Init, 1: T2-4 Count, 2: FiveCount</value>
    private int[] stage2Conditions = {0, 0, 0};

    void Start()
    {
        isLoaded = false;
        runFrames = 0;
        blockStore = GameObject.Find("BlockStore").GetComponent<BlockStore>();
    }

    private void Update() {
        // 에디터 환경에서 SampleScene을 바로 실행했을 때 맵이 로드되지 않는 현상 수정
        if (isLoaded) return;
        else if (!isLoaded && runFrames == 0) {
            Debug.Log("[PlatformGenerator] 로드 대기 중입니다. 다음 프레임에 로드되지 않으면 다시 로드합니다.");
            runFrames++;
            return;
        }
        else {
            Debug.Log("[PlatformGenerator] 블럭이 로드되지 않았습니다. 블럭을 새롭게 로드합니다.");
            LoadingSceneManager.LoadScene("SampleScene");
            return;
        }
    }

    void Sync(){
        declineArea = PlayerPrefs.GetFloat("declineArea");
        sideDecline = PlayerPrefs.GetFloat("sideDecline");
        minMove = PlayerPrefs.GetFloat("minMove");
        minTile = PlayerPrefs.GetInt("minTile");
        minYSelect = PlayerPrefs.GetFloat("minYSelect");
    }

    public IEnumerator GenerateInitialTiles()
    {
        // LoadingSceneManager에 의하여 Update() 첫 실행 직후에 실행되는 함수입니다.
        isLoaded = true;
        Sync();
        if (tileLoader != null)
        {
            tileLoader.LoadAllPrefabs();
        }

        for (int i = 0; i < tileCount; i++)
        {
            GameObject nextPrefab = new();
            if (tileList.Count >= 1 && tileList[^1].y > 115f && stage == 1){
                /* 스테이지 1 마지막 발판 설치 */
                nextPrefab = tileLoader.GetLastBlock(1);
                nextTile(nextPrefab);

                /* 스테이지 2 첫 번째 발판 매칭 */
                stage = 2;
                GameObject firstBlock = GameObject.Find("mountainBlock_FirstBlock");
                PlatformStateManager psm = firstBlock.AddComponent<PlatformStateManager>();
                psm.SetBlockIndex(blockStore.blocks.Count);
                blockStore.AddPrefab(psm);

                tileYList.Add(5f);
                tileList.Add(new Vector3(0, 136f, 0));
    
            } else if (tileList.Count >= 1 && tileList[^1].y > 300f && stage == 2){
                /* 스테이지 2 마지막 발판 설치 */
                nextPrefab = tileLoader.GetLastBlock(2);
                nextTile(nextPrefab);

                /* 스테이지 3 첫 번째 발판 매칭 */
                stage = 3;
                GameObject firstBlock = GameObject.Find("skyBlock_FirstBlock");
                PlatformStateManager psm = firstBlock.AddComponent<PlatformStateManager>();
                psm.SetBlockIndex(blockStore.blocks.Count);
                blockStore.AddPrefab(psm);

                tileYList.Add(5f);
                tileList.Add(new Vector3(0, 321f, 0));
            }
            else {
                nextPrefab = tileLoader.GetNextBlock(stage); 
                nextTile(nextPrefab);
            }

            float currentProgress = 20f + ((float)(i + 1) / tileCount * 80f);
            if (LoadingSceneManager.instance != null)
            {
                LoadingSceneManager.instance.UpdateProgress(currentProgress);
            }

            if ((i + 1) % 10 == 0)
            {
                yield return null;
            }
        }

        blockStore.ChangeBlockStateLayer(0);

        Debug.Log($"Spawn Rule: {declineArea} {minTile} {minMove} {sideDecline} {minYSelect} {tileCount}");
    }

    public void nextTile(GameObject prefab)
    {
        float minYSelectP = minYSelect;
        float x = 0f;
        float y;
        float pointX;
        float rMin;
        float rMax;
        float yTest;
        float yLeftMin;
        float yRightMin;
        float maxYSelect = 7.8f;
        float maxYSelectP = maxYSelect;
        float tigerPos = 0f;
        bool isExistTiger = false;
        int jumpDirection = 0;

        // 다음 타일의 높이 범위 설정
        int[] attributes = PrefabNameTranslator.ToPrefabAttribute(prefab.name);
        float nextTileSize = attributes[1];

        // '최소 타일' 조건 적용
        // minTile 동안 minMove 이상 올라가야 하는 규칙
        if (tileYList.Count >= minTile)
        {
            float lastTileSum = 0f;
            for (int i = Mathf.Max(0, tileYList.Count - (minTile - 2)); i < tileYList.Count; i++)
            {
                lastTileSum += tileYList[i];
            }
            float sumDifference = minMove - lastTileSum;

            if (sumDifference > 2f && sumDifference <= maxYSelect)
            {
                minYSelectP = sumDifference;
            }
            else if (sumDifference > maxYSelect)
            {
                minYSelectP = maxYSelect;
            }
        } else if (tileYList.Count < 2){
            minYSelectP = 2.4f;
            maxYSelectP = 4.5f;
        }

        // 다음 블록의 높이를 랜덤으로 설정 (전 조건문에 의해 결정된 최소 선택 높이 부터 최대 높이까지)
        y = minYSelectP >= maxYSelect ? maxYSelect : Mathf.Round(Random.Range(minYSelectP, maxYSelectP) * 10f) / 10f;

        if (attributes[6] != 0){ // 만약 부착형 블록이면
            /* 부착형 블록 배치 알고리즘 */
            float lastTileX = tileList.Count > 0 ? tileList[^1].x : 0f;
            float lastTileSize = tileList.Count > 0 ? tileList[^1].z : 0f;
            
            int[] _sizes = PlatformGenerateCalculator.GetJumpableSizes(lastTileX, lastTileSize, declineArea, -attributes[6], 2f);
            // 1. 블럭 선택
            if (_sizes[attributes[1] - 2] != 1){ // 설치하고자 하는 블록이 설치 가능한 상태가 아니라면
                /* 다른 블록으로 랜덤 변경 */
                attributes[1] = PlatformGenerateCalculator.GetRandomJumpableTileSize(_sizes);
                prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetChangedSizePrefab(prefab.name, attributes[1]));
            } 

            // 2. pointX, y좌표 선택
            float[] yRange = {0, 0};
            yRange = PlatformGenerateCalculator.GetJumpableYRange(lastTileX, lastTileSize, declineArea, -attributes[6], 2f, attributes[1]);

            if (y > yRange[1] || y < yRange[0]){ // 이미 선택된 y가 설치 가능한 범위 안에 없어서 다시 선택해야 할 때
                y = Random.Range(yRange[0], yRange[1]);
            }

            nextTileSize = attributes[1];
            pointX = -attributes[6] * (4.5f - nextTileSize);
            x = -attributes[6] * (4.5f - nextTileSize / 2f);

            if (pointX > lastTileX){ // pointX가 이전 x좌표 보다 오른쪽에 있어서, 오른쪽으로 점프하는 상황
                jumpDirection = 1;
            } else { // pointX가 이전 x좌표 보다 왼쪽에 있어서, 왼쪽으로 점프하는 상황
                jumpDirection = -1;
            }

        } else { // 만약 x좌표가 자유로운 블록이면
            // 만약 통과 불가능 블럭이면 Side 제외 범위 증가
            float _sideDecline = PrefabNameTranslator.ToPrefabAttribute(prefab.name)[2] == 1 ? sideDecline : sideDecline + 0.5f;

            // 어떤 y에 대한 x 존재성 테스트
            if (y >= 0 && y < declineArea * 7 / 8)
            {
                yTest = -(4f / 7f) * y + 6f * declineArea / 8f;
            }
            else
            {
                yTest = -(1f / 4f) * y + 3f * declineArea / 8f;
            }

            float lastTileX = tileList.Count > 0 ? tileList[^1].x : 0f;
            float lastTileSize = tileList.Count > 0 ? tileList[^1].z : 0f;

            float leftIn = lastTileX - yTest - lastTileSize / 2;
            float rightIn = lastTileX + yTest + lastTileSize / 2;

            // 만약 어떤 y에 대해 대응되는 x가 존재하지 않으면
            if (leftIn < -4.5f + _sideDecline && rightIn > 4.5f - _sideDecline)
            {
                yLeftMin = -(7f / 4f) * (lastTileX - lastTileSize / 2 + 4.5f - _sideDecline) + 21f * declineArea / 16f;
                yRightMin = -(7f / 4f) * (-lastTileX - lastTileSize / 2 + 4.5f - _sideDecline) + 21f * declineArea / 16f;
                y = Mathf.Max(y, Mathf.Min(yLeftMin, yRightMin));
            }

            // y가 제외 범위를 넘어가 별도의 조건이 필요하지 않은 경우
            if (y >= declineArea)
            {
                // rMin 변수를 사용해야 할 상황이 있을 것 같아서 미리 제작해둡니다.
                rMin = PrefabNameTranslator.ToPrefabAttribute(prefab.name)[2] == 1 ? 0 : .5f;
                rMax = y >= 0 && y < 7f ? -(4f / 7f) * y + 6f : -(1f / 4f) * y + 3f;

                float left = lastTileX - rMax - lastTileSize / 2;
                float right = lastTileX + rMax + lastTileSize / 2;

                leftIn = lastTileX - rMin;
                rightIn = lastTileX + rMin;

                

                int side = 0;
                if (leftIn < -4.5f + _sideDecline) side = 2; // 왼쪽에 설치가 가능한 부분이 없을 경우
                if (rightIn > 4.5f - _sideDecline) side = 1; // 오른쪽에 설치가 가능한 부분이 없을 경우

                left = Mathf.Max(left, -4.5f + _sideDecline);
                right = Mathf.Min(right, 4.5f - _sideDecline);

                // 버그 수정
                leftIn = Mathf.Min(leftIn, 4.5f - _sideDecline);
                rightIn = Mathf.Max(rightIn, -4.5f + _sideDecline);

                if (side == 1)
                {
                    pointX = Random.Range(left, leftIn);
                }
                else if (side == 2)
                {
                    pointX = Random.Range(rightIn, right);
                }
                else
                {
                    float rand = Random.Range(0f, leftIn - left + right - rightIn);
                    pointX = rand > (leftIn - left)
                            ? rightIn + rand - (leftIn - left)
                            : left + rand;
                }
            }
            else // y가 제외 범위 내로 있어서 생성 가능 x가 두 구역으로 나뉜 경우
            {
                rMin = y >= 0 && y < declineArea * 7 / 8 ? -(4f / 7f) * y + 6f * declineArea / 8f : -(1f / 4f) * y + 3f * declineArea / 8f;
                rMax = -(4f / 7f) * y + 6f;

                float leftOut = lastTileX - rMax - lastTileSize / 2;
                float rightOut = lastTileX + rMax + lastTileSize / 2;
                leftIn = lastTileX - rMin - lastTileSize / 2;
                rightIn = lastTileX + rMin + lastTileSize / 2;

                int side = 0;
                if (leftIn < -4.5f + _sideDecline) side = 2; // 왼쪽에 설치가 가능한 부분이 없을 경우
                if (rightIn > 4.5f - _sideDecline) side = 1; // 오른쪽에 설치가 가능한 부분이 없을 경우
                // 여기서 두 부분에 설치가 가능한 부분이 없을 경우는 존재하지 않습니다.

                // 측면 최소 너비를 만족하기 위한 Max, Min (변경)
                leftOut = Mathf.Max(leftOut, -4.5f + _sideDecline);
                rightOut = Mathf.Min(rightOut, 4.5f - _sideDecline);

                // 버그 수정
                leftIn = Mathf.Min(leftIn, 4.5f - _sideDecline);
                rightIn = Mathf.Max(rightIn, -4.5f + _sideDecline);

                if (side == 1)
                {
                    pointX = Random.Range(leftOut, leftIn);
                }
                else if (side == 2)
                {
                    pointX = Random.Range(rightIn, rightOut);
                }
                else // 양쪽 모두 설치 가능한 부분이 있어 랜덤을 한 번에 구역을 나누어 실행합니다.
                {
                    float rand = Random.Range(0f, leftIn - leftOut + rightOut - rightIn);
                    pointX = rand > (leftIn - leftOut)
                            ? rightIn + rand - (leftIn - leftOut)
                            : leftOut + rand;
                }
            }

            // 왼쪽으로 점프하는 상황인지, 오른쪽으로 점프하는 상황인지 나누고 x좌표를 설정
            // '좌우반전'은 UP! 글씨 및 여러가지 부적절한 요소가 있어서, 반대에 대응되는 프리팹을 선택하는 것으로 결정.
            if (pointX > lastTileX){ // pointX가 이전 x좌표 보다 오른쪽에 있어서, 오른쪽으로 점프하는 상황
                x = pointX + nextTileSize / 2;
                jumpDirection = 1;
                if (PrefabNameTranslator.ToPrefabAttribute(prefab.name)[3] == 1){
                    prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetOppositePrefab(prefab.name));
                    Debug.Log("[PlatformGenerator] 반대 프리팹을 선택합니다.");
                }
            } else { // pointX가 이전 x좌표 보다 왼쪽에 있어서, 왼쪽으로 점프하는 상황
                x = pointX - nextTileSize / 2;
                jumpDirection = -1;
                if (PrefabNameTranslator.ToPrefabAttribute(prefab.name)[3] == -1){
                    prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetOppositePrefab(prefab.name));
                    Debug.Log("[PlatformGenerator] 반대 프리팹을 선택합니다.");
                }
            }
        }

        // tileYList와 tileList 업데이트
        tileYList.Add(y);
        tileList.Add(new Vector3(x, tileList.Count > 0 ? tileList[^1].y + y : y, nextTileSize));

        int[] _attribute = PrefabNameTranslator.ToPrefabAttribute(prefab.name);

        // 장애물 처리 (Stage 1)
        if (_attribute[0] == 1){
            if (stage1Conditions[0] == 0){ // 아직 T3 블록이 나오지 않았다면
                if (_attribute[2] == 1 && _attribute[1] == 3) // T3 블록에 대해서
                {
                    stage1Conditions[0] = 1;
                    prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetObstaclePrefab(prefab.name)); // 장애물이 있는 프리팹으로 변경
                } 
            } else { // T3 블록이 나오고 나서 부터
                if (_attribute[2] == 1 && (_attribute[1] >= 3) && (_attribute[1] <= 4)){ // 3칸 이상의 도로 타일에 대해서
                    stage1Conditions[1] += 1;
                    if (stage1Conditions[1] == 2){ // 2번째
                        if (Random.Range(1, 3) == 1){ // 50%의 확률
                            stage1Conditions[1] = 0; // 번호 초기화
                            prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetObstaclePrefab(prefab.name)); // 장애물이 있는 프리팹으로 변경
                        }
                    } else if (stage1Conditions[1] == 3){ // 3번째 (장애물 설치 확정)
                        stage1Conditions[1] = 0; // 번호 초기화
                        prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetObstaclePrefab(prefab.name)); // 장애물이 있는 프리팹으로 변경
                    }
                }
            }
        } else if (_attribute[0] == 2){ // 장애물 처리 (Stage 2)
            if (_attribute[2] == 0 && _attribute[3] == 0 && _attribute[1] >= 2){ // 장애물 설치가 가능한 블럭에 대해서
                if (stage2Conditions[0] == 0){ // 만약 초기화되지 않은 상황이라면
                    stage2Conditions = new int[] {1, Random.Range(1, 6), 5}; // 초기화
                }

                stage2Conditions[1]--;
                stage2Conditions[2]--;

                if (stage2Conditions[1] == 0){
                    /* 호랑이 설치 */
                    GameObject _prefab;
                    _prefab = tileLoader.NameToPrefab(PrefabNameTranslator.GetObstaclePrefab(prefab.name));
                    GameObject tiger = _prefab.transform.GetChild(0).gameObject;
                    if (!_prefab.CompareTag("LastBlock") && tiger.GetComponent<TigerManager>().TryAvailable(x)){ // 설치가 가능하다면
                        prefab = _prefab;
                        tigerPos = tiger.GetComponent<TigerManager>().TigerPositionChange(jumpDirection, x);
                        isExistTiger = true;
                    } else {
                        stage2Conditions[1]++;
                        stage2Conditions[2]++;
                        Debug.Log("[PlatformGenerator] 호랑이 장애물을 설치할 수 없어 다음으로 미룹니다.");
                    }
                }

                if (stage2Conditions[2] == 0){
                    stage2Conditions = new int[] {1, Random.Range(1, 6), 5}; // 구역 초기화
                }
            }
        } else if (stage == 3){

        } else if (stage == 4){

        }

        // 새 타일 Instantiate하고 높이, 위치 설정 및 TileLoader에서 프리팹 불러오기
        GameObject newTile = Instantiate(prefab, new Vector3(x, tileList[^1].y, 0), Quaternion.identity);

        // 발판의 현재 월드 좌표를 저장
        Vector3 worldPosition = newTile.transform.position;

        // 새 타일을 Grid의 자식으로 설정
        newTile.transform.SetParent(grid.transform);

        // 발판의 위치를 다시 설정 (로컬 좌표계를 유지)
        newTile.transform.localPosition = worldPosition; // localPosition으로 변경! -> Anchor에 따른 위치 이동 해결

        if (isExistTiger){
            newTile.transform.GetChild(0).position = new Vector2(tigerPos, newTile.transform.GetChild(0).position.y);
        }

        // 플랫폼 상태 컴포넌트 추가
        PlatformStateManager platformManager = newTile.AddComponent<PlatformStateManager>();

        // BlockStore에 BlockIndex 할당
        platformManager.SetBlockIndex(blockStore.blocks.Count);

        // BlockStore에 프리팹을 추가
        blockStore.AddPrefab(platformManager);

        // 플랫폼 배치 완료 로그
        Debug.Log($"블록 배치 완료: {newTile.name}, 좌표: {newTile.transform.position}, {nextTileSize}, {pointX}");
    }
}
