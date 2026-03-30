using DG.Tweening;
using Inscryption;
using QFramework;
using UnityEngine;

public class CardInHandManager : InscryptionController
{
    public bool isHideEnemyCardInfo;

    private GameObject mCardInHand;
    public CardLayout mEnemyCardLayout;
    private RectTransform mEnemyCardSummonPoint;
    public CardLayout mPlayerCardLayout;
    private RectTransform mPlayerMainCardSummonPoint;
    private RectTransform mPlayerSecondCardSummonPoint;

    private void Start()
    {
        mPlayerCardLayout = transform.Find("PlayerCardInHandGroup").GetComponent<CardLayout>();
        mEnemyCardLayout = transform.Find("EnemyCardInHandGroup").GetComponent<CardLayout>();
        mPlayerMainCardSummonPoint = transform.Find("MainCards/Card_1/CardView").GetComponent<RectTransform>();
        mPlayerSecondCardSummonPoint = transform.Find("SecondCards/Card_2/CardView").GetComponent<RectTransform>();
        mEnemyCardSummonPoint = transform.Find("EnemyCardSummonPoint").GetComponent<RectTransform>();

        mCardInHand = Resources.Load<GameObject>("Prefabs/Card_InHand");
        this.SendCommand<FirstInitGameWndCommand>();
    }

    #region 抽卡动画

    /// <summary>
    ///     用于生成一张卡添加到手牌
    /// </summary>
    public void PlayerDrawMainCard(int cardID)
    {
        var _randomNum = Random.Range(1, 5);
        AudioKit.PlaySound("resources://Sound/Cards/cardquick" + _randomNum);
        //生成卡牌并向下移动一段距离
        var CardInHand = Instantiate(mCardInHand, mPlayerCardLayout.transform);
        GameNet.Instance.SyncDrawCard(cardID);
        this.SendCommand(new AddCardToPlayerCardListCommand(CardInHand, false));
        CardInHand.GetComponent<CardInHand>().Init(cardID);
        CardInHand.GetComponent<CardInHand>().isMycard = true;
        //动画逻辑
        CardInHand.transform.position = new Vector3(mPlayerMainCardSummonPoint.position.x,
            mPlayerMainCardSummonPoint.position.y, mPlayerMainCardSummonPoint.position.z);
        var moveDistance = 1f;
        var startPosition = mPlayerMainCardSummonPoint;
        var endPosition = new Vector3(startPosition.position.x, startPosition.position.y - moveDistance,
            startPosition.position.z);
        CardInHand.transform.DOMove(endPosition, 0.5f)
            .OnComplete(() => { mPlayerCardLayout.Add(CardInHand.transform); });

    }

    /// <summary>
    ///     用于抽取一张卡添加到手牌
    /// </summary>
    public void PlayerDrawMainCard()
    {
        var _randomNum = Random.Range(1, 5);
        AudioKit.PlaySound("resources://Sound/Cards/cardquick" + _randomNum);
        if (this.GetSystem<ICardSystem>().mPlayerCardIDList.Count == 0)
        {
            WndManager.Instance.GetWnd<GameWnd>().TryDrawCardWhenCardListEmpty();
            return;
        }
        if (this.GetSystem<ICardSystem>().mPlayerCardIDList == null) return;
        var cardID = this.GetSystem<ICardSystem>().mPlayerCardIDList[0];
        this.GetSystem<ICardSystem>().mPlayerCardIDList.RemoveAt(0);
        GameNet.Instance.SyncDrawCard(cardID);
        //生成卡牌并向下移动一段距离
        var CardInHand = Instantiate(mCardInHand, mPlayerCardLayout.transform);
        this.SendCommand(new AddCardToPlayerCardListCommand(CardInHand, true));
        CardInHand.GetComponent<CardInHand>().Init(cardID);
        CardInHand.GetComponent<CardInHand>().isMycard = true;
                
        //动画逻辑
        CardInHand.transform.position = new Vector3(mPlayerMainCardSummonPoint.position.x,
            mPlayerMainCardSummonPoint.position.y, mPlayerMainCardSummonPoint.position.z);
        var moveDistance = 1f;
        var startPosition = mPlayerMainCardSummonPoint;
        var endPosition = new Vector3(startPosition.position.x, startPosition.position.y - moveDistance,
            startPosition.position.z);
        CardInHand.transform.DOMove(endPosition, 0.5f)
            .OnComplete(() => { mPlayerCardLayout.Add(CardInHand.transform); });
        
    }

    /// <summary>
    ///     用于生成一张卡添加到手牌
    /// </summary>
    public void PlayerDrawSecondCard(int cardID)
    {
        var _randomNum = Random.Range(1, 5);
        AudioKit.PlaySound("resources://Sound/Cards/cardquick" + _randomNum);
        //生成卡牌并向下移动一段距离
        GameNet.Instance.SyncDrawCard(cardID);
        var CardInHand = Instantiate(mCardInHand, mPlayerCardLayout.transform);
        this.SendCommand(new AddCardToPlayerCardListCommand(CardInHand, false));
        CardInHand.GetComponent<CardInHand>().Init(cardID);
        CardInHand.GetComponent<CardInHand>().isMycard = true;
                
        //动画逻辑
        CardInHand.transform.position = new Vector3(mPlayerSecondCardSummonPoint.position.x,
            mPlayerSecondCardSummonPoint.position.y, mPlayerSecondCardSummonPoint.position.z);
        var moveDistance = 1f;
        var startPosition = mPlayerSecondCardSummonPoint;
        var endPosition = new Vector3(startPosition.position.x, startPosition.position.y - moveDistance,
            startPosition.position.z);
        CardInHand.transform.DOMove(endPosition, 0.5f)
            .OnComplete(() => { mPlayerCardLayout.Add(CardInHand.transform); });
        
    }

    /// <summary>
    ///     用于抽取一张卡添加到手牌
    /// </summary>
    public void PlayerDrawSecondCard()
    {
        var _randomNum = Random.Range(1, 5);
        AudioKit.PlaySound("resources://Sound/Cards/cardquick" + _randomNum);
        var cardID = this.GetSystem<ICardSystem>().mPlayerSecondCardID;
        GameNet.Instance.SyncDrawCard(cardID);
        //生成卡牌并向下移动一段距离
        var CardInHand = Instantiate(mCardInHand, mPlayerCardLayout.transform);
        this.SendCommand(new AddCardToPlayerCardListCommand(CardInHand, false));
        CardInHand.GetComponent<CardInHand>().Init(cardID);
        CardInHand.GetComponent<CardInHand>().isMycard = true;
                
        //动画逻辑
        CardInHand.transform.position = new Vector3(mPlayerSecondCardSummonPoint.position.x,
            mPlayerSecondCardSummonPoint.position.y, mPlayerSecondCardSummonPoint.position.z);
        var moveDistance = 1f;
        var startPosition = mPlayerSecondCardSummonPoint;
        var endPosition = new Vector3(startPosition.position.x, startPosition.position.y - moveDistance,
            startPosition.position.z);
        CardInHand.transform.DOMove(endPosition, 0.5f)
            .OnComplete(() => { mPlayerCardLayout.Add(CardInHand.transform); });
        
    }

    /// <summary>
    ///     用于生成敌方卡片
    /// </summary>
    public void EnemyDrawCard(int cardID)
    {
        var _randomNum = Random.Range(1, 5);
        AudioKit.PlaySound("resources://Sound/Cards/cardquick" + _randomNum);
        //生成卡牌并向下移动一段距离
        var CardInHand = Instantiate(mCardInHand, mEnemyCardLayout.transform);
        //初始化逻辑
        CardInHand.GetComponent<CardInHand>().Init(cardID);
        CardInHand.GetComponent<CardInHand>().Back.gameObject.SetActive(true);
        CardInHand.GetComponent<CardInHand>().isMycard = false;
                
        //动画逻辑
        CardInHand.transform.position = new Vector3(mEnemyCardSummonPoint.position.x, mEnemyCardSummonPoint.position.y,
            mEnemyCardSummonPoint.position.z);
        var moveDistance = -1f;
        var startPosition = mEnemyCardSummonPoint;
        var endPosition = new Vector3(startPosition.position.x, startPosition.position.y - moveDistance,
            startPosition.position.z);
        CardInHand.transform.DOMove(endPosition, 0.5f)
            .OnComplete(() => { mEnemyCardLayout.Add(CardInHand.transform); });
        
        this.SendCommand(new AddCardToEnemyListCommand(CardInHand));
        this.SendCommand(new RemoveCardToPlayerCardListCommand(CardInHand));
    }

    #endregion
}