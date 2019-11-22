﻿using LazyScreen.ImageSources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LazyScreen
{
    public partial class Form1 : Form
    {
        private const string PATH = "image.bmp";

        private bool isClosing = false;
        private HttpClient httpClient;
        private (string Image, Wallpaper.Style Style) OrginalSettings;
        private Random rnd;

        public Form1()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;
            httpClient = new HttpClient();
            rnd = new Random();

            imageSources.Items.AddRange(
                Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(x => !x.IsInterface && x.GetInterfaces().Any(y => y == typeof(IImageSource)))
                .Select(x => new FoundImageSource(x))
                .ToArray());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                OrginalSettings = Wallpaper.Get();
                var fileInfo = new FileInfo(PATH);
                if (fileInfo.Exists && OrginalSettings.Image == fileInfo.FullName)
                {
                    string newPath = "";
                    int count = 0;
                    do
                    {
                        newPath = Path.Combine(fileInfo.DirectoryName, "backup" + (count == 0 ? "" : count.ToString()) + "_" + fileInfo.Name);
                        count++;
                    }
                    while (File.Exists(newPath));

                    File.Copy(OrginalSettings.Image, newPath);
                    OrginalSettings.Image = newPath;
                }

                timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Konnte Hintergrund nicht sichern");
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.Visible)
                this.HideForm();
            else
                this.ShowForm();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
                this.HideForm();
        }

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.ShowForm();
        }

        public void ShowForm()
        {
            this.Show();
            this.BringToFront();
        }

        public void HideForm()
        {
            this.Hide();
            this.notifyIcon.ShowBalloonTip(150);
        }

        public async Task DownloadImage(IEnumerable<IImageSource> imageSources)
        {
            try
            {
                var source = imageSources.ElementAtOrDefault(rnd.Next(0, imageSources.Count()));

                if (source is null)
                    return;

                var img = await source.GetImageUrl(httpClient);
                using (var stream = await httpClient.GetStreamAsync(img))
                using (var bmp = new Bitmap(stream))
                    bmp.Save("tmp_file", ImageFormat.Bmp);

                File.Copy("tmp_file", PATH, true);
            }
            finally
            {
                if (File.Exists("tmp_file"))
                    File.Delete("tmp_file");
            }
        }

        private async void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();

                if (comboBox1.SelectedIndex == -1)
                    return;

                await DownloadImage(imageSources.SelectedItems.Cast<FoundImageSource>().Select(x => x.Instance).Cast<IImageSource>());

                if (!isClosing)
                    Wallpaper.Set(new FileInfo(PATH).FullName, (Wallpaper.Style)comboBox1.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Timer Tick");
            }
            finally
            {
                timer.Start();
            }
        }

        private void numericUpDown_ValueChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Interval = (int)TimeSpan.FromHours((int)numericUpDownStunde.Value)
                .Add(TimeSpan.FromMinutes((int)numericUpDownMinute.Value))
                .Add(TimeSpan.FromSeconds((int)numericUpDownSekunde.Value)).TotalMilliseconds;
            timer.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                httpClient?.Dispose();

                isClosing = true;
                timer.Stop();

                Wallpaper.Set(OrginalSettings.Image, OrginalSettings.Style);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Konnte orginal Hintergrund nicht wiederherstellen");
            }
        }
    }
}
