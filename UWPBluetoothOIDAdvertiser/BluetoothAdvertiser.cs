using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Core;

namespace UWPBluetoothOIDAdvertiser
{
    public class BluetoothAdvertiser : INotifyPropertyChanged
    {
        private const ushort MicrosoftCompanyCode = 0x0006; //Microsoft's Bluetooth SIG ID
        private readonly byte[] TeamsProtoID = new byte[] { 0xF0, 0x34 }; // Microsoft Teams Meeting Room
        private readonly byte[] version = new byte[] { 0x0 };

        private BluetoothLEAdvertisementPublisher publisher = new BluetoothLEAdvertisementPublisher();
        public event PropertyChangedEventHandler PropertyChanged;

        private bool isRunning = false;
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            private set
            {
                if (isRunning != value)
                {
                    isRunning = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        private Guid oid;
        public Guid Oid
        {
            get
            {
                return oid;
            }
            set
            {
                if (oid != value)
                {
                    oid = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Oid)));
                    ChangeOID(oid);
                }
            }
        }

        private string status;
        public string Status
        {
            get
            {
                return status;
            }
            private set
            {
                if (status != value)
                {
                    status = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
                }
            }
        }

        private static BluetoothAdvertiser instance = new BluetoothAdvertiser();
        public static BluetoothAdvertiser Intance
        {
            get
            {
                return instance;
            }
        }

        private BluetoothAdvertiser()
        {
            publisher.StatusChanged += Publisher_StatusChanged;
        }

        public void Start()
        {
            publisher.Start();
        }

        public void Stop()
        {
            publisher.Stop();
        }

        private void ChangeOID(Guid oid)
        {
            if (publisher.Status == BluetoothLEAdvertisementPublisherStatus.Started ||
                publisher.Status == BluetoothLEAdvertisementPublisherStatus.Waiting)
            {
                publisher.Stop();
            }

            publisher.Advertisement.ManufacturerData.Clear();
            var paylod = CreatePayload(oid);
            var ind = BitConverter.IsLittleEndian;
            publisher.Advertisement.ManufacturerData.Add(CreatePayload(oid));
        }

        private BluetoothLEManufacturerData CreatePayload(Guid oid)
        {
            return new BluetoothLEManufacturerData(
                companyId: MicrosoftCompanyCode,
                data: TeamsProtoID.Concat(version).Concat(oid.ToByteArray()).ToArray().AsBuffer());
        }

        private async void Publisher_StatusChanged(
            BluetoothLEAdvertisementPublisher sender,
            BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Status = args.Status.ToString();
            });
            Debug.WriteLine(string.Format("Publisher Status Changed: {0}, {1}", args.Status.ToString(), args.Error.ToString()));
        }
    }
}
