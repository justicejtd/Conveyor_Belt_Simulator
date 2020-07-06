using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ProcP.Models
{
    class Processing
    {
        public ProgressBar pb;
        Simulation f;

        public Processing(ProgressBar pb, Simulation f)
        {
            int interval = 3000;
            this.pb = pb;
            this.f = f;

            for (int i = 0; i < 30; i++)
            {
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(interval);
                    f.Dispatcher.Invoke(new Action(() => Start()));
                });
            }
        }

        public void Start()
        {
            try
            {
                if (pb.Dispatcher.CheckAccess())
                {
                    pb.Dispatcher.Invoke(new Action(Start));
                    return;
                }
                pb.Value += 1;
                if (pb.Value == 30)
                {
                    pb.Value = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
