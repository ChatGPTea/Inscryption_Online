using System;

namespace QFramework
{
    public class ScreenTransitionInOut<TIn, TOut> : AbstractAction<ScreenTransitionInOut<TIn, TOut>>
        where TIn : IAction
        where TOut : IAction
    {
        private TIn mIn;
        private Action mOnInFinish;
        private Action<TIn> mOnSetIn;
        private Action<TOut> mOnSetOut;
        private TOut mOut;

        public static ScreenTransitionInOut<TIn, TOut> Allocate(TIn transitionIn, TOut transitionOut)
        {
            var transitionInOut = Allocate();
            transitionInOut.mIn = transitionIn;
            transitionInOut.mOut = transitionOut;
            return transitionInOut;
        }

        public ScreenTransitionInOut<TIn, TOut> OnInFinish(Action onInFinish)
        {
            mOnInFinish = onInFinish;
            return this;
        }

        public ScreenTransitionInOut<TIn, TOut> In(Action<TIn> onSetIn)
        {
            mOnSetIn = onSetIn;
            return this;
        }

        public ScreenTransitionInOut<TIn, TOut> Out(Action<TOut> onSetOut)
        {
            mOnSetOut = onSetOut;
            return this;
        }

        public override void OnStart()
        {
            mOnSetIn?.Invoke(mIn);
            mOnSetOut?.Invoke(mOut);
            ActionKit.Sequence()
                .Append(mIn)
                .Callback(() => mOnInFinish?.Invoke())
                .Append(mOut)
                .StartGlobal();
        }
    }
}