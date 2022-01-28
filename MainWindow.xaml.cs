using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Color = System.Drawing.Color;

namespace TotalCopperArea
{
    public partial class MainWindow : Window
    {
        private CancellationTokenSource cts;
        private Progress<LoadedImage> progress = new Progress<LoadedImage>();

        private List<LoadedImage> _bitmapsList = new List<LoadedImage>();

        private const double dpi_1016 = 1616d;
        private const double dpi_508 = 408d;
        private const double dpi_300 = 148d;
        private const double dpi_254 = 104d;
        private const double dpi_127 = 25d;

        private bool showMoreInfo = false;

        public MainWindow()
        {
            InitializeComponent();

            progress.ProgressChanged += ReportProgress;
        }

        private void Button_SelectFiles_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".bmp";
            dlg.Filter = "Bitmap Files (*.bmp)|*.bmp";
            dlg.Multiselect = true;
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                TextBox_Bitmap1.Text = "";
                _bitmapsList.Clear();
                StackPanel_Container.Children.Clear();

                foreach (var item in dlg.FileNames)
                {
                    TextBox_Bitmap1.Text += item + "\n\n";

                    Bitmap image = new Bitmap(item);
                    LoadedImage newImage = new LoadedImage();

                    string[] nameOfBoard = item.Split('\\');
                    newImage.name = nameOfBoard[nameOfBoard.Length - 1];

                    if (image.HorizontalResolution == image.VerticalResolution)
                    {
                        newImage.image = image;

                        if (image.HorizontalResolution == 1016)
                        {
                            newImage.dpi = dpi_1016;
                            newImage.iDPI = image.HorizontalResolution;
                        } 
                        if (image.HorizontalResolution == 508)
                        {
                            newImage.dpi = dpi_508;
                            newImage.iDPI = image.HorizontalResolution;
                        }
                        if (image.HorizontalResolution == 300)
                        {
                            newImage.dpi = dpi_300;
                            newImage.iDPI = image.HorizontalResolution;
                        }
                        if (image.HorizontalResolution == 254)
                        {
                            newImage.dpi = dpi_254;
                            newImage.iDPI = image.HorizontalResolution;
                        }
                        if (image.HorizontalResolution == 127)
                        {
                            newImage.dpi = dpi_127;
                            newImage.iDPI = image.HorizontalResolution;
                        }  
                    }

                    newImage.totalPixels = (ulong)image.Height * (ulong)image.Width;

                    TextBlock mytext = new TextBlock();
                    newImage.myTextBlock = mytext;
                    StackPanel_Container.Children.Add(newImage.myTextBlock);

                    _bitmapsList.Add(newImage);
                }
            }
        }

        private async void Button_StartAll_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            await CalculusAsyncParallelAsync();
        }
        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            showMoreInfo = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            showMoreInfo = false;
        }

        private void ReportProgress(object sender, LoadedImage e)
        {
            double mm2 = e.pixWhite / e.dpi;
            double cm2 = mm2 / 100d;

            if (showMoreInfo)
            {
                e.myTextBlock.Text = $"Nazwa obrazu: {e.name}, DPI {e.iDPI}\n" +
                       $"Suma pixeli: {e.totalPixels} pole {e.totalPixels / e.dpi:0.00}mm2\n" +
                       $"Ilość czarnych pixeli: {e.pixBlack}\n" +
                       $"Ilość białych pixeli: {e.pixWhite}\n" +
                       $"Ukończone: {(double)e.pixelsCounted / (double)e.totalPixels * 100:0.00}%\n" +
                       $"Pole: {mm2:0.0000}mm2, {cm2:0.0000}cm2\n";
            }
            else
            {
                e.myTextBlock.Text = $"Nazwa obrazu: {e.name}\n" +
                       $"Ukończone: {(double)e.pixelsCounted / (double)e.totalPixels * 100:0.00}%\n" +
                       $"Pole: {mm2:0.0000}mm2, {cm2:0.0000}cm2\n";
            }
        }

        private async Task CalculusAsyncParallelAsync()
        {
            await Task.Run(() =>
            {
                Parallel.ForEach<LoadedImage>(_bitmapsList, (image) =>
                {
                    CalculatePixels(image, progress, cts.Token);
                });
            });
        }
        private void CalculatePixels(LoadedImage image, IProgress<LoadedImage> progress, CancellationToken cancellationToken)
        {
            for (int x = 0; x < image.image.Width; x++)
            {
                for (int y = 0; y < image.image.Height; y++)
                {
                    Color pixelColor = image.image.GetPixel(x, y);
                    if (pixelColor.ToArgb().Equals(Color.White.ToArgb()))
                        image.pixWhite++;
                    else
                        image.pixBlack++;

                    image.pixelsCounted = image.pixWhite + image.pixBlack;
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    image.pixWhite = 0;
                    image.pixBlack = 0;
                    image.pixelsCounted = 0;
                    break;
                }
               
                progress.Report(image);
            }
        }
    }
}
