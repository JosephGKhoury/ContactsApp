using P1FriendsJoe;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P1ListBox2Martin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        List<Contact> buddies = new List<Contact>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var line in File.ReadAllLines("../../../Contacts.txt"))
            {
                string[] values = line.Split(',');
                if (values.Length == 5)
                {
                    Color color = (Color)ColorConverter.ConvertFromString(values[3]);
                    Brush brush = new SolidColorBrush(color);

                    Contact contact = new Contact()
                    {
                        FirstName = values[0],
                        LastName = values[1],
                        Phone = values[2],
                        BirthDate = values[4],
                        Color = brush,
                    };
                    buddies.Add(contact);
                }
            }
            MainListBox.ItemsSource = buddies;
        }
        Contact? editContact = null;

        private void AddFriend(string firstName, string lastName, string phone, string birthDate)
        {
            string colorText = combo3.Text;
            Color color = (Color)ColorConverter.ConvertFromString(colorText);
            Contact contact = new Contact()
            {
                FirstName = firstName,
                LastName = lastName,
                Phone = phone,
                Color = new SolidColorBrush(color),
                BirthDate = birthDate
            };
            buddies.Add(contact);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string firstName = FirstNameTextBox.Text;
            string phone = PhoneTextBox.Text;
            string lastName = LastNameTextBox.Text;
            string birthDate = BirthdateDatePicker.Text;
            string colorText = combo3.Text;
            Color color = (Color)ColorConverter.ConvertFromString(colorText);
            Brush brush = new SolidColorBrush(color);
            if (editContact != null)
            {
                bool firstNameChanged = editContact.FirstName != firstName;
                bool lastNameChanged = editContact.LastName != lastName;

                if (firstNameChanged && lastNameChanged &&
                    buddies.Any((contact) => contact.FirstName == firstName && contact.LastName == lastName))
                {
                    MessageBox.Show($"Namnet {firstName} {lastName} existerar redan");
                }
                else if (firstNameChanged && buddies.Any((contact) => contact.FirstName == firstName && contact.LastName == editContact.LastName))
                {
                    MessageBox.Show($"Förnamnet {firstName} existerar redan");
                }
                else if (lastNameChanged && buddies.Any((contact) => contact.FirstName == editContact.FirstName && contact.LastName == lastName))
                {
                    MessageBox.Show($"Efternamnet {lastName} existerar redan");
                }
                else if (firstName == "" || lastName == "")
                {
                    MessageBox.Show($"Din vän heter väl något!");
                }
                else
                {
                    editContact.FirstName = firstName;
                    editContact.LastName = lastName;
                    editContact.Phone = phone;
                    editContact.BirthDate = birthDate;
                    editContact.Color = brush;
                    FirstNameTextBox.Clear();
                    LastNameTextBox.Clear();
                    PhoneTextBox.Clear();
                    editContact = null;
                }
            }
            else
            {
                if (buddies.Any((contact) => contact.FirstName == firstName && contact.LastName == lastName))
                {
                    MessageBox.Show($"Namnet {firstName} {lastName} existerar redan");
                }
                else if (buddies.Any((contact) => contact.Phone == phone))
                {
                    MessageBox.Show($"Numret {phone} existerar redan");
                }
                else if (firstName == "" || lastName == "")
                {
                    MessageBox.Show($"Din vän heter väl något!");
                }
                else
                {
                    AddFriend(firstName, lastName, phone, birthDate);
                    FirstNameTextBox.Clear();
                    LastNameTextBox.Clear();
                    PhoneTextBox.Clear();
                }
            }
            //denna kod behövs för att kunna adda kontakter efter att man har sorterat
            //annars går det inte att adda kontakter efter sortering
            var filteredContacts = buddies.Where(contact => searchText.Text == "" || (contact.FirstName + " " + contact.LastName).Contains(searchText.Text)).OrderBy(orderParameter);
            MainListBox.ItemsSource = combo1.SelectedIndex == 0 ? filteredContacts : filteredContacts.Reverse();
        }
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainListBox.SelectedItem != null)
            {
                editContact = (Contact)MainListBox.SelectedItem;
                FirstNameTextBox.Text = editContact.FirstName;
                LastNameTextBox.Text = editContact.LastName;
                PhoneTextBox.Text = editContact.Phone;
                BirthdateDatePicker.Text = editContact.BirthDate;
                AddButton.Content = "Save Contact";
            }
        }
        private void combo1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshContacts();
        }
        private object orderParameter(Contact contact)
        {
            if (combo1.SelectedIndex == 0)
            {
                return contact.LastName;
            }
            else
            {
                return contact.FirstName;
            }
        }
        private void combo2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshContacts();
        }
        private void searchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshContacts();
        }
        private void RefreshContacts()
        {
            if (IsLoaded)
            {
                if (combo2.SelectedIndex == 0)
                {
                    MainListBox.ItemsSource = buddies.Where(FilterContact).OrderBy(orderParameter);
                }
                else
                {
                    MainListBox.ItemsSource = buddies.Where(FilterContact).OrderByDescending(orderParameter);
                }
            }
        }
        private bool FilterContact(Contact contact)
        {
            string s = searchText.Text.ToLower();
            return contact.FirstName.ToLower().Contains(s) || contact.LastName.ToLower().Contains(s) || contact.Phone.ToLower().Contains(s) || contact.BirthDate.ToLower().Contains(s);
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (MainListBox.SelectedItem != null)
            {
                Contact contact = (Contact)MainListBox.SelectedItem;
                buddies.Remove(contact);
                MainListBox.Items.Refresh();
                FirstNameTextBox.Clear();
                LastNameTextBox.Clear();
                PhoneTextBox.Clear();
                editContact = null;
            }
            else
            {
                removeBtn.IsEnabled = false;
            }
        }
    }
}
