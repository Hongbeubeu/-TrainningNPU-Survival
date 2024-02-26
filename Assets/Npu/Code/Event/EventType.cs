using System.Collections.Generic;

namespace Npu
{
    public enum EventType
    {
        LanguageChanged = -5,

        None = 0,

        #region Ads

        GdprChanged,
        MediationInitialized,
        RewardedAvailabilityChanged,
        AdsAudioStart,
        AdsAudioFinish,

        #endregion

        #region In App Purchase
        StoreInitialized,
        PurchaseStart,
        PurchaseSuccess,
        PurchaseFinish,

        IapRestoreStart,
        IapRestoreFinish,

        #endregion

        #region Popup & Sheet

        PopupChanged,
        SheetChanged,

        #endregion

        Bootstraped,

        #region Game Specific

        GoldChanged,
        LevelChanged,
        XpChanged,
        DiamondChanged,
        BizPointChanged,
        ReputationChanged,
        ChangeUserName,
        OpenOrCloseShop,
        #endregion        
    }

    public class EventTypeComparer : IEqualityComparer<EventType>
    {
        public bool Equals(EventType x, EventType y)
        {
            return x == y;
        }

        public int GetHashCode(EventType t)
        {
            return (int)t;
        }
    }

}