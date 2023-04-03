using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_project
{
    class MGIF:Meme
    {
        private int frameCount;

        //Call the Meme constructor
        public MGIF(JObject json):base(json)
        {
         
        }

        public int FrameCount { get => frameCount; set => frameCount = value; }

        public void play() { }
        public void pause() { }
    }
}
