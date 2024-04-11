using Xunit.Abstractions;

namespace Tests.Integration;

public class PostTest : IntegrationTestFunctionsBase
{
    public PostTest(ITestOutputHelper output, TestcontainerFixture testcontainer) : base(output, testcontainer)
    {
    }

    
}
