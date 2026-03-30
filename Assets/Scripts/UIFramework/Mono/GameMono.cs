using System.Collections;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Inscryption
{
    public class GameMono : InscryptionController
    {
        //造成伤害相关
        private TMP_Text mPlayerHPTip;
        private TMP_Text mEnemyHPTip;
        private RectTransform coinPrefab;
        private Transform coinParent;

        public void Init()
        {
            mPlayerHPTip = transform.Find("HP/PlayerHPTip").GetComponent<TMP_Text>();
            mEnemyHPTip = transform.Find("HP/EnemyHPTip").GetComponent<TMP_Text>();
            coinParent = transform.Find("HP");
            coinPrefab = Resources.Load<RectTransform>("Prefabs/Coin");
            // ✅ 延迟调用，等 GameWnd 完全初始化后再处理血量
            this.GetSystem<ITimeSystem>().AddDelayTask(0.1f, () => 
            {
                this.GetModel<IRoomModel>().CurrentHealth.Value = 0;
                ReDrawHealth(0);
            });
        }

        public void PlayerAttack()
        {
            this.SendCommand<TurnPlayerAttackCommand>();
        }

        #region 血量显示

        public void GetDamage(Vector3 targetPos, int damage)
        {
            var _randomNum = Random.Range(1, 6);
            AudioKit.PlaySound("resources://Sound/Cards/drop" + _randomNum);

            var delay = 0.1f;
            
            this.GetSystem<ITimeSystem>().AddDelayTask(0.6f, () =>
            {
                ReDrawHealth(damage);
            });
            
            for (int i = 0; i < damage; i++)
            {
                var coin = Instantiate(coinPrefab, coinParent);
                coin.transform.localPosition =
                    new Vector3(Random.Range(-100f, 101f) + targetPos.x, Random.Range(-100f, 101f) + targetPos.y, 0);
                coin.transform.rotation = Quaternion.Euler(new Vector3(0, 0, Random.Range(-100f, 101f)));
                coin.transform.localScale = Vector3.zero;
                StartCoroutine(AnimateSingleCoinSimple(coin, i * delay));
            }
        }

        IEnumerator AnimateSingleCoinSimple(RectTransform coin, float delay)
        {
            if (coin == null) yield break;
            // 动画
            coin.transform.DOScale(new Vector3(0.65f,0.65f,0.65f), 0.3f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                coin.DOAnchorPos(coinParent.transform.localPosition, 0.6f).SetEase(Ease.OutQuart).SetDelay(delay).OnComplete(() =>
                {
                    coin.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        Destroy(coin.gameObject);
                    });
                });
                coin.DORotate(Vector3.zero, 0.6f).SetEase(Ease.OutBack).SetDelay(delay);
            });
        }

        public void ReDrawHealth(int damage)
        {
            var OldHP = this.GetModel<IRoomModel>().CurrentHealth.Value;
            int NewHP = -1000;
            this.GetModel<IRoomModel>().CurrentHealth.Value += damage;
            if (this.GetModel<IRoomModel>().CurrentHealth.Value >= this.GetModel<IRoomModel>().HealthCount.Value)
            {
                //胜利了
                NewHP = this.GetModel<IRoomModel>().HealthCount.Value;
                WndManager.Instance.OpenWnd<WinWnd>();
                WndManager.Instance.GetWnd<WinWnd>().SetFinishTip(true);
                GameNet.Instance.SyncResult();
            }
            else if (this.GetModel<IRoomModel>().CurrentHealth.Value <= -this.GetModel<IRoomModel>().HealthCount.Value)
            {
                //失败了
                NewHP = -this.GetModel<IRoomModel>().HealthCount.Value;

            }
            else
            {
                NewHP = this.GetModel<IRoomModel>().CurrentHealth.Value;
            }

            // 双方都是0的情况
            if (OldHP == 0 && NewHP == 0)
            {
                mPlayerHPTip.text = "0";
                mEnemyHPTip.text = "0";
                return;
            }
    
            // 优势在同一边（不需要切换显示）
            if ((OldHP >= 0 && NewHP >= 0) || (OldHP <= 0 && NewHP <= 0))
            {
                // 我方优势（正数区域）
                if (NewHP >= 0)
                {
                    mPlayerHPTip.DOCounter(OldHP, NewHP, 0.2f);
                    mEnemyHPTip.text = "0";
                }
                // 敌方优势（负数区域）
                else
                {
                    mEnemyHPTip.DOCounter(-OldHP, -NewHP, 0.2f);
                    mPlayerHPTip.text = "0";
                }
                return;
            }
    
            // 优势发生转移（从一边转到另一边）
            if (OldHP > 0 && NewHP < 0)
            {
                // 我方优势 → 敌方优势
                mPlayerHPTip.DOCounter(OldHP, 0, 0.1f).OnComplete(() =>
                {
                    mEnemyHPTip.DOCounter(0, -NewHP, 0.1f);
                });
            }
            else if (OldHP < 0 && NewHP > 0)
            {
                // 敌方优势 → 我方优势
                mEnemyHPTip.DOCounter(-OldHP, 0, 0.1f).OnComplete(() =>
                {
                    mPlayerHPTip.DOCounter(0, NewHP, 0.1f);
                });
            }
            else if (OldHP > 0 && NewHP == 0)
            {
                // 我方优势 → 平衡
                mPlayerHPTip.DOCounter(OldHP, NewHP, 0.2f);
                mEnemyHPTip.text = "0";
            }
            else if (OldHP < 0 && NewHP == 0)
            {
                // 敌方优势 → 平衡
                mEnemyHPTip.DOCounter(OldHP, NewHP, 0.2f);
                mPlayerHPTip.text = "0";
            }
            else if (OldHP == 0 && NewHP > 0)
            {
                // 平衡 → 我方优势
                mPlayerHPTip.DOCounter(OldHP, NewHP, 0.2f);
                mEnemyHPTip.text = "0";
            }
            else if (OldHP == 0 && NewHP < 0)
            {
                // 平衡 → 敌方优势
                mEnemyHPTip.DOCounter(OldHP, -NewHP, 0.2f);
                mPlayerHPTip.text = "0";

            }
            
        }

        #endregion
    }
}