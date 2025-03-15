using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace fergusBOT.config
{
    public class jsonREADER
    {
        public string token { get; set; } = null!;
        public string prefix { get; set; } = null!;

        public async Task ReadJSON()
        {
            using (StreamReader sr = new StreamReader("config.json"))
            {
               string json = await sr.ReadToEndAsync();
               jsonSTRUCTURE? data = JsonConvert.DeserializeObject<jsonSTRUCTURE>(json); //these lines get the data in debug for token

               if (data != null)
               {
                   this.token = data.token; //sets token to the token in the json file
                   this.prefix = data.prefix;
               }
            }
        }
    }

    internal sealed class jsonSTRUCTURE
    {
        public string token { get; set; } = null!;
        public string prefix { get; set; } = null!;
    }
}