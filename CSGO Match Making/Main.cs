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
        private List<Server> Servers = new List<Server>(); // Building the list of servers to ping.

        public Main()
        {
            InitializeComponent();

            #region Firewall
            /*
             * Need to make sure the firewall is on hey?
             */
            Process p = new Process
            { // Create a new process. Note, when creating, () isn't needed as we're setting some properties.
                StartInfo = new ProcessStartInfo
                { // Setting the process start info, only program name needed at the moment.
                    FileName = "netsh",
                    RedirectStandardInput = true, // Redirects the input for sending commands.
                    UseShellExecute = false
                }
            };
            p.Start();

            using (StreamWriter sw = p.StandardInput) // Opening up a StreamWriter for the processes input.
            {
                if (sw.BaseStream.CanWrite) // If we can input/write
                {
                    // Write the needed commands.
                    sw.WriteLine("advfirewall set currentprofile state on");
                    sw.WriteLine("advfirewall set currentprofile firewallpolicy blockinboundalways,allowoutbound");
                    sw.WriteLine("advfirewall set domainprofile state on");
                    sw.WriteLine("advfirewall set privateprofile state on");
                }
                sw.Close(); // Close the StreamWriter.
            }

            p.Kill(); // Kill the process
            #endregion

            foreach (Control c in this.Controls)
            {
                if (CSGO_Match_Making.Properties.Settings.Default.Blocked.Contains(c.Name))
                    ((CheckBox)c).Checked = false;
            }

            /*
             * Adding all the servers plus names.
             */
            Servers.AddRange(new Server[] {

                new Server { Address = "103.10.125.146", Name = "Sydney" }, //SYD

                new Server { Address = "dxb.valve.net", Name = "Dubai" }, //DXB

                new Server { Address = "sto.valve.net", Name = "Russia" }, //STO

                new Server { Address = "vie.valve.net", Name = "Vienna" }, //VIE

                new Server { Address = "lux.valve.net", Name = "Luxembourg" }, //LUX

                new Server { Address = "iad.valve.net", Name = "Sterling" }, //IAD

                new Server { Address = "eat.valve.net", Name = "Seattle" }, //EAT

                new Server { Address = "sgp.valve.net", Name = "Singapore" }, //SGP

                new Server { Address = "155.133.238.162", Name = "Africa" },

                new Server { Address = "205.185.194.34", Name = "Brazil" },

                new Server { Address = "155.133.233.98", Name = "India" }

            });

            CheckPings(); // Initial ping check

            Timer pings = new Timer { Interval = 10000 }; // Create the timer that'll tick every 10 seconds
            pings.Tick += (se, e) => { CheckPings(); }; // On the event of a new tick, CheckPings() will be ran. Note, used Lambda Expression just to give an example of how it can be used.
            pings.Start(); // Start the timer.
        }

        private void CheckPings()
        {
            pingwarning.Visible = true;
            foreach (Server s in Servers) // Foreach server in the Servers list.
            {
                if (!CSGO_Match_Making.Properties.Settings.Default.Blocked.Contains(s.Name.ToLower())) // If currently not block, continue.
                {
                    PingReply r = new Ping().Send(s.Address); // Setup a ping for the server address.
                    if (r.Status == IPStatus.Success) // If ping successful, continue.
                    {
                        foreach (Control c in this.Controls) // Loop through all controls in form to find the relevant ping label.
                        {
                            if (c.Name == s.Name.ToLower() + "Ping") // Checking label with current server name, eg. sydneyPing is a label, s.Name.ToLower() = "sydney". So s.Name.ToLower() + "Ping" will be "sydneyPing". If the control name equals the build name, continue.
                                c.Text = r.RoundtripTime.ToString(); // Change the ping label to the server ping time.
                        }
                    }
                }
            }
            pingwarning.Visible = false;
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            // All checkbox's come to this event, keeps code cleaner, more efficent. Therefore we use the object "sender" to find out which checkbox is currently being toggled.

            CheckBox checkBox = (CheckBox)sender; // Defining the checkbox from the sender.

            StringBuilder setting = new StringBuilder(CSGO_Match_Making.Properties.Settings.Default.Blocked); // A list of all blocked servers is kept in the settings which is now loaded into a StringBuilder.

            if (!checkBox.Checked) // If checkbox isn't checked, therefore blocking a server, continue.
            {
                string Range = string.Empty; // Defining the IP Range to block.

                switch (checkBox.Name) // Getting the checkbox name to select IP Range.
                {
                    case "sydney":
                        Range = "103.10.125.146,103.10.125.154,103.10.125.162"; // Setting the Range.
                        break;

                    case "dubai":
                        Range = "185.25.183.178,185.25.183.162";
                        break;

                    case "russia":
                        Range = "162.254.198.40-162.254.198.43,162.254.198.100-162.254.198.103,155.133.252.34-155.133.252.35,155.133.252.50-155.133.252.51,155.133.252.66-155.133.252.69";
                        break;

                    case "brazil":
                        Range = "205.185.194.34-205.185.194.36,205.185.194.50-205.185.194.52,190.217.33.34,190.217.33.50,190.217.33.66,155.133.246.50,155.133.246.34,155.133.246.66-155.133.246.67,155.133.249.162,155.133.249.178,155.133.249.194";
                        break;

                    case "india":
                        Range = "155.133.233.98-155.133.233.99. 155.133.232.98";
                        break;

                    case "sterling":
                        Range = "162.254.199.170,162.254.199.178,34.200.12.141,34.212.218.15,183.232.225.17-183.232.225.22,125.88.174.11-125.88.174.16,157.255.37.14-157.255.37.19,162.254.192.66,162.254.192.67,162.254.192.83,162.254.192.99,162.254.193.98,162.254.193.71,125.88.173.11-125.88.173.14,220.194.68.11-220.194.68.14,61.182.135.15-61.182.135.18,116.211.132.11-116.211.132.14,183.136.230.11-183.136.230.14";
                        break;

                    case "seattle":
                        Range = "155.133.235.18,155.133.235.34,162.254.195.70,162.254.195.86,155.133.254.130,155.133.254.131-155.133.254.141,205.196.6.66,205.196.6.74";
                        break;

                    case "vienna":
                        Range = "146.66.155.34-146.66.155.37,146.66.155.50-146.66.155.53,155.133.230.35-155.133.230.36,155.133.230.51-155.133.230.52,155.133.230.66-155.133.230.69";
                        break;

                    case "luxembourg":
                        Range = "3.120.105.51,155.133.248.34-155.133.248.37,162.254.197.162-162.254.197.165,162.254.197.178-162.254.197.179,162.254.197.70-162.254.197.75,162.254.196.82,162.254.196.66,146.66.154.20,146.66.154.34,146.66.154.22,146.66.154.35,185.25.182.66-185.25.182.69";
                        break;

                    case "singapore":
                        Range = "18.162.88.34,18.136.39.225,34.85.47.228,34.85.12.153,34.85.40.62,35.221.218.141,35.234.30.129,130.211.244.97,153.254.86.164,153.254.86.180,153.254.86.165,153.254.86.26-153.254.86.28,103.10.124.40,103.10.124.43-103.10.124.45,103.10.124.99,103.10.124.100-103.10.124.102,117.184.42.132-117.184.42.137,121.46.225.11-121.46.225.16,211.95.37.141-211.95.37.146,180.153.252.11-180.153.252.14,111.32.164.141-111.32.164.146,119.90.27.14-119.90.27.19,125.39.181.11-125.39.181.16,155.133.239.25-155.133.239.26,155.133.239.59,155.133.245.34-155.133.245.35";
                        break;

                    case "africa":
                        Range = "155.133.238.162-155.133.238.163,155.133.253.3-155.133.253.4,155.133.253.18-155.133.253.19,155.133.253.34";
                        break;
                }

                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = string.Format("advfirewall firewall add rule name=block{0} dir=out action=block protocol=any remoteip={1}", checkBox.Name, Range), // Same as before except now using string.Format(). Notice the {0} and {1}, the checkBox.Name and Range are going to be replacing the {0} and {1} when this is formatted.
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                }.Start();

                setting.Append(checkBox.Name + "|"); // Append to the setting file, eg. "sydney|". Meaning Sydney has been added to the block list.
            }
            else
            {
                new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "netsh",
                        Arguments = string.Format("advfirewall firewall delete rule name=block{0}", checkBox.Name), // {0} is being replaced by the checkBox name.
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                }.Start();

                setting.Replace(checkBox.Name + "|", ""); // Removing the blocked server, eg. "sydney|" will now be "". If there were other servers this'll still filter out the name, eg. "russia|africa|sydney|india|" would be "russia|africa|india|".
            }

            CSGO_Match_Making.Properties.Settings.Default.Blocked = setting.ToString(); // Making the current application setting equal the current StringBuilder string.
            CSGO_Match_Making.Properties.Settings.Default.Save(); // Saving the settings, written to local computer.

            checkBox.Text = checkBox.Checked ? "Unblocked" : "Blocked"; // Setting the checkbox text depending on the current check state, pretty much an if else statement returning a value. 'if (checkBox.Checked) checkBox.Text = "Unblocked" else checkBox.Text = "Blocked"'
            checkBox.BackColor = checkBox.Checked ? Color.FromArgb(34, 127, 0) : Color.FromArgb(175, 0, 0); // Same as above, returning a value depending on check state. 'if (checkBox.Checked) checkBox.BackColor = Green else checkBox.BackColor = Red'
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }
    }
}
