using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ValveController.Models;
using ValveController.Services;

namespace ValveController.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private HttpClient httpClient;
        private AzureDataService azureDataService;

        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand LogOutCommand { get; set; }
        public DelegateCommand SignUpCommand { get; set; }
        public DelegateCommand ModifyValveCommand { get; set; }

        private bool _isLoggingIn;
        public bool IsLoggingIn
        {
            get { return _isLoggingIn; }
            set { SetProperty(ref _isLoggingIn, value, () => RaisePropertyChanged(nameof(IsNotLoggingIn))); }
        }
        public bool IsNotLoggingIn
        {
            get { return !IsLoggingIn; }
        }

        private bool _isSigningUp;
        public bool IsSigningUp
        {
            get { return _isSigningUp; }
            set { SetProperty(ref _isSigningUp, value, () => RaisePropertyChanged(nameof(IsNotSigningUp))); }
        }
        public bool IsNotSigningUp
        {
            get { return !IsSigningUp; }
        }

        private bool _loginVisibility;
        public bool LoginVisibility
        {
            get { return _loginVisibility; }
            set { SetProperty(ref _loginVisibility, value); }
        }

        private bool _isLoggedIn;
        public bool IsLoggedIn
        {
            get { return _isLoggedIn; }
            set { SetProperty(ref _isLoggedIn, value); }
        }

        private bool _isLoggedInAsAdmin;
        public bool IsLoggedInAsAdmin
        {
            get { return _isLoggedInAsAdmin; }
            set { SetProperty(ref _isLoggedInAsAdmin, value); }
        }

        private string _loggedInUserNameLabel;
        public string LoggedInUserNameLabel
        {
            get { return _loggedInUserNameLabel; }
            set { SetProperty(ref _loggedInUserNameLabel, value); }
        }

        private string _loginUserNameEntry;
        public string LoginUserNameEntry
        {
            get { return _loginUserNameEntry; }
            set { SetProperty(ref _loginUserNameEntry, value); }
        }

        private string _loginUserPasswordEntry;
        public string LoginUserPasswordEntry
        {
            get { return _loginUserPasswordEntry; }
            set { SetProperty(ref _loginUserPasswordEntry, value); }
        }

        private string _signUpUserNameEntry;
        public string SignUpUserNameEntry
        {
            get { return _signUpUserNameEntry; }
            set { SetProperty(ref _signUpUserNameEntry, value); }
        }

        private string _signUpUserPasswordEntry;
        public string SignUpUserPasswordEntry
        {
            get { return _signUpUserPasswordEntry; }
            set { SetProperty(ref _signUpUserPasswordEntry, value); }
        }

        private bool _signUpUserIsAdminSwitch;
        public bool SignUpUserIsAdminSwitch
        {
            get { return _signUpUserIsAdminSwitch; }
            set { SetProperty(ref _signUpUserIsAdminSwitch, value); }
        }

        private string _conectionState;
        public string ConectionState
        {
            get { return _conectionState; }
            set { SetProperty(ref _conectionState, value); }
        }

        private int _percentProgressBarLabel;
        public int PercentProgressBarLabel
        {
            get { return _percentProgressBarLabel; }
            set { SetProperty(ref _percentProgressBarLabel, value); }
        }

        private float _percentProgressBar;
        public float PercentProgressBar
        {
            get { return _percentProgressBar / 100; }
            set { SetProperty(ref _percentProgressBar, value); }
        }

        private int _percentSlider;
        public int PercentSlider
        {
            get { return _percentSlider; }
            set { SetProperty(ref _percentSlider, value); }
        }

        private bool _isValveRotating;
        public bool IsValveRotating
        {
            get { return _isValveRotating; }
            set { SetProperty(ref _isValveRotating, value, () => RaisePropertyChanged(nameof(IsNotValveRotating))); }
        }
        public bool IsNotValveRotating
        {
            get { return !IsValveRotating; }
        }

        public ObservableCollection<Users> UsersList { get; set; }

        //-------------------------------------------------------------CONSTRUCTOR--------------------------------------------------------------------//
        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            httpClient = new HttpClient();
            UsersList = new ObservableCollection<Users>();
            azureDataService = Xamarin.Forms.DependencyService.Get<AzureDataService>();

            Title = "Valve Controller";
            ConectionState = "SEM CONEXÃO";
            LoginVisibility = true;
            IsLoggedIn = false;
            IsLoggedInAsAdmin = false;

            LoginCommand = new DelegateCommand(async () => await ExecuteLoginCommand());
            LogOutCommand = new DelegateCommand(async () => await ExecuteLogOutCommand());
            SignUpCommand = new DelegateCommand(async () => await ExecuteSignUpCommand());
            ModifyValveCommand = new DelegateCommand(async () => await ExecuteModifyValveCommand());

            httpClient.GetAsync("http://192.168.0.120/piscar");
        }

        private async Task ExecuteLoginCommand()
        {
            Exception Error = null;
            try
            {
                IsBusy = true;
                IsLoggingIn = true;
                if (LoginUserNameEntry == "fred" && LoginUserPasswordEntry == "123")
                {
                    LoggedInUserNameLabel = "fred";
                    IsLoggedInAsAdmin = true;
                    ConectionState = "CONECTADO";
                    LoginVisibility = false;
                    IsBusy = false;
                    IsLoggingIn = false;
                    await PageDialogService.DisplayAlertAsync("Sessão iniciada", "Sessão iniciada com sucesso!", "OK");
                    await httpClient.GetAsync("http://192.168.0.120/acender");
                    return;
                }
                var Repository = new Repository();
                var Users = await Repository.GetUsers();
                foreach (var User in Users)
                {
                    if (LoginUserNameEntry == User.Name && LoginUserPasswordEntry == User.Password)
                    {
                        LoggedInUserNameLabel = User.Name;
                        if (User.IsAdmin) { IsLoggedInAsAdmin = true; }
                        else { IsLoggedIn = true; }
                        ConectionState = "CONECTADO";
                        LoginVisibility = false;
                        IsBusy = false;
                        IsLoggingIn = false;
                        await PageDialogService.DisplayAlertAsync("Sessão iniciada", "Sessão iniciada com sucesso!", "OK");
                        await httpClient.GetAsync("http://192.168.0.120/acender");
                        return;
                    }
                }
                IsBusy = false;
                IsLoggingIn = false;
                await PageDialogService.DisplayAlertAsync("Usuário não existe", "Não é possível iniciar sessão sem um cadastro!", "OK");
            }
            catch (Exception ex)
            {
                Error = ex;
                if (Error != null)
                {
                    IsBusy = false;
                    IsLoggingIn = false;
                    await PageDialogService.DisplayAlertAsync("Erro", "Não foi possível se conectar: " + Error.Message, "OK");
                }
            }
        }

        private async Task ExecuteLogOutCommand()
        {
            bool r = await PageDialogService.DisplayAlertAsync("Desconectando", "Tem certeza que deseja se desconectar?", "SIM", "NÃO");
            if (r)
            {
                ConectionState = "SEM CONEXÃO";
                IsLoggedIn = false;
                IsLoggedInAsAdmin = false;
                LoginVisibility = true;
                await PageDialogService.DisplayAlertAsync("Sessão fechada", "Desconectado com sucesso!", "OK");
                await httpClient.GetAsync("http://192.168.0.120/apagar");
            }
        }

        private async Task ExecuteSignUpCommand()
        {
            Exception Error = null;
            try
            {
                IsBusy = true;
                IsSigningUp = true;
                var Repository = new Repository();
                var Users = await Repository.GetUsers();
                foreach (var User in Users)
                {
                    if (SignUpUserNameEntry == User.Name)
                    {
                        IsBusy = false;
                        IsSigningUp = false;
                        await PageDialogService.DisplayAlertAsync("Usuário já existe", "Não é possível adicionar outro usuário com o mesmo nome", "OK");
                        return;
                    }
                }
                await azureDataService.AddUser(SignUpUserNameEntry, SignUpUserPasswordEntry, SignUpUserIsAdminSwitch);
                SyncUsers();
                IsBusy = false;
                IsSigningUp = false;
                await PageDialogService.DisplayAlertAsync("Usuário cadastrado", "Usuário cadastrado com sucesso!", "OK");
            }
            catch (Exception ex)
            {
                Error = ex;
                if (Error != null)
                {
                    IsBusy = false;
                    IsSigningUp = false;
                    await PageDialogService.DisplayAlertAsync("Erro", "Não foi possível se conectar: " + Error.Message, "OK");
                }
            }
        }

        private async Task ExecuteModifyValveCommand()
        {
            Exception Error = null;
            try
            {
                if (IsLoggedIn || IsLoggedInAsAdmin)
                {
                    IsBusy = true;
                    IsValveRotating = true;
                    string r = await httpClient.GetStringAsync("http://192.168.0.120/" + PercentSlider);
                    PercentProgressBar = Convert.ToInt16(r);
                    PercentProgressBarLabel = Convert.ToInt16(r);
                    await PageDialogService.DisplayAlertAsync("Resposta", r, "OK");
                    IsBusy = false;
                    IsValveRotating = false;
                }
                else
                {
                    await PageDialogService.DisplayAlertAsync("Inicie uma sessão", "É necessário iniciar uma sessão para modificar a válvula", "OK");
                }
            }
            catch (Exception ex)
            {
                Error = ex;
                if (Error != null)
                {
                    IsBusy = false;
                    IsValveRotating = false;
                    await PageDialogService.DisplayAlertAsync("Erro", "Não foi possível se conectar: " + Error.Message, "OK");
                }
            }
        }

        async void SyncUsers()
        {
            Exception Error = null;
            UsersList.Clear();
            try
            {
                IsBusy = true;
                var Repository = new Repository();
                var Items = await Repository.GetUsers();
                foreach (var User in Items)
                {
                    UsersList.Add(User);
                }
                IsBusy = false;
            }
            catch (Exception ex)
            {
                Error = ex;
                if (Error != null)
                {
                    IsBusy = false;
                    await PageDialogService.DisplayAlertAsync("Erro", "Não foi possível sincronizar com o banco de dados: " + Error.Message, "OK");
                }
            }
        }
    }
}
