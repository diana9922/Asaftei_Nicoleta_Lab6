using System;
using System.Collections.Generic;
using System.Linq;
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
using AutoLotModel;
using System.Data.Entity;
using System.Data;


namespace Asaftei_Nicoleta_Lab6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //ENUM
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }



    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();
        CollectionViewSource customerViewSource;


        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cust in ctx.Customers on ord.CustId equals
                             cust.CustId
                             join inv in ctx.Inventories on ord.CarId
                 equals inv.CarId
                             select new
                             {
                                 ord.OrderId,
                                 ord.CarId,
                                 ord.CustId,
                                 cust.FirstName,
                                 cust.LastName,
                                 inv.Make,
                                 inv.Color
                             };
            customerOrdersViewSource.Source = queryOrder.ToList();
        }




        public object inventoryViewSource { get; private set; }
        CollectionViewSource customerOrdersViewSource;


        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            private void SetValidationBinding()
            {
                Binding firstNameValidationBinding = new Binding();
                firstNameValidationBinding.Source = customerViewSource;
                firstNameValidationBinding.Path = new PropertyPath("FirstName");
                firstNameValidationBinding.NotifyOnValidationError = true;
                firstNameValidationBinding.Mode = BindingMode.TwoWay;
                firstNameValidationBinding.UpdateSourceTrigger =
               UpdateSourceTrigger.PropertyChanged;
                //string required
                firstNameValidationBinding.ValidationRules.Add(new StringNotEmpty());
                firstNameTextBox.SetBinding(TextBox.TextProperty,
               firstNameValidationBinding);
                Binding lastNameValidationBinding = new Binding();
                lastNameValidationBinding.Source = customerViewSource;
                lastNameValidationBinding.Path = new PropertyPath("LastName");
                lastNameValidationBinding.NotifyOnValidationError = true;
                lastNameValidationBinding.Mode = BindingMode.TwoWay;
                lastNameValidationBinding.UpdateSourceTrigger =
               UpdateSourceTrigger.PropertyChanged;
                //string min length validator
                lastNameValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
                lastNameTextBox.SetBinding(TextBox.TextProperty,
               lastNameValidationBinding); //setare binding nou
            }

            System.Windows.Data.CollectionViewSource customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // customerViewSource.Source = [generic data source]
            customerViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerViewSource")));
            customerOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("customerOrdersViewSource")));

            //customerOrdersViewSource.Source = ctx.Orders.Local;
            customerViewSource.Source = ctx.Customers.Local;
            ctx.Customers.Load();
            ctx.Orders.Load();
            ctx.Inventories.Load();
            cmbCustomers.ItemsSource = ctx.Customers.Local;
            //cmbCustomers.DisplayMemberPath = "FirstName";
            cmbCustomers.SelectedValuePath = "CustId";
            cmbInventory.ItemsSource = ctx.Inventories.Local;
            //cmbInventory.DisplayMemberPath = "Make";
            cmbInventory.SelectedValuePath = "CarId";
            System.Windows.Data.CollectionViewSource inventoryViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("inventoryViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // inventoryViewSource.Source = [generic data source]
            BindDataGrid();






            //CUSTOMERS
            private void btnSave_Click(object sender, RoutedEventArgs e)
            {
                Customer customer = null;
                if (action == ActionState.New)
                {
                    try
                    {
                        //instantiem Customer entity
                        customer = new Customer()
                        {
                            FirstName = firstNameTextBox.Text.Trim(),
                            LastName = lastNameTextBox.Text.Trim()
                        };
                        //adaugam entitatea nou creata in context
                        ctx.Customers.Add(customer);
                        customerViewSource.View.Refresh();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    //using System.Data;
                    catch (DataException ex)
                    { MessageBox.Show(ex.Message);
                    }
                    btnNew.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    btnPrevious.IsEnabled = true;
                    btnNext.IsEnabled = true;
                    BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                    SetValidationBinding();
                }





                else
                           if (action == ActionState.Edit)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        customer.FirstName = firstNameTextBox.Text.Trim();
                        customer.LastName = lastNameTextBox.Text.Trim();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                    // pozitionarea pe item-ul curent
                    customerViewSource.View.MoveCurrentTo(customer);

                    btnNew.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    btnPrevious.IsEnabled = true;
                    btnNext.IsEnabled = true;

                }


                else if (action == ActionState.Delete)
                {
                    try
                    {
                        customer = (Customer)customerDataGrid.SelectedItem;
                        ctx.Customers.Remove(customer);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    customerViewSource.View.Refresh();
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
            }



            private void btnEdit_Click(object sender, RoutedEventArgs e)
            {
                action = ActionState.Edit;
                BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                btnNew.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnSave.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnPrevious.IsEnabled = false;
                btnNext.IsEnabled = false;
                SetValidationBinding();
            }


            private void btnDelete_Click(object sender, RoutedEventArgs e)
            {
                action = ActionState.Delete;

                btnNew.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnSave.IsEnabled = true;
                btnCancel.IsEnabled = true;

                btnPrevious.IsEnabled = false;
                btnNext.IsEnabled = false;
            }

            private void btnNext_Click(object sender, RoutedEventArgs e)
            {
                customerViewSource.View.MoveCurrentToNext();
            }

            private void btnPrevious_Click(object sender, RoutedEventArgs e)
            {
                customerViewSource.View.MoveCurrentToPrevious();
            }




            //Inventory
            private void btnSave1_Click(object sender, RoutedEventArgs e)
            {
                Inventory inventory = null;
                if (action == ActionState.New)
                {
                    try
                    {
                        //instantiem Customer entity
                        inventory = new Inventory()
                        {
                            FirstName = firstNameTextBox.Text.Trim(),
                            LastName = lastNameTextBox.Text.Trim()
                        };
                        //adaugam entitatea nou creata in context
                        ctx.Customers.Add(inventory);
                        customerViewSource.View.Refresh();
                        //salvam modificarile
                        ctx.SaveChanges();
                        //using System.Data;
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    btnNew.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    btnPrevious.IsEnabled = true;
                    btnNext.IsEnabled = true;
                    BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                    BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                    SetValidationBinding();
                }


                else
                           if (action == ActionState.Edit)
                {
                    try
                    {
                        inventory = (Inventory)customerDataGrid.SelectedItem;
                        inventory.FirstName = firstNameTextBox.Text.Trim();
                        inventory.LastName = lastNameTextBox.Text.Trim();
                        //salvam modificarile
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    inventoryViewSource.View.Refresh();
                    // pozitionarea pe item-ul curent
                    inventoryViewSource.View.MoveCurrentTo(inventory);

                    btnNew.IsEnabled = true;
                    btnEdit.IsEnabled = true;
                    btnDelete.IsEnabled = true;
                    btnSave.IsEnabled = false;
                    btnCancel.IsEnabled = false;
                    btnPrevious.IsEnabled = true;
                    btnNext.IsEnabled = true;

                }

                else if (action == ActionState.Delete)
                {
                    try
                    {
                        inventory = (Inventory)inventoryDataGrid.SelectedItem;
                        ctx.Inventory.Remove(inventory);
                        ctx.SaveChanges();
                    }
                    catch (DataException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    inventoryViewSource.View.Refresh();
                }
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                btnPrevious.IsEnabled = true;
                btnNext.IsEnabled = true;
            }


            private void btnEdit_Click(object sender, RoutedEventArgs e)
            {
                action = ActionState.Edit;
                BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                btnNew.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnSave.IsEnabled = true;
                btnCancel.IsEnabled = true;
                btnPrevious.IsEnabled = false;
                btnNext.IsEnabled = false;
                SetValidationBinding();
            }
            private void btnDelete1_Click(object sender, RoutedEventArgs e)
            {
                action = ActionState.Delete;

                btnNew.IsEnabled = false;
                btnEdit.IsEnabled = false;
                btnDelete.IsEnabled = false;
                btnSave.IsEnabled = true;
                btnCancel.IsEnabled = true;

                btnPrevious.IsEnabled = false;
                btnNext.IsEnabled = false;
            }

            private void btnPrevious_Click(object sender, RoutedEventArgs e)
            {
                inventoryViewSource.View.MoveCurrentToPrevious();



                //ORDERS
                private void btnSave2_Click(object sender, RoutedEventArgs e)
                {
                    Order order = null;
                    if (action == ActionState.New)
                    {
                        try
                        {
                            Customer customer = (Customer)cmbCustomers.SelectedItem;
                            Inventory inventory = (Inventory)cmbInventory.SelectedItem;
                            //instantiem Order entity
                            order = new Order()
                            {

                                CustId = customer.CustId,
                                CarId = inventory.CarId
                            };
                            //adaugam entitatea nou creata in context
                            ctx.Orders.Add(order);
                            customerOrdersViewSource.View.Refresh();
                            //salvam modificarile
                            ctx.SaveChanges();
                        }
                        catch (DataException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        BindingOperations.ClearBinding(firstNameTextBox, TextBox.TextProperty);
                        BindingOperations.ClearBinding(lastNameTextBox, TextBox.TextProperty);
                        SetValidationBinding();
                    }
                    else
                   if (action == ActionState.Edit)
                    {
                        dynamic selectedOrder = ordersDataGrid.SelectedItem;
                        try
                        {
                            int curr_id = selectedOrder.OrderId;
                            var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                            if (editedOrder != null)
                            {
                                editedOrder.CustId = Int32.Parse(cmbCustomers.SelectedValue.ToString());
                                editedOrder.CarId = Convert.ToInt32(cmbInventory.SelectedValue.ToString());
                                //salvam modificarile
                                ctx.SaveChanges();
                            }
                        }
                        catch (DataException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        BindDataGrid();
                        // pozitionarea pe item-ul curent
                        customerViewSource.View.MoveCurrentTo(selectedOrder);
                    }
                    else if (action == ActionState.Delete)
                    {
                        try
                        {
                            dynamic selectedOrder = ordersDataGrid.SelectedItem;
                            int curr_id = selectedOrder.OrderId;
                            var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                            if (deletedOrder != null)
                            {
                                ctx.Orders.Remove(deletedOrder);
                                ctx.SaveChanges();
                                MessageBox.Show("Order Deleted Successfully", "Message");
                                BindDataGrid();
                            }
                        }
                        catch (DataException ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                }

            }



        }

        private void cmbInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    } }