using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoApp
{
    internal class Track : ICloneable
    {
        public string Title { get; set; }
        public int Year { get; set; }
        public string SongWriter { get; set; }
        public string LeadVocals { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
