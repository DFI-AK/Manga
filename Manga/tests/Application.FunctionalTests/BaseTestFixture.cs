namespace Manga.Application.FunctionalTests;

using static Manga.Application.FunctionalTests.Testing;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
