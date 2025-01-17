using UnityEngine;


public class ColorManager : MonoBehaviour
{

    public enum ColorMode
    {
        Solid, Random
    }
    public ColorMode colorMode;
    public Color solidColor;


    public GameObject GO_floor;
    public GameObject GO_walls;

    Color color1;
    Color color2;




    void Start()
    {
        SetColor();
    }


    void SetColor()
    {

        if (colorMode == ColorMode.Random)
        {
            // Generate Random Color 
            float randomH = Random.Range(0, 1f);
            color1 = Color.HSVToRGB(randomH, 0.7f, 0.7f);

            // Generate Second Color 
            float h, s, v;
            Color.RGBToHSV(color1, out h, out s, out v);
            color2 = Color.HSVToRGB(h + 0.02f, s + 0.1f, v + 0.1f);
        }
        else if (colorMode == ColorMode.Solid)
        {
            color1 = solidColor;
            color2 = solidColor;
        }


        // Set color to floor material  
        GO_floor.GetComponent<Renderer>().material.SetColor("_ColorBottom", color1);
        GO_floor.GetComponent<Renderer>().material.SetColor("_ColorTop", color2);

        // Set color to walls material 
        for (int i = 0; i < GO_walls.transform.childCount; i++)
        {
            GO_walls.transform.GetChild(i).GetComponent<Renderer>().material.SetColor("_ColorBottom", color2);
            GO_walls.transform.GetChild(i).GetComponent<Renderer>().material.SetColor("_ColorTop", color1);
        }


    }
}
