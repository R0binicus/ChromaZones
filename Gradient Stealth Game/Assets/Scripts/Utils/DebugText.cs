using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class DebugText : MonoBehaviour
{
    public ColourManager colourManager;
    public Text text;
    public Text text2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //text.text = colourManager.colour.ToString();
        text.text = (1f/Time.deltaTime).ToString();
        text2.text = " ";
    }
}
