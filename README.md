# AggieRent

A .NET Core project helping Aggies find their dream home!

## Quick Start

- With authentication and authorization:

  ```shell
  dotnet run --project AggieRent
  ```

- With auth disabled:

  ```shell
  dotnet run --project AggieRent -lp no-auth
  ```

## Unit Testing

```shell
dotnet test
```

## Contributing

### Development Environment Setup

- VS Code is the recommended IDE for working with this project.

- You should have .NET 8.0 available in your environment. To check, run

  ```shell
  dotnet --version
  ```

- Install the [`CSharpier`](https://csharpier.com/) .NET tool as well as the [VS Code extension](https://marketplace.visualstudio.com/items?itemName=csharpier.csharpier-vscode) to conform to the coding style:

  ```shell
  dotnet tool restore
  ```

  To format the source code from commandline, run:

  ```shell
  dotnet dotnet-csharpier .
  ```

  You should also set `Charpier` to be the default formatter, enable `Format on Save` and `Format on Paste` in your VS Code settings.
