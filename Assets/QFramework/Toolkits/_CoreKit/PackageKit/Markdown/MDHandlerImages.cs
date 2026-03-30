/****************************************************************************
 * Copyright (c) 2019 Gwaredd Mountain UNDER MIT License
 * Copyright (c) 2022 liangxiegame UNDER MIT License
 *
 * https://github.com/gwaredd/UnityMarkdownViewer
 * http://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 ****************************************************************************/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace QFramework
{
    public class MDHandlerImages
    {
        public string CurrentPath;
        private readonly List<ImageRequest> mActiveRequests = new();
        private readonly List<AnimatedTexture> mAnimatedTextures = new();

        private Texture mPlaceholder;
        private readonly Dictionary<string, Texture> mTextureCache = new();


        //------------------------------------------------------------------------------

        private string RemapURL(string url)
        {
            if (Regex.IsMatch(url, @"^\w+:", RegexOptions.Singleline)) return url;

            var projectDir = Path.GetDirectoryName(Application.dataPath);

            if (url.StartsWith("/")) return $"file:///{projectDir}{url}";

            var assetDir = Path.GetDirectoryName(CurrentPath);
            return "file:///" + MDUtils.PathNormalise(string.Format("{0}/{1}/{2}", projectDir, assetDir, url));
        }

        //------------------------------------------------------------------------------

        public Texture FetchImage(string url)
        {
            url = RemapURL(url);

            Texture tex;

            if (mTextureCache.TryGetValue(url, out tex)) return tex;

            if (mPlaceholder == null)
            {
                var style = GUI.skin.GetStyle("btnPlaceholder");
                mPlaceholder = style != null ? style.normal.background : null;
            }

            mActiveRequests.Add(new ImageRequest(url));
            mTextureCache[url] = mPlaceholder;

            return mPlaceholder;
        }

        //------------------------------------------------------------------------------

        public bool UpdateRequests()
        {
            var req = mActiveRequests.Find(r => r.Request.isDone);

            if (req == null) return false;

#if UNITY_2020_2_OR_NEWER
            if (req.Request.result == UnityWebRequest.Result.ProtocolError)
#else
            if (req.Request.isHttpError)
#endif
            {
                Debug.LogError(string.Format("HTTP Error: {0} - {1} {2}", req.URL, req.Request.responseCode,
                    req.Request.error));
                mTextureCache[req.URL] = null;
            }
#if UNITY_2020_2_OR_NEWER
            else if (req.Request.result == UnityWebRequest.Result.ConnectionError)
#else
            else if (req.Request.isNetworkError)
#endif
            {
                Debug.LogError(string.Format("Network Error: {0} - {1}", req.URL, req.Request.error));
                mTextureCache[req.URL] = null;
            }
            else if (req.IsGif)
            {
                var anim = req.GetAnimatedTexture();

                if (anim != null && anim.Textures.Count > 0)
                {
                    mTextureCache[req.URL] = anim.Textures[0];

                    if (anim.Textures.Count > 1) mAnimatedTextures.Add(anim);
                }
            }
            else
            {
                mTextureCache[req.URL] = req.GetTexture();
            }

            mActiveRequests.Remove(req);
            return true;
        }


        //------------------------------------------------------------------------------

        public bool UpdateAnimations()
        {
            var update = false;

            foreach (var anim in mAnimatedTextures)
                if (anim.Update())
                {
                    mTextureCache[anim.URL] = anim.Textures[anim.CurrentFrame];
                    update = true;
                }

            return update;
        }


        //------------------------------------------------------------------------------

        public bool Update()
        {
            return UpdateRequests() || UpdateAnimations();
        }

        private class AnimatedTexture
        {
            public int CurrentFrame;
            public double FrameTime;
            public readonly List<Texture2D> Textures = new();
            public readonly List<float> Times = new();
            public readonly string URL = string.Empty;

            public AnimatedTexture(string url)
            {
                URL = url;
                FrameTime = EditorApplication.timeSinceStartup;
            }

            public void Add(Texture2D tex, float delay)
            {
                Textures.Add(tex);
                Times.Add(delay);
            }

            public bool Update()
            {
                var span = EditorApplication.timeSinceStartup - FrameTime;

                if (span < Times[CurrentFrame]) return false;

                FrameTime = EditorApplication.timeSinceStartup;
                CurrentFrame = (CurrentFrame + 1) % Textures.Count;

                return true;
            }
        }

        private class ImageRequest
        {
            public readonly bool IsGif;
            public readonly UnityWebRequest Request;
            public readonly string URL; // original url

            public ImageRequest(string url)
            {
                URL = url;

                if (url.EndsWith(".gif", StringComparison.OrdinalIgnoreCase))
                {
                    IsGif = true;
                    Request = UnityWebRequest.Get(url);
                }
                else
                {
                    IsGif = false;
                    Request = UnityWebRequestTexture.GetTexture(url);
                }

                Request.SendWebRequest();
            }

            public AnimatedTexture GetAnimatedTexture()
            {
                var decoder = new Decoder(Request.downloadHandler.data);
                var img = decoder.NextImage();
                var anim = new AnimatedTexture(URL);

                while (img != null)
                {
                    anim.Add(img.CreateTexture(), img.Delay / 1000.0f);
                    img = decoder.NextImage();
                }

                return anim;
            }

            public Texture GetTexture()
            {
                var handler = Request.downloadHandler as DownloadHandlerTexture;
                return handler?.texture;
            }
        }
    }
}
#endif