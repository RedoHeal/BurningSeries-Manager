using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
//using System.Data.SQLite;

namespace BS.to_Manager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadConfig();
            SetConfig();
        }

        public string[] config = new string[6];
        public int bookmarks_n = 0;
        public ArrayList bookmarks_scan = new ArrayList();
        public string link = "";
        public string site = "";
        public string cbtext = "";
        public int index = 0;

        public void LoadStandardConfig()
        {
            config[0] = "filter1=Anime";
            config[1] = "filter2=";
            config[2] = "filter3=";
            config[3] = "und/oder=oder";
            config[4] = "mehrfach/einzel=mehrfach";
            config[5] = "immererstestaffel=false";
        }

        public void LoadConfig()
        {
            if (File.Exists("config.txt"))
            {
                config = File.ReadAllLines("config.txt", Encoding.UTF8);
                bookmarks_n = File.ReadAllLines("bookmarks_scan.txt", Encoding.UTF8).Length;
            }
            else
            {
                LoadStandardConfig();
                File.WriteAllLines("config.txt", config, Encoding.UTF8);
            }

            if (File.Exists("bookmarks_scan.txt"))
            {
                bookmarks_n = File.ReadAllLines("bookmarks_scan.txt", Encoding.UTF8).Length;
                string[] bookmarks_tmp = new string[bookmarks_n];
                bookmarks_tmp = File.ReadAllLines("bookmarks_scan.txt", Encoding.UTF8);

                for (int i = 0; i < bookmarks_tmp.Length; i++)
                {
                    if (Regex.IsMatch(bookmarks_tmp[i], @"bs\.to/serie/"))
                    {
                        bookmarks_scan.Add(bookmarks_tmp[i].ToString());
                    }
                }
                if (bookmarks_scan.Count > 0)
                {
                    txt_know.Text = bookmarks_scan.Count + " Bookmarks" + "\n" + "ausgelesen..";
                }
                else
                {
                    txt_know.Visibility = Visibility.Collapsed;
                    tb_know.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                string[] bookmarks_scan_array = (string[])bookmarks_scan.ToArray(typeof(string));
                File.WriteAllLines("bookmarks_scan.txt", bookmarks_scan_array, Encoding.UTF8);
            }

        }

        public void SetConfig()
        {
            string[] conf = new string[config.Length];
            string[] items = new string[cb_1.Items.Count];
            ComboBoxItem[] cb_items = new ComboBoxItem[items.Length];

            for (int i = 0; i < conf.Length; i++)
            {
                conf[i] = Regex.Replace(config[i], @".*=([^\n]+)?", "$1");
            }

            for (int i = 0; i < cb_1.Items.Count; i++)
            {
                cb_items[i] = (ComboBoxItem)cb_1.Items.GetItemAt(i);
                items[i] = cb_items[i].Content.ToString();
            }

            cb_2.Items.Clear();
            cb_3.Items.Clear();
            foreach (var item in items)
            {
                cb_2.Items.Add(item);
                cb_3.Items.Add(item);
            }

            for (int i = 0; i < cb_items.Length; i++)
            {
                if (conf[0] == items[i])
                {
                    cb_1.SelectedIndex = i;
                }
                else if (conf[1] == items[i])
                {
                    cb_2.SelectedIndex = i;
                }
                else if (conf[2] == items[i])
                {
                    cb_3.SelectedIndex = i;
                }
            }

            if (conf[3] == "und")
            {
                rb_and.IsChecked = true;
            }

            if (conf[4] == "einzel")
            {
                rb_single.IsChecked = true;
            }

            if (conf[5] == "true")
            {
                chb_first.IsChecked = true;
            }
        }

        public void SaveConfig()
        {
            string filter1 = cb_1.Text;
            string filter2 = cb_2.Text;
            string filter3 = cb_3.Text;

            config[0] = "filter1=" + filter1;

            if (filter2 != null)
            {
                config[1] = "filter2=" + filter2;
            }
            else
            {
                config[1] = "filter2=";
            }

            if (filter3 != null)
            {
                config[2] = "filter3=" + filter3;
            }
            else
            {
                config[2] = "filter3=";
            }

            if (rb_or.IsChecked == true)
            {
                config[3] = "und/oder=oder";
            }
            else
            {
                config[3] = "und/oder=und";
            }

            if (rb_all.IsChecked == true)
            {
                config[4] = "mehrfach/einzel=mehrfach";
            }
            else
            {
                config[4] = "mehrfach/einzel=einzel";
            }

            if (chb_first.IsChecked == true)
            {
                config[5] = "immererstestaffel=true";
            }
            else
            {
                config[5] = "immererstestaffel=false";
            }

            if (File.Exists("config.txt"))
            {
                bool checknew = false;

                for (int i = 0; i < config.Length; i++)
                {
                    if (config[i] != File.ReadAllLines("config.txt", Encoding.UTF8)[i])
                    {
                        checknew = true;
                    }
                }
                if (checknew)
                {
                    File.WriteAllLines("config.txt", config, Encoding.UTF8);
                }
            }
        }

        private void btn_tab1(object sender, RoutedEventArgs e)
        {
            tab2.Visibility = Visibility.Collapsed;
            tab1.Visibility = Visibility.Visible;
        }
        private void btn_tab2(object sender, RoutedEventArgs e)
        {
            tab1.Visibility = Visibility.Collapsed;
            tab2.Visibility = Visibility.Visible;
        }

        // Neue Folgen:
        public string[] RegexExtract()
        {
            using (WebClient web = new WebClient())
            {
                string pattern1 = @".*(\t\t\t\t\t\t<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+(\s\s[^\n]+)[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+<a href=serie[^\n]+class=[^\n]+[\n][^\n]+[\n][^\n]+[\n][^\<]+)[^\n]+.*";
                string pattern2 = @"\t\t\t\t\t\t<a href=serie(/[^/]+/\d+/)[^\n]+[\n][^\n]+info>([^<]+)[^\n]+title=([^>]+)[^\n]+[\n][^\n]+[\n][^\n]+";
                string[] extracts = new string[20];
                string[] urls = new string[20];
                string[] webdata = new string[20];
                string input = web.DownloadString("https://bs.to");
                string cb_1_3 = "(" + cb_1.Text;

                // Vorbearbeitung
                input = Regex.Replace(input, "\"", "", RegexOptions.Singleline);

                // Relevanten Text rausziehen
                input = Regex.Replace(input, pattern1, "$1$2", RegexOptions.Singleline);

                // Relevanten Text > Aufrufbare URL's; (ohne LineBrake)
                string extractURL = Regex.Replace(input, pattern2, @"https://bs.to/serie$1de;", RegexOptions.Singleline);
                extractURL = Regex.Replace(extractURL, @"\n+", "", RegexOptions.Singleline);

                // Relevanten Text > URL;Staffel Episode;Sprache; (ohne LineBrake)
                string extract = Regex.Replace(input, pattern2, @"https://bs.to/serie$1de;$2;$3;", RegexOptions.Singleline);
                extract = Regex.Replace(extract, @"\n+", "", RegexOptions.Singleline);

                if (rb_or.IsChecked == true)
                {
                    if (cb_2.Text != "" && cb_3.Text != "")
                    {
                        cb_1_3 += "|" + cb_2.Text + "|" + cb_3.Text + ")";
                    }
                    else if (cb_2.Text != "")
                    {
                        cb_1_3 += "|" + cb_2.Text + ")";
                    }
                    else if (cb_3.Text != "")
                    {
                        cb_1_3 += "|" + cb_3.Text + ")";
                    }
                    else
                    {
                        cb_1_3 += ")";
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        // Oberste URL aus String(extractURLs) in Array[i](urls) laden
                        urls[i] = Regex.Replace(extractURL, @"(https://bs.to/serie/[^;]+);.*", "$1", RegexOptions.Singleline);

                        // Überprüfe Webseite von urls[i] auf String(cb_1_3)
                        webdata[i] = web.DownloadString(urls[i]);
                        if (Regex.IsMatch(webdata[i], cb_1_3, RegexOptions.Singleline))
                        {
                            // Oberste URL aus String(extract) in Array[i](extracts) laden
                            extracts[i] = Regex.Replace(extract, @"(https://bs.to/serie/[^;]+;[^;]+;[^;]+;).*", "$1", RegexOptions.Singleline);
                        }
                        else
                        {
                            extracts[i] = "";
                        }

                        // Erste URL aus String(extractURLs) entfernen
                        extractURL = extractURL.Substring(Regex.Match(extractURL, @"https://bs.to/serie/[^;]+;").Index + Regex.Match(extractURL, @"https://bs.to/serie/[^;]+;").Length);

                        // Erste URL aus String(extract) entfernen
                        extract = extract.Substring(Regex.Match(extract, @"https://bs.to/serie/[^;]+;[^;]+;[^;]+;").Index + Regex.Match(extract, @"https://bs.to/serie/[^;]+;[^;]+;[^;]+;").Length);
                    }
                }
                else
                {
                    for (int i = 0; i < 20; i++)
                    {
                        // Oberste URL aus String(extractURLs) in Array[i](urls) laden
                        urls[i] = Regex.Replace(extractURL, @"(https://bs.to/serie/[^;]+);.*", "$1", RegexOptions.Singleline);

                        // Überprüfe Webseite von urls[i] auf String(cb_1_3)
                        webdata[i] = web.DownloadString(urls[i]);

                        if (cb_2.Text != "" && cb_3.Text != "")
                        {
                            if (Regex.IsMatch(webdata[i], cb_1.Text, RegexOptions.Singleline) && Regex.IsMatch(webdata[i], cb_2.Text, RegexOptions.Singleline) && Regex.IsMatch(webdata[i], cb_3.Text, RegexOptions.Singleline))
                            {
                                // Oberste URL aus String(extract) in Array[i](extracts) laden
                                extracts[i] = Regex.Replace(extract, @"(https://bs.to/serie/[^;]+;[^;]+;[^;]+;).*", "$1", RegexOptions.Singleline);
                            }
                            else
                            {
                                extracts[i] = "";
                            }
                        }
                        else if (cb_2.Text != "")
                        {
                            if (Regex.IsMatch(webdata[i], cb_1.Text, RegexOptions.Singleline) && Regex.IsMatch(webdata[i], cb_2.Text, RegexOptions.Singleline))
                            {
                                // Oberste URL aus String(extract) in Array[i](extracts) laden
                                extracts[i] = Regex.Replace(extract, @"(https://bs.to/serie/[^;]+;[^;]+;[^;]+;).*", "$1", RegexOptions.Singleline);
                            }
                            else
                            {
                                extracts[i] = "";
                            }
                        }
                        else if (cb_3.Text != "")
                        {
                            if (Regex.IsMatch(webdata[i], cb_1.Text, RegexOptions.Singleline) && Regex.IsMatch(webdata[i], cb_3.Text, RegexOptions.Singleline))
                            {
                                // Oberste URL aus String(extract) in Array[i](extracts) laden
                                extracts[i] = Regex.Replace(extract, @"(https://bs.to/serie/[^;]+;[^;]+;[^;]+;).*", "$1", RegexOptions.Singleline);
                            }
                            else
                            {
                                extracts[i] = "";
                            }
                        }
                        else
                        {
                            cb_1_3 = cb_1.Text;

                            if (Regex.IsMatch(webdata[i], cb_1_3, RegexOptions.Singleline))
                            {
                                // Oberste URL aus String(extract) in Array[i](extracts) laden
                                extracts[i] = Regex.Replace(extract, @"(https://bs.to/serie/[^;]+;[^;]+;[^;]+;).*", "$1", RegexOptions.Singleline);
                            }
                            else
                            {
                                extracts[i] = "";
                            }
                        }

                        // Erste URL aus String(extractURLs) entfernen
                        extractURL = extractURL.Substring(Regex.Match(extractURL, @"https://bs.to/serie/[^;]+;").Index + Regex.Match(extractURL, @"https://bs.to/serie/[^;]+;").Length);

                        // Erste URL aus String(extract) entfernen
                        extract = extract.Substring(Regex.Match(extract, @"https://bs.to/serie/[^;]+;[^;]+;[^;]+;").Index + Regex.Match(extract, @"https://bs.to/serie/[^;]+;[^;]+;[^;]+;").Length);
                    }
                }

                // Array(extracts) übergeben
                return extracts;
            }
        }

        private void btn_list_Click(object sender, RoutedEventArgs e)
        {
            string[] extracts = new string[20];
            bool[] know = new bool[20];
            int n = 1;
            extracts = RegexExtract();

            txt_nr.Clear();
            txt_data.Clear();
            txt_know.Clear();
            cb_meny.Items.Clear();

            for (int i = 0; i < 20; i++)
            {
                if (extracts[i] != "")
                {
                    txt_nr.Text += n.ToString() + "\n\n\n";
                    txt_data.Text += Regex.Replace(extracts[i], @"https://bs.to/serie/([^/]+)[^;]+;S?(\d+)?\s?E?(\d+);([^;]+);", "$1\nStaffel: $2 - Episode: $3 | $4") + "\n\n";

                    foreach (string bookmark in bookmarks_scan)
                    {
                        if (Regex.Replace(extracts[i], @"https://(bs.to/serie/[^/]+).*", "$1") == Regex.Replace(bookmark, @"https://(bs.to/serie/[^/]+).*", "$1"))
                        {
                            know[i] = true;
                        }
                    }
                    if (know[i])
                    {
                        txt_know.Text += "Ja" + "\n\n\n";
                    }
                    else
                    {
                        txt_know.Text += "Nein" + "\n\n\n";
                    }

                    cb_meny.Items.Add(n);
                    n++;
                }
                pb_listing.Value++;
            }
            pb_listing.Value = 0;

            if (cb_meny.SelectedItem == null)
            {
                cb_meny.SelectedIndex = 0;
            }
            SaveConfig();
        }

        private void btn_open_all_Click(object sender, RoutedEventArgs e)
        {
            string[] sites = new string[20];
            int[] nr = new int[20];
            int meny = 0;
            int n = 1;
            sites = RegexExtract();

            if (cb_meny.Text.Length > 0)
            {
                meny = Convert.ToInt32(cb_meny.Text);
            }
            for (int i = 0; i < 20; i++)
            {
                if (sites[i] != "")
                {
                    if (meny > 0 && rb_all.IsChecked == true)
                    {
                        if (chb_first.IsChecked == true)
                        {
                            System.Diagnostics.Process.Start(Regex.Replace(sites[i], @"(https://bs.to/serie/[^/]+)/\d([^;]+);.*", @"$1/1$2")).WaitForExit(5000);
                        }
                        else
                        {
                            System.Diagnostics.Process.Start(Regex.Replace(sites[i], @"(https://bs.to/serie/[^;]+);.*", "$1")).WaitForExit(5000);
                        }
                        meny--;
                    }
                    nr[n] = i;
                    n++;
                }
                pb_opening.Value++;
            }
            pb_opening.Value = 0;
            if (meny > 0 && rb_all.IsChecked == false)
            {
                if (chb_first.IsChecked == true)
                {
                    System.Diagnostics.Process.Start(Regex.Replace(sites[nr[meny]], @"(https://bs.to/serie/[^/]+)/\d([^;]+);.*", @"$1/1$2")).WaitForExit(5000);
                }
                else
                {
                    System.Diagnostics.Process.Start(Regex.Replace(sites[nr[meny]], @"(https://bs.to/serie/[^;]+);.*", "$1")).WaitForExit(5000);
                }
            }
            SaveConfig();
        }

        private void cb_1_DropDownOpened(object sender, EventArgs e)
        {
            int n1 = cb_1.Items.Count;
            string[] items = new string[n1];
            ComboBoxItem[] cb_items = new ComboBoxItem[n1];

            for (int i = 0; i < n1; i++)
            {
                cb_items[i] = (ComboBoxItem)cb_1.Items.GetItemAt(i);
                items[i] = cb_items[i].Content.ToString();
            }

            for (int i = 0; i < n1; i++)
            {
                if (items[i] != cb_2.Text && items[i] != cb_3.Text)
                {
                    cb_items[i].Visibility = Visibility.Visible;
                }
                else
                {
                    cb_items[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void cb_2_DropDownOpened(object sender, EventArgs e)
        {
            string[] cb1_items = new string[cb_1.Items.Count];

            for (int i = 0; i < cb1_items.Length; i++)
            {
                ComboBoxItem typeItem = (ComboBoxItem)cb_1.Items.GetItemAt(i);
                cb1_items[i] = typeItem.Content.ToString();
            }

            cb_2.Items.Clear();
            cb_2.Items.Add("");
            foreach (var item in cb1_items)
            {
                if (item != cb_1.Text && item != cb_3.Text)
                {
                    cb_2.Items.Add(item);
                }
            }
        }

        private void cb_3_DropDownOpened(object sender, EventArgs e)
        {
            string[] cb1_items = new string[cb_1.Items.Count];

            for (int i = 0; i < cb1_items.Length; i++)
            {
                ComboBoxItem typeItem = (ComboBoxItem)cb_1.Items.GetItemAt(i);
                cb1_items[i] = typeItem.Content.ToString();
            }

            cb_3.Items.Clear();
            cb_3.Items.Add("");
            foreach (var item in cb1_items)
            {
                if (item != cb_1.Text && item != cb_2.Text)
                {
                    cb_3.Items.Add(item);
                }
            }
        }
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------

        // Link Creator:
        public string[] RegexExtractLinks()
        {
            string quelldata = "";
            quelldata += site;
            string pattern = @"(serie/[^/]+/[\d+]+/[^/]+/[^/]+/\b(?!unwatch:all|watch:all|\w+(>|\s))\w+)";
            int n = 0;

            while (Regex.IsMatch(quelldata, pattern))
            {
                quelldata = quelldata.Substring(Regex.Match(quelldata, pattern).Index + Regex.Match(quelldata, pattern).Length);
                n++;
            }

            string[] links = new string[n];
            quelldata = site;

            for (int i = 0; i < n; i++)
            {
                links[i] = Regex.Match(quelldata, pattern).ToString();
                quelldata = quelldata.Substring(Regex.Match(quelldata, links[i]).Index + Regex.Match(quelldata, links[i]).Length);
            }

            return links;
        }

        public string[] RegexExtractHoster()
        {
            string[] hosters = RegexExtractLinks();
            string allhoster = "";
            int n = 0;

            for (int i = 0; i < hosters.Length; i++)
            {
                hosters[i] = Regex.Replace(hosters[i], @".*/([^/]+)", "$1");
            }

            foreach (string host in hosters)
            {
                if (Regex.IsMatch(allhoster, host) != true)
                {
                    allhoster += host + "\n";
                    n++;
                }
            }

            string[] hoster = new string[n];
            allhoster = "";
            int ii = 0;

            foreach (string host in hosters)
            {
                if (Regex.IsMatch(allhoster, host) != true)
                {
                    allhoster += host + "\n";
                    hoster[ii] = host;
                    ii++;
                }
            }

            return hoster;
        }

        public string ExtractSite()
        {
            WebClient client = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            if (Regex.IsMatch(link, @"https?://bs\.to/serie/.*"))
            {
                string extract = client.DownloadString(link);
                return extract;
            }
            else
            {
                return null;
            }
        }

        private void btn_links_ausgeben(object sender, RoutedEventArgs e)
        {
            string output = "";
            int index = 0;

            cbtext = cb_hoster.Text;
            tb_output.Clear();
            cb_hoster.Items.Clear();
            link = tb_link.Text;
            site = ExtractSite();

            foreach (var host in RegexExtractHoster())
            {
                cb_hoster.Items.Add(host);

                if (host == cbtext)
                {
                    cb_hoster.SelectedIndex = index;
                }
                index++;
            }

            foreach (string sublink in RegexExtractLinks())
            {
                if (Regex.IsMatch(sublink, cb_hoster.Text))
                {
                    output += "https://bs.to/" + sublink + "\n";
                }
            }

            tb_output.Text = output;
        }

        private void tb_link_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            int index = 0;

            cbtext = cb_hoster.Text;
            cb_hoster.Items.Clear();
            ExtractSite();

            foreach (var host in RegexExtractHoster())
            {
                cb_hoster.Items.Add(host);

                if (host == cbtext)
                {
                    cb_hoster.SelectedIndex = index;
                }
                index++;
            }
        }
        // -------------------------------------------------------------------------------------------------------------------------------------------------------------
    }
}
