using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using ValveController.Models;
using ValveController.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AzureDataService))]

namespace ValveController.Services
{
    public class AzureDataService
    {
        public MobileServiceClient Client { get; set; } = null;
        IMobileServiceSyncTable<Users> usersTable;

        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;

            var appUrl = "https://valvecontroller.azurewebsites.net";

            Client = new MobileServiceClient(appUrl);

            //InitializeDatabase for path
            var path = "syncstore.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<Users>();

            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store);

            //Get our sync table that will call out to azure
            usersTable = Client.GetSyncTable<Users>();
        }

        public async Task SyncUsers()
        {
            try
            {
                await usersTable.PullAsync("usuariosIniciados", usersTable.CreateQuery());
                await Client.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Não foi possível sincronizar com os usuários!" + ex);
            }
        }

        public async Task<Users> AddUser(string name, string password, bool isAdmin)
        {
            await Initialize();
            var user = new Users
            {
                Name = name,
                Password = password,
                IsAdmin = isAdmin
            };
            await usersTable.InsertAsync(user);
            await SyncUsers();
            return user;
        }

        public async Task<Users> RemoveUser(string id)
        {
            await Initialize();
            var user = new Users { Id = id };
            await usersTable.DeleteAsync(user);
            await SyncUsers();
            return user;
        }
    }
}