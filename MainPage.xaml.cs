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

        float zoom_step = 0.1f;
        
        // This is actually scale
        float zoom = 0;

        // Ignore these
        float xOffset = 0f;
        float yOffset = 0f;

        
        public MainPage()
        {
            this.InitializeComponent();
            AdjustScrollBar();

        }

        private void AdjustScrollBar()
        {
            verticalScroll.Maximum = (scaledVideoHeight - clipHeight);
            horizontalScroll.Maximum = (scaledVideoWidth - clipWidth);
        }

        private void UpdateInfo()
        {
            mp.MediaPlayer.Size(0, ref vidWidth, ref vidHeight);

            InfoText.Text =
                $"Scale = {zoom}, ClipGeometry = {mp.MediaPlayer.CropGeometry}" + System.Environment.NewLine +
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

            y = (int)e.NewValue;
            mp.MediaPlayer.CropGeometry = $"{clipWidth}x{clipHeight}+{x}+{y}";
            UpdateInfo();
        }

        private void ScrollBar_ValueChanged_1(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (mp.MediaPlayer == null)
                return;

            x = (int)e.NewValue;
            mp.MediaPlayer.CropGeometry = $"{clipWidth}x{clipHeight}+{x}+{y}";
            UpdateInfo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ZOOM IN
            zoom = zoom + zoom_step;
            mp.MediaPlayer.Scale = zoom;
            AdjustScrollBar();
            UpdateInfo();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // ZOOM OUT
            zoom = zoom - zoom_step;
            mp.MediaPlayer.Scale = zoom;
            AdjustScrollBar();
            UpdateInfo();

        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            // RESET
            x = y = 0;
            zoom = 0;
            mp.MediaPlayer.CropGeometry = "";
            mp.MediaPlayer.Scale = zoom;
            AdjustScrollBar();
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
    }
}
