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
        public object[] parameters;

        /// <summary>
        /// the screen witch hold this popup
        /// </summary>
        public UIScreenBase screen { get; internal set; }

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

        /// <summary>
        /// close and destory popup by itself
        /// </summary>
        /// <returns></returns>
        public virtual async Task DestroyPopup()
        {
            if (screen != null)
            {
                await screen.DestroyPopup(this);
            }
        }
    }
}
