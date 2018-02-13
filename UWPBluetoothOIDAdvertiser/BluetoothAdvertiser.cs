using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.UI.Core;

namespace UWPBluetoothOIDAdvertiser
{
    public class BluetoothAdvertiser : INotifyPropertyChanged
    {

        private const ushort microsoftCompanyCode = 0x0006; // Microsoft's Bluetooth SIG ID
        private const byte protocolVersion = 0x0;
        private const ushort roomBeaconProtocolId = 0xF034; // Example beacon protocol Id
        private BluetoothLEAdvertisementPublisher publisher;

        public event PropertyChangedEventHandler PropertyChanged;

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
                    UpdateAdvertiser();
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
            UpdateAdvertiser();
        }

        public void Start()
        {
            publisher.Start();
        }

        public void Stop()
        {
            publisher.Stop();
        }

        #region Protected
        protected virtual BluetoothLEManufacturerData CreateBluetoothLEManufacturerData(ushort companyId, byte[] data)
        {
            return new BluetoothLEManufacturerData(companyId, data.AsBuffer());
        }

        protected virtual BluetoothLEAdvertisementPublisherStatus GetBluetoothStatus()
        {
            return publisher.Status;
        }

        protected virtual void SubscribeToBluetoothStatusChanged()
        {
            publisher.StatusChanged += Publisher_StatusChanged;
        }

        protected virtual void UnsubscribeToBluetoothStatusChanged()
        {
            publisher.StatusChanged -= Publisher_StatusChanged;
        }
        #endregion

        private BluetoothLEManufacturerData CreateAdvertiserPayload(Guid oid)
        {
            var data = new List<byte>();
            data.AddRange(BitConverter.GetBytes(roomBeaconProtocolId));
            data.Add(protocolVersion);
            data.AddRange(oid.ToByteArray());

            return CreateBluetoothLEManufacturerData(microsoftCompanyCode, data.ToArray());
        }

        private void UpdateAdvertiser()
        {
            try
            {
                if (publisher != null)
                {
                    Stop();
                    UnsubscribeToBluetoothStatusChanged();
                    publisher = null;
                }

                if (Guid.Empty != Oid)
                {
                    publisher = new BluetoothLEAdvertisementPublisher();
                    SubscribeToBluetoothStatusChanged();
                    publisher.Advertisement.ManufacturerData.Add(CreateAdvertiserPayload(oid));
                    Start();
                }
            }
            catch (Exception)
            {
            }
        }

        private async void Publisher_StatusChanged(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Status = args.Status.ToString();
            });
            Debug.WriteLine(string.Format("Publisher Status Changed: {0}, {1}", args.Status.ToString(), args.Error.ToString()));
        }
    }
}