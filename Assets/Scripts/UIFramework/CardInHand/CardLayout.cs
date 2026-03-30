using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Inscryption
{
    public class CardLayout : InscryptionController
    {
        public float CardY;
        public List<Vector3> mTargetPositions = new();

        private readonly List<Transform> mCardList = new();
        private readonly List<Quaternion> mTargetRotations = new();
        

        public void Add(Transform card)
        {
            if (!mCardList.Contains(card))
            {
                mCardList.Add(card);
                SetLayout();
            }
        }

        public void Remove(Transform card)
        {
            if (mCardList.Contains(card))
            {
                mCardList.Remove(card);
                SetLayout();
            }
        }

        #region 设置卡牌布局

        public void SetLayout()
        {
            SetLineLayout();
            ApplyLayoutWithTween();
        }


        private void SetLineLayout()
        {
            mTargetPositions.Clear();
            mTargetRotations.Clear();

            var posOffset = (1f - mCardList.Count) * 1f / 2f;

            if (mCardList.Count == 1)
            {
                mTargetPositions.Add(new Vector3(0f, -CardY, 1f));
                mTargetRotations.Add(Quaternion.Euler(0f, 0f, 0f));
            }
            else if (mCardList.Count > 1)
            {
                for (var i = 0; i < mCardList.Count; i++)
                {
                    mTargetPositions.Add(new Vector3(
                        Mathf.Lerp(posOffset, -posOffset, i / (float)(mCardList.Count - 1)),
                        -CardY,
                        1f));
                    mTargetRotations.Add(Quaternion.Euler(0f, 0f, 0f));
                }
            }
        }

        private void ApplyLayoutWithTween()
        {
            for (var i = 0; i < mCardList.Count; i++)
                if (i < mTargetPositions.Count && i < mTargetRotations.Count && mCardList[i] != null)
                {
                    int index = i;
                    mCardList[i].transform.GetComponent<CardInHand>().CardIndex = index;
                    if (mCardList[i].transform.GetComponent<CardInHand>().mCurrentState == CardInHandState.Selected)
                    {
                        mCardList[i].transform.GetComponent<CardInHand>().Select();
                    }
                    mCardList[i].transform.GetComponent<CardInHand>().mCurrentState = CardInHandState.OnAnim;
                    mCardList[i].DOMove(mTargetPositions[i], 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
                    {
                        mCardList[index].transform.GetComponent<CardInHand>().mCurrentState = CardInHandState.Idle;
                    });
                }
        }

        #endregion
    }
}