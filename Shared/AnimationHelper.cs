using System;
using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        class AnimationHelper
        {
            Animation EnterAnimation, ExitAnimation;
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
                        m => m.Y(0).X(ParentView.ActualWidth),
                        m => m.X(0));

                if (FromView != ParentView)
                    ExitAnimation = Animation.Create(FromView, x => x.X(-ParentView.ActualWidth));
            }

            async Task SlideBack()
            {
                if (ToView != ParentView)
                    EnterAnimation = await ParentView.AddWithAnimation(ToView,
                        m => m.Y(0).X(-ParentView.ActualWidth),
                        m => m.X(0));

                if (FromView != ParentView)
                    ExitAnimation = Animation.Create(FromView, x => x.X(ParentView.ActualWidth));
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