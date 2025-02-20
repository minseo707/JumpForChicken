using UnityEngine;
using UnityEngine.Tilemaps;

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
        /// <returns>프리팹 이름 문자열</returns>
        public static string ToPrefabName(int stage, string tileType, int blockIndex, bool isObstacle = false){
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

            if (tileType.EndsWith("L")){
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
                if (_attributes[1] == 3){
                    tileType = _attributes[3] == 1? "F5_L" : "F5"; // Opposite Prefab
                } else if (_attributes[1] == 4){
                    tileType = _attributes[3] == 1? "F6_L" : "F6"; // Opposite Prefab
                }
                return ToPrefabName(_attributes[0], tileType, _attributes[4], _attributes[5] == 1 ? true : false);
            }
            Debug.LogError("[PrefabNameTranslator] 반대로 전환할 수 없습니다.");
            return "";
        }


        public static string GetObstaclePrefab(string prefabName){
            int[] _attributes = ToPrefabAttribute(prefabName);
            string tileType = "";
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
            }
            return "";
        }

        /// <summary>
        /// 프리팹 이름을 속성으로 변환합니다.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns>[0: stage, 1: nextTileSize, 2: oneWay, 3: blockL, 4: blockIndex ,5: isObstacle]</returns>
        public static int[] ToPrefabAttribute(string prefabName){
            int stage = 1;
            int nextTileSize = 2;
            int oneWay = 0; // 통과 가능: 1, 통과 불가능: 0
            int blockL = 0; // L자블록 X: 0, L자블록 오른쪽: 1, L자블록 왼쪽: -1
            int blockIndex = 0;
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
            else if (_pfname.StartsWith("5")) {
                nextTileSize = 3;
                blockL = 1;
            }
            else if (_pfname.StartsWith("6")){
                nextTileSize = 4;
                blockL = 1;
            }
            _pfname = _pfname.Substring(2);

            if (_pfname.StartsWith("L")){
                _pfname = _pfname.Substring(1);
                blockL = -1;
            }

            blockIndex = int.Parse(_pfname[0].ToString());

            if (_pfname.StartsWith("O")) isObstacle = 1;

            return new int[] {stage, nextTileSize, oneWay, blockL, blockIndex, isObstacle};
        }
    }
}