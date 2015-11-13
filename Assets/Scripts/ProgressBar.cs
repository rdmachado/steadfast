using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {

    public enum ValueType
    {
        Percentage,
        Float
    }

    public float Min = 0, Max = 100, Value = 0;
    public int DecimalPlaces = 1;
    public ValueType valueType = ValueType.Percentage;
    public Color color = Color.white;
    public bool HideWhenValueEqualsMin = true;


    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (HideWhenValueEqualsMin && Value == Min)
            GetComponent<Text>().text = "";
        else
        {
            string format = "";
            if (DecimalPlaces > 0)
            {
                format = "N" + DecimalPlaces;
            }
            else
            {
                format = "0";
            }
            if (valueType == ValueType.Percentage)
            {
                format += " \\%";
            }
            GetComponent<Text>().color = color;
            GetComponent<Text>().text = Value.ToString(format);
        }
    }

    public void Clear()
    {
        GetComponent<Text>().text = "";
    }
}
