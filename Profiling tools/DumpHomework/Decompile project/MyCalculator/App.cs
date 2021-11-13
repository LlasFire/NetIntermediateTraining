// Decompiled with JetBrains decompiler
// Type: MyCalculatorv1.App
// Assembly: MyCalculatorv1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8E4247A5-25E4-47A6-84F4-A414933F7536
// Assembly location: C:\Users\Vasili_Isakau\source\repos\NetIntermediateTraining\Profiling tools\DumpHomework\MyCalculator.exe

using System;
using System.Windows;

namespace MyCalculatorv1
{
    public class App : Application
    {
        public void InitializeComponent() => this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);

        [STAThread]
        public static void Main()
        {
            App app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}
