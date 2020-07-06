using UnityEngine;
using System.IO;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GentlesDebugTools.MFD
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
            public static GameObject GetGameObjectFromAssetBundle(AssetBundle bundle, string name)
            {
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

            /// <summary>
            /// Retrieves a specified AssetBundle from "path".
            /// </summary>
            /// <param name="path"></param>
            /// <returns></returns>
            public static AssetBundle GetAssetBundleFromPath(string path)
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
                return bundle;
            }

            public static byte[] GetFileAsBytes(string path)
            {
                byte[] temp = File.ReadAllBytes(path);
                return temp;
            }

            public static async Task<byte[]> GetFileAsBytesAsync(string path)
            {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true))
                {
                    byte[] buffer = new byte[0x1000];
                    await fileStream.ReadAsync(buffer, 0, buffer.Length);
                    return buffer;
                }
            }

            /// <summary>
            /// Synchronously loads and returns a Texture2D from 'path'. Include file extension. Default is 2048x2048
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
            /// <summary>
            /// Asynchronously loads and returns a Texture2D from 'path'. Include file extension. Default is 2048x2048
            /// </summary>
            /// <param name="path"></param>
            /// <param name="width"></param>
            /// <param name="height"></param>
            /// <returns></returns>
            public static async Task<Texture2D> LoadImageFromFileAsync(string path, int width = 2048, int height = 2048)
            {
                byte[] temp = await GetFileAsBytesAsync(path);
                Texture2D image = new Texture2D(width, height);
                image.LoadImage(temp);
                return image;
            }
        }
    }

    /// <summary>
    /// All of the button positions on the DebugMFD.
    /// </summary>
    public enum DebugMFDButtons
    {
        Left1, Left2, Left3, Left4, Right1, Right2, Right3, Right4, TopLeft, TopMiddle, TopRight
    }

    /// <summary>
    /// All of the information texts on the DebugMFD. 1-4 is Top-Bottom. FS1 is the "full screen" text.
    /// </summary>
    public enum DebugMFDInfoTexts
    {
        Left1, Left2, Left3, Middle1, Middle2, Middle3, Right1, Right2, Right3, FS1
    }


    /// <summary>
    /// The page descriptors that are utilized by the DebugMFD.
    /// </summary>
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

        /// <summary>
        /// Returns an identical copy of this page.
        /// </summary>
        /// <returns></returns>
        public DebugMFDPage Copy()
        {
            return new DebugMFDPage(PageName, ModName, PageButtons, InfoTexts, FocusLost);
        }

        /// <summary>
        /// Returns an identical copy of this page.
        /// </summary>
        /// <param name="PageBackground"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Informs the page it has lost focus.
        /// </summary>
        public void LostFocusSend()
        {
            this.FocusLost?.Invoke();
        }

        /// <summary>
        /// Returns true if these pages are identical.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
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

        public override int GetHashCode()
        {
            var hashCode = 1527807641;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PageName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ModName);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<DebugMFDInfoTexts, string>>.Default.GetHashCode(InfoTexts);
            hashCode = hashCode * -1521134295 + EqualityComparer<Dictionary<DebugMFDButtons, DebugMFDButton>>.Default.GetHashCode(PageButtons);
            hashCode = hashCode * -1521134295 + EqualityComparer<Texture2D>.Default.GetHashCode(PageBackground);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            if(obj is DebugMFDPage page)
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
            return false;
        }

        public static bool operator ==(DebugMFDPage lhs, DebugMFDPage rhs)
        {
            if (rhs.PageName == lhs.PageName && rhs.ModName == lhs.ModName && rhs.InfoTexts == lhs.InfoTexts && rhs.PageBackground == lhs.PageBackground && rhs.FocusLost == lhs.FocusLost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(DebugMFDPage lhs, DebugMFDPage rhs)
        {
            if (rhs.PageName != lhs.PageName && rhs.ModName != lhs.ModName && rhs.InfoTexts != lhs.InfoTexts && rhs.PageBackground != lhs.PageBackground && rhs.FocusLost != lhs.FocusLost)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    /// The interactable buttons on the top and sides of the MFD.
    /// </summary>
    public struct DebugMFDButton
    {
        /// <summary>
        /// Button's position on the DebugMFD.
        /// </summary>
        public DebugMFDButtons ButtonPosition;
        /// <summary>
        /// The description of the button displayed on the MFD and the tooltip.
        /// </summary>
        public string ButtonDescription;
        /// <summary>
        /// The action that is invoked when the button is pressed.
        /// </summary>
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

        /// <summary>
        /// Returns true if these buttons are identical.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
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

        public override bool Equals(object obj)
        {
            if(obj is DebugMFDButton button)
            {
                if (button.ButtonDescription == this.ButtonDescription && button.ButtonPosition == this.ButtonPosition && button.ButtonPressedEvent == this.ButtonPressedEvent)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 1361224670;
            hashCode = hashCode * -1521134295 + ButtonPosition.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ButtonDescription);
            hashCode = hashCode * -1521134295 + EqualityComparer<UnityAction>.Default.GetHashCode(ButtonPressedEvent);
            return hashCode;
        }

        public static bool operator ==(DebugMFDButton lhs, DebugMFDButton rhs)
        {
            if (rhs.ButtonDescription == lhs.ButtonDescription && rhs.ButtonPosition == lhs.ButtonPosition && rhs.ButtonPressedEvent == lhs.ButtonPressedEvent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool operator !=(DebugMFDButton lhs, DebugMFDButton rhs)
        {
            if (rhs.ButtonDescription != lhs.ButtonDescription && rhs.ButtonPosition != lhs.ButtonPosition && rhs.ButtonPressedEvent != lhs.ButtonPressedEvent)
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
