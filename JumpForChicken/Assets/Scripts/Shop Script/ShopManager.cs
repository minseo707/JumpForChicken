using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// [상점] 상점 아이템 선택 및 구매 총괄
/// </summary>
public class ShopManager : MonoBehaviour
{
    private bool _selected; // for first update only
    private bool first;
    private bool selectedItemIsPurchased;
    private int selectedHatCode = -1;
    private int selectedClothesCode = -1;
    private int selectedMotorcycleCode = -1;
    private int lastSelectedClassifyCode = -1;

    private bool haveHat;
    private bool haveClothes;
    private bool haveMotorcycle;
    public GameObject previewText;
    public GameObject currentChickenText;
    private TextMeshProUGUI currentChickenTMP;

    private readonly int[] hatPrice = {0, 10, 20, 30, 40};
    private readonly int[] clothesPrice = {0, 10, 20, 30, 40};
    private readonly int[] motorcyclePrice = {0, 10, 20, 30, 40};

    public GameObject purchaseButton;
    private PurchaseButtonManager pbm;

    public GameObject[] buttons;
    private ItemSelectManager[] buttonItemSelectManagers;

    public GameObject player;
    private PlayerAnimationManager pam;

    public GameObject bike;
    private BikeSpriteManager bsm;

    public GameObject starPrefab;
    private int starCycle = 120;

    void Awake(){
        buttonItemSelectManagers = new ItemSelectManager[15];
        for (int i = 0; i < 15; i++) buttonItemSelectManagers[i] = buttons[i].GetComponent<ItemSelectManager>();
        pbm = purchaseButton.GetComponent<PurchaseButtonManager>();
        pam = player.GetComponent<PlayerAnimationManager>();
        bsm = bike.GetComponent<BikeSpriteManager>();
        currentChickenTMP = currentChickenText.GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        DataManager.Instance.LoadGameData();

        for (int i = 0; i < 5; i++) buttonItemSelectManagers[i].isPurchased = DataManager.Instance.gameData.hasHelmets[i];
        for (int i = 0; i < 5; i++) buttonItemSelectManagers[i + 5].isPurchased = DataManager.Instance.gameData.hasClothes[i];
        for (int i = 0; i < 5; i++) buttonItemSelectManagers[i + 10].isPurchased = DataManager.Instance.gameData.hasMotorcycle[i];

        first = true; // 매개변수 전달
        SelectHatContainer(DataManager.Instance.gameData.equippedGoods[0]);
        SelectClothesContainer(DataManager.Instance.gameData.equippedGoods[1]);
        SelectMotorcycleContainer(DataManager.Instance.gameData.equippedGoods[2]);
        first = false;
        _selected = false;
        for (int i = 0; i < 15; i++) buttonItemSelectManagers[i].ItsNotMe();
        currentChickenTMP.text = $"x {DataManager.Instance.gameData.chickens}";

        starCycle = 120;
    }

    void Update(){
        if (haveHat && haveClothes && haveMotorcycle){
            previewText.SetActive(false);
        } else {
            previewText.SetActive(true);
        }

        if (!_selected){
            pbm.DeactivateButton();
            pbm.ChangePriceText(-1);
            pbm.ChangeTextColor("gray");
        }

        starCycle--;
        if (starCycle <= 0){
            starCycle = Random.Range(67, 333);
            GameObject star = Instantiate(starPrefab);
            star.transform.localPosition = new Vector3(Random.Range(-3.1f, 3.1f), Random.Range(5.3f, 6.5f), 1);
            star.transform.localScale = new Vector3(star.transform.localScale.x * (2 * Random.Range(1, 3) - 3), star.transform.localScale.y, 1);
        }
    }

    public void SelectHatContainer(int itemCode = 0){
        _selected = !first;
        if (selectedHatCode != itemCode || lastSelectedClassifyCode != 0) ChangeHatSelect(itemCode);
        selectedHatCode = itemCode;
    }

    public void SelectClothesContainer(int itemCode = 0){
        _selected = !first;
        if (selectedClothesCode != itemCode || lastSelectedClassifyCode != 1) ChangeClothesSelect(itemCode);
        selectedClothesCode = itemCode;
    }

    public void SelectMotorcycleContainer(int itemCode = 0){
        _selected = !first;
        if (selectedMotorcycleCode != itemCode || lastSelectedClassifyCode != 2) ChangeMotorcycleSelect(itemCode);
        selectedMotorcycleCode = itemCode;
    }

    private void ChangeHatSelect(int itemCode){
        ChangePlayerItem(itemCode, 0);
        selectedItemIsPurchased = DataManager.Instance.gameData.hasHelmets[itemCode];
        for (int i = 0; i < 5; i++) buttonItemSelectManagers[i].Unselect();
        for (int i = 0; i < 15; i++) buttonItemSelectManagers[i].ItsNotMe();
        buttonItemSelectManagers[itemCode].Select();
    }

    private void ChangeClothesSelect(int itemCode){
        ChangePlayerItem(itemCode, 1);
        selectedItemIsPurchased = DataManager.Instance.gameData.hasClothes[itemCode];
        for (int i = 0; i < 5; i++) buttonItemSelectManagers[i + 5].Unselect();
        for (int i = 0; i < 15; i++) buttonItemSelectManagers[i].ItsNotMe();
        buttonItemSelectManagers[itemCode + 5].Select();
    }

    private void ChangeMotorcycleSelect(int itemCode){
        ChangePlayerItem(itemCode, 2);
        selectedItemIsPurchased = DataManager.Instance.gameData.hasMotorcycle[itemCode];
        for (int i = 0; i < 5; i++) buttonItemSelectManagers[i + 10].Unselect();
        for (int i = 0; i < 15; i++) buttonItemSelectManagers[i].ItsNotMe();
        buttonItemSelectManagers[itemCode + 10].Select();
    }


    public void ChangePlayerItem(int itemCode, int classifyCode){
        // Purchase Button Controls
        if (classifyCode == 0){
            if (_selected) { // 사용자가 직접 클릭함
                pbm.ChangePriceText(hatPrice[itemCode]);
                pam.hatCode = itemCode;
                pam.LoadPlayerSprite(true, false);
                lastSelectedClassifyCode = 0;
            }
            if (DataManager.Instance.gameData.hasHelmets[itemCode]){
                selectedItemIsPurchased = true;
                haveHat = true;
                pbm.DeactivateButton();
                pbm.ChangeTextColor("green"); // 이미 구매 함
                DataManager.Instance.gameData.equippedGoods[classifyCode] = itemCode; // 장착
                DataManager.Instance.SaveGameData();
                pam.needUpdate = true;
            } else {
                haveHat = false;
                selectedItemIsPurchased = false;
                if (hatPrice[itemCode] <= DataManager.Instance.gameData.chickens){
                    pbm.ChangeTextColor("white");
                    pbm.ActivateButton();
                } else {
                    pbm.ChangeTextColor("red");
                    pbm.DeactivateButton();
                }
            }
        } else if (classifyCode == 1){
            if (_selected){ // 사용자가 직접 클릭함
                pbm.ChangePriceText(clothesPrice[itemCode]);
                pam.clothesCode = itemCode;
                pam.LoadPlayerSprite(false, true);
                lastSelectedClassifyCode = 1;
            }
            if (DataManager.Instance.gameData.hasClothes[itemCode]){
                selectedItemIsPurchased = true;
                haveClothes = true;
                pbm.DeactivateButton();
                pbm.ChangeTextColor("green"); // 이미 구매 함
                DataManager.Instance.gameData.equippedGoods[classifyCode] = itemCode; // 장착
                DataManager.Instance.SaveGameData();
                pam.needUpdate = true;
            } else {
                haveClothes = false;
                selectedItemIsPurchased = false;
                if (clothesPrice[itemCode] <= DataManager.Instance.gameData.chickens){
                    pbm.ChangeTextColor("white");
                    pbm.ActivateButton();
                } else {
                    pbm.ChangeTextColor("red");
                    pbm.DeactivateButton();
                }
            }
        } else { // 오토바이
            if (_selected){ // 사용자가 직접 클릭함
                pbm.ChangePriceText(motorcyclePrice[itemCode]);
                bsm.LoadBikeSprite(false, itemCode);
                lastSelectedClassifyCode = 2;
            }
            if (DataManager.Instance.gameData.hasMotorcycle[itemCode]){
                selectedItemIsPurchased = true;
                haveMotorcycle = true;
                pbm.DeactivateButton();
                pbm.ChangeTextColor("green"); // 이미 구매 함
                DataManager.Instance.gameData.equippedGoods[classifyCode] = itemCode; // 장착
                DataManager.Instance.SaveGameData();
            } else {
                haveMotorcycle = false;
                selectedItemIsPurchased = false;
                if (motorcyclePrice[itemCode] <= DataManager.Instance.gameData.chickens){
                    pbm.ChangeTextColor("white");
                    pbm.ActivateButton();
                } else {
                    pbm.ChangeTextColor("red");
                    pbm.DeactivateButton();
                }
            }
        }
    }


    public void OnClickPurchaseButton(){
        if (lastSelectedClassifyCode == 0){
            DataManager.Instance.gameData.chickens -= hatPrice[selectedHatCode];
            currentChickenTMP.text = $"x {DataManager.Instance.gameData.chickens}";
            haveHat = true;
            selectedItemIsPurchased = true;
            pbm.ChangeTextColor("green");
            pbm.DeactivateButton();
            buttonItemSelectManagers[lastSelectedClassifyCode * 5 + selectedHatCode].isPurchased = true;
            buttonItemSelectManagers[lastSelectedClassifyCode * 5 + selectedHatCode].Select();
            DataManager.Instance.gameData.equippedGoods[0] = selectedHatCode;
            DataManager.Instance.gameData.hasHelmets[selectedHatCode] = true;
            DataManager.Instance.SaveGameData();
            pam.needUpdate = true;
        } else if (lastSelectedClassifyCode == 1){
            DataManager.Instance.gameData.chickens -= clothesPrice[selectedClothesCode];
            currentChickenTMP.text = $"x {DataManager.Instance.gameData.chickens}";
            haveClothes = true;
            selectedItemIsPurchased = true;
            pbm.ChangeTextColor("green");
            pbm.DeactivateButton();
            buttonItemSelectManagers[lastSelectedClassifyCode * 5 + selectedClothesCode].isPurchased = true;
            buttonItemSelectManagers[lastSelectedClassifyCode * 5 + selectedClothesCode].Select();
            DataManager.Instance.gameData.equippedGoods[1] = selectedClothesCode;
            DataManager.Instance.gameData.hasClothes[selectedClothesCode] = true;
            DataManager.Instance.SaveGameData();
            pam.needUpdate = true;
        } else {
            DataManager.Instance.gameData.chickens -= motorcyclePrice[selectedMotorcycleCode];
            currentChickenTMP.text = $"x {DataManager.Instance.gameData.chickens}";
            haveMotorcycle = true;
            selectedItemIsPurchased = true;
            pbm.ChangeTextColor("green");
            pbm.DeactivateButton();
            buttonItemSelectManagers[lastSelectedClassifyCode * 5 + selectedMotorcycleCode].isPurchased = true;
            buttonItemSelectManagers[lastSelectedClassifyCode * 5 + selectedMotorcycleCode].Select();
            DataManager.Instance.gameData.equippedGoods[2] = selectedMotorcycleCode;
            DataManager.Instance.gameData.hasMotorcycle[selectedMotorcycleCode] = true;
            DataManager.Instance.SaveGameData();
        }
    }

    public void OnClickResetButton(){
        DataManager.Instance.ResetGameData();
        Debug.Log("[ShopManager] Reset Game Data");
        SceneManager.LoadScene("ShopScene");
    }

}
