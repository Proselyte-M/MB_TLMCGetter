using System;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MusicBeePlugin
{

    public class TlmcConfig
    {
        public enum OutputFormat
        {
            Original = 0,
            Both = 1,
            Translation = 2
        }

        public OutputFormat format { get; set; } = OutputFormat.Both;
        public bool fuzzy { get; set; } = false;
    }

    public partial class Plugin
    {
        private const string ProviderName = "TlmcMetadataReader";
        private const string ConfigFilename = "TlmcMetadataReader_config";
        private const string NoTranslateFilename = "TlmcMetadataReader_notranslate";
        private TlmcConfig _config = new TlmcConfig();
        private ComboBox _formatComboBox = null;
        private CheckBox _fuzzyCheckBox = null;

        private MusicBeeApiInterface mbApiInterface;
        private PluginInfo about = new PluginInfo();

        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);
            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "TLMC第三方信息备注";
            about.Description = "通过从thwiki获取东方曲目信息";
            about.Author = "来自TLMC（707882390）群的Proselyte";
            about.TargetApplication = "TLMC第三方信息备注";   //  the name of a Plugin Storage device or panel header for a dockable panel
            about.Type = PluginType.General;
            about.VersionMajor = 1;  // your plugin version
            about.VersionMinor = 0;
            about.Revision = 1;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = (ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents);
            about.ConfigurationPanelHeight = 0;   // height in pixels that musicbee should reserve in a panel for config settings. When set, a handle to an empty panel will be passed to the Configure function
            string noTranslatePath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), NoTranslateFilename);
            string configPath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), ConfigFilename);
            if (File.Exists(configPath))
            {
                try
                {
                    _config = JsonConvert.DeserializeObject<TlmcConfig>(File.ReadAllText(configPath, Encoding.UTF8));
                }
                catch (Exception ex)
                {
                    mbApiInterface.MB_Trace("[NeteaseMusic] Failed to load config" + ex);
                }
            }
            if (File.Exists(noTranslatePath))
            {
                File.Delete(noTranslatePath);
                _config.format = TlmcConfig.OutputFormat.Original;
                SaveSettingsInternal();
            }
            return about;
        }

        private void SaveSettingsInternal()
        {
            string configPath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), ConfigFilename);
            var json = JsonConvert.SerializeObject(_config);
            File.WriteAllText(configPath, json, Encoding.UTF8);
        }


        public bool Configure(IntPtr panelHandle)
        {
            // 将任何永久设置保存在此路径的子文件夹中
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            // 面板句柄只有在您设置时才会被设置。配置面板高度为非零值
            // 请记住，面板宽度根据用户选择的字体进行缩放
            //  如果关于。配置面板高度设置为 0，可以显示自己的弹窗
            if (panelHandle != IntPtr.Zero)
            {
                Panel configPanel = (Panel)Panel.FromHandle(panelHandle);
                Label prompt = new Label();
                prompt.AutoSize = true;
                prompt.Location = new Point(0, 0);
                prompt.Text = "prompt:";
                TextBox textBox = new TextBox();
                textBox.Bounds = new Rectangle(60, 0, 100, textBox.Height);
                configPanel.Controls.AddRange(new Control[] { prompt, textBox });

            }

            return false;
        }

        // 当用户在 MusicBee 首选项屏幕中单击“应用”或“保存”时由 MusicBee 调用。
        // 由您决定是否有任何更改并需要更新
        public void SaveSettings()
        {
            // save any persistent settings in a sub-folder of this path
            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
        }

        // MusicBee正在关闭插件（插件被用户禁用或MusicBee正在关闭）
        public void Close(PluginCloseReason reason)
        {
        }

        // 卸载此插件 - 清理所有持久化的文件
        public void Uninstall()
        {
            var dataPath = mbApiInterface.Setting_GetPersistentStoragePath();
            var p = Path.Combine(dataPath, NoTranslateFilename);
            if (File.Exists(p)) File.Delete(p);
            string configPath = Path.Combine(mbApiInterface.Setting_GetPersistentStoragePath(), ConfigFilename);
            if (File.Exists(configPath)) File.Delete(configPath);
        }


        public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album,
            bool synchronisedPreferred, string provider)
        {

            return null;
        }

        // 接收来自MusicBee的事件通知
        // 你需要开始。ReceiveNotificationFlags = PlayerEvents 用于接收所有通知，而不仅仅是启动事件
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            
            
            XDocument xDocument = new XDocument();
            // 根据通知类型执行一些操作
            switch (type)
            {
                case NotificationType.PluginStartup:
                    // 执行启动初始化
                    switch (mbApiInterface.Player_GetPlayState())
                    {
                        case PlayState.Playing:
                            {

                                break;
                            }

                        case PlayState.Paused:
                            // ...
                            break;
                    }
                    break;
                case NotificationType.TrackChanged:
                    {
                        try
                        {
                            TlmcMediaWiki.Mediawiki mediawiki = new TlmcMediaWiki.Mediawiki();
                            TlmcMediaWiki.ReadXML readXML = new TlmcMediaWiki.ReadXML();
                            string albumArtist = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.AlbumArtist);
                            string album = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Album);
                            string dataPath = mbApiInterface.Setting_GetPersistentStoragePath() + "TlmcAlbumMetadata" + "\\" + albumArtist;
                            if (File.Exists(dataPath+"\\"+album+".xml"))
                            { 
                                xDocument=XDocument.Load(dataPath + "\\" + album + ".xml");                            
                            }
                            else 
                            { 
                                xDocument = mediawiki.GetAlbumInfo(sourceFileUrl, albumArtist, album);
                                if (!string.IsNullOrEmpty(readXML.ReadAlbumSingle(xDocument, "名称")))
                                {
                                    WriteXml(xDocument, dataPath, album + ".xml");
                                }
                                
                            }
                           
                            //MessageBox.Show(xDocument.ToString());
                           

                            mbApiInterface.Library_SetFileTag(
                                readXML.ReadFilePath(xDocument), MetaDataType.Custom1, readXML.ReadAlbumSingle(xDocument, "角色"));
                            mbApiInterface.Library_SetFileTag(
                                readXML.ReadFilePath(xDocument), MetaDataType.Custom2, readXML.ReadAlbumSingle(xDocument, "名称"));
                            mbApiInterface.Library_SetFileTag(
                                readXML.ReadFilePath(xDocument), MetaDataType.Custom3, readXML.ReadAlbumSingle(xDocument, "制作方"));
                            mbApiInterface.Library_SetFileTag(
                                readXML.ReadFilePath(xDocument), MetaDataType.Custom4, readXML.ReadAlbumSingle(xDocument, "首发日期"));
                            mbApiInterface.Library_SetFileTag(
                                readXML.ReadFilePath(xDocument), MetaDataType.Custom5, readXML.ReadAlbumSingle(xDocument, "编号"));

                            //MessageBox.Show(mbApiInterface.Library_SetFileTag(sourceFileUrl, MetaDataType.Custom1, "由插件创建的tag").ToString());
                            mbApiInterface.Library_CommitTagsToFile(sourceFileUrl);
                            mbApiInterface.MB_RefreshPanels();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            xDocument.Elements("root").Remove();
                        }
                    }
                    


                    // ...
                    break;
            }
            xDocument = new XDocument();
        }

        public static void WriteXml(XDocument xml, string path, string filename)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (FileStream file = new FileStream(path + "\\" + filename, FileMode.Create))
            {
                xml.Save(file);
            }
        }



        // 返回此插件支持的歌词或插图提供程序名称数组
        // 提供程序将被逐个迭代并传递给 检索歌词/按照用户在 MusicBee 标签（2） 首选项屏幕中设置的顺序检索图稿功能，直到找到匹配项
        //public string[] GetProviders()
        //{
        //    return null;
        //}

        // 从请求的提供商返回所请求的艺术家/标题的歌词
        // 仅当插件类型 = 歌词检索时才需要
        // 如果未找到歌词，则返回 null
        //public string RetrieveLyrics(string sourceFileUrl, string artist, string trackTitle, string album, bool synchronisedPreferred, string provider)
        //{
        //    return null;
        //}

        // 从请求的提供程序返回图稿二进制数据的 Base64 字符串表示形式
        // 仅当插件类型 = 图稿检索时才需要
        // 如果未找到图稿，则返回 null
        //public string RetrieveArtwork(string sourceFileUrl, string albumArtist, string album, string provider)
        //{
        //    //Return Convert.ToBase64String(artworkBinaryData)
        //    return null;
        //}

        //  此功能的存在向 MusicBee 表明该插件具有可停靠面板。MusicBee将创建控件并将其作为面板参数传递
        //  如果需要，可以将自己的控件添加到面板中
        //  您可以使用mbApiInterface.MB_SetPanelScrollableArea功能控制面板的可滚动区域
        //  要为面板设置MusicBee标题，请设置。目标应用程序在上面的初始化函数到面板标题文本
        //public int OnDockablePanelCreated(Control panel)
        //{
        //    //    返回面板的高度并在此处执行任何初始化
        //    //    MusicBee will call panel.Dispose() when the user removes this panel from the layout configuration
        //    //    < 0 indicates to MusicBee this control is resizable and should be sized to fill the panel it is docked to in MusicBee
        //    //    = 0 indicates to MusicBee this control resizeable
        //    //    > 0 indicates to MusicBee the fixed height for the control.Note it is recommended you scale the height for high DPI screens(create a graphics object and get the DpiY value)
        //    float dpiScaling = 0;
        //    using (Graphics g = panel.CreateGraphics())
        //    {
        //        dpiScaling = g.DpiY / 96f;
        //    }
        //    panel.Paint += panel_Paint;
        //    return Convert.ToInt32(100 * dpiScaling);
        //}

        // 此功能的存在向MusicBee表示，当单击面板标题时，上面创建的可停靠面板将显示菜单项
        // 返回将显示的工具条菜单项的列表
        //public List<ToolStripItem> GetHeaderMenuItems()
        //{
        //    List<ToolStripItem> list = new List<ToolStripItem>();
        //    list.Add(new ToolStripMenuItem("A menu item"));
        //    return list;
        //}

        //private void panel_Paint(object sender, PaintEventArgs e)
        //{
        //    e.Graphics.Clear(Color.Red);
        //    TextRenderer.DrawText(e.Graphics, "hello", SystemFonts.CaptionFont, new Point(10, 10), Color.Blue);
        //}

    }
}
