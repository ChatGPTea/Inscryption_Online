/****************************************************************************
 * Copyright (c) 2018 ~ 2022 liangxiegame UNDER MIT LICENSE
 *
 * https://qframework.cn
 * https://github.com/liangxiegame/QFramework
 * https://gitee.com/liangxiegame/QFramework
 *
 * Latest Update: 2021.2.15 14:18 Add Simple ECA Pattern
 ****************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace QFramework
{
    public class DeprecateActionKit
    {
        [RuntimeInitializeOnLoadMethod]
        private static void InitNodeSystem()
        {
            // cache list			

            // cache node
            SafeObjectPool<DelayAction>.Instance.Init(50, 50);
            SafeObjectPool<EventAction>.Instance.Init(50, 50);
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     延时执行节点
    /// </summary>
    [Serializable]
    [ActionGroup("ActionKit")]
    public class DelayAction : ActionKitAction, IPoolable, IResetable
    {
        [SerializeField] public float DelayTime;

        public Action OnDelayFinish { get; set; }

        public float CurrentSeconds { get; set; }

        public void OnRecycled()
        {
            OnDelayFinish = null;
            DelayTime = 0.0f;
            Reset();
        }

        public bool IsRecycled { get; set; }

        public static DelayAction Allocate(float delayTime, Action onDelayFinish = null)
        {
            var retNode = SafeObjectPool<DelayAction>.Instance.Allocate();
            retNode.DelayTime = delayTime;
            retNode.OnDelayFinish = onDelayFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }

        protected override void OnReset()
        {
            CurrentSeconds = 0.0f;
        }

        protected override void OnExecute(float dt)
        {
            CurrentSeconds += dt;
            Finished = CurrentSeconds >= DelayTime;
            if (Finished && OnDelayFinish != null) OnDelayFinish();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<DelayAction>.Instance.Recycle(this);
        }
    }

    public static class DelayActionExtensions
    {
        public static IDeprecateAction Delay<T>(this T selfBehaviour, float seconds, Action delayEvent)
            where T : MonoBehaviour
        {
            var delayAction = DelayAction.Allocate(seconds, delayEvent);
            selfBehaviour.ExecuteNode(delayAction);
            return delayAction;
        }

        public static IActionChain Delay(this IActionChain selfChain, float seconds)
        {
            return selfChain.Append(DelayAction.Allocate(seconds));
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     安枕执行节点
    /// </summary>
    [Serializable]
    [ActionGroup("ActionKit")]
    public class DelayFrameAction : ActionKitAction, IPoolable, IResetable
    {
        [SerializeField] public int FrameCount;

        private int StartFrame;

        public Action OnDelayFrameFinish { get; set; }

        public float CurrentSeconds { get; set; }

        public void OnRecycled()
        {
            OnDelayFrameFinish = null;
            FrameCount = 0;
            Reset();
        }

        public bool IsRecycled { get; set; }

        public static DelayFrameAction Allocate(int frameCount, Action onDelayFrameFinish = null)
        {
            var retNode = SafeObjectPool<DelayFrameAction>.Instance.Allocate();
            retNode.FrameCount = frameCount;
            retNode.OnDelayFrameFinish = onDelayFrameFinish;
            retNode.CurrentSeconds = 0.0f;
            return retNode;
        }

        protected override void OnReset()
        {
            CurrentSeconds = 0.0f;
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            StartFrame = Time.frameCount;
        }

        protected override void OnExecute(float dt)
        {
            Finished = Time.frameCount - StartFrame >= FrameCount;

            if (Finished && OnDelayFrameFinish != null) OnDelayFrameFinish();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<DelayFrameAction>.Instance.Recycle(this);
        }
    }

    public static class DelayFrameActionExtensions
    {
        public static void DelayFrame<T>(this T selfBehaviour, int frameCount, Action delayFrameEvent)
            where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayFrameAction.Allocate(frameCount, delayFrameEvent));
        }

        public static void NextFrame<T>(this T selfBehaviour, Action nextFrameEvent)
            where T : MonoBehaviour
        {
            selfBehaviour.ExecuteNode(DelayFrameAction.Allocate(1, nextFrameEvent));
        }

        public static IActionChain DelayFrame(this IActionChain selfChain, int frameCount)
        {
            return selfChain.Append(DelayFrameAction.Allocate(frameCount));
        }

        public static IActionChain NextFrame(this IActionChain selfChain)
        {
            return selfChain.Append(DelayFrameAction.Allocate(1));
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     延时执行节点
    /// </summary>
    [OnlyUsedByCode]
    public class EventAction : ActionKitAction, IPoolable
    {
        private Action mOnExecuteEvent;

        public void OnRecycled()
        {
            Reset();
            mOnExecuteEvent = null;
        }

        public bool IsRecycled { get; set; }

        /// <summary>
        ///     TODO:这里填可变参数会有问题
        /// </summary>
        /// <param name="onExecuteEvents"></param>
        /// <returns></returns>
        public static EventAction Allocate(params Action[] onExecuteEvents)
        {
            var retNode = SafeObjectPool<EventAction>.Instance.Allocate();
            Array.ForEach(onExecuteEvents, onExecuteEvent => retNode.mOnExecuteEvent += onExecuteEvent);
            return retNode;
        }

        /// <summary>
        ///     finished
        /// </summary>
        protected override void OnExecute(float dt)
        {
            if (mOnExecuteEvent != null) mOnExecuteEvent.Invoke();

            Finished = true;
        }

        protected override void OnDispose()
        {
            SafeObjectPool<EventAction>.Instance.Recycle(this);
        }
    }

    [OnlyUsedByCode]
    public class OnlyBeginAction : ActionKitAction, IPoolable, IPoolType
    {
        private Action<OnlyBeginAction> mBeginAction;

        public void OnRecycled()
        {
            mBeginAction = null;
        }

        public bool IsRecycled { get; set; }

        public void Recycle2Cache()
        {
            SafeObjectPool<OnlyBeginAction>.Instance.Recycle(this);
        }

        public static OnlyBeginAction Allocate(Action<OnlyBeginAction> beginAction)
        {
            var retSimpleAction = SafeObjectPool<OnlyBeginAction>.Instance.Allocate();

            retSimpleAction.mBeginAction = beginAction;

            return retSimpleAction;
        }

        protected override void OnBegin()
        {
            if (mBeginAction != null) mBeginAction.Invoke(this);
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     like filter, add condition
    /// </summary>
    [OnlyUsedByCode]
    public class UntilAction : ActionKitAction, IPoolable
    {
        private Func<bool> mCondition;

        void IPoolable.OnRecycled()
        {
            Reset();
            mCondition = null;
        }

        bool IPoolable.IsRecycled { get; set; }

        public static UntilAction Allocate(Func<bool> condition)
        {
            var retNode = SafeObjectPool<UntilAction>.Instance.Allocate();
            retNode.mCondition = condition;
            return retNode;
        }

        protected override void OnExecute(float dt)
        {
            Finished = mCondition.Invoke();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<UntilAction>.Instance.Recycle(this);
        }
    }

    internal class OnDestroyDisposeTrigger : MonoBehaviour
    {
        private HashSet<IDisposable> mDisposables = new();

        private void OnDestroy()
        {
            if (Application.isPlaying)
            {
                foreach (var disposable in mDisposables) disposable.Dispose();

                mDisposables.Clear();
                mDisposables = null;
            }
        }

        public void AddDispose(IDisposable disposable)
        {
            if (!mDisposables.Contains(disposable)) mDisposables.Add(disposable);
        }
    }

    internal class OnDisableDisposeTrigger : MonoBehaviour
    {
        private HashSet<IDisposable> mDisposables = new();

        private void OnDisable()
        {
            if (Application.isPlaying)
            {
                foreach (var disposable in mDisposables) disposable.Dispose();

                mDisposables.Clear();
                mDisposables = null;
            }
        }

        public void AddDispose(IDisposable disposable)
        {
            if (!mDisposables.Contains(disposable)) mDisposables.Add(disposable);
        }
    }

    internal static class IDisposableExtensions
    {
        /// <summary>
        ///     与 GameObject 绑定销毁
        /// </summary>
        /// <param name="self"></param>
        /// <param name="component"></param>
        public static void DisposeWhenGameObjectDestroyed(this IDisposable self, Component component)
        {
            var onDestroyDisposeTrigger = component.gameObject.GetComponent<OnDestroyDisposeTrigger>();
            if (!onDestroyDisposeTrigger)
                onDestroyDisposeTrigger = component.gameObject.AddComponent<OnDestroyDisposeTrigger>();

            onDestroyDisposeTrigger.AddDispose(self);
        }
    }

    /// <summary>
    ///     时间轴执行节点
    /// </summary>
    public class Timeline : ActionKitAction
    {
        private float mCurTime;

        public Action<string> OnKeyEventsReceivedCallback = null;

        /// <summary>
        ///     refator 2 one list? all in one list;
        /// </summary>
        public Queue<TimelinePair> TimelineQueue = new();

        public Timeline(params TimelinePair[] pairs)
        {
            foreach (var pair in pairs) TimelineQueue.Enqueue(pair);
        }

        public Action OnTimelineBeganCallback
        {
            get => OnBeganCallback;
            set => OnBeganCallback = value;
        }

        public Action OnTimelineEndedCallback
        {
            get => OnEndedCallback;
            set => OnEndedCallback = value;
        }

        protected override void OnReset()
        {
            mCurTime = 0.0f;

            foreach (var timelinePair in TimelineQueue) timelinePair.Node.Reset();
        }

        protected override void OnExecute(float dt)
        {
            mCurTime += dt;

            foreach (var pair in TimelineQueue.Where(pair => pair.Time < mCurTime && !pair.Node.Finished))
                if (pair.Node.Execute(dt))
                    Finished = TimelineQueue.Count(timelinePair => !timelinePair.Node.Finished) == 0;
        }

        public void Append(TimelinePair pair)
        {
            TimelineQueue.Enqueue(pair);
        }

        public void Append(float time, IDeprecateAction node)
        {
            TimelineQueue.Enqueue(new TimelinePair(time, node));
        }

        protected override void OnDispose()
        {
            foreach (var timelinePair in TimelineQueue) timelinePair.Node.Dispose();

            TimelineQueue.Clear();
            TimelineQueue = null;
        }

        public class TimelinePair
        {
            public IDeprecateAction Node;
            public float Time;

            public TimelinePair(float time, IDeprecateAction node)
            {
                Time = time;
                Node = node;
            }
        }
    }

    public class KeyEventAction : ActionKitAction, IPoolable
    {
        private string mEventName;
        private Timeline mTimeline;

        public void OnRecycled()
        {
            mTimeline = null;
            mEventName = null;
        }

        public bool IsRecycled { get; set; }

        public static KeyEventAction Allocate(string eventName, Timeline timeline)
        {
            var keyEventAction = SafeObjectPool<KeyEventAction>.Instance.Allocate();

            keyEventAction.mEventName = eventName;
            keyEventAction.mTimeline = timeline;

            return keyEventAction;
        }

        protected override void OnBegin()
        {
            base.OnBegin();

            mTimeline.OnKeyEventsReceivedCallback?.Invoke(mEventName);

            Finish();
        }

        protected override void OnDispose()
        {
            SafeObjectPool<KeyEventAction>.Instance.Recycle(this);
        }
    }

    public class AsyncNode : ActionKitAction
    {
        public HashSet<IDeprecateAction> mActions = new();

        public void Add(IDeprecateAction action)
        {
            mActions.Add(action);
        }

        protected override void OnExecute(float dt)
        {
            foreach (var action in mActions.Where(action => action.Execute(dt)))
            {
                mActions.Remove(action);
                action.Dispose();
            }
        }
    }

    public class QueueNode : ActionKitAction
    {
        private readonly Queue<IDeprecateAction> mQueue = new(20);

        public void Enqueue(IDeprecateAction action)
        {
            mQueue.Enqueue(action);
        }

        protected override void OnExecute(float dt)
        {
            if (mQueue.Count != 0 && mQueue.Peek().Execute(dt)) mQueue.Dequeue().Dispose();
        }
    }

    public interface IResetable
    {
        void Reset();
    }

    [OnlyUsedByCode]
    public class RepeatNode : ActionKitAction, INode
    {
        private int mCurRepeatCount;

        private IDeprecateAction mNode;

        public int RepeatCount = 1;

        public RepeatNode(IDeprecateAction node, int repeatCount = -1)
        {
            RepeatCount = repeatCount;
            mNode = node;
        }

        public IDeprecateAction CurrentExecutingNode
        {
            get
            {
                var currentNode = mNode;
                var node = currentNode as INode;
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }

        protected override void OnReset()
        {
            if (null != mNode) mNode.Reset();

            mCurRepeatCount = 0;
            Finished = false;
        }

        protected override void OnExecute(float dt)
        {
            if (RepeatCount == -1)
            {
                if (mNode.Execute(dt)) mNode.Reset();

                return;
            }

            if (mNode.Execute(dt))
            {
                mNode.Reset();
                mCurRepeatCount++;
            }

            if (mCurRepeatCount == RepeatCount) Finished = true;
        }

        protected override void OnDispose()
        {
            if (null != mNode)
            {
                mNode.Dispose();
                mNode = null;
            }
        }
    }

    /// <summary>
    ///     序列执行节点
    /// </summary>
    [OnlyUsedByCode]
    public class SequenceNode : ActionKitAction, INode, IResetable
    {
        protected List<IDeprecateAction> mExcutingNodes = ListPool<IDeprecateAction>.Get();
        protected List<IDeprecateAction> mNodes = ListPool<IDeprecateAction>.Get();

        public SequenceNode(params IDeprecateAction[] nodes)
        {
            foreach (var node in nodes)
            {
                mNodes.Add(node);
                mExcutingNodes.Add(node);
            }
        }

        public int TotalCount => mExcutingNodes.Count;

        public IDeprecateAction CurrentExecutingNode
        {
            get
            {
                var currentNode = mExcutingNodes[0];
                var node = currentNode as INode;
                return node == null ? currentNode : node.CurrentExecutingNode;
            }
        }

        protected override void OnReset()
        {
            mExcutingNodes.Clear();
            foreach (var node in mNodes)
            {
                node.Reset();
                mExcutingNodes.Add(node);
            }
        }

        protected override void OnExecute(float dt)
        {
            if (mExcutingNodes.Count > 0)
            {
                // 如果有异常，则进行销毁，不再进行下边的操作
                if (mExcutingNodes[0].Disposed && !mExcutingNodes[0].Finished)
                {
                    Dispose();
                    return;
                }

                while (mExcutingNodes[0].Execute(dt))
                {
                    mExcutingNodes.RemoveAt(0);

                    OnCurrentActionFinished();

                    if (mExcutingNodes.Count == 0) break;
                }
            }

            Finished = mExcutingNodes.Count == 0;
        }

        protected virtual void OnCurrentActionFinished()
        {
        }

        public SequenceNode Append(IDeprecateAction appendedNode)
        {
            mNodes.Add(appendedNode);
            mExcutingNodes.Add(appendedNode);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mNodes)
            {
                mNodes.ForEach(node => node.Dispose());
                mNodes.Clear();
                mNodes.Release2Pool();
                mNodes = null;
            }

            if (null != mExcutingNodes)
            {
                mExcutingNodes.Release2Pool();
                mExcutingNodes = null;
            }
        }
    }

    /// <summary>
    ///     并发执行的协程
    /// </summary>
    [OnlyUsedByCode]
    public class SpawnNode : ActionKitAction
    {
        private int mFinishCount;
        protected List<ActionKitAction> mNodes = ListPool<ActionKitAction>.Get();

        public SpawnNode(params ActionKitAction[] nodes)
        {
            mNodes.AddRange(nodes);

            foreach (var nodeAction in nodes) nodeAction.OnEndedCallback += IncreaseFinishCount;
        }

        protected override void OnReset()
        {
            mNodes.ForEach(node => node.Reset());
            mFinishCount = 0;
        }

        public override void Finish()
        {
            for (var i = mNodes.Count - 1; i >= 0; i--) mNodes[i].Finish();

            base.Finish();
        }

        protected override void OnExecute(float dt)
        {
            for (var i = mNodes.Count - 1; i >= 0; i--)
            {
                var node = mNodes[i];
                if (!node.Finished && node.Execute(dt))
                    Finished = mNodes.Count == mFinishCount;
            }
        }

        private void IncreaseFinishCount()
        {
            mFinishCount++;
        }

        public void Add(params ActionKitAction[] nodes)
        {
            mNodes.AddRange(nodes);

            foreach (var nodeAction in nodes) nodeAction.OnEndedCallback += IncreaseFinishCount;
        }

        protected override void OnDispose()
        {
            foreach (var node in mNodes)
            {
                node.OnEndedCallback -= IncreaseFinishCount;
                node.Dispose();
            }

            mNodes.Release2Pool();
            mNodes = null;
        }
    }

    public class ActionKitFSM
    {
        private readonly Dictionary<Type, ActionKitFSMState> mStates = new();
        private readonly ActionKitFSMTransitionTable mTrasitionTable = new();
        public ActionKitFSMState CurrentState { get; private set; }
        public Type PreviousStateType { get; private set; }

        public void Update()
        {
            if (CurrentState != null) CurrentState.Update();
        }

        public void FixedUpdate()
        {
            if (CurrentState != null) CurrentState.FixedUpdate();
        }


        public void AddState(ActionKitFSMState state)
        {
            mStates.Add(state.GetType(), state);
        }

        public void AddTransition(ActionKitFSMTransition transition)
        {
            mTrasitionTable.Add(transition);
        }

        public bool HandleEvent<TTransition>() where TTransition : ActionKitFSMTransition
        {
            foreach (var transition in mTrasitionTable.TypeIndex.Get(typeof(TTransition)))
                if (transition.SrcStateTypes.Contains(CurrentState.GetType()))
                {
                    var currentState = CurrentState;
                    var nextState = mStates[transition.DstStateType];
                    CurrentState.Exit();
                    transition.OnTransition(currentState, nextState);
                    CurrentState = nextState;
                    CurrentState.Enter();
                    return true;
                }

            return false;
        }

        public void ChangeState<TState>() where TState : ActionKitFSMState
        {
            ChangeState(typeof(TState));
        }

        public void ChangeState(Type stateType)
        {
            if (CurrentState.GetType() != stateType)
            {
                PreviousStateType = CurrentState.GetType();
                CurrentState.Exit();
                CurrentState = mStates[stateType];
                CurrentState.Enter();
            }
        }

        public void BackToPreviousState()
        {
            ChangeState(PreviousStateType);
        }

        public void StartState<T>()
        {
            CurrentState = mStates[typeof(T)];
            CurrentState.Enter();
        }
    }

    public class ActionKitFSMState
    {
        public void Enter()
        {
            OnEnter();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void FixedUpdate()
        {
            OnFixedUpdate();
        }

        public void Exit()
        {
            OnExit();
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnFixedUpdate()
        {
        }

        protected virtual void OnExit()
        {
        }
    }

    public class ActionKitFSMState<T> : ActionKitFSMState
    {
        protected T mTarget;

        public ActionKitFSMState(T target)
        {
            mTarget = target;
        }

        public T Target => mTarget;
    }

    public class ActionKitFSMTransition
    {
        public virtual HashSet<Type> SrcStateTypes { get; set; }

        public virtual Type DstStateType { get; set; }

        public virtual void OnTransition(ActionKitFSMState srcState, ActionKitFSMState dstState)
        {
        }

        public ActionKitFSMTransition AddSrcState<T>() where T : ActionKitFSMState
        {
            if (SrcStateTypes == null)
                SrcStateTypes = new HashSet<Type>
                {
                    typeof(T)
                };
            else
                SrcStateTypes.Add(typeof(T));

            return this;
        }

        public ActionKitFSMTransition WithDstState<T>() where T : ActionKitFSMState
        {
            DstStateType = typeof(T);
            return this;
        }
    }


    public class ActionKitFSMTransition<TSrcState, TDstState> : ActionKitFSMTransition
    {
        private readonly Type mDstStateType = typeof(TDstState);
        private readonly Type mSrcStateType = typeof(TSrcState);

        public override HashSet<Type> SrcStateTypes => new() { mSrcStateType };

        public override Type DstStateType => mDstStateType;
    }

    public struct EnumStateChangeEvent<T> where T : struct, IComparable, IConvertible, IFormattable
    {
        public GameObject Target;
        public EnumStateMachine<T> TargetStateMachine;
        public T NewState;
        public T PreviousState;

        public EnumStateChangeEvent(EnumStateMachine<T> stateMachine)
        {
            Target = stateMachine.Target;
            TargetStateMachine = stateMachine;
            NewState = stateMachine.CurrentState;
            PreviousState = stateMachine.PreviousState;
        }
    }

    public interface IEnumStateMachine
    {
        bool TriggerEvents { get; set; }
    }

    public class EnumStateMachine<T> : IEnumStateMachine where T : struct, IComparable, IConvertible, IFormattable
    {
        /// the name of the target gameobject
        public GameObject Target;

        /// <summary>
        ///     Creates a new StateMachine, with a targetName (used for events, usually use GetInstanceID()), and whether you want
        ///     to use events with it or not
        /// </summary>
        /// <param name="targetName">Target name.</param>
        /// <param name="triggerEvents">If set to <c>true</c> trigger events.</param>
        public EnumStateMachine(GameObject target, bool triggerEvents)
        {
            Target = target;
            TriggerEvents = triggerEvents;
        }

        /// the current character's movement state
        public T CurrentState { get; protected set; }

        /// the character's movement state before entering the current one
        public T PreviousState { get; protected set; }

        /// If you set TriggerEvents to true, the state machine will trigger events when entering and exiting a state. 
        /// Additionnally, if you also use a StateMachineProcessor, it'll trigger events for the current state on FixedUpdate, LateUpdate, but also
        /// on Update (separated in EarlyUpdate, Update and EndOfUpdate, triggered in this order at Update()
        /// To listen to these events, from any class, in its Start() method (or wherever you prefer), use MMEventManager.StartListening(gameObject.GetInstanceID().ToString()+"XXXEnter",OnXXXEnter);
        /// where XXX is the name of the state you're listening to, and OnXXXEnter is the method you want to call when that event is triggered.
        /// MMEventManager.StartListening(gameObject.GetInstanceID().ToString()+"CrouchingEarlyUpdate",OnCrouchingEarlyUpdate); for example will listen to the Early Update event of the Crouching state, and 
        /// will trigger the OnCrouchingEarlyUpdate() method.
        public bool TriggerEvents { get; set; }

        /// <summary>
        ///     切换状态
        /// </summary>
        /// <param name="newState">New state.</param>
        public virtual void ChangeState(T newState)
        {
            // if the "new state" is the current one, we do nothing and exit
            if (newState.Equals(CurrentState)) return;

            // we store our previous character movement state
            PreviousState = CurrentState;
            CurrentState = newState;

            if (TriggerEvents) TypeEventSystem.Global.Send(new EnumStateChangeEvent<T>(this));
        }

        /// <summary>
        ///     返回上一个状态
        /// </summary>
        public virtual void RestorePreviousState()
        {
            CurrentState = PreviousState;

            if (TriggerEvents) TypeEventSystem.Global.Send(new EnumStateChangeEvent<T>(this));
        }
    }

    public class ActionKitFSMTransitionTable : Table<ActionKitFSMTransition>
    {
        public TableIndex<Type, ActionKitFSMTransition> TypeIndex = new(t => t.GetType());

        protected override void OnAdd(ActionKitFSMTransition item)
        {
            TypeIndex.Add(item);
        }

        protected override void OnRemove(ActionKitFSMTransition item)
        {
            TypeIndex.Remove(item);
        }

        protected override void OnClear()
        {
            TypeIndex.Clear();
        }

        public override IEnumerator<ActionKitFSMTransition> GetEnumerator()
        {
            return TypeIndex.Dictionary.SelectMany(src => src.Value).GetEnumerator();
        }

        protected override void OnDispose()
        {
            TypeIndex.Dispose();
        }
    }

    /// <summary>
    ///     FSM 基于枚举的状态机
    /// </summary>
    public class FSM<TStateEnum, TEventEnum> : IDisposable
    {
        /// <summary>
        ///     FSM onStateChagned.
        /// </summary>
        public delegate void FSMOnStateChanged(params object[] param);

        private readonly Action<TStateEnum, TStateEnum> mOnStateChanged;

        /// <summary>
        ///     The m state dict.
        /// </summary>
        private readonly Dictionary<TStateEnum, FSMState<TStateEnum>>
            mStateDict = new();

        public FSM(Action<TStateEnum, TStateEnum> onStateChanged = null)
        {
            mOnStateChanged = onStateChanged;
        }

        /// <summary>
        ///     The state of the m current.
        /// </summary>
        public TStateEnum State { get; private set; }

        public void Dispose()
        {
            Clear();
        }

        /// <summary>
        ///     Adds the state.
        /// </summary>
        /// <param name="name">Name.</param>
        private void AddState(TStateEnum name)
        {
            mStateDict[name] = new FSMState<TStateEnum>(name);
        }

        /// <summary>
        ///     Adds the translation.
        /// </summary>
        /// <param name="fromState">From state.</param>
        /// <param name="name">Name.</param>
        /// <param name="toState">To state.</param>
        /// <param name="onStateChagned">Callfunc.</param>
        public void AddTransition(TStateEnum fromState, TEventEnum name, TStateEnum toState,
            Action<object[]> onStateChagned = null)
        {
            if (!mStateDict.ContainsKey(fromState)) AddState(fromState);

            if (!mStateDict.ContainsKey(toState)) AddState(toState);

            mStateDict[fromState].TranslationDict[name] =
                new FSMTranslation<TStateEnum, TEventEnum>(fromState, name, toState, onStateChagned);
        }

        /// <summary>
        ///     Start the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Start(TStateEnum name)
        {
            State = name;
        }

        /// <summary>
        ///     Handles the event.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="param">Parameter.</param>
        public void HandleEvent(TEventEnum name, params object[] param)
        {
            if (State != null && mStateDict[State].TranslationDict.ContainsKey(name))
            {
                var tempTranslation = mStateDict[State].TranslationDict[name];

                if (tempTranslation.OnTranslationCallback != null) tempTranslation.OnTranslationCallback.Invoke(param);

                if (mOnStateChanged != null) mOnStateChanged.Invoke(State, tempTranslation.ToState);

                State = tempTranslation.ToState;
            }
        }

        /// <summary>
        ///     Clear this instance.
        /// </summary>
        public void Clear()
        {
            foreach (var keyValuePair in mStateDict)
            {
                foreach (var translationDictValue in keyValuePair.Value.TranslationDict.Values)
                    translationDictValue.OnTranslationCallback = null;

                keyValuePair.Value.TranslationDict.Clear();
            }

            mStateDict.Clear();
        }

        /// <summary>
        ///     QFSM state.
        /// </summary>
        public class FSMState<TName>
        {
            /// <summary>
            ///     The translation dict.
            /// </summary>
            public readonly Dictionary<TEventEnum, FSMTranslation<TName, TEventEnum>> TranslationDict = new();

            public TName Name;

            public FSMState(TName name)
            {
                Name = name;
            }
        }

        /// <summary>
        ///     Translation
        /// </summary>
        public class FSMTranslation<TStateName, KEventName>
        {
            public TStateName FromState;
            public KEventName Name;
            public Action<object[]> OnTranslationCallback; // 回调函数
            public TStateName ToState;

            public FSMTranslation(TStateName fromState, KEventName name, TStateName toState,
                Action<object[]> onStateChagned)
            {
                FromState = fromState;
                ToState = toState;
                Name = name;
                OnTranslationCallback = onStateChagned;
            }
        }
    }

    /// <summary>
    ///     QFSM lite.
    ///     基于字符串的状态机
    /// </summary>
    public class QFSMLite
    {
        /// <summary>
        ///     FSM callfunc.
        /// </summary>
        public delegate void FSMCallfunc(params object[] param);

        /// <summary>
        ///     The m state dict.
        /// </summary>
        private readonly Dictionary<string, QFSMState> mStateDict = new();

        /// <summary>
        ///     The state of the m current.
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        ///     Adds the state.
        /// </summary>
        /// <param name="name">Name.</param>
        public void AddState(string name)
        {
            mStateDict[name] = new QFSMState(name);
        }

        /// <summary>
        ///     Adds the translation.
        /// </summary>
        /// <param name="fromState">From state.</param>
        /// <param name="name">Name.</param>
        /// <param name="toState">To state.</param>
        /// <param name="callfunc">Callfunc.</param>
        public void AddTranslation(string fromState, string name, string toState, FSMCallfunc callfunc)
        {
            mStateDict[fromState].TranslationDict[name] = new QFSMTranslation(fromState, name, toState, callfunc);
        }

        /// <summary>
        ///     Start the specified name.
        /// </summary>
        /// <param name="name">Name.</param>
        public void Start(string name)
        {
            State = name;
        }

        /// <summary>
        ///     Handles the event.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="param">Parameter.</param>
        public void HandleEvent(string name, params object[] param)
        {
            if (State != null && mStateDict[State].TranslationDict.ContainsKey(name))
            {
                var tempTranslation = mStateDict[State].TranslationDict[name];
                tempTranslation.OnTranslationCallback(param);
                State = tempTranslation.ToState;
            }
        }

        /// <summary>
        ///     Clear this instance.
        /// </summary>
        public void Clear()
        {
            mStateDict.Clear();
        }

        /// <summary>
        ///     QFSM state.
        /// </summary>
        private class QFSMState
        {
            /// <summary>
            ///     The translation dict.
            /// </summary>
            public readonly Dictionary<string, QFSMTranslation> TranslationDict = new();

            public string Name;

            public QFSMState(string name)
            {
                Name = name;
            }
        }

        /// <summary>
        ///     Translation
        /// </summary>
        public class QFSMTranslation
        {
            public string FromState;
            public string Name;
            public FSMCallfunc OnTranslationCallback; // 回调函数
            public string ToState;

            public QFSMTranslation(string fromState, string name, string toState, FSMCallfunc onTranslationCallback)
            {
                FromState = fromState;
                ToState = toState;
                Name = name;
                OnTranslationCallback = onTranslationCallback;
            }
        }
    }

    public interface IDeprecateAction : IDisposable
    {
        bool Disposed { get; }

        bool Finished { get; }

        bool Execute(float delta);

        void Reset();

        void Finish();
    }

    [Serializable]
    public class ActionData
    {
        [SerializeField] public string ActionName;
        [SerializeField] public string AcitonData;
    }

    public interface INode
    {
        IDeprecateAction CurrentExecutingNode { get; }
    }

    [Serializable]
    public abstract class ActionKitAction : IDeprecateAction
    {
        protected bool mOnBeginCalled;
        public Action OnBeganCallback;
        public Action OnDisposedCallback;
        public Action OnEndedCallback;

        #region ResetableSupport

        public void Reset()
        {
            Finished = false;
            mOnBeginCalled = false;
            mDisposed = false;
            OnReset();
        }

        #endregion


        #region IExecutable Support

        public bool Execute(float dt)
        {
            if (mDisposed) return true;

            // 有可能被别的地方调用
            if (Finished) return Finished;

            if (!mOnBeginCalled)
            {
                mOnBeginCalled = true;
                OnBegin();

                if (OnBeganCallback != null) OnBeganCallback.Invoke();
            }

            if (!Finished) OnExecute(dt);

            if (Finished)
            {
                if (OnEndedCallback != null) OnEndedCallback.Invoke();

                OnEnd();
            }

            return Finished || mDisposed;
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            if (mDisposed) return;
            mDisposed = true;

            OnBeganCallback = null;
            OnEndedCallback = null;

            if (OnDisposedCallback != null) OnDisposedCallback.Invoke();

            OnDisposedCallback = null;
            OnDispose();
        }

        #endregion

        protected virtual void OnReset()
        {
        }

        protected virtual void OnBegin()
        {
        }

        /// <summary>
        ///     finished
        /// </summary>
        protected virtual void OnExecute(float dt)
        {
        }

        protected virtual void OnEnd()
        {
        }

        protected virtual void OnDispose()
        {
        }

        #region IAction Support

        bool IDeprecateAction.Disposed => mDisposed;

        protected bool mDisposed;

        public bool Finished { get; protected set; }

        public virtual void Finish()
        {
            Finished = true;
        }

        public void Break()
        {
            Finished = true;
        }

        #endregion
    }

    public class OnlyUsedByCodeAttribute : Attribute
    {
    }

    public class ActionGroupAttribute : Attribute
    {
        public readonly string GroupName;

        public ActionGroupAttribute(string groupName)
        {
            GroupName = groupName;
        }
    }

    /// <summary>
    ///     支持链式方法
    /// </summary>
    public class SequenceNodeChain : ActionChain
    {
        private SequenceNode mSequenceNode;

        public SequenceNodeChain()
        {
            mSequenceNode = new SequenceNode();
        }

        protected override ActionKitAction mNode => mSequenceNode;

        public override IActionChain Append(IDeprecateAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }

    public class RepeatNodeChain : ActionChain
    {
        private RepeatNode mRepeatNodeAction;

        private SequenceNode mSequenceNode;

        public RepeatNodeChain(int repeatCount)
        {
            mSequenceNode = new SequenceNode();
            mRepeatNodeAction = new RepeatNode(mSequenceNode, repeatCount);
        }

        protected override ActionKitAction mNode => mRepeatNodeAction;

        public override IActionChain Append(IDeprecateAction node)
        {
            mSequenceNode.Append(node);
            return this;
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            if (null != mRepeatNodeAction) mRepeatNodeAction.Dispose();

            mRepeatNodeAction = null;

            mSequenceNode.Dispose();
            mSequenceNode = null;
        }
    }

    public interface IDisposeWhen : IDisposeEventRegister
    {
        IDisposeEventRegister DisposeWhen(Func<bool> condition);
    }

    public interface IDisposeEventRegister
    {
        void OnDisposed(Action onDisposedEvent);

        IDisposeEventRegister OnFinished(Action onFinishedEvent);
    }

    public static class IActionExtension
    {
        private static readonly WaitForEndOfFrame mEndOfFrame = new();

        public static T ExecuteNode<T>(this T selBehaviour, IDeprecateAction commandNode) where T : MonoBehaviour
        {
            selBehaviour.StartCoroutine(commandNode.Execute());
            return selBehaviour;
        }

        public static IEnumerator Execute(this IDeprecateAction selfNode)
        {
            if (selfNode.Finished) selfNode.Reset();

            while (!selfNode.Execute(Time.deltaTime)) yield return mEndOfFrame;
        }
    }

    public static class IActionChainExtention
    {
        public static IActionChain Repeat<T>(this T selfbehaviour, int count = -1) where T : MonoBehaviour
        {
            var retNodeChain = new RepeatNodeChain(count) { Executer = selfbehaviour };
            retNodeChain.DisposeWhenGameObjectDestroyed(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain Sequence<T>(this T selfbehaviour) where T : MonoBehaviour
        {
            var retNodeChain = new SequenceNodeChain { Executer = selfbehaviour };
            retNodeChain.DisposeWhenGameObjectDestroyed(selfbehaviour);
            return retNodeChain;
        }

        public static IActionChain OnlyBegin(this IActionChain selfChain, Action<OnlyBeginAction> onBegin)
        {
            return selfChain.Append(OnlyBeginAction.Allocate(onBegin));
        }


        /// <summary>
        ///     Same as Delayw
        /// </summary>
        /// <param name="senfChain"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static IActionChain Wait(this IActionChain senfChain, float seconds)
        {
            return senfChain.Append(DelayAction.Allocate(seconds));
        }

        public static IActionChain Event(this IActionChain selfChain, params Action[] onEvents)
        {
            return selfChain.Append(EventAction.Allocate(onEvents));
        }


        public static IActionChain Until(this IActionChain selfChain, Func<bool> condition)
        {
            return selfChain.Append(UntilAction.Allocate(condition));
        }
    }

    public interface IActionChain : IDeprecateAction
    {
        MonoBehaviour Executer { get; set; }

        IActionChain Append(IDeprecateAction node);

        IDisposeWhen Begin();
    }

    public abstract class ActionChain : ActionKitAction, IActionChain, IDisposeWhen
    {
        private Func<bool> mDisposeCondition;

        private bool mDisposeWhenCondition;
        private Action mOnDisposedEvent;

        protected abstract ActionKitAction mNode { get; }
        public MonoBehaviour Executer { get; set; }

        public abstract IActionChain Append(IDeprecateAction node);

        public IDisposeWhen Begin()
        {
            Executer.ExecuteNode(this);
            return this;
        }

        public IDisposeEventRegister DisposeWhen(Func<bool> condition)
        {
            mDisposeWhenCondition = true;
            mDisposeCondition = condition;
            return this;
        }

        IDisposeEventRegister IDisposeEventRegister.OnFinished(Action onFinishedEvent)
        {
            OnEndedCallback += onFinishedEvent;
            return this;
        }

        public void OnDisposed(Action onDisposedEvent)
        {
            mOnDisposedEvent = onDisposedEvent;
        }

        protected override void OnExecute(float dt)
        {
            if (mDisposeWhenCondition && mDisposeCondition != null && mDisposeCondition.Invoke())
                Finish();
            else
                Finished = mNode.Execute(dt);
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            Dispose();
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            Executer = null;
            mDisposeWhenCondition = false;
            mDisposeCondition = null;

            if (mOnDisposedEvent != null) mOnDisposedEvent.Invoke();

            mOnDisposedEvent = null;
        }
    }
}