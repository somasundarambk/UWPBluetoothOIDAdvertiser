using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPBluetoothOIDAdvertiser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        BluetoothAdvertiser Advertiser
        {
            get
            {
                return BluetoothAdvertiser.Intance;
            }
        }
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void LogIn_Click(object sender, RoutedEventArgs e)
        {
            await WAMAuthentication.Instance.AuthenticateDefaultUser();
            if (WAMAuthentication.Instance.AccessToken != null)
            {
                UserName.Text = "Now signed in : " + WAMAuthentication.Instance.Name;
                AliasStackPanel.Visibility = Visibility.Visible;
            }
        }

        private async void LookupAliasButton_Click(object sender, RoutedEventArgs e)
        {
            var id = await GraphSearch.SearchByAlias(Alias.Text);
            if (!string.IsNullOrEmpty(id))
            {
                OIDStackPanel.Visibility = Visibility.Visible;
                OID.Text = id;
                Advertiser.Oid = new System.Guid(id);
            }
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Advertiser.Start();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            Advertiser.Stop();
        }
    }
}
