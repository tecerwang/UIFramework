using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    public abstract class UIPopupBase : MonoBehaviour
    {
        /// <summary>
        /// Popup name
        /// </summary>
        public abstract string popupName { get; }

        /// <summary>
        /// paramaters
        /// </summary>
        public object[] paramaters;

        /// <summary>
        /// component is already instantiated,but not actived
        /// </summary>
        public abstract Task OnPopupGoingShow();

        /// <summary>
        /// component is already instantiated and actived, shown in game
        /// </summary>
        public abstract Task OnPopupShown();

        /// <summary>
        /// component is actived, and going to inactive self
        /// </summary>
        public abstract Task OnPopupGoingLeave();

        /// <summary>
        /// component is already inactived and going to destory
        /// </summary>
        public abstract Task OnPopupHidden();
    }
}
