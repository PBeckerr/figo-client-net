# FigoClientNet

This is an API client to access your data in [figo.io](https://www.figo.io/). It is written as .NET Standard library to be used in .NET Framework projects and .NET Core projects as well. 

## Status

![Build status](https://travis-ci.org/paulbecker/FigoClientNet.svg?branch=master)
[![NuGet](https://img.shields.io/nuget/dt/PaulBecker.FigoClientNet.svg)](https://www.nuget.org/packages/TaurusSoftware.BillomatNet/)

## Usage

### Connect to your figo.io instance

First of all, you need to implement the [workflow of figo.io to request a authcode](http://docs.figo.io/regshield/ongoing-ais.html#section/Introduction/Connecting-a-financial-source) and then use this code to authenticate against the api (check Sample 1).

FigoClientNet is divided into several services, which correspond to the sections of the REST-API (e.g. `AccountService` for accounts, `TransactionService` for transactions and so on). 
Every service can be instanciated providing the credentials to authorize against your instance. 
You need to store and maybe refresh the AccessToken, `AccessTokenService` provides a `CheckAndRevalidateIfNeededAsync`-method to help you with the management of the lifetime of your `AccessToken`.
 
*Sample 1 (login, a auth code can be used one time and the redirectUri has to be the same as the one used to request the authcode)*
```
var config = new Configuration
{
    Username = "your username",
    Password = "your password",
    BasePath = "figo_service_url" 
};

var accessTokenService = new AccessTokenService(config);
var accessTokenDto = await accessTokenService.LoginAsync(myAuthCode, , myWorkflowRedirectUri);
```

*Sample 2 (querying transactions for bank account)*
```
var config = new Configuration
{
    Username = "your username",
    Password = "your password",
    BasePath = "figo_service_url" 
};
//check and refresh token if necessary
var authService = new AccessTokenService(configuration);
var (token, refreshed) = await authService.CheckAndRevalidateIfNeededAsync(myStoredToken);

var maxDate = DateTime.Now;
var transactionService = new TransactionsService(config);
var transactions = await transactionService.GetAllAsync(token,
                                                        bankAccountId,
                                                        new AccountListFilter {Since = maxDate});
if(refreshed)
{
   //Your logic to store the new token because it has been refreshed
}
```

For ease of use the services also support IConfiguration as constructor parameter
*appsettings.json*
```
figo: {
    username: "my_username",
    password: "my_password",
    serviceurl: "figo_service_url"
}
```

## Project Status

The REST-API itself contains a whole bunch of functionality and the wrapper will be updated periodically after figo.io changes their api. This wrapper is still under development and new functions will be added 
successively. You can find detailed information about recent changes in [change log](CHANGELOG.md).

## Contribution

Bug reports and pull requests are welcome on [GitHub](https://github.com/DevelappersGmbH/FigoClientNet). Please check the [contribution guide](CONTRIBUTING.MD).This project is intended to be a safe, welcoming space for collaboration, and contributors are expected to our [code of conduct](CODE_OF_CONDUCT.md).