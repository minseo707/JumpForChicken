using System;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

namespace PrefabName {
    public class PrefabNameTranslator : MonoBehaviour 
    {
        /// <summary>
        /// 프리팹 속성을 이름으로 변환합니다.
        /// </summary>
        /// <param name="stage">1, 2, 3, 4</param>
        /// <param name="tileType">타일 타입 이름</param>
        /// <param name="isLTile">L타일인지 여부</param>
        /// <param name="blockIndex">블록 인덱스</param>
        /// <param name="isObstacle">장애물인지 여부</param>
        /// <param name="isBlockR">통과 가능 블록 방향</param>
        /// <returns>프리팹 이름 문자열</returns>
        public static string ToPrefabName(int stage, string tileType, int blockIndex, bool isObstacle = false, bool isBlockR = false){
            string result = "";
            switch (stage){
                case 1:
                    result += "cityBlock_";
                    break;
                case 2:
                    result += "mountainBlock_";
                    break;
                case 3:
                    result += "skyBlock_";
                    break;
                case 4:
                    result += "spaceBlock_";
                    break;
                default:
                    break;
            }

            result += tileType;

            if (tileType.EndsWith("L") || tileType.EndsWith("R")){
                result += blockIndex.ToString();
            } else {
                result += $"_{blockIndex}";
            }

            if (isObstacle) result += "O";

            return result;
        }

        /// <summary>
        /// 반대에 대응되는 타일 이름을 반환합니다.
        /// (L자 블록만 가능합니다.)
        /// </summary>
        /// <param name="prefabName">프리팹 이름</param>
        /// <returns>반대 프리팹 이름</returns>
        public static string GetOppositePrefab(string prefabName){
            int[] _attributes = ToPrefabAttribute(prefabName);
            string tileType = "";
            if (_attributes[3] != 0){ // L자 블록이면
                if (_attributes[1] == 2){
                    tileType = _attributes[3] == 1? "F0_L" : "F0"; // Opposite Prefab
                } else if (_attributes[1] == 3){
                    tileType = _attributes[3] == 1? "F5_L" : "F5"; // Opposite Prefab
                } else if (_attributes[1] == 4){
                    tileType = _attributes[3] == 1? "F6_L" : "F6"; // Opposite Prefab
                }
                return ToPrefabName(_attributes[0], tileType, _attributes[4], _attributes[5] == 1 ? true : false);
            }
            Debug.LogError("[PrefabNameTranslator] 반대로 전환할 수 없습니다.");
            return "";
        }


        /// <summary>
        /// 한 칸 더 큰 너비를 가진 프리팹을 반환합니다.
        /// (2스테이지 통과 가능 블럭만 가능합니다.)
        /// </summary>
        /// <param name="prefabName">프리팹 이름</param>
        /// <returns></returns>
        public static string GetChangedSizePrefab(string prefabName, int size){
            int[] _attributes = ToPrefabAttribute(prefabName);
            if (_attributes[0] == 2){
                string tileType = "";
                _attributes[1] = size;
                if (_attributes[6] == 1){
                    tileType = 'T'+size.ToString()+"_R";
                } else if (_attributes[6] == -1){
                    tileType = 'T'+size.ToString()+"";
                }
                return ToPrefabName(_attributes[0], tileType, _attributes[4], _attributes[5] == 1 ? true : false);
            }
            return "";
        }

        /// <summary>
        /// 장애물이 포함된 프리팹을 반환합니다.
        /// (1, 2 스테이지만 가능합니다.)
        /// </summary>
        /// <param name="prefabName">프리팹 이름</param>
        /// <returns></returns>
        public static string GetObstaclePrefab(string prefabName){
            int[] _attributes = ToPrefabAttribute(prefabName);
            string tileType;
            if (_attributes[0] == 1){ // 스테이지 1에서
                if (_attributes[1] == 3 && _attributes[2] == 1){ // 타일 사이즈 3이면
                    tileType = "T3";
                    return ToPrefabName(_attributes[0], tileType, _attributes[4], true);
                } else if (_attributes[1] == 4 && _attributes[2] == 1){ // 타일 사이즈 4면
                    tileType = "T4";
                    return ToPrefabName(_attributes[0], tileType, _attributes[4], true);
                } else {
                    Debug.LogError("[PrefabNameTranslator] 해당 프리팹은 대응되는 장애물이 없습니다.: " + prefabName);
                    return "";
                }
            } else if (_attributes[0] == 2){ // 스테이지 2에서
                if (_attributes[1] == 2 && _attributes[2] == 0 && _attributes[3] == 0){
                    tileType = "F2";
                    return ToPrefabName(_attributes[0], tileType, _attributes[4], true);
                } else if (_attributes[1] == 3 && _attributes[2] == 0 && _attributes[3] == 0){
                    tileType = "F3";
                    return ToPrefabName(_attributes[0], tileType, _attributes[4], true);
                } else if (_attributes[1] == 4 && _attributes[2] == 0 && _attributes[3] == 0){
                    tileType = "F4";
                    return ToPrefabName(_attributes[0], tileType, _attributes[4], true);
                } else {
                    Debug.LogError("[PrefabNameTranslator] 해당 프리팹은 대응되는 장애물이 없습니다.: " + prefabName);
                    return "";
                }
            }
            return "";
        }

        /// <summary>
        /// 프리팹 이름을 속성으로 변환합니다.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns>[0: stage, 1: nextTileSize, 2: oneWay, 3: blockL, 4: blockIndex, 5: isObstacle, 6: blockR]</returns>
        public static int[] ToPrefabAttribute(string prefabName){
            if (prefabName == "cityBlock_LastBlock") return new int[] {1, 4, 0, 0, 1, 0, 0};
            if (prefabName == "mountainBlock_LastBlock") return new int[] {2, 4, 0, 0, 1, 0, 0};
            if (prefabName == "skyBlock_LastBlock") return new int[] {3, 4, 0, 0, 1, 0, 0};

            int stage = 1;
            int nextTileSize = 2;
            int oneWay; // 통과 가능: 1, 통과 불가능: 0
            int blockL = 0; // L자블록 X: 0, L자블록 오른쪽: 1, L자블록 왼쪽: -1
            int blockR = 0; // 벽 부착 블록 X: 0, 오른쪽에서 점프(R): 1, 왼쪽에서 점프: -1
            int blockIndex;
            int isObstacle = 0; // isObstacle: 1,

            string _pfname = prefabName;

            if (_pfname.Contains("cityBlock"))
            {
                _pfname = _pfname.Replace("cityBlock_", "");
                stage = 1;
            } else if (_pfname.Contains("mountainBlock")){
                _pfname = _pfname.Replace("mountainBlock_", "");
                stage = 2;
            } else if (_pfname.Contains("skyBlock")){
                _pfname = _pfname.Replace("skyBlock_", "");
                stage = 3;
            } else if (_pfname.Contains("spaceBlock")){
                _pfname = _pfname.Replace("spaceBlock_", "");
                stage = 4;
            } else {
                Debug.LogError("Prefab name error : " + _pfname);
            }

            oneWay = _pfname.StartsWith("T") ? 1 : 0;
            _pfname = _pfname.Substring(1);

            // 추후 스테이지 변경에 따라 수정
            if (_pfname.StartsWith("2")) nextTileSize = 2;
            else if (_pfname.StartsWith("3")) nextTileSize = 3;
            else if (_pfname.StartsWith("4")) nextTileSize = 4;
            else if (_pfname.StartsWith("5") && oneWay == 1){
                nextTileSize = 5;
            }
            else if (_pfname.StartsWith("5") && oneWay == 0) {
                nextTileSize = 3;
                blockL = 1;
            }
            else if (_pfname.StartsWith("6")){
                nextTileSize = 4;
                blockL = 1;
            }
            else if (_pfname.StartsWith("0")){
                nextTileSize = 2;
                blockL = 1;
            }
            _pfname = _pfname.Substring(2);

            if (_pfname.StartsWith("L")){
                _pfname = _pfname.Substring(1);
                blockL = -1;
            }

            if (stage == 2){
                if (_pfname.StartsWith("R")){
                    _pfname = _pfname.Substring(1);
                    blockR = 1;
                } else if (oneWay == 1){
                    blockR = -1;
                }
            }

            blockIndex = int.Parse(_pfname[0].ToString());

            if (_pfname.StartsWith("O")) isObstacle = 1; // 알파벳 O

            return new int[] {stage, nextTileSize, oneWay, blockL, blockIndex, isObstacle, blockR};
        }
    }

    public class PlatformGenerateCalculator {
        /// <summary>
        /// 점프 가능 구역 함수 x->y
        /// </summary>
        /// <param name="x">구역 함수의 x좌표</param>
        /// <returns>구역 함수의 y좌표</returns>
        public static float Jumpable(float x){
            float absX = Mathf.Abs(x);
            if (absX > 2 && absX <= 6) return -7f * absX / 4f + 21f/2f;
            if (absX > 1.25 && absX <= 2) return 7f;
            if (absX > 1 && absX <= 1.25) return -4f * absX + 12f;
            if (absX <= 1) return 8f;

            Debug.LogWarning("[PrefabNameTranslator: PlatformGenerateCalculator.Jumpable] Invalid diff X: " + x);
            return 0;
        }

        public static float Decline(float x, float d){
            float u;
            u = Jumpable(x * 8f / d) * d / 8f;
            return u >= 0 ? u : 0;
        }

        /// <summary>
        /// 가능한 Next Tile Size 배열 구하는 함수
        /// </summary>
        /// <param name="a">이전 블록의 x좌표</param>
        /// <param name="size">이전 블록의 너비</param>
        /// <param name="decline">제외 범위</param>
        /// <param name="direction">오른쪽 부착: 1, 왼쪽 부착: -1</param>
        /// <returns>가능한 Next Tile Size 배열 int[] {0, 0, 0, 0}</returns>
        public static int[] GetJumpableSizes(float a, float size, float decline, int direction, float minYSelect){
            int[] jumpableSizes = new int[] {0, 0, 0, 0}; // 2, 3, 4, 5

            if (direction != 1 && direction != -1) {
                Debug.LogError("[PrefabNameTranslator: PlatformGenerateCalculator.GetJumpableSizes] Invalid direction: " + direction);
                return jumpableSizes;
            }    
            
            float yMax = 0f;
            float yMin = 0f;
            float k = 0;

            for (int i = 2; i < 6; i++) // 2, 3, 4, 5
            {
                k = (4.5f - i) * direction;
                yMax = Mathf.Abs(k - a) >= size / 2f ? 
                    Jumpable(k - a - size / 2f * Sgn(k - a)):
                    8f;
                yMin = Mathf.Abs(k - a) <= size / 2f ?
                    Decline(k - a - size / 2f * Sgn(k - a), decline):
                    decline;

                if (yMax > Mathf.Max(minYSelect, yMin)){
                    /* 설치 가능 */
                    jumpableSizes[i - 2] = 1;
                } else {
                    jumpableSizes[i - 2] = 0;    
                }
            }

            return jumpableSizes;
        }

        /// <summary>
        /// 어떤 부착형 블록에 대한 점프 가능한 Y좌표의 범위를 구하는 함수
        /// </summary>
        /// <param name="a">이전 블록의 x좌표</param>
        /// <param name="size">이전 블록의 너비</param>
        /// <param name="decline">제외 범위</param>
        /// <param name="direction">오른쪽 부착: 1, 왼쪽 부착: -1</param>
        /// <param name="nextTileSize">부착형 블록의 사이즈 (별도의 알고리즘으로 선택)</param>
        /// <returns>[0: yMin, 1: yMax]</returns>
        public static float[] GetJumpableYRange(float a, float size, float decline, int direction, float minYSelect, int nextTileSize){
            float yMax = 0f;
            float yMin = 0f;

            if (direction != 1 && direction != -1) {
                Debug.LogError("[PrefabNameTranslator: PlatformGenerateCalculator.GetJumpableYRange] Invalid direction: " + direction);
                return new float[] {yMin, yMax};
            }    

            float k = 0;

            k = (4.5f - nextTileSize) * direction;
            yMax = Mathf.Abs(k - a) >= size / 2f ? 
                Jumpable(k - a - size / 2f * Sgn(k - a)):
                8f;
            yMin = Mathf.Abs(k - a) <= size / 2f ?
                Decline(k - a - size / 2f * Sgn(k - a), decline):
                decline;

            if (yMax > Mathf.Max(minYSelect, yMin)){
                /* 설치 가능 */
                return new float[] {yMin, yMax};
            } else {
                /* 설치 불가능 */
                Debug.LogError("[PrefabNameTranslator: PlatformGenerateCalculator] The Next Tile Size is NOT Jumpable" + nextTileSize);
                return new float[] {yMin, yMax};
            }
            
        }

        /// <summary>
        /// 점프 가능한 블럭 중에서 랜덤 선택하는 함수
        /// </summary>
        /// <param name="jumpableSizes">설치 가능한 부착형 타일의 사이즈 배열</param>
        /// <returns>랜덤 선택된 타일 사이즈</returns>
        public static int GetRandomJumpableTileSize(int[] jumpableSizes){
            var availableSizes = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                if (jumpableSizes[i] == 1)
                    availableSizes.Add(i + 2);
            }

            if (availableSizes.Count == 0)
            {
                Debug.LogWarning("[PrefabNameTranslator: PlatformGenerateCalculator.GetRandomJumpableTileSize] 사용 가능한 타일 크기가 없습니다.");
                return 5; // Max Size
            }

            return availableSizes[UnityEngine.Random.Range(0, availableSizes.Count)];
        }

        /// <summary>
        /// 실수의 부호를 반환하는 함수
        /// </summary>
        /// <param name="f">float</param>
        /// <returns>-1 or 0 or 1</returns>
        private static int Sgn(float f){
            if (f == 0) return 0;
            return f > 0 ? 1 : -1;
        }
    }
}