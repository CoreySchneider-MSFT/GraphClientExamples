using Azure.Identity;
using GraphClientSettings;
using GraphClientV5;
using GraphClientV4;
using GraphClientV3;
using Newtonsoft.Json;
using System.Runtime;

//Get Settings from the settings class library. 
var settings = Settings.LoadSettings();

//create client secret to pass to each client. 
var clientSecretCredential = new ClientSecretCredential(settings.TenantId, settings.ClientId, settings.ClientSecret);



// See https://aka.ms/new-console-template for more information
//Console.WriteLine($"{JsonConvert.SerializeObject(clientSecretCredential, Formatting.Indented)}");

var clientv5 = new GraphClientV5.GraphClientV5(clientSecretCredential);
var clientv4 = new GraphClientV4.GraphClientV4(clientSecretCredential);
var clientv3 = new GraphClientV3.GraphClientV3(clientSecretCredential);