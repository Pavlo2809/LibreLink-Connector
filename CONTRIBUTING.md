# Contributing to LibreLink Connector

First off, thank you for considering contributing to LibreLink Connector! It's people like you that make this project better.

## Code of Conduct

By participating in this project, you are expected to uphold our code of conduct:
- Be respectful and inclusive
- Be patient and welcoming
- Be collaborative
- Focus on what is best for the community

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When you create a bug report, include as many details as possible:

- **Use a clear and descriptive title**
- **Describe the exact steps to reproduce the problem**
- **Provide specific examples** to demonstrate the steps
- **Describe the behavior you observed** and what you expected
- **Include screenshots** if relevant
- **Include your environment details** (Windows version, .NET version, etc.)

### Suggesting Enhancements

Enhancement suggestions are tracked as GitHub issues. When creating an enhancement suggestion:

- **Use a clear and descriptive title**
- **Provide a detailed description** of the suggested enhancement
- **Explain why this enhancement would be useful**
- **List any similar features** in other applications if applicable

### Pull Requests

1. **Fork the repository** and create your branch from `main`
2. **Make your changes** following our coding standards
3. **Test your changes** thoroughly
4. **Update documentation** if needed
5. **Commit your changes** with clear commit messages
6. **Push to your fork** and submit a pull request

#### Pull Request Guidelines

- Follow the existing code style and conventions
- Write clear, descriptive commit messages
- Include comments in your code where necessary
- Update the README.md or other documentation if needed
- Add an entry to CHANGELOG.md under [Unreleased]
- Ensure all tests pass (if applicable)

## Development Setup

### Prerequisites

- Windows 10 or later
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code (optional)

### Building the Project

```powershell
# Clone your fork
git clone https://github.com/YOUR-USERNAME/LibreLinkConnector.git
cd LibreLinkConnector

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

### Project Structure

```
LibreLinkConnector/
├── Models/           # Data models and entities
├── Services/         # Business logic and API clients
├── Views/            # XAML views
├── Presenters/       # MVP presenters
├── Themes/           # UI themes
└── MainWindow.xaml   # Main application window
```

## Coding Standards

### C# Style Guidelines

- Follow Microsoft's [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)
- Use PascalCase for class names, method names, and public members
- Use camelCase for local variables and private fields
- Use meaningful and descriptive names
- Add XML documentation comments for public APIs

### XAML Guidelines

- Use meaningful x:Name attributes
- Follow consistent indentation (4 spaces)
- Group related properties together
- Use data binding where appropriate

### Git Commit Messages

- Use the present tense ("Add feature" not "Added feature")
- Use the imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit the first line to 72 characters or less
- Reference issues and pull requests when relevant

Examples:
```
Add glucose trend chart visualization
Fix login error handling for invalid credentials
Update README with new installation instructions
```

## Testing

- Test your changes manually before submitting
- Ensure the application builds without errors
- Test on a clean Windows installation if possible
- Verify that existing features still work correctly

## Documentation

- Update README.md if you change functionality
- Update BUILD.md if you change build process
- Add entries to CHANGELOG.md for notable changes
- Comment complex code sections

## Questions?

Feel free to open an issue with the label "question" if you need clarification on anything.

## License

By contributing to LibreLink Connector, you agree that your contributions will be licensed under the MIT License.

---

Thank you for contributing! 🎉
