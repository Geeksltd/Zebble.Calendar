using System.Threading.Tasks;

namespace Zebble
{
    partial class Calendar
    {
        class HeaderView : Stack
        {
            TextView Title;
            TextView Previous;
            TextView Next;

            public AsyncEvent TitleTapped;
            public AsyncEvent PreviousTapped;
            public AsyncEvent NextTapped;

            public string TitleText
            {
                get => Title.Text;
                set => Title.Text = value ?? Title.Text;
            }

            public string PreviousText
            {
                get => Previous.Text;
                set => Previous.Text = value ?? Previous.Text;
            }

            public string NextText
            {
                get => Next.Text;
                set => Next.Text = value ?? Next.Text;
            }

            public bool TitleEnabled
            {
                get => Title.Enabled;
                set => Title.Enabled = value;
            }

            public bool PreviousEnabled
            {
                get => Previous.Enabled;
                set => Previous.Enabled = value;
            }

            public bool NextEnabled
            {
                get => Next.Enabled;
                set => Next.Enabled = value;
            }

            public bool TitleVisible
            {
                get => Title.Visible;
                set => Title.Visible = value;
            }

            public bool PreviousVisible
            {
                get => Previous.Visible;
                set => Previous.Visible = value;
            }

            public bool NextVisible
            {
                get => Next.Visible;
                set => Next.Visible = value;
            }

            public HeaderView()
            {
                Direction = RepeatDirection.Horizontal;
                Id = "Header";

                Previous = new TextView { Text = "❰", Id = "Previous" };
                Title = new TextView { Id = "Title" };
                Next = new TextView { Text = "❱", Id = "Next" };

                TitleTapped = new AsyncEvent();
                PreviousTapped = new AsyncEvent();
                NextTapped = new AsyncEvent();

                Title.Tapped.HandleWith(() => TitleTapped.Raise());
                Previous.Tapped.HandleWith(() => PreviousTapped.Raise());
                Next.Tapped.HandleWith(() => NextTapped.Raise());
            }

            public override async Task OnInitializing()
            {
                await base.OnInitializing();
                await Add(Previous);
                await Add(Title);
                await Add(Next);
            }
        }
    }
}