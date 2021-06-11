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
using System.Net;
using WPFDemoAsyncProgramming1.Models;

namespace WPFDemoAsyncProgramming1
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

        private void runSyncButton_Click(object sender, RoutedEventArgs e)
        {
            //Start a stopwatch to time how long this takes
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            RunDownload();

            //Stop the stopwatch and get how many milliseconds this took
            stopWatch.Stop();
            var elapsedMs = stopWatch.ElapsedMilliseconds;

            results.Text += $"Total execution time: {elapsedMs}.";
        }

        private async void runAsyncButton_Click(object sender, RoutedEventArgs e)
        {
            //Start a stopwatch to time how long this takes
            var stopWatch = System.Diagnostics.Stopwatch.StartNew();

            //Call our Async method to run our download, and wait for it to finish (await) so we don't stop our stopwatch too soon
            ////await RunDownloadAsync();

            //Uncomment to run our downloads in parallel (at the same time) in async
            await RunDownloadAsyncParallel();

            //Stop the stopwatch and get how many milliseconds this took
            stopWatch.Stop();
            var elapsedMs = stopWatch.ElapsedMilliseconds;

            results.Text += $"Total execution time: {elapsedMs}.";
        }

        private void RunDownload()
        {
            //Clear the results window
            results.Text = "";

            //Get the list of sites then download the general info. from each one
            List<string> sites = GetSiteList();
            foreach(string site in sites)
            {
                WebSiteDataModel wsdm = DownloadSiteInfo(site);
                ReportWebsiteInfo(wsdm);
            }
        }

        private async Task RunDownloadAsync()
        {
            //Clear the results window
            results.Text = "";

            //Get the list of sites then download the general info. from each one
            List<string> sites = await Task.Run(() => GetSiteList());
            foreach (string site in sites)
            {
                WebSiteDataModel wsdm = await Task.Run(() => DownloadSiteInfo(site));
                ReportWebsiteInfo(wsdm);
            }
        }

        private async Task RunDownloadAsyncParallel()
        {
            //Clear the results window
            results.Text = "";

            //Get the list of sites then download the general info. from each one
            List<string> sites = await Task.Run(() => GetSiteList());

            //Create a list of Tasks so we can run them all at once
            List<Task<WebSiteDataModel>> tasks = new List<Task<WebSiteDataModel>>();

            //Create and run a Task for each url in our site list
            foreach (string site in sites)
            {
                tasks.Add(Task.Run(() => DownloadSiteInfo(site)));                             
            }

            //Wait for all of the tasks to be done, then load the results into an array of our website data model
            WebSiteDataModel[] webSiteDataArray = await Task.WhenAll(tasks);

            //Loop through each item in our website data model array and report on the info.
            foreach(WebSiteDataModel w in webSiteDataArray)
            {
                ReportWebsiteInfo(w);
            }
        }

        private List<string> GetSiteList()
        {
            List<string> siteList = new List<string>();

            siteList.Add("http://www.yahoo.com");
            siteList.Add("http://www.google.com");
            siteList.Add("http://www.microsoft.com");
            siteList.Add("http://www.cnn.com");
            siteList.Add("http://www.codeproject.com");
            siteList.Add("http://www.stackoverflow.com");

            return siteList;
        }

        private WebSiteDataModel DownloadSiteInfo(string url)
        {
            WebSiteDataModel webSiteDataModel = new WebSiteDataModel();
            webSiteDataModel.WebsiteUrl = url;

            //Create a new webclient so we can talk to websites
            WebClient client = new WebClient();

            //Use the webclient to download the general info. (using DownloadString) from the website at the specified URL           
            webSiteDataModel.WebsiteData = client.DownloadString(url);

            return webSiteDataModel;
        }

        private void ReportWebsiteInfo(WebSiteDataModel websiteInfo)
        {
            results.Text += $"{websiteInfo.WebsiteUrl} downloaded: {websiteInfo.WebsiteData.Length} characters. {Environment.NewLine}";
        }

    }
}
