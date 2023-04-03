using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CS_project
{
    class MPicture:Meme
    {
        int box_count;

        public MPicture(JObject data) : base(data)
        {
        }

        public int Box_count { get => box_count; set => box_count = value; }

    }
}
