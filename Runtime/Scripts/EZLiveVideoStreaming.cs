using RenderHeads.Media.AVProVideo;
using Evereal.YoutubeDLPlayer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Ezphera.MultiVideoPlayer
{
    [RequireComponent(typeof(YTDLParser))]
    public class EZLiveVideoStreaming : MonoBehaviour
    {
        [Header("Configuración")]

        [Tooltip("Poner la url que se necesita para el \"en vivo\", recuerde que si no es un live streaming no saldrá video...")]
        public string url = "https://www.twitch.tv/tfue";

        [Tooltip("Aquí es importante saber si queremos el video en alta calidad 'Best' o en alto rendimiento - baja calidad 'worst' ejemplo: \"--format worst[protocol=m3u8]\"")]
        public string options = "--format worst[protocol=m3u8]";

        [Tooltip("Este \"Media Player\" debe estar en la escena y referenciado aquí. ¡De no ser así puede tener un error en el proceso!")]
        public MediaPlayer mediaPlayer;

        /// <summary>
        /// Este componente es el encargado de parsear la url a un m3u8 para poderla reproducir desde unity, necesitamos que este objeto tenga ese componente
        /// </summary>
        YTDLParser ytdlParser;

        /// <summary>
        /// Esta clase le colocamos aqui para evitar estar creando una cada que hace una consulta, en su lugar se reemplaza la información que hay en esta unica clase creada.
        /// </summary>
        private MediaInfo mediaInfo;

        [Tooltip("¡Habilite esto si necesita que el video inicie en el método start de unity!")]
        public bool _autoLoad;

        /// <summary>
        /// Esto es un callback que es llamado cuando se ha terminado el proceso de traer el enlace del streaming.
        /// Se llamara en ambos casos, 1 cuando trae un enlace correctamente y 2 cuando sucede algún error
        /// </summary>
        public Action<string, bool> OnParceUrl;

        private void Awake()
        {
            ytdlParser = GetComponent<YTDLParser>();//Aquí traemos el parser para no tener que arrastrarlo en el inspector
            ytdlParser.SetOptions(options);//Aqui le mandamos las opciones de cómo queremos el formato del video
        }
        private void Start()
        {
            if (_autoLoad)//Si esta variable es true comenzamos el proceso de parsear el enlace del video streaming
            {
                StartCoroutine(ytdlParser.PrepareAndParse(url));
            }
        }
        private void OnEnable()
        {
            ytdlParser.parseCompleted += ParseCompleted;//Usaremos el callback cuando se completa el proceso del parser
            ytdlParser.errorReceived += ErrorReceived;//Usamos este callback para cuando detectamos un error en el proceso del parser
        }
        private void OnDisable()
        {
            ytdlParser.parseCompleted -= ParseCompleted;//Deshabilitamos el callback para evitar que de pronto se llame más de una vez cada que iniciamos el objeto
            ytdlParser.errorReceived -= ErrorReceived;//Deshabilitamos el callback para evitar que de pronto se llame más de una vez cada que iniciamos el objeto
        }

        /// <summary>
        /// Este es el metodo llamado con el callback para saber la url retornada y usarla en el reproductor
        /// </summary>
        /// <param name="info">Este trae la información del resultado de la consulta</param>
        private void ParseCompleted(MediaInfo info)
        {
            mediaInfo = info;//llenamos la clase creada anteriormente con los datos nuevos retornados
            if (mediaPlayer)// Si el mediaplayer no es nullo seguimos con la lógica
            {
                mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.AbsolutePathOrURL, mediaInfo.url);
                
                if (!string.IsNullOrEmpty(mediaInfo.url))//Aqui significa que si trajo información del video
                {
                    OnParceUrl(mediaInfo.url, true);//Llamamos el callback que creamos para utilizarlo a conveniencia en nuestro proyecto
                }
                else //Hubo un error o simplemente el video no era un live streaming
                {
                    OnParceUrl(mediaInfo.url, false);//Llamamos el callback que creamos para utilizarlo a conveniencia en nuestro proyecto
                }
            }
            else //Si el mediaplayer es nulo retornamos con error 
            {
                OnParceUrl(mediaInfo.url, false);//Llamamos el callback que creamos para utilizarlo a conveniencia en nuestro proyecto
            }
        }

        /// <summary>
        /// Cuando el parser encuentra un error en su proceso no continua y arroja este callback que nosotros registramos en OnEnable
        /// </summary>
        /// <param name="error">Este valor es lo que identifica cuál fue el error en el proceso del parser</param>
        private void ErrorReceived(YTDLParser.ErrorEvent error)
        {
            //Si queremos saber cuál es el error lo llamamos así: error.message;
            OnParceUrl(null, false);//Llamamos nuestro callback para que el usuario pueda volver a recargar el video
        }

    }
}