using UnityEngine;
using UnityEngine.Video;
using System;
using System.Diagnostics;
using System.Collections;

namespace GentlesDebugTools
{
    class YoutubeDLPlayer : CustomMFDPage
    {
        private RenderTexture rT;

        private AudioSource audioSource;
        private VideoPlayer videoPlayer;
        private string VideoURL = "https://www.youtube.com/watch?v=SkBQ95gN0fo";

        public override void Awake()
        {
            PageName = "";
            ModName = "";

            base.Awake();
        }

        public override void Start()
        {
            PageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(mfd.debugtoolsModDirectory + "Backgrounds/loading.png");
            PageButtonName = "Youtube Player";

            base.Start();

            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.url = "";
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            videoPlayer.SetTargetAudioSource(0, audioSource);

            rT = new RenderTexture(640, 480, 16);

            DownloadManual();
        }

        public override void InitButtons()
        {
            base.InitButtons();

            //Play/Pause buttons
            PageButtons[DebugMFDButtons.TopLeft] = new DebugMFDButton(DebugMFDButtons.TopLeft, "Play", this.Play);
            PageButtons[DebugMFDButtons.TopRight] = new DebugMFDButton(DebugMFDButtons.TopRight, "Pause", this.Pause);
        }

        public override void InitTexts()
        {
            base.InitTexts();

            foreach (DebugMFDInfoTexts item in Enum.GetValues(typeof(DebugMFDInfoTexts)))
            {
                //Empty because we don't want to see anything on the page that would obstruct the video
                InfoTexts[item] = "";
            }
        }

        public override void InitPage()
        {
            base.InitPage();
        }

        public override void UpdateThisPage()
        {
            base.UpdateThisPage();
        }

        public override void SwitchTo()
        {
            base.SwitchTo();
        }

        public override void LostFocus()
        {
            base.LostFocus();
            Pause();
        }

        public void Play()
        {
            if (videoPlayer.url != "")
            {
                mfd.ScreenBackground.SetTexture("_MainTex", rT);
                videoPlayer.Play();
            }
        }

        public void Pause()
        {
            if (videoPlayer.isPlaying)
            {
                videoPlayer.Pause();
            }
        }

        public void DownloadManual()
        {
            string arguments = VideoURL + @" -o """ + mfd.debugtoolsModDirectory + @"Videos\0.mp4""" + @" -f ""bestvideo[ext=mp4]""";
            //string arguments = VideoURL + " -o '" + mfd.debugtoolsModDirectory + @"Videos\" + " -f bestvideo[ext=mp4]";

            ProcessStartInfo info = new ProcessStartInfo()
            {
                FileName = mfd.debugtoolsModDirectory + @"youtube-dl\youtube-dl.exe",
                WorkingDirectory = mfd.debugtoolsModDirectory + @"youtube-dl\",
                Arguments = arguments,
                UseShellExecute = true
            };
            Process downloader = Process.Start(info);
            downloader.OutputDataReceived += ManualDownloaderDataReceived;
            downloader.Exited += FinishedManualDownload;
            StartCoroutine(ManualDownloadChecker(downloader));
        }

        private IEnumerator ManualDownloadChecker(Process pro)
        {
            while (!pro.HasExited) { yield return new WaitForEndOfFrame(); }
            FinishedManualDownload();
        }

        private void FinishedManualDownload(object sender = null, EventArgs e = null)
        {
            UnityEngine.Debug.Log("YoutubeDLPlayer: Finished downloading video, beginning playback.");
            videoPlayer.url = mfd.debugtoolsModDirectory + @"Videos\0.mp4";
            PageBackground = mfd.defaultPageBackground;
            UpdateThisPage();
            Play();
        }

        private void ManualDownloaderDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityEngine.Debug.Log("YoutubeDLPlayer: youtube-dl-console: " + e.Data);
        }
    }
}
