
using System.Collections.ObjectModel;


namespace WpfApp1.Source
{
   public class MasterSettings
    {
          public string image { get; set; }
        public string path { get; set; }
        public string imageEx { get; set; }
        public string title { get; set; }
        public double scale { get; set; }
          public double scaleMM { get; set; }
          public double dxfScale { get; set; }
          public double latitude { get; set; }
          public double longitude { get; set; }
          public double lineScale { get; set; }
        public double drawingThickness { get; set; }
 

        public double SetLineScale(double zoom)
        {
            lineScale = 1/ (zoom / drawingThickness*5 );

            return lineScale;
        }

        public ObservableCollection<PVmodule> moduleTypes = new ObservableCollection<PVmodule>();

        public MasterSettings()
        {
            scale = 1;
            drawingThickness = 10;
            lineScale = 4;

        }
    }
}
