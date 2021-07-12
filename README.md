# Nice Identity

 > NICE Identity and Access Management
 
 IdAM is currently used in the following services:
 
 * [Identity management](https://github.com/nice-digital/identity-management)
 * [Consultation comments](https://github.com/nice-digital/consultations)
 * EPPI
  
 
### Browser support

- IE: 11 and above
- Chrome: (Current - 1) and Current
- Edge: (Current - 1) and Current
- Firefox: (Current - 1) and Current
- Safari: (Current - 1) and Current

### Technical stack
- [.NET Core 3.1](https://github.com/dotnet/core) on the server
    - [xUnit.net](https://xunit.github.io/) for .NET unit tests
    - [Shouldly](https://github.com/shouldly/shouldly) for .NET assertions
    - [KDiff](http://kdiff3.sourceforge.net/) for diffing approvals tests
- [SQL Server](https://www.microsoft.com/en-gb/sql-server/sql-server-2017) as our database
    - [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) as an ORM
    - [EF Core In-Memory Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/) for integration tests
- [React](https://reactjs.org/) front-end
    - [Jest](https://facebook.github.io/jest/) for JavaScript tests
    - [ESLint](https://eslint.org/) for JavaScript linting
- [SASS](https://sass-lang.com/) as a CSS pre-processor
- [WebdriverIO](http://webdriver.io/) for automated functional testing
- [NICE Design System](https://github.com/nice-digital/nice-design-system/) for NICE styling front-end
    - [NICE Icons](https://github.com/nice-digital/nice-icons) for icon webfont
- [Webpack](https://webpack.js.org/) bundling and minification.
- [Babel](https://babeljs.io/) for javascript transpilation

### Getting Started

Solution is only tested in Visual Studio 2019. ymmv in other IDE's.

Install [NPM Task Runner Visual Studio Extension](https://marketplace.visualstudio.com/items?itemName=MadsKristensen.NPMTaskRunner) 

Install [KDiff3](http://kdiff3.sourceforge.net/) kdiff is currently expected to be installed here: `C:\Program Files\KDiff3\kdiff3.exe` changing this install location will mean the integration test diffing will fallback to vsDiffMerge.

Install [Node.js](https://nodejs.org/en/download/)

In Visual Studio, go to Tools > Options > Projects and Solutions > Web Package Management 
add the path to the Node installation at the top of the list. It'll be either `C:\Program Files\nodejs` or `C:\Program Files (x86)\nodejs` depending on whether you installed the x64 or x86 version of Node.js.

#### Secrets.json

The application's uses appsettings.json to store configuration. However, since this is a public repository, confidential configuration information (e.g. db connection string) is stored in secrets.json
In order to run the application correctly (with it having access to a database), you'll need to acquire (from another dev) or create a secrets.json file with the correct configuration information in. For more  information see: [https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio)

#### Nuget sources

The application currently uses a logging nuget package located on a local nuget server. You'll need to ask another developer on the project to get access to this nuget server. 

#### Webpack bundling

The Javascript in the project should be written in ES6 syntax. It gets transpiled to ES5 by babel, using webpack.
The project has been configured via Task Runner Explorer in Visual Studio to run `webpack --watch` when the project is opened. If you don't use Visual Studio, you'll need to start a command prompt running that command in order to build the javascript and CSS.



