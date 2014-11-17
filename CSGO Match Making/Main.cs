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

            CheckPings(); // Initial ping check

            Timer pings = new Timer { Interval = 5000 }; // Create the timer that'll tick every 5 seconds
            pings.Tick += (se, e) => { CheckPings(); }; // On the event of a new tick, CheckPings() will be ran. Note, used Lambda Expression just to give an example of how it can be used.
            pings.Start(); // Start the timer.
        }

        private void CheckPings()
        {
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
                        Range = "103.10.125.0-103.10.125.255"; // Setting the Range.
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
    }
}
