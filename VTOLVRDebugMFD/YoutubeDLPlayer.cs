using UnityEngine;
using UnityEngine.Video;
using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GentlesDebugTools.MFD
{
    class YoutubeDLPlayer : CustomMFDPage
    {
        private RenderTexture rT;

        private AudioSource audioSource;
        private VideoPlayer videoPlayer;
        private bool PlayInBackground = false;

        private Dictionary<int, string> Videos;
        private Dictionary<int, string> VideoLocalLinks;
        private int chosenVideo = 0;

        public override void Awake()
        {
            PageName = "";
            ModName = "";
            PageButtonName = "YT Player";

            base.Awake();
        }

        public override void Start()
        {
            PageBackground = DebugMFDUtilities.FileLoader.LoadImageFromFile(mfd.debugtoolsModDirectory + "Backgrounds/loading.png");

            base.Start();

            videoPlayer = gameObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.url = "";
            videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0.5f;
            audioSource.playOnAwake = false;

            videoPlayer.SetTargetAudioSource(0, audioSource);

            Videos = new Dictionary<int, string>();
            VideoLocalLinks = new Dictionary<int, string>();

            rT = new RenderTexture(640, 480, 16);
            videoPlayer.targetTexture = rT;

            GetVideoLinks();
            DownloadManual();
        }

        private void GetVideoLinks()
        {
            string[] links = File.ReadAllLines(mfd.debugtoolsModDirectory + @"videos.txt");
            for (int i = 0; i < links.Length; i++)
            {
                Videos.Add(i, links[i]);
            }
        }

        public override void InitButtons()
        {
            base.InitButtons();

            foreach (DebugMFDButtons item in Enum.GetValues(typeof(DebugMFDButtons)))
            {
                PageButtons[item] = new DebugMFDButton(item, "");
            }

            //Play/Pause buttons
            PageButtons[DebugMFDButtons.TopLeft] = new DebugMFDButton(DebugMFDButtons.TopLeft, "Play", this.Play);
            PageButtons[DebugMFDButtons.TopRight] = new DebugMFDButton(DebugMFDButtons.TopRight, "Pause", this.Pause);
            PageButtons[DebugMFDButtons.Right1] = new DebugMFDButton(DebugMFDButtons.Right1, "                 VOL+", this.VolumeUp);
            PageButtons[DebugMFDButtons.Right2] = new DebugMFDButton(DebugMFDButtons.Right2, "                 VOL-", this.VolumeDown);
            PageButtons[DebugMFDButtons.Right3] = new DebugMFDButton(DebugMFDButtons.Right3, "                 NEXT", this.NextVideo);
            PageButtons[DebugMFDButtons.Right4] = new DebugMFDButton(DebugMFDButtons.Right4, "                 PREV", this.PreviousVideo);
            PageButtons[DebugMFDButtons.Left4] = new DebugMFDButton(DebugMFDButtons.Left4, "\nPLAY-IN-BG", this.PlayInBackgroundToggle);
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

            Play();
        }

        public override void LostFocus()
        {
            base.LostFocus();
            if (!PlayInBackground)
            {
                Pause();
            }
        }

        public void Play()
        {
            if (videoPlayer.url != "")
            {
                mfd.ScreenBackground.SetTexture("_MainTex", rT);
                if (!videoPlayer.isPlaying)
                {
                    StartCoroutine(WaitForVideoEnd());
                }
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

        private IEnumerator WaitForVideoEnd()
        {
            yield return new WaitForSecondsRealtime((float)videoPlayer.length + 1f);
            if (!videoPlayer.isPlaying)
            {
                NextVideo();
            }
        }

        public void DownloadManual()
        {
            for(int i = 0; i < Videos.Count; i++)
            {
                string arguments = Videos[i] + @" -o """ + mfd.debugtoolsModDirectory + @"Videos\" + i + @".mp4""" + @" -f ""bestvideo[ext=mp4]+bestaudio[ext=m4a]/best[ext=mp4]/best""";
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
                StartCoroutine(ManualDownloadChecker(downloader, i));
            }
        }

        private IEnumerator ManualDownloadChecker(Process pro, int videoIndex)
        {
            while (!pro.HasExited) { yield return new WaitForEndOfFrame(); }
            FinishedManualDownload(videoIndex);
        }

        private void FinishedManualDownload(int VideoIndex)
        {
            UnityEngine.Debug.Log("YoutubeDLPlayer: Finished downloading video, beginning playback.");
            VideoLocalLinks[VideoIndex] = "file://" + mfd.debugtoolsModDirectory + @"Videos\" + VideoIndex + ".mp4";
            PageBackground = mfd.defaultPageBackground;
            UpdateThisPage();
            if(VideoIndex == chosenVideo)
            {
                Play();
            }
        }

        private void ManualDownloaderDataReceived(object sender, DataReceivedEventArgs e)
        {
            UnityEngine.Debug.Log("YoutubeDLPlayer: youtube-dl-console: " + e.Data);
        }

        private void VolumeUp()
        {
            audioSource.volume += 0.1f;
        }

        private void VolumeDown()
        {
            audioSource.volume -= 0.1f;
        }

        private void PlayInBackgroundToggle()
        {
            PlayInBackground = !PlayInBackground;
        }

        private void NextVideo()
        {
            chosenVideo++;
            if(chosenVideo > VideoLocalLinks.Count - 1) { chosenVideo = 0; }

            videoPlayer.url = VideoLocalLinks[chosenVideo];
            videoPlayer.Play();
        }

        private void PreviousVideo()
        {
            chosenVideo--;
            if(chosenVideo < 0) { chosenVideo = 0; }

            videoPlayer.url = VideoLocalLinks[chosenVideo];
            videoPlayer.Play();
        }

        private void OnDisable()
        {
            //Delete all downloaded videos so that we can download potentially changed ones on the next run.
            for(int i = 0; i < VideoLocalLinks.Count; i++)
            {
                if (File.Exists(VideoLocalLinks[i]))
                {
                    File.Delete(VideoLocalLinks[i]);
                }
            }
        }

        private void OnDestroy()
        {
            //Delete all downloaded videos so that we can download potentially changed ones on the next run.
            for (int i = 0; i < VideoLocalLinks.Count; i++)
            {
                if (File.Exists(VideoLocalLinks[i]))
                {
                    File.Delete(VideoLocalLinks[i]);
                }
            }
        }
    }
}
