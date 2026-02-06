# assura-backend
backend Repository for Assura

1. Presentation Layer (Assura.API)
This is the entry point. Its only job is to handle HTTP requests and return responses.

Good: You've kept the logic out and put it in Core.

Pro Tip: Keep your controllers "thin." They should just call a service and return an Ok() or BadRequest().

2. Business Logic Layer (Assura.Core)
This is the "brain" of your app.

Observation: You have your Models (Entities) here. This is correct because the Business Logic needs to know what the data looks like.

Refinement: If you decide to move toward Clean Architecture later, you might rename this to Assura.Domain and separate the "Services" into an Assura.Application layer. But for a standard N-Tier setup, what you have works great.

3. Data Access Layer (Assura.Infrastructure)
This handles the "how" of data—SQL, MongoDB, or even third-party APIs.

Good: Separating Repositories from the DbContext is a smart move for testability.

Dependency Flow: Remember, Infrastructure should implement the interfaces defined in Core.


# 1. Solution එක හදන්න
dotnet new sln -n Assura

# 2. .NET 8 පාවිච්චි කරලා layers හදන්න
dotnet new webapi -n Assura.API --framework net8.0
dotnet new classlib -n Assura.Core --framework net8.0
dotnet new classlib -n Assura.Infrastructure --framework net8.0
dotnet new xunit -n Assura.Tests --framework net8.0

# 3. Solution එකට projects ඇතුළත් කරන්න
dotnet sln add Assura.API/Assura.API.csproj
dotnet sln add Assura.Core/Assura.Core.csproj
dotnet sln add Assura.Infrastructure/Assura.Infrastructure.csproj
dotnet sln add Assura.Tests/Assura.Tests.csproj



# connect with dependancies
dotnet add Assura.Infrastructure/Assura.Infrastructure.csproj reference Assura.Core/Assura.Core.csproj
dotnet add Assura.API/Assura.API.csproj reference Assura.Core/Assura.Core.csproj
dotnet add Assura.API/Assura.API.csproj reference Assura.Infrastructure/Assura.Infrastructure.csproj
