using FluentAssertions;
using StoreIt.Sharing.Models;
using StoreIt.Sharing.Services;
using Xunit;

namespace StoreIt.Maui.Tests;

public sealed class CardImportServiceTests
{
    [Fact]
    public async Task ImportAsync_ValidLink_PersistsAllCardsOnce()
    {
        var serializer = new CardSerializer();
        var store = new RecordingImportStore();
        var sut = new CardImportService(serializer, store);
        var expected = new CardShareData(
            "Delhaize", "Loyalty", null, null, "123456", "#FF6B35", true, false);

        var result = await sut.ImportAsync(serializer.CreateLink(expected));

        result.Should().ContainSingle().Which.Should().Be(expected);
        store.SavedBatches.Should().ContainSingle();
        store.SavedBatches[0].Should().Equal(expected);
    }

    [Fact]
    public async Task ImportAsync_InvalidLink_DoesNotPersist()
    {
        var store = new RecordingImportStore();
        var sut = new CardImportService(new CardSerializer(), store);

        var act = () => sut.ImportAsync(new Uri("storeit://import?d=invalid"));

        await act.Should().ThrowAsync<InvalidCardPayloadException>();
        store.SavedBatches.Should().BeEmpty();
    }

    private sealed class RecordingImportStore : ICardImportStore
    {
        public List<IReadOnlyCollection<CardShareData>> SavedBatches { get; } = [];

        public Task<int> SaveCardsAsync(
            IReadOnlyCollection<CardShareData> cards,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SavedBatches.Add(cards.ToArray());
            return Task.FromResult(cards.Count);
        }
    }
}
