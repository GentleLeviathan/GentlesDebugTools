using UnityEngine;
using UnityEngine.Video;
using System.Collections.Generic;
using System.Collections;
using System.Web;
using System.Net;
using System;
using YoutubeExtractor;
using System.Linq;
using UnityEngine.Networking;

namespace GentlesDebugTools
{
    class YouTubePlayer : MonoBehaviour, CustomMFDPage
    {
        private DebugMFD mfd;

        public Dictionary<DebugMFDInfoTexts, string> InfoTexts;
        public Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons;
        private RenderTexture rT;
        private DebugMFDPage _thisPage;

        private AudioSource audioSource;
        private VideoPlayer videoPlayer;
        private string VideoURL = "https://www.youtube.com/watch?v=SkBQ95gN0fo";

        private AudioClip audio;
        private string mostRecentVideoPath;

        public string PageButtonName => "YT Player";
        public DebugMFDButton homeButton { get => new DebugMFDButton(DebugMFDButtons.TopMiddle, "HOME", mfd.GoHome); }
        public DebugMFDButton thisPageButton { get => new DebugMFDButton(DebugMFDButtons.TopRight, PageButtonName, this.SwitchTo); }

        public string PageName => "";
        public string ModName => "";
        public DebugMFDPage thisPage { get => new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, LostFocus); set => new DebugMFDPage(_thisPage.PageName, _thisPage.ModName, _thisPage.PageButtons, _thisPage.InfoTexts, LostFocus); }

        private void Awake()
        {
            mfd = DebugMFD.instance;

            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.url = "";
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            rT = new RenderTexture(640, 480, 16);

            PageButtons = new Dictionary<DebugMFDButtons, DebugMFDButton>(11);
            InfoTexts = new Dictionary<DebugMFDInfoTexts, string>(10);
            _thisPage = thisPage;

            InitButtons();
            InitPage();
        }

        private void UpdateDisplay()
        {
            UpdateThisPage();
        }

        public void Play()
        {
            if(videoPlayer.url != "" && audioSource.clip != null)
            {
                videoPlayer.Play();
                if (videoPlayer.isPaused)
                {
                    audioSource.UnPause();
                }
                else
                {
                    audioSource.Play();
                }
            }
        }

        public void Pause()
        {
            videoPlayer.Pause();
            audioSource.Pause();
        }

        public void InitButtons()
        {
            //Initialization
            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                PageButtons[item] = new DebugMFDButton(item, "");
            }

            //Make the "go home" button.
            PageButtons[DebugMFDButtons.TopMiddle] = homeButton;

            //Play/Pause buttons
            PageButtons[DebugMFDButtons.TopLeft] = new DebugMFDButton(DebugMFDButtons.TopLeft, "Play", this.Play);
            PageButtons[DebugMFDButtons.TopRight] = new DebugMFDButton(DebugMFDButtons.TopRight, "Pause", this.Pause);
        }

        public void InitTexts()
        {
            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                //Empty because we don't want to see anything on the page that would obstruct the video
                InfoTexts[item] = "";
            }
        }

        public void InitPage()
        {
            if (mfd != null)
            {
                mfd.home.ReplaceHomePageButton(thisPageButton);
                //If we are already the active page OR the active page is currently null
                if (mfd.activePage.Equals(this._thisPage) || !mfd.activePage.Equals(null))
                {
                    mfd.SetPage(_thisPage);
                }
            }

            StartCoroutine(InitYTExtractorAndGetLinks());
        }

        private IEnumerator InitYTExtractorAndGetLinks()
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(VideoURL);
            VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 480);

            if (video.RequiresDecryption) { DownloadUrlResolver.DecryptDownloadUrl(video); }

            VideoDownloader downloader = new VideoDownloader(video, "Videos/0.mp4");
            downloader.Execute();

            while (downloader.BytesToDownload != null)
            {
                yield return new WaitForEndOfFrame();
            }

            VideoInfo audio = videoInfos.Where(info => info.CanExtractAudio).OrderByDescending(info => info.AudioBitrate).First();

            if (audio.RequiresDecryption) { DownloadUrlResolver.DecryptDownloadUrl(audio); }

            AudioDownloader aDownloader = new AudioDownloader(audio, "Videos/AudioTracks/0" + audio.AudioExtension);
            aDownloader.Execute();

            while (aDownloader.BytesToDownload != null)
            {
                yield return new WaitForEndOfFrame();
            }
            mostRecentVideoPath = "Videos/0.mp4";

            StartCoroutine(GetExtractedVideoAndAudio(audio.AudioExtension));
        }

        private IEnumerator GetExtractedVideoAndAudio(string audioExtension)
        {

            //Video
            videoPlayer.url = mostRecentVideoPath;
            videoPlayer.targetTexture = rT;
            mfd.ScreenBackground.SetTexture("_MainTex", rT);

            //Audio
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("Videos/AudioTracks/0" + audioExtension, UnityEngine.AudioType.MPEG))
            {
                yield return www.SendWebRequest();

                if (www.isHttpError)
                {
                    yield return null;
                }
                else
                {
                    audio = DownloadHandlerAudioClip.GetContent(www);
                    audioSource.clip = audio;
                }
            }
        }

        private void UpdateThisPage()
        {
            //For updating the page itself, and the page displayed on the MFD.
            thisPage.Update(PageButtons, InfoTexts);
            if (mfd.activePage.Equals(this._thisPage) || !mfd.activePage.Equals(null))
            {
                mfd.SetPage(_thisPage);
            }
        }

        //Switch to this page!
        public void SwitchTo()
        {
            if (mfd != null && !this.thisPage.Equals(new DebugMFDPage()))
            {
                mfd.SetPage(_thisPage);
            }

            if(videoPlayer.url != "" && audioSource.clip != null)
            {
                Play();
            }
        }

        public void LostFocus()
        {
            Pause();
        }

        //Dumb workaround for a System.Web assembly FileNotFoundException.
        private void Irrelevant()
        {
            WebUtility.UrlDecode("");
            HttpUtility.UrlDecode("");
        }
    }
}

