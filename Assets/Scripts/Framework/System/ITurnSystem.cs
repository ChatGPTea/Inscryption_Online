using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace Inscryption
{
    public enum TurnState
    {
        PlayerFirstTurnBegin,
        PlayerTurnBegin,
        AfterDrawCard,
        PlayerAttack,
        EnemyTurn,
        EnemyAttack,
        NoMatch,
    }

    public interface ITurnSystem : ISystem
    {
        BindableProperty<TurnState> State { get; set; }
        List<CardInSlot> EffectList { get; set; }
    }

    public class TurnSystem : AbstractSystem, ITurnSystem
    {
        public BindableProperty<TurnState> State { get; set; } = new BindableProperty<TurnState>();
        public List<CardInSlot> EffectList { get; set; } = new List<CardInSlot>();

        public bool HasDuringFirstTurn = false;

        protected override void OnInit()
        {
            //等有了判断再加上
            State.Value = TurnState.NoMatch;
            HasDuringFirstTurn = false;
            State.Register(newValue => { TurnStateManager(newValue); });
        }

        private void TurnStateManager(TurnState state)
        {
            var PlayerSlots = this.GetModel<ISlotsModel>().PlayerCardInSlotDic;
            var EnemySlots = this.GetModel<ISlotsModel>().EnemyCardInSlotDic;
            switch (state)
            {
                case TurnState.PlayerFirstTurnBegin:
                    break;
                case TurnState.PlayerTurnBegin:
                    var gameWnd = WndManager.Instance.GetWnd<GameWnd>();
                    gameWnd.PlayerTurnBegin();
                    gameWnd.mPlayerName.text = gameWnd.PlayerName + " 行动中";
                    gameWnd.mEnemyName.text = gameWnd.EnemyName;
                    this.GetModel<ISlotsModel>().OnTurnBegin(true);
                    break;
                case TurnState.AfterDrawCard:
                    break;
                case TurnState.PlayerAttack:
                    //进入玩家造物攻击阶段
                    //入队
                    EffectList.Clear();
                    for (var i = 0; i < PlayerSlots.Count; i++)
                    {
                        if (PlayerSlots[i]?.GetComponent<CardInSlot>() != null)
                        {
                            var card = PlayerSlots[i].GetComponent<CardInSlot>();
                            if (card.mData.Attack == 0) continue;
                            EffectList.Add(card);
                        }
                    }

                    if (EffectList.Count == 0)
                    {
                        State.Value = TurnState.EnemyTurn;
                        HasDuringFirstTurn = true;
                    }

                    //执行
                    for (var i = 0; i < EffectList.Count; i++)
                    {
                        var index = i;
                        if (i == EffectList.Count - 1)
                        {
                            //等最后一个造物攻击完毕
                            this.GetSystem<ITimeSystem>().AddDelayTask(i * 0.1f + 1.3f, () =>
                            {
                                this.GetModel<ISlotsModel>().OnTurnEnd(true);
                                State.Value = TurnState.EnemyTurn;
                                HasDuringFirstTurn = true;
                            });
                        }

                        this.GetSystem<ITimeSystem>().AddDelayTask(index * 0.1f, () =>
                        {
                            for (var j = 0; j < EffectList[index].Effects.Count; j++)
                            {
                                var Effect = EffectList[index].Effects[j];
                                Effect.Init();
                                Effect.OnAttackExecute();
                            }
                        });
                    }

                    break;
                case TurnState.EnemyTurn:
                    var gameWnd1 = WndManager.Instance.GetWnd<GameWnd>();
                    gameWnd1.mPlayerName.text = gameWnd1.PlayerName;
                    gameWnd1.mEnemyName.text = gameWnd1.EnemyName + " 行动中";
                    this.GetModel<ISlotsModel>().OnTurnBegin(false);
                    break;
                case TurnState.EnemyAttack:
                    //进入敌方造物攻击阶段
                    //入队
                    AudioKit.PlaySound("resources://Sound/Cards/combatbell_ring");
                    EffectList.Clear();

                    for (var i = 0; i < EnemySlots.Count; i++)
                    {
                        if (EnemySlots[i]?.GetComponent<CardInSlot>() != null)
                        {
                            var card = EnemySlots[i].GetComponent<CardInSlot>();
                            if (card.mData.Attack == 0) continue;
                            EffectList.Add(card);
                        }
                    }

                    if (EffectList.Count == 0)
                    {
                        if (HasDuringFirstTurn)
                        {
                            State.Value = TurnState.PlayerTurnBegin;
                        }
                        else
                        {
                            State.Value = TurnState.PlayerTurnBegin;
                            HasDuringFirstTurn = true;
                        }
                    }

                    //执行
                    for (var i = 0; i < EffectList.Count; i++)
                    {
                        var index = i;
                        if (i == EffectList.Count - 1)
                        {
                            //等最后一个造物攻击完毕
                            this.GetSystem<ITimeSystem>().AddDelayTask(i * 0.1f + 1.3f, () =>
                            {
                                if (HasDuringFirstTurn)
                                {
                                    State.Value = TurnState.PlayerTurnBegin;
                                }
                                else
                                {
                                    State.Value = TurnState.PlayerTurnBegin;
                                    HasDuringFirstTurn = true;
                                }

                                this.GetModel<ISlotsModel>().OnTurnEnd(false);
                            });
                        }

                        this.GetSystem<ITimeSystem>().AddDelayTask(index * 0.1f, () =>
                        {
                            for (var j = 0; j < EffectList[index].Effects.Count; j++)
                            {
                                var Effect = EffectList[index].Effects[j];
                                Effect.Init();
                                Effect.OnAttackExecute();
                            }
                        });
                    }

                    break;
                case TurnState.NoMatch:
                    break;
            }
        }
    }
}