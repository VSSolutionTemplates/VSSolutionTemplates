using JumpStreetMobile.Shared.Model;
using JumpStreetMobile.XForms.View;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace JumpStreetMobile.XForms
{
	public class App : Application
	{
        public App ()
		{
            // The root page of your application
            MainPage = new TodoListView();
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

