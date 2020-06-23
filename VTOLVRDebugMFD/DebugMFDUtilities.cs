using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Collections.Generic;

namespace VTOLVRDebug
{
    public static class DebugMFDUtilities
    {
        public static class FileLoader
        {
            /// <summary>
            /// Retrieves a specified GameObject (such as a prefab) from an AssetBundle. 'name' is the prefab name; include .prefab extension.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            public static GameObject GetAssetBundleAsGameObject(string path, string name)
            {
                Debug.Log("AssetBundleLoader: Attempting to load AssetBundle...");
                AssetBundle bundle = AssetBundle.LoadFromFile(path);
                if (bundle != null)
                {
                    Debug.Log("AssetBundleLoader: Success.");
                }
                else
                {
                    Debug.Log("AssetBundleLoader: Couldn't load AssetBundle from path: '" + path + "'");
                }

                Debug.Log("AssetBundleLoader: Attempting to retrieve: '" + name + "' as type: 'GameObject'.");
                var temp = bundle.LoadAsset(name, typeof(GameObject));

                if (temp != null)
                {
                    Debug.Log("AssetBundleLoader: Success.");
                    return (GameObject)temp;
                }
                else
                {
                    Debug.Log("AssetBundleLoader: Couldn't retrieve GameObject from AssetBundle.");
                    return null;
                }
            }

            public static byte[] GetFileAsBytes(string path)
            {
                byte[] temp = File.ReadAllBytes(path);
                return temp;
            }

            /// <summary>
            /// Synchronously loads and returns a Texture2D from 'path'. Include file extension.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public static Texture2D LoadImageFromFile(string path, int width = 2048, int height = 2048)
            {
                byte[] temp = GetFileAsBytes(path);
                Texture2D image = new Texture2D(width, height);
                image.LoadImage(temp);
                return image;
            }
        }
    }

    public enum DebugMFDButtons
    {
        Left1, Left2, Left3, Left4, Right1, Right2, Right3, Right4, TopLeft, TopMiddle, TopRight
    }

    public enum DebugMFDInfoTexts
    {
        Left1, Left2, Left3, Middle1, Middle2, Middle3, Right1, Right2, Right3, FS1
    }

    public struct DebugMFDPage
    {
        public string PageName;
        public string ModName;
        public Dictionary<DebugMFDInfoTexts, string> InfoTexts;
        public Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons;

        public delegate void LostFocus();
        public event LostFocus FocusLost;

        public Texture2D PageBackground;

        public DebugMFDPage(string PageName, string ModName, Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons, Dictionary<DebugMFDInfoTexts, string> InfoTexts, LostFocus focusLostEvent = null)
        {
            this.PageName = PageName;
            this.ModName = ModName;
            this.InfoTexts = InfoTexts;
            this.PageBackground = null;
            this.PageButtons = PageButtons;
            this.FocusLost = focusLostEvent;
        }

        public DebugMFDPage(string PageName, string ModName, Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons, Dictionary<DebugMFDInfoTexts, string> InfoTexts, Texture2D PageBackground, LostFocus focusLostEvent = null)
        {
            this.PageName = PageName;
            this.ModName = ModName;
            this.InfoTexts = InfoTexts;
            this.PageBackground = PageBackground;
            this.PageButtons = PageButtons;
            this.FocusLost = focusLostEvent;
        }

        public DebugMFDPage Copy()
        {
            return new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, FocusLost);
        }

        public DebugMFDPage Copy(Texture2D PageBackground)
        {
            return new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, PageBackground, FocusLost);
        }

        public void Update(Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons, Dictionary<DebugMFDInfoTexts, string> InfoTexts)
        {
            this.InfoTexts = InfoTexts;
            this.PageButtons = PageButtons;
        }

        public void Update(Dictionary<DebugMFDButtons, DebugMFDButton> PageButtons, Dictionary<DebugMFDInfoTexts, string> InfoTexts, Texture2D PageBackground)
        {
            this.InfoTexts = InfoTexts;
            this.PageButtons = PageButtons;
            this.PageBackground = PageBackground;
        }

        public void LostFocusSend()
        {
            this.FocusLost?.Invoke();
        }

        public bool Equals(DebugMFDPage page)
        {
            if (page.PageName == this.PageName && page.ModName == this.ModName && page.InfoTexts == this.InfoTexts && page.PageBackground == this.PageBackground && page.FocusLost == this.FocusLost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public struct DebugMFDButton
    {
        public DebugMFDButtons ButtonPosition;
        public string ButtonDescription;
        public UnityAction ButtonPressedEvent;

        public DebugMFDButton(DebugMFDButtons ButtonPosition, string ButtonDescription)
        {
            this.ButtonPosition = ButtonPosition;
            this.ButtonDescription = ButtonDescription;
            this.ButtonPressedEvent = null;
        }

        public DebugMFDButton(DebugMFDButtons ButtonPosition, string ButtonDescription, UnityAction ButtonPressedEvent)
        {
            this.ButtonPosition = ButtonPosition;
            this.ButtonDescription = ButtonDescription;
            this.ButtonPressedEvent = ButtonPressedEvent;
        }

        public void Update()
        {

        }

        public bool Equals(DebugMFDButton button)
        {
            if(button.ButtonDescription == this.ButtonDescription && button.ButtonPosition == this.ButtonPosition && button.ButtonPressedEvent == this.ButtonPressedEvent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
