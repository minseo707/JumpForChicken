using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    // boolean
    public bool isMove;
    public bool isJumping;
    public bool isFalling;
    public bool onGround;
    public bool isFirstJump;
    public bool isIDLE1;
    public bool isCrashing;

    // value
    public int lookAt;


    // Animation Manager 구현에 유용한 변수 선언
    public bool isStop;
    public bool isJumpReady;

    public int aFrame;
    private int animationCode;

    private GameObject SkinSpritesObject;
    private GameObject hatSpriteObject; // hatSprites를 가져오기 위함
    private GameObject clothesSpriteObject; // clothesSprites를 가져오기 위함


    // Sprites
    private SpriteRenderer spriteRenderer;

    public Sprite[] nudeSprites;

    private Sprite[] hatSprites;
    private Sprite[] clothesSprites;

    /// <summary>
    /// 0-1: Standing Right1 -> 0
    /// 2-3: Standing Right2 -> 1
    /// 4-5: Standing Left1  -> 2
    /// 6-7: Standing Left2  -> 3
    /// 8-11: Walking Right  -> 4
    /// 12-15: Walking Left  -> 5
    /// 16-18: Ready to Jump Right -> 6
    /// 19-21: Ready to Jump Left -> 7
    /// 22-23: Jumping Right -> 8
    /// 24-25: Falling Right -> 9
    /// 26-27: Jumping Left -> 10
    /// 28-29: Falling Left -> 11
    /// 30: Crasing Right -> 12
    /// 31: Crasing Left -> 13
    /// </summary>
    
    // 스킨을 적용할 오브젝트
    public GameObject hat;
    private SpriteRenderer hatSpriteRenderer;
    public GameObject clothes;
    private SpriteRenderer clothesSpriteRenderer;
    

    // 임시 스킨 고유번호 (추후 저장 데이터와 연동할 예정)
    public int hatCode = 0;
    public int clothesCode = 0;

    private int lastAnimationCode;
    public bool needUpdate = false;

    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        DataManager.Instance.LoadGameData();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        hatSpriteRenderer = hat.GetComponent<SpriteRenderer>();
        clothesSpriteRenderer = clothes.GetComponent<SpriteRenderer>();
        SkinSpritesObject = transform.Find("SkinSprites").gameObject;
        startPos = transform.position;

        hatCode = DataManager.Instance.gameData.equippedGoods[0];
        clothesCode = DataManager.Instance.gameData.equippedGoods[1];

        // 플레이어 아이템 스프라이트 불러오기
        LoadPlayerSprite();

        StatueReset();
    }

    public void StatueReset(){
        aFrame = 0;
        lookAt = 1;
        isMove = false;
        isJumping = false;
        isFalling = false;
        isCrashing = false;
        transform.position = startPos;
        animationCode = 0;
        SpriteChangeIndex(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (needUpdate){
            SpriteChangeIndex(lastAnimationCode);
        }

        isStop = !isMove && !isJumping && !isFalling && !isCrashing && !isJumpReady;

        if (isStop){
            if (isIDLE1){
                if (lookAt == 1){
                    if (animationCode != 0){
                        animationCode = 0;
                        aFrame = 0;
                        SpriteChangeIndex(0); // default sprite
                    } else if (animationCode == 0 && aFrame == 38){
                        SpriteChangeIndex(1);
                    } else if (animationCode == 0 && aFrame == 77){
                        SpriteChangeIndex(0);
                        aFrame = 0;
                    }

                } else {
                    if (animationCode != 2){
                        animationCode = 2;
                        aFrame = 0;
                        SpriteChangeIndex(4); // default sprite
                    } else if (animationCode == 2 && aFrame == 38){
                        SpriteChangeIndex(5);
                    } else if (animationCode == 2 && aFrame == 77){
                        SpriteChangeIndex(4);
                        aFrame = 0;
                    }
                }
            } else {
                if (lookAt == 1){
                    if (animationCode != 1){
                        animationCode = 1;
                        aFrame = 0;
                        SpriteChangeIndex(2); // default sprite
                    } else if (animationCode == 1 && aFrame == 38){
                        SpriteChangeIndex(3);
                    } else if (animationCode == 1 && aFrame == 77){
                        SpriteChangeIndex(2);
                        aFrame = 0;
                    }

                } else {
                    if (animationCode != 3){
                        animationCode = 3;
                        aFrame = 0;
                        SpriteChangeIndex(6); // default sprite
                    } else if (animationCode == 3 && aFrame == 38){
                        SpriteChangeIndex(7);
                    } else if (animationCode == 3 && aFrame == 77){
                        SpriteChangeIndex(6);
                        aFrame = 0;
                    }
                }
            }
        } else { // !isStop
            if (isMove && !isJumping && !isFalling && !isCrashing){ // 걷는다면
                if (lookAt == 1){
                    if (animationCode == 5){
                        animationCode = 4;
                        if ((0 <= aFrame) && (aFrame <= 9)){
                            SpriteChangeIndex(8);
                        } else if ((10 <= aFrame) && (aFrame <= 19)){
                            SpriteChangeIndex(9);
                        } else if ((20 <= aFrame) && (aFrame <= 29)){
                            SpriteChangeIndex(10);
                        } else if ((30 <= aFrame) && (aFrame <= 39)){
                            SpriteChangeIndex(11);
                        }
                    } else if (animationCode != 4 && animationCode != 5){
                        animationCode = 4;
                        aFrame = 0;
                        SpriteChangeIndex(8); // default sprite
                    } else {
                        if (aFrame == 10){
                            SpriteChangeIndex(9);
                        } else if (aFrame == 20){
                            SpriteChangeIndex(10);
                        } else if (aFrame == 30){
                            SpriteChangeIndex(11);
                        } else if (aFrame >= 40){
                            SpriteChangeIndex(8);
                            aFrame = 0;
                        }
                    }
                } else {
                    if (animationCode == 4){
                        animationCode = 5;
                        if ((0 <= aFrame) && (aFrame <= 9)){
                            SpriteChangeIndex(12);
                        } else if ((10 <= aFrame) && (aFrame <= 19)){
                            SpriteChangeIndex(13);
                        } else if ((20 <= aFrame) && (aFrame <= 29)){
                            SpriteChangeIndex(14);
                        } else if ((30 <= aFrame) && (aFrame <= 39)){
                            SpriteChangeIndex(15);
                        }
                    } else if (animationCode != 5 && animationCode != 4){
                        animationCode = 5;
                        aFrame = 0;
                        SpriteChangeIndex(12); // default sprite
                    } else {
                        if (aFrame == 10){
                            SpriteChangeIndex(13);
                        } else if (aFrame == 20){
                            SpriteChangeIndex(14);
                        } else if (aFrame == 30){
                            SpriteChangeIndex(15);
                        } else if (aFrame >= 40){
                            SpriteChangeIndex(12);
                            aFrame = 0;
                        }
                    }
                }
            }
            
            if (isJumpReady && !isCrashing){
                if (lookAt == 1){
                    if (animationCode != 6){
                        animationCode = 6;
                        aFrame = 0;
                        SpriteChangeIndex(16);; // default sprite
                    } else {
                        if (aFrame == 8){
                            SpriteChangeIndex(17);
                        } else if (aFrame == 16){
                            SpriteChangeIndex(18);
                        }
                    }
                } else { // look at -1
                    if (animationCode != 7){
                        animationCode = 7;
                        aFrame = 0;
                        SpriteChangeIndex(19); // default sprite
                    } else {
                        if (aFrame == 8){
                            SpriteChangeIndex(20);
                        } else if (aFrame == 16){
                            SpriteChangeIndex(21);
                        }
                    }
                }
            }
            
            if (isJumping && !isCrashing){
                if (lookAt == 1){
                    if (animationCode != 8){
                        animationCode = 8;
                        aFrame = 0;
                        SpriteChangeIndex(22);
                    } else {
                        if (aFrame == 16){
                            SpriteChangeIndex(23);
                        }
                    }
                } else {
                    if (animationCode != 10){
                        animationCode = 10;
                        aFrame = 0;
                        SpriteChangeIndex(26);
                    } else {
                        if (aFrame == 16){
                            SpriteChangeIndex(27);
                        }
                    }
                }
            }

            if (isFalling && !isCrashing){
                if (lookAt == 1){
                    if (animationCode != 9){
                        animationCode = 9;
                        aFrame = 0;
                        SpriteChangeIndex(24);
                    } else {
                        if (aFrame == 29){
                            SpriteChangeIndex(25);
                        }
                    }
                } else {
                    if (animationCode != 11){
                        animationCode = 11;
                        aFrame = 0;
                        SpriteChangeIndex(28);
                    } else {
                        if (aFrame == 29){
                            SpriteChangeIndex(29);
                        }
                    }
                }
            }

            if (isCrashing){
                if (lookAt == 1){
                    if (animationCode != 12){
                        animationCode = 12;
                        aFrame = 0;
                        SpriteChangeIndex(30);
                    }
                } else {
                    if (animationCode != 13){
                        animationCode = 13;
                        aFrame = 0;
                        SpriteChangeIndex(31);
                    }
                }
            }
        }

        if (Time.timeScale != 0) aFrame++;
    }

    private void SpriteChangeIndex(int index){
        spriteRenderer.sprite = nudeSprites[index];
        hatSpriteRenderer.sprite = hatSprites[index];
        clothesSpriteRenderer.sprite = clothesSprites[index];
        lastAnimationCode = index;
    }

    public void LoadPlayerSprite(bool hat = true, bool clothes = true){
        if (hat){
            // 모자 불러오기
            switch (hatCode){
                case 0: // hatCode 0: Hat_Default
                    hatSpriteObject = SkinSpritesObject.transform.Find("Hat_Default").gameObject;
                    break;
                case 1: // hatCode 1: Hat_Sunglasses
                    hatSpriteObject = SkinSpritesObject.transform.Find("Hat_Sunglasses").gameObject;
                    break;
                case 2: // hatCode 2: Hat_Fedora
                    hatSpriteObject = SkinSpritesObject.transform.Find("Hat_Fedora").gameObject;
                    break;
                case 3: // hatCode 3: Hat_BlackHelmet
                    hatSpriteObject = SkinSpritesObject.transform.Find("Hat_BlackHelmet").gameObject;
                    break;
                case 4: // hatCode 4: Hat_Crown
                    hatSpriteObject = SkinSpritesObject.transform.Find("Hat_Crown").gameObject;
                    break;
                default:
                    hatSpriteObject = SkinSpritesObject.transform.Find("Hat_Default").gameObject;
                    Debug.LogWarning("[PlayerAnimationManager] No hat sprite found with the code: " + hatCode);
                    break;
            }
        }

        if (clothes){
            // 의상 불러오기
            switch (clothesCode){
                case 0: // clothesCode 0: Clothes_Default
                    clothesSpriteObject = SkinSpritesObject.transform.Find("Clothes_Default").gameObject;
                    break;
                case 1: // clothesCode 1: Clothes_LeatherJacket
                    clothesSpriteObject = SkinSpritesObject.transform.Find("Clothes_LeatherJacket").gameObject;
                    break;
                case 2: // clothesCode 2: Clothes_Suit
                    clothesSpriteObject = SkinSpritesObject.transform.Find("Clothes_Suit").gameObject;
                    break;
                case 3: // clothesCode 3: Clothes_Black
                    clothesSpriteObject = SkinSpritesObject.transform.Find("Clothes_Black").gameObject;
                    break;
                case 4: // clothesCode 4: Clothes_Mangto
                    clothesSpriteObject = SkinSpritesObject.transform.Find("Clothes_Mangto").gameObject;
                    break;
                default:
                    clothesSpriteObject = SkinSpritesObject.transform.Find("Clothes_Default").gameObject;
                    Debug.LogWarning("[PlayerAnimationManager] No clothes sprite found with the code: " + clothesCode);
                    break;
            }
        }

        hatSprites = hatSpriteObject.GetComponent<SkinSpritesStore>().sprites;
        clothesSprites = clothesSpriteObject.GetComponent<SkinSpritesStore>().sprites;
    }
}
