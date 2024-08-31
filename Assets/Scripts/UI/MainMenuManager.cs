using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    
    public Menu[] menus;
    protected readonly Dictionary<string, Menu> MenuByName = new Dictionary<string, Menu>();
    
    private Menu activeMenu;
    public bool isActiveMenu;

    protected void Awake()
    {
        Instance = this;
        foreach (var menu in menus)
        {
            MenuByName.Add(menu.menuName, menu);
        }
    }

    public void OpenMenu(string menuName)
    {
        foreach (Menu menu in menus)
        {
            if(menu.menuName == menuName)
            {
                menu.gameObject.SetActive(true);
            }
            else
            {
                menu.gameObject.SetActive(false);
            }
        }

        isActiveMenu = MenuByName.TryGetValue(menuName, out activeMenu);
    }
    
    public void OpenMenuSeparate(string menuName)
    {
        MenuByName[menuName].gameObject.SetActive(true);
    }
    
    public void CloseMenuSeparate(string menuName)
    {
        MenuByName[menuName].gameObject.SetActive(false);
    }

    public void CloseActiveMenu()
    {
        activeMenu.gameObject.SetActive(false);
        activeMenu = null;
        isActiveMenu = false;
    }
    
    public void CloseMenus()
    {
        OpenMenu("");
    }
    
    /***
     * Toggles a menu's open state, while also closing all other menus that are open.
     * Returns the new open state of the menu.
     */
    public bool ToggleMenu(string menuName, params string[] ignore)
    {
        bool returnValue = false;
        foreach (var menu in menus)
        {
            if (menu.menuName == menuName)
            {
                menu.gameObject.SetActive(!menu.gameObject.activeSelf);
                returnValue = menu.gameObject.activeSelf;
                isActiveMenu = returnValue;
                activeMenu = returnValue ? menu : null;
            }
            else if (!ignore.Contains(menu.menuName))
            {
                menu.gameObject.SetActive(false);
            }
        }
        return returnValue;
    }

    public bool ToggleMenuSeparate(string menuName)
    {
        bool returnValue = false;
        foreach (var menu in menus)
        {
            if (menu.menuName == menuName)
            {
                menu.gameObject.SetActive(!menu.gameObject.activeSelf);
                returnValue = menu.gameObject.activeSelf;
                break;
            }
        }
        return returnValue;
    }
}
