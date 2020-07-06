using com.sun.org.apache.regexp.@internal;
using com.sun.xml.@internal.ws.server;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using java.nio.channels;
using java.util.concurrent.locks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace ProcP.Models
{
    public class MonitorLuggages
    {
        public bool IsBeltActive { get; set; }
     

        public MonitorLuggages()
        {
            IsBeltActive = false;
        }

        public void EnterBelt(Storyboard pathAnimationStoryboard, Luggage luggage)
        {
            lock(this)
            {
                Console.WriteLine("Monitor enters");
                while (IsBeltActive)
                {
                    pathAnimationStoryboard.Pause(luggage.GetPath());
                    Console.WriteLine($"{luggage.Id} thread is waiting");
                    Monitor.Wait(this);
                }
                if (pathAnimationStoryboard.GetIsPaused(luggage.GetPath()))
                {
                    Console.WriteLine($"{luggage.Id} animation resume");
                    pathAnimationStoryboard.Resume(luggage.GetPath());
                }
                IsBeltActive = true;
            }
            
        }

        public void ExitBelt()
        {
            lock(this)
            {
                IsBeltActive = false;
                Console.WriteLine("Exiting belt and notifying waiting threads");
                Monitor.PulseAll(this);
            }
        }
    }
}
