using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSGO_Match_Making
{
    public partial class Main : Form
    {
        private List<Server> Servers = new List<Server>();

        public Main()
        {
            InitializeComponent();

            Process p = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = "netsh",
                    RedirectStandardInput = true,
                    UseShellExecute = false
                }
            };
            p.Start();

            using (StreamWriter sw = p.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine("advfirewall set currentprofile state on");
                    sw.WriteLine("advfirewall set currentprofile firewallpolicy blockinboundalways,allowoutbound");
                    sw.WriteLine("advfirewall set domainprofile state on");
                    sw.WriteLine("advfirewall set privateprofile state on");
                }
                sw.Close();
            }

            p.Kill();


            foreach (Control c in this.Controls)
            {
                if (CSGO_Match_Making.Properties.Settings.Default.Blocked.Contains(c.Name))
                    ((CheckBox)c).Checked = false;
            }

            Servers.AddRange(new Server[] {
                new Server { Address = "syd.valve.net", Name = "Sydney" },
                new Server { Address = "dxb.valve.net", Name = "Dubai" },
                new Server { Address = "sto.valve.net", Name = "Russia" },
                new Server { Address = "vie.valve.net", Name = "Vienna" },
                new Server { Address = "lux.valve.net", Name = "Luxembourg" },
                new Server { Address = "208.78.164.1", Name = "Sterling" },
                new Server { Address = "eat.valve.net", Name = "Seattle" },
                new Server { Address = "sgp-1.valve.net", Name = "Singapore" },
                new Server { Address = "197.80.200.1", Name = "Africa" },
                new Server { Address = "gru.valve.net", Name = "Brazil" },
                new Server { Address = "116.202.224.146", Name = "India" }
            });

            CheckPings();

            Timer pings = new Timer { Interval = 5000 };
            pings.Tick += (se, e) => { CheckPings(); };
            pings.Start();
        }

        private void CheckPings()
        {
            foreach (Server s in Servers)
            {
                if (!CSGO_Match_Making.Properties.Settings.Default.Blocked.Contains(s.Name.ToLower()))
                {
                    PingReply r = new Ping().Send(s.Address);
                    if (r.Status == IPStatus.Success)
                    {
                        foreach (Control c in this.Controls)
                        {
                            if (c.Name == s.Name.ToLower() + "Ping")
                                c.Text = r.RoundtripTime.ToString();
                        }
                    }
                }
            }
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;

            StringBuilder setting = new StringBuilder(CSGO_Match_Making.Properties.Settings.Default.Blocked);

            if (!checkBox.Checked)
            {
                string Range = string.Empty;

                switch (checkBox.Name)
                {
                    case "sydney":
                        Range = "103.10.125.0-103.10.125.255";
                        break;

                    case "dubai":
                        Range = "185.25.183.0-185.25.183.255";
                        break;

                    case "russia":
                        Range = "146.66.156.0-146.66.156.255,146.66.157.0-146.66.157.255,185.25.180.0-185.25.180.255,185.25.181.0-185.25.181.255";
                        break;

                    case "brazil":
                        Range = "209.197.29.0-209.197.29.255,209.197.25.0-209.197.25.255,205.185.194.0-205.185.194.255";
                        break;

                    case "india":
                        Range = "180.149.41.0-180.149.41.255,116.202.224.146";
                        break;

                    case "sterling":
                        Range = "208.78.164.0-208.78.164.255,208.78.165.0-208.78.165.255,208.78.166.0-208.78.166.255";
                        break;

                    case "seattle":
                        Range = "192.69.96.0-192.69.96.255,192.69.97.0-192.69.97.255";
                        break;

                    case "vienna":
                        Range = "146.66.155.0-146.66.155.255,185.25.182.0-185.25.182.255";
                        break;

                    case "Luxembourg":
                        Range = "146.66.152.0-146.66.152.255,146.66.158.0-146.66.158.255,146.66.159.0-146.66.159.255";
                        break;

                    case "singapore":
                        Range = "103.28.54.0-103.28.54.255,103.28.55.0-103.28.55.255,103.10.124.0-103.10.124.255";
                        break;

                    case "africa":
                        Range = "152.111.192.0-152.111.192.255,197.80.200.0-197.80.200.255,196.38.180.0-196.38.180.255";
                        break;
                }

                new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "netsh",
                        Arguments = string.Format("advfirewall firewall add rule name=block{0} dir=out action=block protocol=any remoteip={1}", checkBox.Name, Range),
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                }.Start();

                setting.Append(checkBox.Name + "|");
            }
            else
            {
                new Process {
                    StartInfo = new ProcessStartInfo {
                        FileName = "netsh",
                        Arguments = string.Format("advfirewall firewall delete rule name=block{0}", checkBox.Name),
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                }.Start();

                setting.Replace(checkBox.Name + "|", "");
            }

            CSGO_Match_Making.Properties.Settings.Default.Blocked = setting.ToString();
            CSGO_Match_Making.Properties.Settings.Default.Save();

            checkBox.Text = checkBox.Checked ? "Unblocked" : "Blocked";
            checkBox.BackColor = checkBox.Checked ? Color.FromArgb(34, 127, 0) : Color.FromArgb(175, 0, 0);
        }
    }
}
