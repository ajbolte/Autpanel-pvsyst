using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace WpfApp1.Source
{
    [Serializable] public class SaveFile
    {

      
        public ObservableCollection<PanelTest> PanelTests;
        public MasterSettings masterSettings;
        public SaveFile()
        {

        }
    }
}