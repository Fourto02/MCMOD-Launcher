using System;
using System.Runtime;
using System.Net.Http;
using System.Windows;
using System.Windows.Input;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.IO.Compression;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Threading;
using System.Data;
using System.Windows.Media.Animation;


namespace FourtoMC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum LauncherStatus
    {
        Download,
        Updates,
        ready,
        failed,
        downloadingGame,
        downloadingUpdate
    }
    public partial class MainWindow : Window
    {
        //create variable
        private string rootPath;
        private string updatePath;
        private string versionGameFile;
        private string versionUpdate;
        private string gameZip;
        private string updateZip;
        private string gameExe;
        private int fileSize;
        private int downloadingSize;
        private string gameUrl;
        private string cmd;
        private string cmdlocal;
        private string updateUrl;
        private string urlVersion;
        private string urlVersionUpdate;
        private string gameFolder;
        private string homepage;
        private double maxsize;
        private LauncherStatus _status;
        static readonly HttpClient htc = new HttpClient();
        static readonly string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        static readonly string[] SizeType = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        private bool _stop;
        private string folder;
        internal LauncherStatus Status
        {


            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.Download:
                        Status_txt.Text = "ดาวน์โหลดเลย!";
                        break;
                    case LauncherStatus.Updates:
                        Status_txt.Text = "อัปเดต";
                        break;
                    case LauncherStatus.ready:
                        Status_txt.Text = "เล่น";
                        break;
                    case LauncherStatus.failed:
                        Status_txt.Text = "อัปเดตไม่สำเร็จ ลองใหม่อีกครั้ง";
                        break;
                    case LauncherStatus.downloadingGame:
                        Status_txt.Text = "กำลังดาวน์โหลด";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        Status_txt.Text = "กำลังอัปเดต";
                        break;
                    default:
                        break;
                }
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            //set variable
            rootPath = Directory.GetCurrentDirectory();
            homepage = "https://www.dropbox.com/scl/fi/5aerp7mspw6rojqyl2iwy/updatelist.txt?rlkey=9aohf07aw3vaffhrprrauhusk&st=cns32cdl&dl=0&raw=1";
            folder = "FourtoMC";
            String ExcName = "FourtoMC";
            updatePath = Path.Combine(rootPath,folder);
            versionGameFile = Path.Combine(rootPath,"Version.txt");
            versionUpdate = Path.Combine(rootPath,"Update.txt");
            gameZip = Path.Combine(rootPath, folder + ".zip");
            updateZip = Path.Combine(rootPath, "Update.zip");
            gameFolder = folder;
            gameExe = Path.Combine(rootPath,folder, ExcName + ".exe");
            updateUrl = "https://www.dropbox.com/scl/fi/hrhna3vomg69bfwx9obgm/mods.zip?rlkey=ih3f1dpuqupyszpcew6fper9w&st=b8efuz18&dl=1";
            gameUrl = "https://www.dropbox.com/scl/fi/ulqacf0awbwjblil2u1wf/FourtoMC.zip?rlkey=wn1hqrupjivj81zmakwlxvn44&st=17es1h19&dl=1";
            urlVersion = "https://www.dropbox.com/scl/fi/by9c7mcbhxicw2bu33adl/Version.txt?rlkey=diiwrzehsx63loitygq3acx9p&st=f23hdynz&dl=1";
            urlVersionUpdate = "https://www.dropbox.com/scl/fi/74r28fiqyytzz8v3msnl5/Update.txt?rlkey=hvg9mmekyiq0j9t5242rgwxg6&st=4rsirdlq&dl=1";
            cmdlocal = Path.Combine(rootPath, "csupdater.bat"); 
            cmd = "";

        }
        static string SizeSuffix(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }


            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);


            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));


            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }


            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeSuffixes[mag]);
        }
        static string SizeLoading(Int64 value, int decimalPlaces = 1)
        {
            if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if (value < 0) { return "-" + SizeLoading(-value, decimalPlaces); }
            if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }


            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);


            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));


            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }


            return string.Format("{0:n" + decimalPlaces + "} {1}",
                adjustedSize,
                SizeType[mag]);
        }








        private void Window_ContentRendered(object sender, EventArgs e)
        {
            CheckVersion();
            if (File.Exists(cmdlocal))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(cmdlocal);
                startInfo.WorkingDirectory = Path.Combine(rootPath);
                Process.Start(startInfo);
            }
        }




        private void Install()
        {
            if (File.Exists(versionUpdate))
            {
                Version localVersion = new Version(File.ReadAllText(versionUpdate));


                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString(urlVersionUpdate));


                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        InstallGameFiles(true, onlineVersion);
                    }
                    else
                    {
                        Status = LauncherStatus.ready;
                        Downloadinfo_DB.Visibility = Visibility.Hidden;
                    }
                }
                catch (Exception ex)
                {
                    Status = LauncherStatus.failed;
                    MessageBox.Show($"Error checking for game updates: {ex}");
                }
            }
            else
            {
                InstallGameFiles(false, Version.zero);
            }
        }


        private void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient wc = new WebClient();
                if (_isUpdate)
                {
                    Status = LauncherStatus.downloadingUpdate;
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(UpdateGameCompletedCallback);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChanged);
                    wc.DownloadFileAsync(new Uri(updateUrl), updateZip, _onlineVersion);
                }
                else
                {
                    Status = LauncherStatus.downloadingGame;
                    _onlineVersion = new Version(wc.DownloadString(urlVersion));
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChanged);
                    wc.DownloadFileAsync(new Uri(gameUrl), gameZip, _onlineVersion);
                }
            }


            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }


        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                unzip(gameFolder + ".zip", rootPath);
                File.Delete(gameZip);
                File.WriteAllText(versionGameFile, onlineVersion);
                Status = LauncherStatus.ready;
                CheckUpdate();
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
            }
            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, folder);
                Process.Start(startInfo);

                Close();
            }
        }


        void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            DownloadStatus_txt.Text = "กำลังดาวน์โหลด";
            downloadingSize = ((int)e.BytesReceived);
            int Bytes = 0;
            SizeLoading(downloadingSize, Bytes);
            DownloadingSize_Text.Text = SizeSuffix(downloadingSize);
            DownloadPercent_Text.Text = e.ProgressPercentage.ToString();
            DownloadPercent_PgBar.Value = ((float)downloadingSize / e.TotalBytesToReceive);


        }


        private void UpdateGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                File.Delete("mods");
                unzip("Update.zip", updatePath);
                File.Delete(updateZip);
                File.WriteAllText(versionUpdate, onlineVersion);
                Status = LauncherStatus.ready;
                Downloadinfo_DB.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
                Downloadinfo_DB.Visibility = Visibility.Hidden;
            }
        }


        private void unzip(string filename, string path)
        {
            using (ZipArchive source = ZipFile.Open(filename, ZipArchiveMode.Read, null))
            {
                foreach (ZipArchiveEntry entry in source.Entries)
                {
                    string fullPath = Path.GetFullPath(Path.Combine(path, entry.FullName));


                    if (Path.GetFileName(fullPath).Length != 0)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                        // The boolean parameter determines whether an existing file that has the same name as the destination file should be overwritten
                        entry.ExtractToFile(fullPath, true);
                    }
                }
            }
        }
        private async void CheckVersion()
        {
            DownloadPercent_PgBar.IsIndeterminate = true;
            if (Status == LauncherStatus.downloadingUpdate || Status == LauncherStatus.downloadingGame)
            { }
            else
            {
                htc.CancelPendingRequests();


                if (File.Exists(versionGameFile)  && File.Exists(gameExe))
                {
                    try
                    {
                        HttpResponseMessage response = await htc.GetAsync(urlVersion);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Version OnlineVer = new Version(responseBody);
                        Version localVersion = new Version(File.ReadAllText(versionGameFile));


                        if (OnlineVer.IsDifferentThan(localVersion))
                        {
                            Status = LauncherStatus.Download;
                            CheckSizeDownload(gameUrl);
                            DownloadStatus_txt.Text = "อัปเดตใหญ่";
                            Downloadinfo_DB.Visibility = Visibility.Visible;
                            Install();
                        }
                        else
                        {
                            Status = LauncherStatus.ready;
                            Downloadinfo_DB.Visibility = Visibility.Hidden;
                            DownloadPercent_PgBar.IsIndeterminate = false;
                            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
                            {
                                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                                startInfo.WorkingDirectory = Path.Combine(rootPath, folder);
                                Process.Start(startInfo);

                                Close();
                            }
                        }
                    }
                    catch (Exception EMS)
                    {
                        {
                            Status = LauncherStatus.Download;
                            DownloadStatus_txt.Text = "ดาวน์โหลด";
                            CheckSizeDownload(gameUrl);
                            DownloadPercent_PgBar.IsIndeterminate = false;
                        }
                    }
                }
                else
                {
                    Status = LauncherStatus.Download;
                    DownloadStatus_txt.Text = "ดาวน์โหลด";
                    CheckSizeDownload(gameUrl);
                    DownloadPercent_PgBar.IsIndeterminate = true;
                    Install();
                }
            }
        }


        private async void CheckUpdate()
        {
            if (Status == LauncherStatus.downloadingUpdate || Status == LauncherStatus.downloadingGame)
            {
            }
            else
            {
                htc.CancelPendingRequests();


                if (File.Exists(versionGameFile) && File.Exists(gameExe))
                {
                    try
                    {
                        HttpResponseMessage response = await htc.GetAsync(urlVersionUpdate);
                        response.EnsureSuccessStatusCode();
                        string responseBody = await response.Content.ReadAsStringAsync();
                        Version OnlineVer = new Version(responseBody);
                        Version localVersion = new Version(File.ReadAllText(versionUpdate));


                        if (OnlineVer.IsDifferentThan(localVersion))
                        {
                            Status = LauncherStatus.Updates;
                            CheckSizeDownload(updateUrl);
                            DownloadStatus_txt.Text = "อัปเดต";
                            Downloadinfo_DB.Visibility = Visibility.Visible;
                            DownloadPercent_PgBar.IsIndeterminate = false;
                        }
                        else
                        {
                            //CheckforUpdates();
                            Status = LauncherStatus.Updates;
                            Downloadinfo_DB.Visibility = Visibility.Collapsed;
                            Mouse.OverrideCursor = Cursors.Arrow;
                            DownloadPercent_PgBar.IsIndeterminate = false;
                            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
                            {
                                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                                startInfo.WorkingDirectory = Path.Combine(rootPath, folder);
                                Process.Start(startInfo);

                                Close();
                            }
                        }
                    }
                    catch (Exception EMS)
                    {
                        Status = LauncherStatus.Updates;
                        CheckSizeDownload(updateUrl);
                        DownloadStatus_txt.Text = "อัปเดต";
                        Downloadinfo_DB.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    Status = LauncherStatus.Download;
                    CheckSizeDownload(gameUrl);
                    Install();
                }
            }
        }


        private void texttest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.tiktok.com/@ffourto");
        }


        private void texttest_MouseEnter(object sender, MouseEventArgs e)
        {
        }


        private void Close_bt_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void minisize_bt_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        async void CheckSizeDownload(string Url)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Head, Url);
                var response = await htc.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Int64 total_bytes = Convert.ToInt64(response.Content.Headers.ContentLength);
                    fileSize = ((int)total_bytes);
                    int I = 0;
                    DownloadSize_txt.Text = fileSize.ToString();
                    DownloadSize_txt.Text = SizeSuffix(fileSize);
                }
                DownloadPercent_PgBar.IsIndeterminate = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("กรุณาตรวจสอบการเชื่อมต่อ Internet");
            }
        }

        struct Version
        {
            internal static Version zero = new Version(0, 0, 0);


            private short major;
            private short minor;
            private short subMinor;


            internal Version(short _major, short _minor, short _subMinor)
            {
                major = _major;
                minor = _minor;
                subMinor = _subMinor;
            }
            internal Version(string _version)
            {
                string[] versionStrings = _version.Split('.');
                if (versionStrings.Length != 3)
                {
                    major = 0;
                    minor = 0;
                    subMinor = 0;
                    return;
                }


                major = short.Parse(versionStrings[0]);
                minor = short.Parse(versionStrings[1]);
                subMinor = short.Parse(versionStrings[2]);
            }


            internal bool IsDifferentThan(Version _otherVersion)
            {
                if (major != _otherVersion.major)
                {
                    return true;
                }
                else
                {
                    if (minor != _otherVersion.minor)
                    {
                        return true;
                    }
                    else
                    {
                        if (subMinor != _otherVersion.subMinor)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            public override string ToString()
            {
                return $"{major}.{minor}.{subMinor}";
            }
        }
    }
}


