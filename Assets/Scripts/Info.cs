using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Info : MonoBehaviour
{
    public void CallHome()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void VisitDinvStudio()
    {
        Application.OpenURL("https://assetstore.unity.com/publishers/26983");
    }
    public void VisitViktor()
    {
        Application.OpenURL("https://v-ktor.itch.io");
    }
    public void VisitSindwiller()
    {
        Application.OpenURL("https://opengameart.org/users/sindwiller");
    }
    public void VisitCraftPix()
    {
        Application.OpenURL("https://craftpix.net");
    }
    public void VisitAlucard()
    {
        Application.OpenURL("https://opengameart.org/users/alucard");
    }
    public void VisitLukeRustltd()
    {
        Application.OpenURL("https://opengameart.org/users/lukerustltd");
    }
    public void VisitBuch()
    {
        Application.OpenURL("https://opengameart.org/users/buch");
    }
    public void VisitOgreBane()
    {
        Application.OpenURL("https://opengameart.org/users/ogrebane");
    }
    public void VisitQuitschie()
    {
        Application.OpenURL("https://opengameart.org/users/quitschie");
    }
    public void VisitRemaxim()
    {
        Application.OpenURL("https://opengameart.org/users/remaxim");
    }
    public void VisitJesusLastra()
    {
        Application.OpenURL("https://opengameart.org/users/jalastram");
    }
    public void VisitThorChristopherArisland()
    {
        Application.OpenURL("https://opengameart.org/users/tcarisland");
    }
    public void VisitDanSchuller()
    {
        Application.OpenURL("http://howtomakeanrpg.com/a/pages/about.html");
    }
    public void VisitUnityTechnologies()
    {
        Application.OpenURL("https://unity.com");
    }
}