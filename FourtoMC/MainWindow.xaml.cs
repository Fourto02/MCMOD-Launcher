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
        private string namefile;
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
        private string modfolder;
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
                        PlayButton.Content = "ดาวน์โหลดเลย!";
                        break;
                    case LauncherStatus.Updates:
                        PlayButton.Content = "อัปเดต";
                        break;
                    case LauncherStatus.ready:
                        PlayButton.Content = "เล่น";
                        break;
                    case LauncherStatus.failed:
                        PlayButton.Content = "อัปเดตไม่สำเร็จ ลองใหม่อีกครั้ง";
                        break;
                    case LauncherStatus.downloadingGame:
                        PlayButton.Content = "กำลังดาวน์โหลด";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        PlayButton.Content = "กำลังอัปเดต";
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
            String ExcName = "Launch";
            updatePath = Path.Combine(rootPath, folder );
            versionGameFile = Path.Combine(rootPath, folder, "Version.txt");
            namefile = Path.Combine(rootPath, folder, "Name.txt");
            versionUpdate = Path.Combine(rootPath, folder, "Update.txt");
            gameZip = Path.Combine(rootPath, folder + ".zip");
            updateZip = Path.Combine(rootPath, "Update.zip");
            modfolder = Path.Combine(rootPath, folder,"mods");
            gameFolder = folder;
            gameExe = Path.Combine(rootPath, folder, ExcName + ".bat");
            updateUrl = "https://www.dropbox.com/scl/fi/hrhna3vomg69bfwx9obgm/mods.zip?rlkey=ih3f1dpuqupyszpcew6fper9w&st=b8efuz18&dl=1";
            gameUrl = "https://www.dropbox.com/scl/fi/730is4xri5763jp505ag9/FourtoMC.zip?rlkey=1kxuayqxjubdk0b7i1acsrnim&st=nxla9u6c&dl=1";
            urlVersion = "https://www.dropbox.com/scl/fi/pfiw9puem38ugza2ckesy/Version.txt?rlkey=2wbhzj0b4nukiaueoozk3dzem&st=uxbffl7o&dl=1";
            urlVersionUpdate = "https://www.dropbox.com/scl/fi/4ar3l1f5o54vouwsh8d4i/Update.txt?rlkey=2euv18zme88ca5cg4nuuoza96&st=lvswgxcu&dl=1";
            cmdlocal = Path.Combine(rootPath, folder, "Launch.bat");
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
            if (File.Exists(namefile))
            {
                string name = File.ReadAllText(namefile);
                Name_edt.Text = name;
            }
        }


        private void Install()
        {
            PlayButton.IsEnabled = false;
            if (File.Exists(versionUpdate))
            {
                Version localVersion = new Version(File.ReadAllText(versionUpdate));


                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString(urlVersionUpdate));


                    if (onlineVersion.IsDifferentThan(localVersion))
                    {
                        PlayButton.IsEnabled = false;
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
                    PlayButton.IsEnabled = false;
                    Status = LauncherStatus.downloadingUpdate;
                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(UpdateGameCompletedCallback);
                    wc.DownloadProgressChanged += new DownloadProgressChangedEventHandler(Wc_DownloadProgressChanged);
                    wc.DownloadFileAsync(new Uri(updateUrl), updateZip, _onlineVersion);
                }
                else
                {
                    PlayButton.IsEnabled = false;
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
                if (Directory.Exists(gameFolder))
                {
                    Directory.Delete(folder, true);
                }
                unzip(gameFolder + ".zip", rootPath);
                File.Delete(gameZip);
                File.WriteAllText(versionGameFile, onlineVersion);
                Status = LauncherStatus.ready;
                PlayButton.IsEnabled = true;
                CheckUpdate();
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
                PlayButton.IsEnabled = true;
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
                if (Directory.Exists(modfolder))
                {
                    Directory.Delete(modfolder,true);
                }
                unzip("Update.zip", updatePath);
                File.Delete(updateZip);
                File.WriteAllText(versionUpdate, onlineVersion);
                Status = LauncherStatus.ready;
                PlayButton.IsEnabled = true;
                Downloadinfo_DB.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error finishing download: {ex}");
                Downloadinfo_DB.Visibility = Visibility.Hidden;
                PlayButton.IsEnabled = true;
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
            PlayButton.IsEnabled = false;
            if (Status == LauncherStatus.downloadingUpdate || Status == LauncherStatus.downloadingGame)
            {  }
            else
            {
                htc.CancelPendingRequests();
                PlayButton.Content = "กำลังตรวจสอบ";


                if (File.Exists(versionGameFile) && File.Exists(gameExe))
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
                            PlayButton.IsEnabled = false;
                        }
                        else
                        {
                            Status = LauncherStatus.ready;
                            Downloadinfo_DB.Visibility = Visibility.Hidden;
                            DownloadPercent_PgBar.IsIndeterminate = false;
                            PlayButton.IsEnabled = true;
                            CheckUpdate();


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
                }
            }
        }


        private async void CheckUpdate()
        {
            PlayButton.IsEnabled = false;
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
                            PlayButton.IsEnabled = false;
                            DownloadPercent_PgBar.IsIndeterminate = false;


                        }
                        else
                        {
                            //CheckforUpdates();
                            Status = LauncherStatus.ready;
                            Downloadinfo_DB.Visibility = Visibility.Collapsed;
                            PlayButton.IsEnabled = true;
                            Mouse.OverrideCursor = Cursors.Arrow;
                            DownloadPercent_PgBar.IsIndeterminate = false;
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
                    PlayButton.IsEnabled = true;
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


                PlayButton.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("กรุณาตรวจสอบการเชื่อมต่อ Internet");
            }
        }


        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(gameExe) && Status == LauncherStatus.ready)
            {
                string p1 = @"""C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\runtime\java-runtime-gamma\windows\java-runtime-gamma\bin\javaw.exe"" ""-Dos.name=Windows 10"" -Dos.version=10.0 -XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump -Xss1M ""-Djava.library.path=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\versions\Forge 1.20.1\natives"" ""-Djna.tmpdir=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\versions\Forge 1.20.1\natives"" ""-Dorg.lwjgl.system.SharedLibraryExtractPath=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\versions\Forge 1.20.1\natives"" ""-Dio.netty.native.workdir=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\versions\Forge 1.20.1\natives"" -Dminecraft.launcher.brand=minecraft-launcher -Dminecraft.launcher.version=2.3.173 -cp ""C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\cpw\mods\securejarhandler\2.1.10\securejarhandler-2.1.10.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\ow2\asm\asm\9.7.1\asm-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\ow2\asm\asm-commons\9.7.1\asm-commons-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\ow2\asm\asm-tree\9.7.1\asm-tree-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\ow2\asm\asm-util\9.7.1\asm-util-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\ow2\asm\asm-analysis\9.7.1\asm-analysis-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\accesstransformers\8.0.4\accesstransformers-8.0.4.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\antlr\antlr4-runtime\4.9.1\antlr4-runtime-4.9.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\eventbus\6.0.5\eventbus-6.0.5.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\forgespi\7.0.1\forgespi-7.0.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\coremods\5.2.4\coremods-5.2.4.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\cpw\mods\modlauncher\10.0.9\modlauncher-10.0.9.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\unsafe\0.2.0\unsafe-0.2.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\mergetool\1.1.5\mergetool-1.1.5-api.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\electronwill\night-config\core\3.6.4\core-3.6.4.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\electronwill\night-config\toml\3.6.4\toml-3.6.4.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\maven\maven-artifact\3.8.5\maven-artifact-3.8.5.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\jodah\typetools\0.6.3\typetools-0.6.3.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecrell\terminalconsoleappender\1.2.0\terminalconsoleappender-1.2.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\jline\jline-reader\3.12.1\jline-reader-3.12.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\jline\jline-terminal\3.12.1\jline-terminal-3.12.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\spongepowered\mixin\0.8.5\mixin-0.8.5.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\openjdk\nashorn\nashorn-core\15.4\nashorn-core-15.4.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\JarJarSelector\0.3.19\JarJarSelector-0.3.19.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\JarJarMetadata\0.3.19\JarJarMetadata-0.3.19.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\cpw\mods\bootstraplauncher\1.1.2\bootstraplauncher-1.1.2.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\JarJarFileSystems\0.3.19\JarJarFileSystems-0.3.19.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\fmlloader\1.20.1-47.3.22\fmlloader-1.20.1-47.3.22.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\minecraftforge\fmlearlydisplay\1.20.1-47.3.22\fmlearlydisplay-1.20.1-47.3.22.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\github\oshi\oshi-core\6.2.2\oshi-core-6.2.2.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\google\code\gson\gson\2.10\gson-2.10.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\google\guava\failureaccess\1.0.1\failureaccess-1.0.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\google\guava\guava\31.1-jre\guava-31.1-jre.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\ibm\icu\icu4j\71.1\icu4j-71.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\tlauncher\authlib\4.0.43.1\authlib-4.0.43.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\mojang\blocklist\1.0.10\blocklist-1.0.10.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\mojang\brigadier\1.1.8\brigadier-1.1.8.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\mojang\datafixerupper\6.0.8\datafixerupper-6.0.8.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\mojang\logging\1.1.1\logging-1.1.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\tlauncher\patchy\2.2.101\patchy-2.2.101.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\com\mojang\text2speech\1.17.9\text2speech-1.17.9.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\commons-codec\commons-codec\1.15\commons-codec-1.15.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\commons-io\commons-io\2.11.0\commons-io-2.11.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\commons-logging\commons-logging\1.2\commons-logging-1.2.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-buffer\4.1.82.Final\netty-buffer-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-codec\4.1.82.Final\netty-codec-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-common\4.1.82.Final\netty-common-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-handler\4.1.82.Final\netty-handler-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-resolver\4.1.82.Final\netty-resolver-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-transport-classes-epoll\4.1.82.Final\netty-transport-classes-epoll-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-transport-native-unix-common\4.1.82.Final\netty-transport-native-unix-common-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\io\netty\netty-transport\4.1.82.Final\netty-transport-4.1.82.Final.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\it\unimi\dsi\fastutil\8.5.9\fastutil-8.5.9.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\java\dev\jna\jna-platform\5.12.1\jna-platform-5.12.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\java\dev\jna\jna\5.12.1\jna-5.12.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\net\sf\jopt-simple\jopt-simple\5.0.4\jopt-simple-5.0.4.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\commons\commons-compress\1.21\commons-compress-1.21.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\commons\commons-lang3\3.12.0\commons-lang3-3.12.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\httpcomponents\httpclient\4.5.13\httpclient-4.5.13.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\httpcomponents\httpcore\4.4.15\httpcore-4.4.15.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\logging\log4j\log4j-api\2.19.0\log4j-api-2.19.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\logging\log4j\log4j-core\2.19.0\log4j-core-2.19.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\apache\logging\log4j\log4j-slf4j2-impl\2.19.0\log4j-slf4j2-impl-2.19.0.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\joml\joml\1.10.5\joml-1.10.5.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-glfw\3.3.1\lwjgl-glfw-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-glfw\3.3.1\lwjgl-glfw-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-glfw\3.3.1\lwjgl-glfw-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-glfw\3.3.1\lwjgl-glfw-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-jemalloc\3.3.1\lwjgl-jemalloc-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-jemalloc\3.3.1\lwjgl-jemalloc-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-jemalloc\3.3.1\lwjgl-jemalloc-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-jemalloc\3.3.1\lwjgl-jemalloc-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-openal\3.3.1\lwjgl-openal-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-openal\3.3.1\lwjgl-openal-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-openal\3.3.1\lwjgl-openal-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-openal\3.3.1\lwjgl-openal-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-opengl\3.3.1\lwjgl-opengl-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-opengl\3.3.1\lwjgl-opengl-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-opengl\3.3.1\lwjgl-opengl-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-opengl\3.3.1\lwjgl-opengl-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-stb\3.3.1\lwjgl-stb-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-stb\3.3.1\lwjgl-stb-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-stb\3.3.1\lwjgl-stb-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-stb\3.3.1\lwjgl-stb-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-tinyfd\3.3.1\lwjgl-tinyfd-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-tinyfd\3.3.1\lwjgl-tinyfd-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-tinyfd\3.3.1\lwjgl-tinyfd-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl-tinyfd\3.3.1\lwjgl-tinyfd-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl\3.3.1\lwjgl-3.3.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl\3.3.1\lwjgl-3.3.1-natives-windows.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl\3.3.1\lwjgl-3.3.1-natives-windows-arm64.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\lwjgl\lwjgl\3.3.1\lwjgl-3.3.1-natives-windows-x86.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries\org\slf4j\slf4j-api\2.0.1\slf4j-api-2.0.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\versions\Forge 1.20.1\Forge 1.20.1.jar"" -Djava.net.preferIPv6Addresses=system ""-DignoreList=bootstraplauncher,securejarhandler,asm-commons,asm-util,asm-analysis,asm-tree,asm,JarJarFileSystems,client-extra,fmlcore,javafmllanguage,lowcodelanguage,mclanguage,forge-,Forge 1.20.1.jar"" -DmergeModules=jna-5.10.0.jar,jna-platform-5.10.0.jar ""-DlibraryDirectory=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries"" -p ""C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/cpw/mods/bootstraplauncher/1.1.2/bootstraplauncher-1.1.2.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/cpw/mods/securejarhandler/2.1.10/securejarhandler-2.1.10.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/org/ow2/asm/asm-commons/9.7.1/asm-commons-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/org/ow2/asm/asm-util/9.7.1/asm-util-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/org/ow2/asm/asm-analysis/9.7.1/asm-analysis-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/org/ow2/asm/asm-tree/9.7.1/asm-tree-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/org/ow2/asm/asm/9.7.1/asm-9.7.1.jar;C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries/net/minecraftforge/JarJarFileSystems/0.3.19/JarJarFileSystems-0.3.19.jar"" --add-modules ALL-MODULE-PATH --add-opens java.base/java.util.jar=cpw.mods.securejarhandler --add-opens java.base/java.lang.invoke=cpw.mods.securejarhandler --add-exports java.base/sun.security.util=cpw.mods.securejarhandler --add-exports jdk.naming.dns/com.sun.jndi.dns=java.naming -Xmx10873M -XX:+UnlockExperimentalVMOptions -XX:+UseG1GC -XX:G1NewSizePercent=20 -XX:G1ReservePercent=20 -XX:MaxGCPauseMillis=50 -XX:G1HeapRegionSize=32M -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true -Djava.net.preferIPv4Stack=true ""-Dminecraft.applet.TargetDirectory=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC"" ""-DlibraryDirectory=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\libraries"" ""-Dlog4j.configurationFile=C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\assets\log_configs\client-1.12.xml"" cpw.mods.bootstraplauncher.BootstrapLauncher --username ";
                string name = Name_edt.Text.ToString();
                string p2 = @" --version ""Forge 1.20.1"" --gameDir ""C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC"" --assetsDir ""C:\Program Files (x86)\FourtoMC\FourtoMC\FourtoMC\FourtoMC\assets"" --assetIndex 5 --uuid fc528523581647579148ff97d06fe1f3 --accessToken null --clientId null --xuid null --userType mojang --versionType modified --width 1600 --height 900 --launchTarget forgeclient --fml.forgeVersion 47.3.22 --fml.mcVersion 1.20.1 --fml.forgeGroup net.minecraftforge --fml.mcpVersion 20230612.114412 --fullscreen";
                File.WriteAllText(cmdlocal, p1 + name +p2);
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, gameFolder);
                Process.Start(startInfo);




                this.WindowState = WindowState.Minimized;
                Mouse.OverrideCursor = Cursors.Arrow;
            }
            else if (Status == LauncherStatus.Download)
            {
                try
                {
                    HttpResponseMessage response = await htc.GetAsync(urlVersion);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    Version onlineVersion = new Version(responseBody);
                    InstallGameFiles(false, onlineVersion);
                    PlayButton.IsEnabled = false;
                }
                catch
                {


                }
            }
            else if (Status == LauncherStatus.Updates)
            {
                Install();
                PlayButton.IsEnabled = false;
            }
            else if (Status == LauncherStatus.failed)
            {
                Install();
                PlayButton.IsEnabled = false;
            }
            else if (Status == LauncherStatus.ready)
            {
                CheckVersion();
                Downloadinfo_DB.Visibility = Visibility.Visible;
                PlayButton.IsEnabled = false;
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

        private void Name_edt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(namefile))
            {
                File.WriteAllText(namefile, Name_edt.Text.ToString());
            }
        }
    }
}


