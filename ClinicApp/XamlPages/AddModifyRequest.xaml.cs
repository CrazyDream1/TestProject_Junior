using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ClinicApp.EntityModels;
using ClinicApp.HelperClasses;

namespace ClinicApp.XamlPages
{
    /// <summary>
    /// Interaction logic for AddModifyRequest.xaml
    /// </summary>
    public partial class AddModifyRequest : Page
    {
        private Frame mainFrame;
        private PatientCard m_patient;
        private Request m_request;
        private Boolean isModifyingMode;
        public AddModifyRequest(PatientCard currentPatient, Frame frame)
        {
            InitializeComponent();
            m_patient = currentPatient;
            mainFrame = frame;
            isModifyingMode = false;
        }
        public AddModifyRequest(PatientCard currentPatient, Request requestToModify, Frame frame)
            :this(currentPatient, frame)
        {
            m_request = requestToModify;
            isModifyingMode = true;
            InitializeFields(requestToModify);
        }
        
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            datePickerRequest.SelectedDate = DateTime.Now;
        }

        private async void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                btn.IsEnabled = false;
                txtNotify.Content = "";
                //the message will notify the user if the operation has been done successfully
                StringBuilder messageBuilder = new StringBuilder();
                Boolean status = true;
                //get all the entered values from the UI
                var dateOfRequest = datePickerRequest.SelectedDate;
                var purpose = txtBoxPurpose.Text;
                var requestType = (TypeOfRequest)Enum.Parse(typeof(TypeOfRequest),
                    ((ComboBoxItem)comboBlockRequest.SelectedValue).Tag.ToString());
                ClinicDataRepository repository = new ClinicDataRepository();
                #region Checking if a user exists in DB
                if (m_patient == null)
                {
                    status = false;
                    messageBuilder.Append("Пользователя с таким ID не существует в базе данных\n");
                }
                #endregion
                #region Checking entered date of request
                if (dateOfRequest == null)
                {
                    status = false;
                    messageBuilder.Append("Дата обращения - обязательное поле для ввода.\n");
                }
                else
                {
                    if (dateOfRequest > DateTime.Now)
                    {
                        status = false;
                        messageBuilder.Append("Дата рождения - не может быть в будущем.\n");
                    }
                    else
                    {
                        if (dateOfRequest < BaseDate.BaseDateConst)
                        {
                            status = false;
                            messageBuilder.Append("Дата рождения - не может быть настолько в прошлом.\n");
                        }
                    }
                }
                #endregion
                #region Checking entered purpose of request
                if (String.IsNullOrEmpty(purpose))
                {
                    status = false;
                    messageBuilder.Append("Цель обращения - обязательное поле для ввода.");

                }
                #endregion
                #region Submitting the reqiest
                if (status)
                {
                    Boolean isSucceed;
                    #region adding mode
                    if (!isModifyingMode)
                    {
                        var newRequest = new Request()
                        {
                            Patient = m_patient,
                            DateOfRequest = dateOfRequest.GetValueOrDefault(DateTime.Now),
                            Purpose = purpose,
                            RequestType = requestType
                        };
                        isSucceed = await repository.AddRequest(newRequest);

                    }
                    #endregion
                    #region modifying mode
                    else
                    {
                        m_request.DateOfRequest = dateOfRequest.GetValueOrDefault(DateTime.Now);
                        m_request.Purpose = purpose;
                        m_request.RequestType = requestType;
                        m_request.Patient = m_patient;
                        isSucceed = repository.ModifyRequest(m_request);
                    }
                    #endregion
                    if (isSucceed && mainFrame.CanGoBack)
                    {
                        mainFrame.GoBack();
                    }
                }
                #endregion
                else
                {
                    txtNotify.Foreground = Brushes.Black;
                    txtNotify.BorderBrush = Brushes.Red;
                    txtNotify.Content = messageBuilder.ToString();
                }
            }
            catch (InvalidOperationException)
            {
                var err =   "Операция не может быть выполнена.\n" +
                            "Скорее всего какие то проблемы с базой данных. \n" +
                            "Рекомендую обратиться к разработчику";
                MessageBox.Show(err, "Ошибка");
                throw;
            }
            catch
            {
                //better to close the application than proceed with unknown error
                MessageBox.Show("Что то пошло не так. Приложение будет закрыто.", "Ошибка!");
                throw;
            }
            finally
            {
                //activate the submit button
                btnSubmit.IsEnabled = true;
            }
        }
        private void InitializeFields(Request requestToModify)
        {
            datePickerRequest.SelectedDate = requestToModify.DateOfRequest;
            txtBoxPurpose.Text = requestToModify.Purpose;
            comboBlockRequest.SelectedIndex = (Int32)requestToModify.RequestType;
        }

        private void btnGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (mainFrame.CanGoBack)
                mainFrame.GoBack();
        }
    }
}
