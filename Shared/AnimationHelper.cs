using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        class AnimationHelper
        {
            Animation EnterAnimation = null, ExitAnimation = null;
            View ParentView, FromView, ToView;
            AnimationType Type;
            const int NEXT_DURATION = 1000;
            const int PREV_DURATION = 1000;
            const int CHANGE_DURATION = 1000;
            public AnimationHelper(View parentView, View fromView, View toView, AnimationType type)
            {
                ParentView = parentView;
                FromView = fromView;
                ToView = toView;
                Type = type;
            }
            public async Task Run()
            {
                (FromView as Page).Perform(x => x.IsNavigatedAwayFrom = ParentView == View.Root);
                (ToView as Page).Perform(x => x.IsNavigatedAwayFrom = false);

                switch (Type)
                {
                    case AnimationType.NextPage: await SlideForward(); break;
                    case AnimationType.PreviousPage: await SlideBack(); break;
                    case AnimationType.Change: await Fade(); break;
                    default: await None(); break;
                }

                await Await();
            }
            async Task SlideForward()
            {
                if (ToView != ParentView)
                    EnterAnimation = await ParentView.AddWithAnimation(ToView,
                        m => m.Y(0).X(Device.Screen.Width),
                        m => m.X(0));

                if (FromView != ParentView)
                    ExitAnimation = Animation.Create(FromView, x => x.X(-Device.Screen.Width));
            }

            async Task SlideBack()
            {
                if (ToView != ParentView)
                    EnterAnimation = await ParentView.AddWithAnimation(ToView,
                        m => m.Y(0).X(-Device.Screen.Width),
                        m => m.X(0));

                if (FromView != ParentView)
                    ExitAnimation = Animation.Create(FromView, x => x.X(Device.Screen.Width));
            }

            async Task Fade()
            {
                if (ToView != ParentView)
                    EnterAnimation = await ParentView.AddWithFadeIn(ToView);
            }
            async Task None()
            {
                if (ToView == ParentView) return;

                await UIWorkBatch.Run(() => ParentView.Add(ToView, awaitNative: true));
            }

            async Task Await()
            {
                if (EnterAnimation != null)
                {
                    var exitAni = Task.CompletedTask;

                    if (ExitAnimation != null)
                        exitAni = EnterAnimation.OnNativeStart(() => FromView.Animate(ExitAnimation));
                    await EnterAnimation.Task;

                    await exitAni;
                }
                else if (ExitAnimation != null) await FromView.Animate(ExitAnimation);
            }

        }

    }
    
}
