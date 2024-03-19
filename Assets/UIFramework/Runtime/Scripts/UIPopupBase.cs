using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UIFramework
{
    public abstract class UIPopupBase : MonoBehaviour
    {
        public enum State
        {
            created,
            goingShow,
            shown,
            present,
            goingLeave,
            hidden
        }

        public State curState { get; private set; } = State.created;

        /// <summary>
        /// 等待直到达到 target 状态
        /// </summary>
        /// <param name="targetState"></param>
        /// <returns></returns>
        public async Task AwaitAfterTargetState(State targetState)
        {
            while (true)
            {
                if ((int)targetState <= (int)curState)
                {
                    return;
                }
                await MonoBehaviourHelper.AwaitNextFrame();
            }
        }

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

        public async Task UpdatePopupState(State state)
        {
            curState = state;
            switch (state)
            {
                case State.goingShow: await this.OnPopupGoingShow(); break;
                case State.shown:
                    {
                        await this.OnPopupShown();
                        this.curState = State.present;
                        break;
                    }
                case State.goingLeave: await this.OnPopupGoingLeave(); break;
                case State.hidden: await this.OnPopupHidden(); break;
            }
        }

        /// <summary>
        /// component is already instantiated,but not actived
        /// </summary>
        protected abstract Task OnPopupGoingShow();

        /// <summary>
        /// component is already instantiated and actived, shown in game
        /// </summary>
        protected abstract Task OnPopupShown();

        /// <summary>
        /// component is actived, and going to inactive self
        /// </summary>
        protected abstract Task OnPopupGoingLeave();

        /// <summary>
        /// component is already inactived and going to destory
        /// </summary>
        protected abstract Task OnPopupHidden();

        /// <summary>
        /// close and destory popup by itself
        /// </summary>
        /// <returns></returns>
        protected async Task<bool> DestroyPopup()
        {
            if (screen != null)
            {
                return await screen.DestroyPopup(this);
            }
            return false;
        }
    }
}
