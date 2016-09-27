using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CatUI : MonoBehaviour 
{
    private float _Progress = 0;
    private string _Name = "";
    private string _Desc1 = "";
    private string _Desc2 = "";

    public void SetName(string name)
    {
        Text text = transform.FindChild("name").GetComponent<Text>();
        text.text = name;
        _Name = name;
    }

    public void SetDesc1(string desc)
    {
        Text text = transform.FindChild("desc1").GetComponent<Text>();
        text.text = desc;
        _Desc1 = desc;
    }

    public void SetDesc2(string desc)
    {
        Text text = transform.FindChild("desc2").GetComponent<Text>();
        text.text = desc;
        _Desc2 = desc;
    }

    public void SetProgress(float progress)
    {
        Slider slider = transform.FindChild("Slider").GetComponent<Slider>();
        slider.value = progress;
        _Progress = progress;
    }
}
