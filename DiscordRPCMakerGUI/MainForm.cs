using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DiscordRPC;

namespace DiscordRPCMakerGUI
{
    public partial class MainForm : Form
    {
        private static bool IsStarted = false;
        private static DiscordRpcClient client;
        private Dictionary<string, RichTextBox> allBoxes = new();
        private NotifyIcon notiIcon;
        public MainForm()
        {
            InitializeComponent();
            PresetLoader.ReadPresets();
            notiIcon = new NotifyIcon();
            notiIcon.Text = "Senkos RPC Maker";
            notiIcon.Icon = this.Icon;
            notiIcon.Visible = false;
            notiIcon.DoubleClick += NotiIcon_DoubleClick;
        }

        private void NotiIcon_DoubleClick(object? sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
            notiIcon.Visible = false;
        }

        private void PresenceBtn_Click(object sender, EventArgs e)
        {
            try
            {
                //MessageBox.Show("", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                string largeImageKey = null;
                string largeImageText = null;
                string smallImageKey = null;
                string smallImageText = null;
                string btn1Name = null;
                string btn1URL = null;
                string btn2Name = null;
                string btn2URL = null;
                List<DiscordRPC.Button> buttons = new List<DiscordRPC.Button>();
                if (HasKey(ClientIDBox.Text))
                {
                    MessageBox.Show("You must set a Client ID", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var id = this.ClientIDBox.Text;
                if (HasKey(DetailsBox.Text))
                {
                    MessageBox.Show("You must set the Details", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var details = this.DetailsBox.Text;
                if (HasKey(StateBox.Text))
                {
                    MessageBox.Show("You must set the State", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                var state = this.StateBox.Text;
                if (!HasKey(LIKBox.Text))
                {
                    largeImageKey = this.LIKBox.Text;
                    largeImageText = this.LITBox.Text;
                }
                if (!HasKey(SIKBox.Text))
                {
                    smallImageKey = this.SIKBox.Text;
                    smallImageText = this.SITBox.Text;
                }
                if (!HasKey(BN1Box.Text))
                {
                    btn1Name = this.BN1Box.Text;
                    btn1URL = this.BU1Box.Text;
                    Uri.TryCreate(btn1URL, UriKind.Absolute, out var uri1);
                    btn2Name = this.BN2Box.Text;
                    btn2URL = this.BU2Box.Text;
                    Uri.TryCreate(btn2URL, UriKind.Absolute, out var uri2);
                    if (!string.IsNullOrEmpty(btn1Name) && btn1Name != "Button Name 1")
                        buttons.Add(new DiscordRPC.Button() { Label = btn1Name, Url = uri1.AbsoluteUri });
                    if (!string.IsNullOrEmpty(btn2Name) && btn2Name != "Button Name 2")
                        buttons.Add(new DiscordRPC.Button() { Label = btn2Name, Url = uri2.AbsoluteUri });
                }
                if (client == null)
                    client = new DiscordRpcClient(id);
                if (client.ApplicationID != id)
                    client = new DiscordRpcClient(id);
                if (IsStarted)
                {
                    client.ClearPresence();
                    PresenceBtn.Text = "Start Presence";
                    PresenceBtn.BackColor = Color.White;
                    IsStarted = false;
                }
                else
                {
                    RichPresence rp = new RichPresence();
                    rp.Details = details;
                    rp.State = state;
                    Assets assets = null;
                    if (largeImageKey != null)
                    {
                        if (smallImageKey != null)
                        {
                            assets = new Assets()
                            {
                                LargeImageKey = largeImageKey,
                                LargeImageText = largeImageText,
                                SmallImageKey = smallImageKey,
                                SmallImageText = smallImageText,
                            };
                        }
                        else
                        {
                            assets = new Assets()
                            {
                                LargeImageKey = largeImageKey,
                                LargeImageText = largeImageText,
                            };
                        }
                    }
                    if (assets != null)
                    {
                        rp.Assets = assets;
                    }
                    if (buttons[0] != null)
                    {
                        rp.Buttons = buttons.ToArray();
                    }
                    client.SetPresence(rp);
                    if (!client.IsInitialized)
                        client.Initialize();
                    PresenceBtn.Text = "Stop Presence";
                    PresenceBtn.BackColor = Color.Red;
                    IsStarted = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            foreach (Control control in this.Controls)
            {
                if (control is RichTextBox)
                {
                    allBoxes.Add(control.Text, control as RichTextBox);
                }
            }
            foreach(RichTextBox box in allBoxes.Values)
            {
                box.GotFocus += Box_GotFocus;
                box.LostFocus += Box_LostFocus;
                box.KeyDown += Box_KeyDown;
            }
            this.ActiveControl = PresenceBtn;
            LoadPresets();
        }
        private void LoadPresets()
        {
            foreach(Control control in PresetPlacement.Controls)
            {
                if (control != null)
                {
                    PresetPlacement.Controls.Remove(control);
                }
            }
            foreach (var preset in PresetLoader.Presets)
            {
                System.Windows.Forms.Button btn = new System.Windows.Forms.Button();
                btn.Text = preset.Name;
                btn.ForeColor = Color.Black;
                btn.BackColor = Color.White;
                btn.Size = new Size(320, 43);
                btn.FlatStyle = FlatStyle.Flat;
                btn.Click += (s, e) =>
                {
                    ButtonAction(preset);
                };
                PresetPlacement.Controls.Add(btn);
            }
        }
        public void ButtonAction(Preset preset)
        {
            if (IsDelMode)
            {
                DialogResult warn = MessageBox.Show("Are you sure you want to delete this preset?", $"WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (warn == DialogResult.Yes)
                {
                    foreach (Control c in PresetPlacement.Controls)
                    {
                        if (c is System.Windows.Forms.Button btn)
                        {
                            if (btn.Text == preset.Name)
                            {
                                PresetPlacement.Controls.Remove(c);
                            }
                            else
                                btn.BackColor = Color.White;
                        }
                    }
                    LoadnDel.BackColor = Color.White;
                    IsDelMode = false;
                    PresetLoader.Presets.Remove(preset);
                    PresetLoader.SavePreset();
                    LoadPresets();
                    return;
                }
                else
                    return;
            }
            DialogResult box = MessageBox.Show("Would you like to load this preset?", $"Preset {preset.Name}", MessageBoxButtons.YesNo);
            if (box == DialogResult.Yes)
            {
                PresetName.Text = preset.Name;
                ClientIDBox.Text = preset.clientId;
                DetailsBox.Text = preset.details;
                StateBox.Text = preset.state;
                if (preset.largeImageKey != null)
                {
                    LIKBox.Text = preset.largeImageKey;
                    LITBox.Text = preset.largeImageText;
                }
                if (preset.smallImageKey != null)
                {
                    SIKBox.Text = preset.smallImageKey;
                    SITBox.Text = preset.smallImageText;
                }
                if (preset.button1Name != null && preset.button2Name != null && preset.button1Url != null && preset.button2Url != null)
                {
                    BN1Box.Text = preset.button1Name;
                    BU1Box.Text = preset.button1Url;
                    BN2Box.Text = preset.button2Name;
                    BU2Box.Text = preset.button2Url;
                }
            }
            else
            {
                return;
            }
        }

        private void Box_KeyDown(object? sender, KeyEventArgs e)
        {
            if(e.Control == true && e.KeyCode == Keys.V)
            {
                var box = (RichTextBox)sender;
                string s = (string)Clipboard.GetDataObject().GetData(DataFormats.Text);
                box.SelectedText += s;
                e.Handled = true; // disable Ctrl+V
            }
        }

        private void Box_LostFocus(object? sender, EventArgs e)
        {
            if (sender is RichTextBox box)
            {
                var str = allBoxes.FirstOrDefault(x => x.Value == box).Key;
                if (string.IsNullOrEmpty(box.Text))
                {
                    box.Text = str;
                    box.ForeColor = Color.DimGray;
                }
            }
        }

        private void Box_GotFocus(object? sender, EventArgs e)
        {
            if (sender is RichTextBox box)
            {
                var str = allBoxes.FirstOrDefault(x => x.Value == box).Key;
                if (box.Text == str)
                {
                    box.Text = string.Empty;
                    box.ForeColor = Color.Black;
                }
            }
        }

        private void SavePreset_Click(object sender, EventArgs e)
        {
            foreach(var presetr in PresetLoader.Presets)
            {
                if (PresetName.Text == presetr.Name)
                {
                    DialogResult result = MessageBox.Show("Would you like to update this preset?", "Warning", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                    if (HasKey(ClientIDBox.Text))
                    {
                        MessageBox.Show("You must have a Client ID Set to save this as a preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (HasKey(DetailsBox.Text))
                    {
                        MessageBox.Show("You must have details set to save this as a preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (HasKey(StateBox.Text))
                    {
                        MessageBox.Show("You must have the state set to save this as a preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    presetr.Name = PresetName.Text;
                    presetr.clientId = ClientIDBox.Text;
                    presetr.details = DetailsBox.Text;
                    presetr.state = StateBox.Text;
                    if (!HasKey(LIKBox.Text))
                    {
                        presetr.largeImageKey = LIKBox.Text;
                        if (HasKey(LITBox.Text))
                        {
                            presetr.largeImageText = "Senko's RPC Maker";
                        }
                        else
                        {
                            presetr.largeImageText = LITBox.Text;
                        }
                    }
                    if (!HasKey(SIKBox.Text))
                    {
                        presetr.smallImageKey = SIKBox.Text;
                        if (HasKey(SITBox.Text))
                        {
                            presetr.smallImageText = "https://github.com/ItsSenkoo";
                        }
                        else
                        {
                            presetr.smallImageText = SITBox.Text;
                        }
                    }
                    if (!HasKey(BN1Box.Text) && !HasKey(BN2Box.Text) && !HasKey(BU1Box.Text) && !HasKey(BU2Box.Text))
                    {
                        presetr.button1Name = BN1Box.Text;
                        presetr.button1Url = BU1Box.Text;
                        presetr.button2Name = BN2Box.Text;
                        presetr.button2Url = BU2Box.Text;
                    }
                    PresetLoader.SavePreset();
                    return;
                }
            }
            if (HasKey(PresetName.Text))
            {
                MessageBox.Show("Please set a name for this preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (HasKey(ClientIDBox.Text))
            {
                MessageBox.Show("You must have a Client ID Set to save this as a preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (HasKey(DetailsBox.Text))
            {
                MessageBox.Show("You must have details set to save this as a preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (HasKey(StateBox.Text))
            {
                MessageBox.Show("You must have the state set to save this as a preset", "ERROR!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Preset preset = new Preset();
            preset.Name = PresetName.Text;
            preset.clientId = ClientIDBox.Text;
            preset.details = DetailsBox.Text;
            preset.state = StateBox.Text;
            if (!HasKey(LIKBox.Text))
            {
                preset.largeImageKey = LIKBox.Text;
                if(HasKey(LITBox.Text))
                {
                    preset.largeImageText = "Senko's RPC Maker";
                }
                else
                {
                    preset.largeImageText = LITBox.Text;
                }
            }
            if (!HasKey(SIKBox.Text))
            {
                preset.smallImageKey = SIKBox.Text;
                if (HasKey(SITBox.Text))
                {
                    preset.smallImageText = "https://github.com/ItsSenkoo";
                }
                else
                {
                    preset.smallImageText = SITBox.Text;
                }
            }
            if (!HasKey(BN1Box.Text) && !HasKey(BN2Box.Text) && !HasKey(BU1Box.Text) && !HasKey(BU2Box.Text))
            {
                preset.button1Name = BN1Box.Text;
                preset.button1Url = BU1Box.Text;
                preset.button2Name = BN2Box.Text;
                preset.button2Url = BU2Box.Text;
            }
            PresetLoader.Presets.Add(preset);
            PresetLoader.SavePreset();
            LoadPresets();
        }
        public bool HasKey(string key)
        {
            return allBoxes.ContainsKey(key);
        }
        private static bool IsDelMode = false;
        private void LoadnDel_Click(object sender, EventArgs e)
        {
            if (IsDelMode)
            {
                foreach (Control c in PresetPlacement.Controls)
                {
                    if (c is System.Windows.Forms.Button btn)
                    {
                        btn.BackColor = Color.White;
                    }
                }
                LoadnDel.BackColor = Color.White;
                IsDelMode = false;
            }
            else
            {
                foreach (Control c in PresetPlacement.Controls)
                {
                    if (c is System.Windows.Forms.Button btn)
                    {
                        btn.BackColor = Color.Red;
                    }
                }
                LoadnDel.BackColor = Color.Red;
                IsDelMode = true;
            }
        }

        private void HideBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            notiIcon.Visible = true;
            notiIcon.BalloonTipTitle = "Senko's RPC Maker";
            notiIcon.BalloonTipText = "Application Hidden, check the system tray to reopen the application!";
            notiIcon.ShowBalloonTip(3000);
            this.ShowInTaskbar = false;
        }
    }
}
