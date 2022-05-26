using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPCMakerGUI
{
    public class Preset
    {
        public string Name { get; set; }
        public string clientId { get; set; }
        public string state { get; set; }
        public string details { get; set; }
        public string largeImageKey { get; set; } 
        public string largeImageText { get; set; }
        public string smallImageKey { get; set; }
        public string smallImageText { get; set; }
        public string button1Name { get; set; }
        public string button1Url { get; set; }
        public string button2Name { get; set; }
        public string button2Url { get; set; }
        public Preset(string name, string clientid, string State, string Details, string LargeImageKey, string SmallImageKey, string LargeImageText, string SmallImageText, string Button1Name, string Button1Url, string Button2Name, string Button2Url)
        {
            Name = name;
            clientId = clientid;
            state = State;
            details = Details;
            largeImageKey = LargeImageKey;
            smallImageKey = SmallImageKey;
            largeImageText = LargeImageText;
            smallImageText = SmallImageText;
            button1Name = Button1Name;
            button1Url = Button1Url;
            button2Name = Button2Name;
            button2Url = Button2Url;
        }
        public Preset()
        {

        }
        public Preset(Preset preset)
        {
            this.clientId = preset.clientId;
            this.state = preset.state;
            this.details = preset.details;
            this.largeImageText = preset.largeImageText;
            this.smallImageText = preset.smallImageText;
            this.largeImageKey = preset.largeImageKey;
            this.smallImageKey = preset.smallImageKey;
            this.button1Name = preset.button1Name;
            this.button1Url = preset.button1Url;
            this.button2Name = preset.button2Name;
            this.button2Url = preset.button2Url;
        }
    }
    public static class PresetLoader
    {
        public static List<Preset> Presets = new List<Preset>();
        public static void SavePreset()
        {
            using (StreamWriter file = new StreamWriter($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json"))
            {
                file.Write(JsonConvert.SerializeObject(Presets, Formatting.Indented));
            }
        }
        public static void ReadPresets()
        {
            if (!Directory.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko"))
                Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko");
            if (File.Exists($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json"))
            {
                string presetstring = File.ReadAllText($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json");
                if (!string.IsNullOrEmpty(presetstring))
                {
                    JArray PresetArray = JArray.Parse(presetstring);
                    foreach (JObject preset in PresetArray)
                    {
                        Preset getpreset = new Preset();
                        getpreset.Name = preset["Name"].ToObject<string>();
                        getpreset.clientId = preset["clientId"].ToObject<string>();
                        getpreset.state = preset["state"].ToObject<string>();
                        getpreset.details = preset["details"].ToObject<string>();
                        getpreset.largeImageKey = preset["largeImageKey"].ToObject<string>();
                        getpreset.smallImageKey = preset["smallImageKey"].ToObject<string>();
                        getpreset.largeImageText = preset["largeImageText"].ToObject<string>();
                        getpreset.smallImageText = preset["smallImageText"].ToObject<string>();
                        if (preset["button1Name"] == null)
                        {
                            getpreset.button1Name = null;
                            getpreset.button1Url = null;
                            getpreset.button2Name = null;
                            getpreset.button2Url = null;
                        }
                        else
                        {
                            getpreset.button1Name = preset["button1Name"].ToObject<string>();
                            getpreset.button1Url = preset["button1Url"].ToObject<string>();
                            getpreset.button2Name = preset["button2Name"].ToObject<string>();
                            getpreset.button2Url = preset["button2Url"].ToObject<string>();
                        }
                        Presets.Add(getpreset);
                    }
                }
            }
            else
            {
                FileStream file = File.Create($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}Low/Senko/Presets.json");
                file.Close();
            }
        }
    }
}
