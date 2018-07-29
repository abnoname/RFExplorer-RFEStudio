using System;
using System.Collections.Generic;

namespace RFEStudio
{
    public class SpectrumData_Element
    {
        public double freq { get; set; } = 0;
        public double value { get; set; } = 0;
    }

    public interface IRFExplorer_Device
    {
        int baud { get; set; }
        int port { get; set; }
        bool Connect();
        bool Close();
        void Set_Frequency(double f1, double f2);
        void Tick();
        List<SpectrumData_Element> GetSpectrumData();
        List<SpectrumData_Element> GetSpectrumDataMax();
        double freq_1 { get; }
        double freq_2 { get; }
    }
}
