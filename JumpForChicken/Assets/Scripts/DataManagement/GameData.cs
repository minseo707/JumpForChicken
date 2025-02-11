/// <summary>
/// 게임 데이터를 정의하는 클래스
/// </summary>
public class GameData {
    public int highScore = 0; // 최대 기록
    public int chickens = 0; // 현재 재화 (치킨)

    /// <summary>
    /// 플레이어가 현재 장착하고 있는 아이템의 인덱스 배열,
    /// [0]: 헬멧, [1]: 옷, [2]: 오토바이 순서로 저장됩니다.
    /// 원소는 hatCode, clothesCode와 같은 아이템 코드입니다.
    /// </summary>
    public int[] equippedGoods = {0, 0, 0};

    /// <summary>
    /// 가지고 있는 헬멧 (0번은 기본)
    /// </summary>
    public bool[] hasHelmets = {true, false, false, false, false};

    /// <summary>
    /// 가지고 있는 옷 (0번은 기본)
    /// </summary>
    public bool[] hasClothes = {true, false, false, false, false};

    /// <summary>
    /// 가지고 있는 오토바이 (0번은 기본)
    /// </summary>
    public bool[] hasMotorcycle = {true, false, false, false, false};

}

/// <summary>
/// 설정 데이터를 정의하는 클래스
/// </summary>
public class SettingsData {
    /// <summary>
    /// 볼륨 설정을 나타내는 배열입니다. 
    /// [0]: 전체 볼륨, [1]: 배경음악 볼륨, [2]: 효과음 볼륨,
    /// 값의 범위는 0.0(음소거)에서 1.0(최대)입니다.
    /// </summary>
    public float[] volumes = {1f, 1f, 1f};
}