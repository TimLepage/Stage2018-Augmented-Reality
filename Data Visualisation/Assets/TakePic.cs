using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Text;
using UnityEngine.UI;

public class TakePic : MonoBehaviour
{

    private string ocrPostURL = "http://api.ocr.space/parse/image";//URL of OCR API post call
    private string imageText = "";
    private string imagePath = "";
    private float[,] array;

    public GameObject axis;
    public LineRenderer lr;
    public TextMesh textMesh;
    public TextMesh keyValues;
    public Button button;
    public int resWidth = 1280;
    public int resHeight = 720;


    public void Onclick()
    {
        axis.SetActive(true); //hides the previous graph
        lr.positionCount = 0;
        button.interactable = false; //prevents the user from spaming the button
        textMesh.text = "Loading...";
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        GetComponent<Camera>().targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        GetComponent<Camera>().Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        GetComponent<Camera>().targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        //System.IO.File.WriteAllBytes("C:\\Users\\timle\\Pictures\\screen.png", bytes);
        System.IO.File.WriteAllBytes(Application.persistentDataPath + "\\screen.png", bytes);
        Debug.Log(string.Format("Took screenshot to: {0}", Application.persistentDataPath + "\\screen.png"));

        StartCoroutine(GetText());//launch coroutine with all the functionalities
    }

    IEnumerator GetText()
    {
        //this.imagePath = "C:\\Users\\timle\\Pictures\\test_image5.png";
        this.imagePath = Application.persistentDataPath + "\\screen.png";
        string encodedImage = this.encodeImageBase64(imagePath);//encode the screenshot in Base64
        yield return StartCoroutine(getTextFromImage(encodedImage));
        this.array = parseTab(imageText);
        textMesh.text = ArrayFloatToString(array);
        CreateGraphic(array);
        button.interactable = true;
    }

    /**
     * Function to parse the json into an array of float
     * 
     * */
    private float[,] parseTab(string _imageText)
    {
        byte[] bytes = Encoding.Default.GetBytes(_imageText);
        string imageTextUTF8 = Encoding.UTF8.GetString(bytes);

        int pos_ini = 0;
        int pos_end = 0;

        ArrayList wordList = new ArrayList();
        float[] numbers = new float[100];

        while (imageTextUTF8.IndexOf("WordText\":\"", pos_end) > -1)
        { //store extracted words in a list

            pos_ini = imageTextUTF8.IndexOf("WordText\":\"", pos_end) + "WordText\":\"".Length;
            pos_end = imageTextUTF8.IndexOf("\",", pos_ini);

            wordList.Add(imageTextUTF8.Substring(pos_ini, pos_end - pos_ini));
        }
        int i = 0;
        foreach (string word in wordList)
        {//look every word

            float n;
            if (float.TryParse(word, out n))//if this word is a number
            {
                numbers[i] = n;
                i++;
            }
        }

        float[,] res = new float[2, i];
        for (int j = 0; j < i / 2; j++)
        {
            res[0, j] = numbers[j];
            Debug.Log(res[0, j].ToString());
        }
        for (int j = i / 2; j < i; j++)
        {
            res[1, j - i / 2] = numbers[j];
            Debug.Log(res[1, j - i / 2].ToString());
        }
        return res;
    }


    /**
	* 
	* Function to encode a PNG image in Base64 and return the result in a String
	* 
	**/
    private string encodeImageBase64(String _imagePath)
    {

        Texture2D texture = LoadPNG(_imagePath);//load PNG image from path

        string base64Img = "";
        base64Img = System.Convert.ToBase64String(texture.EncodeToJPG(20));

        return base64Img;
    }

    /**
	* 
	* Function to load PNG image from path into a Texture2D object which is returned
    * 
	**/
    private static Texture2D LoadPNG(string _imagePath)
    {

        Texture2D texture = null;
        byte[] fileData;

#if UNITY_IOS || UNITY_ANDROID
        if (File.Exists(Application.persistentDataPath + _imagePath))
        {
            fileData = File.ReadAllBytes(Application.persistentDataPath + _imagePath);
            texture = new Texture2D(1, 1);
            texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
#endif

#if UNITY_EDITOR
        if (File.Exists(_imagePath))
        {
            fileData = File.ReadAllBytes(_imagePath);
            texture = new Texture2D(1, 1);
            texture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            Debug.Log("Image loaded to texture");
        }
#endif

        return texture;
    }


    /**
     * Function to call the OCR webservice and return the result
     * */
    IEnumerator getTextFromImage(string _encodedImage)
    {

        WWWForm form = new WWWForm();//call form
        form.AddField("apikey", "36223397b588957");
        form.AddField("isOverlayRequired", "true");
        form.AddField("base64Image", "data:image/png;base64," + _encodedImage);

        WWW www = new WWW(ocrPostURL, form);
        yield return StartCoroutine(WaitForRequest(www));//launch async coroutine and wait until it ends

        imageText = www.text;//save response in global variable
    }

    /**
	* 
	* IEnumerator for wait call response
	* 
	**/
    IEnumerator WaitForRequest(WWW _www)
    {
        yield return _www;
        if (_www.error == null)
        {
            Debug.Log("WWW Ok!: " + _www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + _www.error);
        }
    }

    //Getter for the array
    public float[,] GetArray()
    {
        return this.array;
    }

    /**
     * Function that transforms the float array to a string in order to display it
     * 
     * */
    private string ArrayFloatToString(float[,] array)
    {
        string res = "";
        for (int j = 0; j < array.GetLength(1) / 2; j++)
        {
            for (int i = 0; i < array.GetLength(0); i++)
            {
                res += array[i, j].ToString() + " ";
            }
            res += "\n";
        }

        return res;
    }

    /**
     * Function that generates the graphic associated to the array in parameter
     * 
     * */
    private void CreateGraphic(float[,] array)
    {
        lr.positionCount = array.GetLength(1) / 2;
        for (int j = 0; j < array.GetLength(1) / 2; j++)
        {
            lr.SetPosition(j, new Vector3(array[0, j] / 10, array[1, j] / 10, 0f));
        }
        KeyValues(array);
        axis.SetActive(true);
    }

    /**
     * Functions thats displays the key values of the array
     * 
     * */
    private void KeyValues(float[,] array)
    {
        float min = float.MaxValue;
        float max = 0;
        float minAt = 0;
        float maxAt = 0;
        for (int j = 0; j < array.GetLength(1) / 2; j++)
        {
            if (array[1, j] < min)
            {
                min = array[1, j];
                minAt = array[0, j];
            }
            if (array[1, j] > max)
            {
                max = array[1, j];
                maxAt = array[0, j];
            }
        }
        keyValues.text = "Min: " + min.ToString() + " at " + minAt.ToString() + "\nMax: " + max.ToString() + " at " + maxAt.ToString();
    }
}