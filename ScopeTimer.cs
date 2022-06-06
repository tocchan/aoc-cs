using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AoC2021
{
    internal class ScopeTimer : IDisposable
    {
        public ScopeTimer(string label)
        { 
            m_label = label; 
            m_stopwatch = Stopwatch.StartNew();    
        }

        public void Dispose() => Dispose(true); 

        protected virtual void Dispose(bool disposing)
        {
            m_stopwatch.Stop();
            if (!string.IsNullOrEmpty(m_label)) 
            { 
                Console.WriteLine( Util.ApplyMarkup($"[cyan]{m_label} took {FormatTime()}.") ); 
            }
        }

        private string FormatTime()
        {
            double seconds = m_stopwatch.Elapsed.TotalSeconds;
            if (seconds > 0.5)
            {
                string time = seconds.ToString("0.000");
                return $"[red]{time}s"; 
            } 
            else if (seconds > 0.001)
            {
                string time = (1000.0 * seconds).ToString("0.000");
                return $"[yellow]{time}ms"; 
            }
            else
            {
                string time = (1000000.0 * seconds).ToString("0.000");
                return $"[green]{time}us"; 
            }
        }

        private string m_label; 
        private Stopwatch m_stopwatch; 
    }
}
