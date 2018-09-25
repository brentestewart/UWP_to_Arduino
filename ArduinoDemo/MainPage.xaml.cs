using Microsoft.Maker.RemoteWiring;
using Microsoft.Maker.Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ArduinoDemo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private RemoteDevice Arduino { get; set; }
        private const string PotPin = "A0";
        private string _output = "No output";

        public string Output
        {
            get => _output;
            set
            {
                if(value != _output)
                _output = value;
                OnPropertyChanged();
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            DataContext = this;

            InitializeArduio();
        }

        private async void InitializeArduio()
        {
            var devices = await UsbSerial.listAvailableDevicesAsync();
            var uno = devices[0];
            var connection = new UsbSerial(uno);
            Arduino = new RemoteDevice(connection);

            Arduino.DeviceReady += Setup;
            Arduino.DeviceConnectionFailed += Failure;
            Arduino.AnalogPinUpdated += PotUpdated;
            connection.begin(57600, SerialConfig.SERIAL_8N1);
        }

        private async void PotUpdated(string pin, ushort value)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                   Output = value.ToString();
                });
        }

        private void Failure(string message)
        {
        }

        private void Setup()
        {
            Arduino.pinMode(PotPin, PinMode.ANALOG);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
