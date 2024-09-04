using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject shopWindow,hairScroll,pantsScroll,glassesScroll,shirtsScroll,shoesScroll;
    RectTransform shopWindowRectTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowShopWindow()
    {
        
        Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        TMP_Text buttonText = clickedButton.GetComponentInChildren<TMP_Text>();
        
        if ( buttonText.text == "hair")
        { 
            CloseAllScrolls();
            shopWindow.SetActive(true);
            SetShopWindowToRight();
            hairScroll.SetActive(true);
        }
        else if ( buttonText.text == "pants")
        {   
            CloseAllScrolls();
            shopWindow.SetActive(true);
            SetShopWindowToRight();
            pantsScroll.SetActive(true);
        }
        else if ( buttonText.text == "glasses")
        { 
            CloseAllScrolls();
            shopWindow.SetActive(true);
            SetShopWindowToLeft();
            glassesScroll.SetActive(true);
        }
        else if ( buttonText.text == "shirts")
        { 
            CloseAllScrolls();
            shopWindow.SetActive(true);
            SetShopWindowToLeft();
            shirtsScroll.SetActive(true);
        }
        else if ( buttonText.text == "shoes")
        {
            CloseAllScrolls();
            shopWindow.SetActive(true);
            SetShopWindowToLeft();
            shoesScroll.SetActive(true);
        }
    }
    public void CloseAllScrolls(){
        shoesScroll.SetActive(false);
        hairScroll.SetActive(false);
        shirtsScroll.SetActive(false);
        pantsScroll.SetActive(false);
        glassesScroll.SetActive(false);
    }

    public void SetShopWindowToLeft(){
        shopWindowRectTransform =shopWindow.GetComponent<RectTransform>();
        shopWindowRectTransform.offsetMax = new Vector2(-1294f, -73f); 
        shopWindowRectTransform.offsetMin = new Vector2(24f, 427f);
    }

    public void SetShopWindowToRight(){
        shopWindowRectTransform =shopWindow.GetComponent<RectTransform>();
        shopWindowRectTransform.offsetMax = new Vector2(15.15f, -73.2139f); 
        shopWindowRectTransform.offsetMin = new Vector2(1303.15f, 427.21f);
    }
}
