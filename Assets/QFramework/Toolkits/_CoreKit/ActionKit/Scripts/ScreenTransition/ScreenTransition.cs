using UnityEngine;

namespace QFramework
{
    public partial class ActionKit
    {
        public static class ScreenTransition
        {
            public static ScreenTransitionFade FadeIn()
            {
                return ScreenTransitionFade.Allocate()
                    .FromAlpha(0)
                    .ToAlpha(1)
                    .Duration(1.0f)
                    .Color(Color.black);
            }

            public static ScreenTransitionFade FadeOut()
            {
                return ScreenTransitionFade.Allocate()
                    .FromAlpha(1)
                    .ToAlpha(0)
                    .Duration(1.0f)
                    .Color(Color.black);
            }

            public static ScreenTransitionInOut<ScreenTransitionFade, ScreenTransitionFade> FadeInOut()
            {
                return ScreenTransitionInOut<ScreenTransitionFade, ScreenTransitionFade>.Allocate(
                    FadeIn(),
                    FadeOut()
                );
            }
        }
    }
}