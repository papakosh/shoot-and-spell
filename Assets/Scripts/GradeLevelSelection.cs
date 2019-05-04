using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GradeLevelSelection : MonoBehaviour
{
    public string[] preschool;
    public string[] kindergarten;
    public string[] first;
    public string[] second;
    public string[] third;
    public string[] fourth;

    public void ChoosePreschool()
    {
        PlayerPrefs.SetString("GradeLevel", "Preschool");
        PlayerPrefs.SetString("TargetWord", preschool[Random.Range(0, preschool.Length)]);
        LoadScene();
    }

    public void ChooseKindergarten()
    {
        PlayerPrefs.SetString("GradeLevel", "Kindergarten");
        PlayerPrefs.SetString("TargetWord", kindergarten[Random.Range(0, kindergarten.Length)]);
        LoadScene();
    }

    public void ChooseFirstGrade()
    {
        PlayerPrefs.SetString("GradeLevel", "First Grade");
        PlayerPrefs.SetString("TargetWord", first[Random.Range(0, first.Length)]);
        LoadScene();
    }

    public void ChooseSecondGrade()
    {
        PlayerPrefs.SetString("GradeLevel", "Second Grade");
        PlayerPrefs.SetString("TargetWord", second[Random.Range(0, second.Length)]);
        LoadScene();
    }

    public void ChooseThirdGrade()
    {
        PlayerPrefs.SetString("GradeLevel", "Third Grade");
        PlayerPrefs.SetString("TargetWord", third[Random.Range(0, third.Length)]);
        LoadScene();
    }
    public void ChooseFourthGrade()
    {
        PlayerPrefs.SetString("GradeLevel", "Fourth Grade");
        PlayerPrefs.SetString("TargetWord", fourth[Random.Range(0, fourth.Length)]);
        LoadScene();
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Game");
    }
}
