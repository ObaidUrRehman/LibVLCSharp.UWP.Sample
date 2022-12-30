using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace LibVLCSharp.UWP.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        int x = 0;
        int y = 0;

        // The actual dimensions of the video hardcoded for now but
        // could be retrived from the video. This is also the clip rectangle for the crop geometry
        int clipWidth = 1280;
        int clipHeight = 534;

        private uint vidWidth;
        private uint vidHeight;


        // The scaled dimensions after zoom
        int scaledVideoHeight => (int)(clipHeight * zoom);
        int scaledVideoWidth => (int)(clipWidth * zoom);

        float zoomFactor = 0f;

        // This is actually scale
        float zoom = 0;

        // Ignore these
        float xOffset = 0f;
        float yOffset = 0f;


        System.Drawing.Rectangle clipRect = new System.Drawing.Rectangle(0, 0, 640, 360);



        public MainPage()
        {
            this.InitializeComponent();
            //AdjustScrollBar();

        }

        private void AdjustScrollBar()
        {


            // Get video dimensions
            mp.MediaPlayer.Size(0, ref vidWidth, ref vidHeight);

            verticalScroll.Maximum = (vidHeight - clipRect.Height);
            horizontalScroll.Maximum = (vidWidth - clipRect.Width);



        }

        private void UpdateInfo()
        {
            mp.MediaPlayer.Size(0, ref vidWidth, ref vidHeight);

            InfoText.Text =
                $"Scale = {mp.MediaPlayer.Scale}, ClipGeometry = {mp.MediaPlayer.CropGeometry}" + System.Environment.NewLine +
                $"Video Dimensions = {zoom * clipWidth } x { zoom * clipHeight}" + System.Environment.NewLine +
                $"Video Dimensions From Player = {vidWidth } x { vidHeight}" + System.Environment.NewLine +
                $"H.Scroll = {horizontalScroll.Value} / {horizontalScroll.Maximum}" + System.Environment.NewLine +
                $"V.Scroll = {verticalScroll.Value} / {verticalScroll.Maximum}" + System.Environment.NewLine +
                $"Offset (x, y) = {xOffset} , {yOffset}" + System.Environment.NewLine;
        }

        private void ScrollBar_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (mp.MediaPlayer == null)
                return;

            clipRect.Y = (int)e.NewValue;

            ApplyCrop();
            UpdateInfo();

        }

        private void ScrollBar_ValueChanged_1(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (mp.MediaPlayer == null)
                return;

            clipRect.X = (int)e.NewValue;

            ApplyCrop();
            UpdateInfo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ZOOM IN
            zoomFactor = (float)(zoomFactor + 0.25);

            // Use scale to Zoom in
            if (zoomFactor < 0)
            {
                mp.MediaPlayer.Scale = (float)(mp.MediaPlayer.Scale) + System.Math.Abs(zoomFactor);
            }
            else if (zoomFactor == 0)
            {
                mp.MediaPlayer.Scale = 0;
            }
            else // Use clip geometry to Zoom in
            {
                mp.MediaPlayer.Size(0, ref vidWidth, ref vidHeight);

                // max zoom in check
                if ((int)vidWidth - (vidWidth * zoomFactor) == 0)
                {
                    //revert zoom step
                    zoomFactor = (float)(zoomFactor - 0.25);
                    return;
                }

                
                clipRect = new System.Drawing.Rectangle(0, 0,
                    (int)((int)vidWidth - (vidWidth * zoomFactor)),
                    (int)((int)vidHeight - (vidHeight * zoomFactor)));

                ApplyCrop();
                AdjustScrollBar();
            }

            UpdateInfo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // ZOOM OUT
            zoomFactor = (float)(zoomFactor - 0.25);

            mp.MediaPlayer.Size(0, ref vidWidth, ref vidHeight);

            // Zoom out using clip geometry
            if (clipRect.Width < vidWidth && clipRect.Height < vidHeight)
            {
                if (zoomFactor == 0)
                    clipRect = new System.Drawing.Rectangle(0, 0,
                    (int)(vidWidth),
                    (int)(vidHeight));

                else
                    clipRect = new System.Drawing.Rectangle(0, 0,
                        (int)(clipRect.Width + (vidWidth * zoomFactor)),
                        (int)(clipRect.Height + (vidHeight * zoomFactor)));
                
                ApplyCrop();
            }
            else // zoom out using scale
            {
                double initialScaleFactor = 0;
                if (vidHeight * vidWidth > mp.ActualHeight * mp.ActualWidth)
                {
                    initialScaleFactor = ((mp.ActualHeight * mp.ActualWidth) / (vidHeight * vidWidth)) * 100;
                }
                else if (mp.ActualHeight * mp.ActualWidth > vidHeight * vidWidth)
                {
                    initialScaleFactor = ((vidHeight * vidWidth) / (mp.ActualHeight * mp.ActualWidth)) * 100;
                }
                else if (mp.ActualWidth == vidWidth && mp.ActualHeight == vidHeight)
                {
                    initialScaleFactor = 1;
                }


                if (mp.MediaPlayer.Scale == 0f)
                {
                    mp.MediaPlayer.Scale = (float)(initialScaleFactor);
                }
                else
                {
                    mp.MediaPlayer.Scale = mp.MediaPlayer.Scale + zoomFactor;
                }
            }

            UpdateInfo();
            AdjustScrollBar();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            // RESET
            x = y = 0;
            zoom = 0;
            zoomFactor = 0;
            mp.MediaPlayer.CropGeometry = "";
            mp.MediaPlayer.Scale = zoom;
            UpdateInfo();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {

            if (mp.MediaPlayer.State == Shared.VLCState.Playing)
                mp.MediaPlayer.Pause();
            else
                mp.MediaPlayer.Play();

            UpdateInfo();

        }

        private void ButtonUp_Click(object sender, RoutedEventArgs e)
        {


        }

        private void ButtonDown_Click(object sender, RoutedEventArgs e)
        {
            clipRect.Y = clipRect.Y - 20;
            ApplyCrop();
        }

        private void ApplyCrop()
        {
            mp.MediaPlayer.CropGeometry = $"{clipRect.Width + clipRect.X}x{clipRect.Height + clipRect.Y}+{clipRect.X}+{clipRect.Y}";
            UpdateInfo();
        }

        private void scale_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            mp.MediaPlayer.Scale = (float)e.NewValue;
            UpdateInfo();
        }
    }
}
