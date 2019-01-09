# Nice Identity

 > NICE Identity and Access Management
 
### Browser support

- IE: 8 and above
- Chrome: (Current - 1) and Current
- Edge: (Current - 1) and Current
- Firefox: (Current - 1) and Current
- Safari: (Current - 1) and Current

### Technical stack
- [.NET Core 2.1](https://github.com/dotnet/core) on the server
    - [xUnit.net](https://xunit.github.io/) for .NET unit tests
    - [Shouldly](https://github.com/shouldly/shouldly) for .NET assertions
    - [KDiff](http://kdiff3.sourceforge.net/) for diffing approvals tests
- [SQL Server](https://www.microsoft.com/en-gb/sql-server/sql-server-2017) as our database
    - [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore) as an ORM
    - [EF Core In-Memory Database Provider](https://docs.microsoft.com/en-us/ef/core/providers/in-memory/) for integration tests
- [Razor](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-2.2) front-end
    - [Jest](https://facebook.github.io/jest/) for JavaScript tests
    - [ESLint](https://eslint.org/) for JavaScript linting
    - [jQuery 1.12.4)(https://jquery.com/browser-support/) for AJAX suport. using last version supporting IE8
- [SASS](https://sass-lang.com/) as a CSS pre-processor
- [Modernizr](https://modernizr.com/) for feature detection
- [WebdriverIO](http://webdriver.io/) for automated functional testing
- [NICE Design System](https://nhsevidence.github.io/nice-design-system/) for NICE styling
    - [NICE Icons](https://github.com/nhsevidence/nice-icons) for icon webfont


### Getting Started

Install [KDiff3](http://kdiff3.sourceforge.net/)

Install [Node.js](https://nodejs.org/en/download/)

In Visual Studio 2017, go to Tools > Options > Projects and Solutions > Web Package Management 
add the path to the Node installation at the top of the list. It'll be either `C:\Program Files\nodejs` or `C:\Program Files (x86)\nodejs` depending on whether you installed the x64 or x86 version of Node.js.
