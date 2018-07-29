using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Series;
using Xamarin.Forms;

namespace RFEStudio.ViewModels
{
    public partial class ViewModel
    {
        private PlotModel m_Sweep_PlotModel;
        private StairStepSeries m_spectrum;
        private StairStepSeries m_spectrum_max;

        private double m_freq_1 = 0;
        private double m_freq_2 = 0;

        public ICommand cmdUpdateSettings { protected set; get; }
        public ICommand cmdConnect { protected set; get; }
        public ICommand cmdInfo { protected set; get; }

        public void InitializeCommands_Main()
        {
            cmdUpdateSettings = new Command<string>((o) =>
            {
                if (m_freq_2 >= m_freq_1)
                {
                    m_rf_dev.Set_Frequency(m_freq_1, m_freq_2);
                }
            });

            cmdConnect = new Command<string>((o) =>
            {
                m_rf_dev.Connect();

                freq_1 = m_rf_dev.freq_1;
                freq_2 = m_rf_dev.freq_2;
            });

            cmdInfo = new Command<string>((o) =>
            {
                Notify_Show("RFEStudio - RF Explorer GTK# GUI\n(C)2018 Franz Neumann\nnetinside2000@gmx.de\nhttps://www.engineering-jena.de\n\nLibraries:\nRFExplorerCommunicator.dll\nby Ariel Rocholl\nLicense LGPL 3.0\nhttps://github.com/RFExplorer/RFExplorer-for-.NET");
            });
        }

        public void Setup_Oxyplot()
        {
            m_spectrum = new OxyPlot.Series.StairStepSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.Blue,
                LineStyle = LineStyle.Solid
            };
            m_spectrum_max = new OxyPlot.Series.StairStepSeries()
            {
                StrokeThickness = 2,
                Color = OxyColors.Gray,
                LineStyle = LineStyle.Solid
            };

            sweep_plot_model = new PlotModel
            {
                PlotAreaBackground = OxyColors.White,
                Title = "RF Sweep dBm",
                PlotType = PlotType.XY,
                Axes = {
                    new OxyPlot.Axes.LinearAxis () {
                        Minimum = -120,
                        Maximum = 0,
                        Position = OxyPlot.Axes.AxisPosition.Right,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dash
                    },
                    new OxyPlot.Axes.LinearAxis () {
                        Position = OxyPlot.Axes.AxisPosition.Bottom,
                        MajorGridlineStyle = LineStyle.Solid,
                        MinorGridlineStyle = LineStyle.Dash
                    }
                },
                Series =
                {
                    m_spectrum_max,
                    m_spectrum
                }
            };
        }

        private void Setup_RFDevice()
        {
            m_rf_dev.Connect();

            Thread.Sleep(500);

            freq_1 = m_rf_dev.freq_1;
            freq_2 = m_rf_dev.freq_2;
        }

        private void Setup_Transceive_Task()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    m_rf_dev.Tick();

                    await Task.Delay(100);

                    m_spectrum.Points.Clear();
                    m_spectrum_max.Points.Clear();
                    foreach (SpectrumData_Element m in m_rf_dev.GetSpectrumData())
                    {
                        m_spectrum.Points.Add(new DataPoint(
                            m.freq, m.value)
                        );
                    }
                    foreach (SpectrumData_Element m in m_rf_dev.GetSpectrumDataMax())
                    {
                        m_spectrum_max.Points.Add(new DataPoint(
                            m.freq, m.value)
                        );
                    }

                    sweep_plot_model.InvalidatePlot(true);
                }
            });
        }

        public int baud
        {
            set
            {
                if (m_rf_dev.baud != value)
                {
                    m_rf_dev.baud = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return m_rf_dev.baud;
            }
        }

        public int port
        {
            set
            {
                if (m_rf_dev.port != value)
                {
                    m_rf_dev.port = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return m_rf_dev.port;
            }
        }

        public PlotModel sweep_plot_model
        {
            set
            {
                m_Sweep_PlotModel = value;
                OnPropertyChanged();
            }
            get
            {
                return m_Sweep_PlotModel;
            }
        }

        public double freq_1
        {
            set
            {
                if (m_freq_1 != value)
                {
                    m_freq_1 = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return m_freq_1;
            }
        }

        public double freq_2
        {
            set
            {
                if (m_freq_2 != value)
                {
                    m_freq_2 = value;
                    OnPropertyChanged();
                }
            }
            get
            {
                return m_freq_2;
            }
        }
    }
}