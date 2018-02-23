using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace GoodApp.Helper
{
    public class MediaPlayerHelper
    {
        private static MediaPlayerHelper instance;

        MediaPlayer mediaPlayer;

        public bool IsPlaying { get; set; }

        private MediaPlayerHelper()
        {
        }

        public static MediaPlayerHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MediaPlayerHelper();
                }
                return instance;
            }
        }

        public void StartAudio(string uri)
        {
            mediaPlayer = new MediaPlayer
            {
                Source = MediaSource.CreateFromUri(new Uri(uri, UriKind.RelativeOrAbsolute))
            };
            mediaPlayer.Play();

            IsPlaying = true;
        }

        public void Pause()
        {
            mediaPlayer.Pause();

            IsPlaying = false;
        }

        public void Resume()
        {
            mediaPlayer.Play();

            IsPlaying = true;
        }

        public void Stop()
        {
            mediaPlayer.Dispose();

            IsPlaying = false;
        }


    }
}
