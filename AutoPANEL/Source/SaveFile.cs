﻿// Decompiled with JetBrains decompiler
// Type: WpfApp1.Source.SaveFile
// Assembly: AutoPANEL, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: C212B403-A08A-4409-8AAB-FE76C4D4CFFE
// Assembly location: C:\Release\Debug\Debug\AutoPANEL.exe

using System;
using System.Collections.ObjectModel;

namespace WpfApp1.Source
{
  [Serializable]
  public class SaveFile
  {
    public ObservableCollection<PanelTest> PanelTests;
    public MasterSettings masterSettings;
  }
}
