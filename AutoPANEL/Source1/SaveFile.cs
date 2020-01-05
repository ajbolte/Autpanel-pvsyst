using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace WpfApp1.Source
{
    [Serializable] public class SaveFile
    {
        public double scale = 1;
       public double dxfScale = 1;
       public string imagePath = "";
      
        public List<PanelTest> PanelTests = new List<PanelTest>();

        public SaveFile()
        {

        }
    }
}