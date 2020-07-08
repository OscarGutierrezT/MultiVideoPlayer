using Evereal.YoutubeDLPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EZYTDLParserCustomOptions : MonoBehaviour
{
    /// <summary>
    /// Este componente lo debe tener atachado el objeto al cual atachamos este script, de lo contrario 
    /// se gererara un error por falta de referencia
    /// </summary>
    YTDLParser ytdlParser;

    [Tooltip("Puede ver las opciones disponibles en: https://github.com/ytdl-org/youtube-dl#options")]
    public string options = "--format worst[ext=mp4]";
    private void Awake()
    {
        ytdlParser = GetComponent<YTDLParser>();//Aquí traemos el parser para no tener que arrastrarlo en el inspector
        ytdlParser.SetOptions(options);//Aqui le mandamos las opciones de cómo queremos el formato del video
    }
}
