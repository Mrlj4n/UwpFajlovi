using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using Windows.Storage;
using Newtonsoft.Json;
using Windows.UI.Popups;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UwpFajloviVezba001
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<Osoba> listaOsoba =
            new ObservableCollection<Osoba>();
        private string ime = "Osobe.json";
        internal ObservableCollection<Osoba> ListaOsoba
        {
            get
            {
                return listaOsoba;
            }
        }

        private int Id = 0;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Osoba os1 = new Osoba { OsobaId = 1, Ime = "Ime1", Prezime = "Prezime1" };
            //Osoba os2 = new Osoba { OsobaId = 2, Ime = "Ime2", Prezime = "Prezime2" };
            //Osoba os3 = new Osoba { OsobaId = 3, Ime = "Ime3", Prezime = "Prezime3" };
            //listaOsoba.Add(os1);
            //listaOsoba.Add(os2);
            //listaOsoba.Add(os3);
            if (await Citaj())
            {
                GridView1.ItemsSource = listaOsoba;                
                Id = listaOsoba[listaOsoba.Count - 1].OsobaId;
            }
        }

        private void Resetuj()
        {
            TextBoxId.ClearValue(TextBox.TextProperty);
            TextBoxIme.ClearValue(TextBox.TextProperty);
            TextBoxPrezime.ClearValue(TextBox.TextProperty);
            GridView1.SelectedIndex = -1;
        }

        private async Task<bool> Validacija()
        {
            if (string.IsNullOrWhiteSpace(TextBoxIme.Text))
            {
                await new MessageDialog("Unesite ime").ShowAsync();
                TextBoxIme.Focus(FocusState.Pointer);
                return false;
            }
            if (string.IsNullOrWhiteSpace(TextBoxPrezime.Text))
            {
                await new MessageDialog("Unesite prezime").ShowAsync();
                TextBoxPrezime.Focus(FocusState.Pointer);
                return false;
            }

            return true;
        }

        private async Task Sacuvaj()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile fajl = 
                    await folder.CreateFileAsync(ime, CreationCollisionOption.ReplaceExisting);
                if (listaOsoba.Count>0)
                {
                    string jsonString = JsonConvert.SerializeObject(listaOsoba);
                    await FileIO.WriteTextAsync(fajl, jsonString);
                    TextBlock1.Text = "";

                }
                else
                {
                    await fajl.DeleteAsync();
                }

                await new MessageDialog("Podaci sacuvani").ShowAsync();
            }
            catch (Exception xcp)
            {
                TextBlock1.Text = xcp.Message;
            }
        }

        private async Task<bool> Citaj()
        {
            StorageFolder folder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile fajl = await folder.GetFileAsync(ime);
                string jsonString = await FileIO.ReadTextAsync(fajl);
                listaOsoba = JsonConvert.DeserializeObject<ObservableCollection<Osoba>>(jsonString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void GridView1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridView1.SelectedIndex>-1)
            {
                Osoba os = GridView1.SelectedItem as Osoba;
                TextBoxId.Text = os.OsobaId.ToString();
                TextBoxIme.Text = os.Ime;
                TextBoxPrezime.Text = os.Prezime;
            }
        }

        private void ButtonResetuj_Click(object sender, RoutedEventArgs e)
        {
            Resetuj();
        }

        private async void ButtonUbaci_Click(object sender, RoutedEventArgs e)
        {
            if (await Validacija())
            {
                Id++;
                Osoba os = new Osoba { 
                    OsobaId = Id,
                    Ime = TextBoxIme.Text,
                    Prezime = TextBoxPrezime.Text
                };

                listaOsoba.Add(os);
                GridView1.SelectedIndex = listaOsoba.Count - 1;
                //GridView1.UpdateLayout();
                GridView1.ScrollIntoView(os);
                await Sacuvaj();
            }
        }

        private async void Obrisi_Click(object sender, RoutedEventArgs e)
        {
            int indeks = GridView1.SelectedIndex;
            if (indeks >-1)
            {
                listaOsoba.RemoveAt(indeks);
                Resetuj();
                await Sacuvaj();
            }
            else
            {
                await new MessageDialog("Odaberite osobu").ShowAsync();
            }
        }

        private async void ButtonPromeni_Click(object sender, RoutedEventArgs e)
        {
            int indeks = GridView1.SelectedIndex;

            if (indeks > -1)
            {
                if (await Validacija())
                {
                    Osoba os = listaOsoba[indeks];
                    os.Ime = TextBoxIme.Text;
                    os.Prezime = TextBoxPrezime.Text;

                    GridView1.ItemsSource = null;
                    GridView1.ItemsSource = listaOsoba;
                    GridView1.SelectedIndex = indeks;
                    GridView1.UpdateLayout();
                    GridView1.ScrollIntoView(os);
                    await Sacuvaj();
                }
                

                
            }
            else
            {
                TextBlock1.Text = "Odaberite osobu";
            }
        }
    }
}
