using System;
using System.Threading;
using RFExplorerCommunicator;
using System.Collections.Generic;

namespace RFEStudio.GTK
{
    public class RFExplorer_Device : IRFExplorer_Device
    {
        int m_baud = 500000;
        int m_port = 0;

        String m_receive_buf = "";
        RFECommunicator m_dev;

        private List<SpectrumData_Element> m_spectrum;
        private List<SpectrumData_Element> m_spectrum_max;

        public RFExplorer_Device()
        {
            m_spectrum = new List<SpectrumData_Element>();
            m_spectrum_max = new List<SpectrumData_Element>();

            m_dev = new RFECommunicator(true);

            m_dev.PortClosedEvent += new EventHandler(OnRFE_PortClosed);
            m_dev.ReportInfoAddedEvent += new EventHandler(OnRFE_ReportLog);
            m_dev.ReceivedConfigurationDataEvent += new EventHandler(OnRFE_ReceivedConfigData);
            m_dev.UpdateDataEvent += new EventHandler(OnRFE_UpdateData);
        }

        private void OnRFE_ReceivedConfigData(object sender, EventArgs e)
        {
            Console.WriteLine(m_receive_buf);

            m_dev.SendCommand_DisableScreenDump();
            m_dev.SendCommand_Realtime();

            lock (m_dev.SweepData)
            {
                m_dev.SweepData.CleanAll();
            }
        }

        private void OnRFE_UpdateData(object sender, EventArgs e)
        {
            lock (m_dev.SweepData)
            {
                lock (m_spectrum)
                {
                    ConvertFromRFE(m_dev.SweepData.GetData(m_dev.SweepData.Count - 1), m_spectrum);
                }
                lock (m_spectrum_max)
                {
                    ConvertFromRFE(m_dev.SweepData.MaxHoldData, m_spectrum_max);
                }

                if (m_dev.SweepData.Count > 100)
                {
                    m_dev.SweepData.CleanAll();
                }
            }
        }

        private void OnRFE_ReportLog(object sender, EventArgs e)
        {
            EventReportInfo objArg = (EventReportInfo)e;
            Console.WriteLine(objArg.Data);
        }

        private void OnRFE_PortClosed(object sender, EventArgs e)
        {
            Console.WriteLine("RF Explorer PortClosed");
        }

        private void ConvertFromRFE(RFESweepData inp, List<SpectrumData_Element> outp)
        {
            outp.Clear();

            for (ushort i = 0; i < inp.TotalSteps; i++)
            {
                outp.Add( new SpectrumData_Element
                    {
                        freq = inp.StartFrequencyMHZ + inp.StepFrequencyMHZ * i,
                        value = inp.GetAmplitudeDBM(i)
                    }
                );
            }
        }

        public int baud
        {
            get
            {
                return m_baud;
            }
            set
            {
                m_baud = value;
            }
        }

        public int port
        {
            get
            {
                return m_port;
            }
            set
            {
                m_port = value;
            }
        }

        public bool Connect()
        {
            if (m_dev.GetConnectedPorts())
            {
                m_dev.ConnectPort(m_dev.ValidCP2101Ports[m_port], m_baud, RFECommunicator.IsUnixLike() && !RFECommunicator.IsMacOS(), true);
            }

            if (m_dev.PortConnected)
            {
                Console.WriteLine("Connected to port " + m_dev.ValidCP2101Ports[m_port]);

                m_dev.SendCommand_RequestConfigData();

                return true;
            }
            else
            {
                Console.WriteLine("ERROR: no port available, please review your connection");

                return false;
            }
        }

        public bool Close()
        {
            m_dev.Close();

            return true;
        }

        public void Set_Frequency(double f1, double f2)
        {
            m_dev.UpdateDeviceConfig(f1, f2);
        }

        public void Tick()
        {
            if (m_dev.PortConnected)
            {
                m_dev.ProcessReceivedString(true, out m_receive_buf);
            }
        }

        public List<SpectrumData_Element> GetSpectrumData()
        {
            //get copy of list
            lock (m_spectrum)
            {
                return m_spectrum.GetRange(0, m_spectrum.Count);
            }
        }

        public List<SpectrumData_Element> GetSpectrumDataMax()
        {
            //get copy of list
            lock (m_spectrum_max)
            {
                return m_spectrum_max.GetRange(0, m_spectrum_max.Count);
            }
        }

        public double freq_1
        {
            get
            {
                if (m_dev.PortConnected)
                {
                    m_dev.SendCommand_RequestConfigData();
                    m_dev.ProcessReceivedString(true, out m_receive_buf);
                    return m_dev.StartFrequencyMHZ;
                }
                return -1;
            }
        }

        public double freq_2
        {
            get
            {
                if (m_dev.PortConnected)
                {
                    m_dev.SendCommand_RequestConfigData();
                    m_dev.ProcessReceivedString(true, out m_receive_buf);
                    return m_dev.StopFrequencyMHZ;
                }
                return -1;
            }
        }
    }
}
