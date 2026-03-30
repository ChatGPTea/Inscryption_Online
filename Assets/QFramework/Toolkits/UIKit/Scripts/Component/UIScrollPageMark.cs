/****************************************************************************
 * Copyright (c) 2018.8 liangxie
 *
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace QFramework
{
    public class UIScrollPageMark : MonoBehaviour
    {
        public UIScrollPage scrollPage;
        public ToggleGroup toggleGroup;
        public Toggle togglePrefab;

        [Tooltip("页签中心位置")] public Vector2 centerPos;

        [Tooltip("每个页签之间的间距")] public Vector2 interval;

        public List<Toggle> toggleList = new();

        private void Awake()
        {
            AdjustTogglePos();
            scrollPage.AddPageChangeListener(OnScrollPageChanged);
        }

        public void OnScrollPageChanged(int pageCount, int currentPageIndex)
        {
            if (pageCount != toggleList.Count)
            {
                if (pageCount > toggleList.Count)
                {
                    var cc = pageCount - toggleList.Count;
                    for (var i = 0; i < cc; i++) toggleList.Add(CreateToggle(i));
                }
                else if (pageCount < toggleList.Count)
                {
                    while (toggleList.Count > pageCount)
                    {
                        var t = toggleList[toggleList.Count - 1];
                        toggleList.Remove(t);
                        DestroyImmediate(t.gameObject);
                    }
                }

                AdjustTogglePos();
            }

            toggleGroup.gameObject.SetActive(pageCount > 1);
            if (currentPageIndex >= 0)
                for (var i = 0; i < toggleList.Count; i++)
                    if (i == currentPageIndex) toggleList[i].isOn = true;
                    else toggleList[i].isOn = false;
        }

        private Toggle CreateToggle(int i)
        {
            var t = Instantiate(togglePrefab);
            t.gameObject.SetActive(true);
            t.transform.SetParent(toggleGroup.transform);
            t.transform.localScale = Vector3.one;
            t.transform.localPosition = Vector3.zero;
            return t;
        }

        private void AdjustTogglePos()
        {
            var startPos = centerPos - 0.5f * (toggleList.Count - 1) * interval;
            for (var i = 0; i < toggleList.Count; i++)
                toggleList[i].GetComponent<RectTransform>().anchoredPosition = startPos + i * interval;
        }
    }
}