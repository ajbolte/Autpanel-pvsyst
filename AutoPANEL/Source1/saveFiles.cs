
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;

namespace WpfApp1.Source
{
    [Serializable]public class saveFiles
    {
        public int testInt;
        //public List<PointCollection> points = new List<PointCollection>();
       public PointCollection points = new PointCollection();
        public saveFiles(int addon)
        {
            testInt = 5+addon;
        }
    }
}