using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using RedLearner;

namespace RedAlertUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public RedLearner.RedLearner Learner { get; set; }

        public App()
        {
            Learner = new RedLearner.RedLearner();
            Learner.ClassifySingleImage(Path.Combine(Environment.CurrentDirectory + "\\assets\\images", "spino_27.jpg"));            
        }
    }
}
