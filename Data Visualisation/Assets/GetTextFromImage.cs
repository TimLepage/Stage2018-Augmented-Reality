using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class GetTextFromImage : MonoBehaviour {

    private string ocrPostURL = "http://api.ocr.space/parse/image";//URL of OCR API post call
    private string imageText = "";
    private string imagePath = "";
    private float[,] array;

    // Use this for initialization
    void Start()
    {//when the parent prefab is instantiated it starts

        StartCoroutine(GetText());//launch coroutine with all the functionalities
    }

    IEnumerator GetText()
    {
        this.imagePath = "C:\\Users\\timle\\Pictures\\test_image5.png";
        string encodedImage = this.encodeImageBase64(imagePath);//encode the screenshot in Base64
        yield return StartCoroutine(getTextFromImage(encodedImage));
        this.array = parseTab(imageText);
    }

    /**
     * Function to parse the xml into an array of float
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
            if (float.TryParse(word, out n)) //if this word is a number
                numbers[i] = n; 
            i++;
        }

        float[,] res = new float[2, i];
        for(int j = 0; j < i/2; j++)
        {
            res[0, j] = numbers[j];
            Debug.Log(res[0,j].ToString());
        }
        for (int j = i/2; j < i; j++)
        {
            res[1, j-i/2] = numbers[j];
            Debug.Log(res[1, j-i/2].ToString());
        }
        return res;
    }

    // Update is called once per frame
    void Update () {
		
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
}
