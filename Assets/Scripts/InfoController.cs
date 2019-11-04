using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * Description: Manages url linking for on-screen text
 * Details: 
 * LoadMainMenu: Returns to main menu
 * Visit (Destination): Opens specified url
 */
public class InfoController : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(DataController.MAIN_MENU_SCENE);
    }

    public void VisitDinvStudioLink()
    {
        Application.OpenURL("https://assetstore.unity.com/publishers/26983");
    }
    public void VisitViktorLink()
    {
        Application.OpenURL("https://v-ktor.itch.io");
    }
    public void VisitSindwillerLink()
    {
        Application.OpenURL("https://opengameart.org/users/sindwiller");
    }
    public void VisitCraftPixLink()
    {
        Application.OpenURL("https://craftpix.net");
    }
    public void VisitAlucardLink()
    {
        Application.OpenURL("https://opengameart.org/users/alucard");
    }
    public void VisitLukeRustltdLink()
    {
        Application.OpenURL("https://opengameart.org/users/lukerustltd");
    }
    public void VisitBuchLink()
    {
        Application.OpenURL("https://opengameart.org/users/buch");
    }
    public void VisitJesusLastraLink()
    {
        Application.OpenURL("https://opengameart.org/users/jalastram");
    }
    public void VisitThorChristopherArislandLink()
    {
        Application.OpenURL("https://opengameart.org/users/tcarisland");
    }
    public void VisitDanSchullerLink()
    {
        Application.OpenURL("http://howtomakeanrpg.com/a/pages/about.html");
    }
    public void VisitUnityTechnologiesLink()
    {
        Application.OpenURL("https://unity.com");
    }
}