using UnityEngine;
using UnityEngine.SceneManagement;

/**
 * @Copyright 2020 Crowswood Games (Company), Brian Navarro aka PapaKosh (Developer)
 * 
 * Description: Manages url linking for on-screen text under the credits section.
 * 
 * Details:
 * Methods-
 * LoadMainMenu: Returns to main menu
 * VisitDinvStudioLink: Opens specified url
 * VisitViktorLink: Opens specified url
 * VisitSindwillerLink: Opens specified url
 * VisitCraftPixLink: Opens specified url
 * VisitAlucardLink: Opens specified url
 * VisitLukeRustltdLink: Opens specified url
 * VisitMicheleBuchBucelliLink: Opens specified url
 * VisitJesusLastraLink: Opens specified url
 * VisitThorChristopherArislandLink: Opens specified url
 * VisitDanSchullerLink: Opens specified url
 * VisitUnityTechnologiesLink: Opens specified url
 * VisitGreatHeightsAudioLink: Opens specified url
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
    public void VisitMicheleBuchBucelliLink()
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
    public void VisitGreatHeightsAudioLink()
    {
        Application.OpenURL("https://assetstore.unity.com/publishers/28429");
    }
}