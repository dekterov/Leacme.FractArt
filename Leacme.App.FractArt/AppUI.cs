// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Leacme.Lib.FractArt;

namespace Leacme.App.FractArt {

	public class AppUI {

		private StackPanel rootPan = (StackPanel)Application.Current.MainWindow.Content;
		private Library lib = new Library();

		private TextBox sizeWf = App.TextBox;
		private TextBox sizeHf = App.TextBox;
		private Image imgPrev = new Image();
		private (StackPanel holder, Slider slider, TextBlock value) slGran = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slFaul = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slCrac = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slIter = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slDir = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slLen = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slOct = App.HorizontalSliderWithValue;
		private (StackPanel holder, Slider slider, TextBlock value) slFreq = App.HorizontalSliderWithValue;
		private Button btGct = App.Button;
		private Button btPn = App.Button;
		private Button btMd = App.Button;
		private Button btFf = App.Button;
		private Button savBt = App.Button;

		public AppUI() {

			var blur1 = App.TextBlock;
			blur1.TextAlignment = TextAlignment.Center;
			blur1.Text = "Generate procedural images by adjusting their properties via sliders";

			var sizeHol = App.HorizontalStackPanel;
			sizeHol.HorizontalAlignment = HorizontalAlignment.Center;

			var slidHol = App.HorizontalStackPanel;
			slidHol.HorizontalAlignment = HorizontalAlignment.Center;

			var sizeWBl = App.TextBlock;
			sizeWBl.Text = "Width (pixels):";
			sizeWf.Width = 50;
			sizeWf.Text = "1280";
			var sizeHbl = App.TextBlock;
			sizeHbl.Text = "Height (pixels):";
			sizeHf.Width = 50;
			sizeHf.Text = "720";

			var granLb = App.TextBlock;
			granLb.Text = "Granularity %:";

			var faulLb = App.TextBlock;
			faulLb.Text = "Faults %:";

			var cracLb = App.TextBlock;
			cracLb.Text = "Cracked %:";

			var dirLb = App.TextBlock;
			dirLb.Text = "Crack Dir Change %:";

			var lenLb = App.TextBlock;
			lenLb.Text = "Crack Length %:";

			var octLb = App.TextBlock;
			octLb.Text = "Octaves %:";

			var freqLb = App.TextBlock;
			freqLb.Text = "Frequency %:";

			slGran.slider.Minimum = slFaul.slider.Minimum = slCrac.slider.Minimum = slIter.slider.Minimum = slDir.slider.Minimum = slLen.slider.Minimum = slOct.slider.Minimum = slFreq.slider.Minimum = 1;
			slGran.slider.Maximum = slFaul.slider.Maximum = slCrac.slider.Maximum = slIter.slider.Maximum = slDir.slider.Maximum = slLen.slider.Maximum = slOct.slider.Maximum = slFreq.slider.Maximum = 100;
			slGran.slider.Value = slFaul.slider.Value = slCrac.slider.Value = slIter.slider.Value = slDir.slider.Value = slLen.slider.Value = slOct.slider.Value = slFreq.slider.Value = 10;

			sizeHol.Children.AddRange(new List<IControl> { sizeWBl, sizeWf, sizeHbl, sizeHf, granLb, slGran.holder, faulLb, slFaul.holder, cracLb, slCrac.holder });
			slidHol.Children.AddRange(new List<IControl> { dirLb, slDir.holder, lenLb, slLen.holder, octLb, slOct.holder, freqLb, slFreq.holder });

			var scPr = App.TextBlock;
			scPr.TextAlignment = TextAlignment.Center;
			scPr.Text = "Scaled Preview:";

			imgPrev.Height = 300;
			imgPrev.Width = imgPrev.Height / GetHeightToWidthRatio();

			var btH = App.HorizontalStackPanel;
			btH.HorizontalAlignment = HorizontalAlignment.Center;

			btGct.Content = "Cellular Texture";
			btPn.Content = "Perlin Noise";
			btMd.Width = 150;
			btMd.Content = "Midpoint Displacement";
			btFf.Content = "Fault Formation";

			btH.Children.AddRange(new List<IControl> { btGct, btPn, btMd, btFf });

			btGct.Click += async (z, zz) => {
				await ExecuteGenerationLogicClick(zz);
			};
			btPn.Click += async (z, zz) => {
				await ExecuteGenerationLogicClick(zz);
			};
			btMd.Click += async (z, zz) => {
				await ExecuteGenerationLogicClick(zz);
			};
			btFf.Click += async (z, zz) => {
				await ExecuteGenerationLogicClick(zz);
			};

			var botHol = App.HorizontalStackPanel;
			botHol.HorizontalAlignment = HorizontalAlignment.Center;

			var randBt = App.Button;
			randBt.Content = "Randomize Properties";
			randBt.Width = 150;
			randBt.Click += (z, zz) => {
				slGran.slider.Value = new Random().Next(1, 100);
				slFaul.slider.Value = new Random().Next(1, 100);
				slCrac.slider.Value = new Random().Next(1, 100);
				slIter.slider.Value = new Random().Next(1, 100);
				slDir.slider.Value = new Random().Next(1, 100);
				slLen.slider.Value = new Random().Next(1, 100);
				slOct.slider.Value = new Random().Next(1, 100);
				slFreq.slider.Value = new Random().Next(1, 100);
			};

			savBt.Content = "Save image to...";
			savBt.IsEnabled = false;

			botHol.Children.AddRange(new List<IControl> { randBt, savBt });
			savBt.Click += async (z, zz) => {
				var savePath = await SaveFile();
				if (!string.IsNullOrWhiteSpace(savePath) && imgPrev?.Source != null) {
					imgPrev.Source.Save(savePath + ".jpg");
				}
			};

			Dispatcher.UIThread.InvokeAsync(() => {
				var ranBtnList = new List<IControl> { btGct, btPn, btMd, btFf };
				randBt.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
				ranBtnList.ElementAt(new Random().Next(ranBtnList.Count)).RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
			});

			rootPan.Children.AddRange(new List<IControl> { blur1, sizeHol, slidHol, btH, scPr, imgPrev, botHol });
		}

		private async Task ExecuteGenerationLogicClick(RoutedEventArgs zz) {
			if (IsDimensionsValid()) {
				((App)Application.Current).LoadingBar.IsIndeterminate = true;
				int width = int.Parse(sizeWf.Text);
				int height = int.Parse(sizeHf.Text);
				if (zz.Source.Equals(btGct)) {
					using (var ts = await lib.GetCellularTexture(width, height, (int)slGran.slider.Value)) {
						var tb = new Bitmap(ts);
						imgPrev.Source = tb;
					}
				} else if (zz.Source.Equals(btPn)) {
					using (var ts = await lib.GetPerlinNoise(width, height, (int)slOct.slider.Value, (int)slFreq.slider.Value)) {
						var tb = new Bitmap(ts);
						imgPrev.Source = tb;
					}
				} else if (zz.Source.Equals(btMd)) {
					using (var ts = await lib.GetMidpointDisplacement(width, height, (int)slCrac.slider.Value, (int)slIter.slider.Value, (int)slDir.slider.Value, (int)slLen.slider.Value)) {
						var tb = new Bitmap(ts);
						imgPrev.Source = tb;
					}
				} else if (zz.Source.Equals(btFf)) {
					using (var ts = await lib.GetFaultFormation(width, height, (int)slFaul.slider.Value)) {
						var tb = new Bitmap(ts);
						imgPrev.Source = tb;
					}
				}
			((App)Application.Current).LoadingBar.IsIndeterminate = false;
			}
			savBt.IsEnabled = true;
		}

		private bool IsDimensionsValid() {
			if (string.IsNullOrWhiteSpace(sizeWf.Text)
				|| string.IsNullOrWhiteSpace(sizeHf.Text)
				|| !int.TryParse(sizeWf.Text, out var outSizeWf)
				|| !int.TryParse(sizeHf.Text, out var outSizeHf)
				|| outSizeHf < 1 || outSizeHf > 5000
				|| outSizeWf < 1 || outSizeWf > 5000) {
				return false;
			} else return true;
		}

		private double GetHeightToWidthRatio() {
			if (IsDimensionsValid()) {
				return int.Parse(sizeHf.Text) / int.Parse(sizeWf.Text);
			} else return 1;
		}

		private async Task<string> SaveFile() {
			var dialog = new SaveFileDialog() {
				Title = "Save Generated Image",
				InitialDirectory = Directory.GetCurrentDirectory(),
			};
			var res = await dialog.ShowAsync(Application.Current.MainWindow);
			return (!string.IsNullOrWhiteSpace(res)) ? res : string.Empty;
		}
	}
}