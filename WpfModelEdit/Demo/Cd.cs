using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp
{
    internal class Cd
    {
        public string Artist { get; set; }

        [DisplayName("The album title")]
        public string AlbumTitle { get; set; }

        public List<Track> Tracks { get; set; }

        public Cd()
        {
            Tracks = new List<Track>();
        }
    }
}
