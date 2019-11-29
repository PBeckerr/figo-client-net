using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Figo.Client;
using Figo.Client.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Tests
{
    [SingleThreaded]
    public class Tests
    {
        //TODO: Setup authcode returned by regshield
        public const string AuthCode = "";
        public const string ReturnUrl = "https://web.notexsting.com";
        public AccessTokenDto AccessToken;
        public IConfiguration Config = TestHelper.GetIConfigurationRoot(TestContext.CurrentContext.TestDirectory);

        [Test]
        public async Task Accounts()
        {
            var accountApi = new AccountService(this.Config);
            var accounts = await accountApi.GetAllAsync(this.AccessToken);

            Assert.IsTrue(accounts.Accounts.Any());
            foreach (var accountsAccount in accounts.Accounts)
            {
                Console.WriteLine(accountsAccount.ToString());
            }
        }

        [Test]
        [Order(2)]
        public async Task LoggedInOrReverified()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "accesstoken.json");
            this.AccessToken = JsonConvert.DeserializeObject<AccessTokenDto>(File.ReadAllText(path));

            if (!this.AccessToken.IsValid)
            {
                var authService = new AccessTokenService(this.Config);
                this.AccessToken = (await authService.CheckAndRevalidateIfNeededAsync(this.AccessToken).ConfigureAwait(false)).token;
                File.WriteAllText(path, JsonConvert.SerializeObject(this.AccessToken));
            }
        }

        [Test]
        [Order(1)]
        public async Task Login()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "accesstoken.json");
            //only do it if there is no accesstoken stored
            if (!File.Exists(path))
            {
                var authService = new AccessTokenService(this.Config);
                this.AccessToken = await authService.LoginAsync(AuthCode, ReturnUrl).ConfigureAwait(false);
                File.WriteAllText(path, JsonConvert.SerializeObject(this.AccessToken));
                Assert.IsNotNull(this.AccessToken);
            }
            else
            {
                this.AccessToken = JsonConvert.DeserializeObject<AccessTokenDto>(File.ReadAllText(path));
                Assert.IsNotNull(this.AccessToken);
            }
        }


        [Test]
        public async Task Securities()
        {
            var accountApi = new SecuritiesService(this.Config);
            var accounts = await accountApi.GetAllAsync(this.AccessToken).ConfigureAwait(false);

            Assert.IsTrue(accounts.Securities.Any());
            foreach (var accountsAccount in accounts.Securities)
            {
                Console.WriteLine(accountsAccount.ToString());
            }
        }

        [Test]
        public async Task ShieldToken()
        {
            var init = new InitiationService(this.Config);
            var url = await init.GetRedirectUriForShieldTokenAsync(Guid.NewGuid().ToString(), ReturnUrl).ConfigureAwait(false);

            //TODO: Set path to your browser
            Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", url.AbsoluteUri);
        }


        [Test]
        public async Task StandingOrder()
        {
            var accountApi = new StandingOrderService(this.Config);
            var accounts = await accountApi.GetAllAsync(this.AccessToken).ConfigureAwait(false);

            Assert.IsNotNull(accounts);
        }

        [Test]
        public async Task Sync()
        {
            var init = new InitiationService(this.Config);
            var url = await init.GetRedirectUriForSyncAccountsAsync(Guid.NewGuid().ToString(),
                                                                    ReturnUrl,
                                                                    new List<string>
                                                                    {
                                                                        "A476900205707853831.1"
                                                                    }).ConfigureAwait(false);

            //TODO: Set path to your browser
            Process.Start(@"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe", url.AbsoluteUri);
        }

        [Test]
        public async Task Transactions()
        {
            var accountApi = new TransactionsService(this.Config);
            var accounts = await accountApi.GetAllAsync(this.AccessToken).ConfigureAwait(false);

            Assert.IsTrue(accounts.Transactions.Any());
        }
    }


    public static class TestHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot(string basePath)
        {
            return new ConfigurationBuilder()
                   .SetBasePath(basePath)
                   .AddJsonFile("appsettings.json", true)
                   .Build();
        }
    }
}