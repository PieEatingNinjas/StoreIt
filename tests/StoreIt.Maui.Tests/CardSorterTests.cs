using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using StoreIt.Maui.Models;
using StoreIt.Maui.Sorting;
using Xunit;

namespace StoreIt.Maui.Tests;

/// <summary>
/// Unit tests for <see cref="CardSorter"/>, covering the acceptance scenarios and edge
/// cases from specs/001-card-sorting. Tests target the pure sort function contract:
/// favorites first (IsFavorite desc), then the selected mode within each group, with a
/// stable sort (ties keep input order).
/// </summary>
public class CardSorterTests
{
    private static readonly DateTime BaseTime = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>All three real sort modes, for theories that must hold for every mode.</summary>
    public static IEnumerable<object[]> AllSortModes()
    {
        yield return new object[] { CardSortMode.LastAccessed };
        yield return new object[] { CardSortMode.NameAscending };
        yield return new object[] { CardSortMode.NameDescending };
    }

    private static CustomerCard Card(
        string name = "Card",
        bool isFavorite = false,
        DateTime? lastUsed = null,
        int id = 0,
        string description = "")
        => new()
        {
            Id = id,
            Name = name,
            IsFavorite = isFavorite,
            LastUsed = lastUsed ?? BaseTime,
            Description = description,
        };

    // ---------------------------------------------------------------------
    // FR-002 / SC-001 — favorites always first, for ALL modes
    // ---------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(AllSortModes))]
    public void Sort_MixedFavorites_PlacesEveryFavoriteBeforeEveryNonFavorite(CardSortMode mode)
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "Zeta", isFavorite: false, lastUsed: BaseTime.AddHours(10), id: 1),
            Card(name: "Alpha", isFavorite: true, lastUsed: BaseTime.AddHours(1), id: 2),
            Card(name: "Mike", isFavorite: false, lastUsed: BaseTime.AddHours(5), id: 3),
            Card(name: "Bravo", isFavorite: true, lastUsed: BaseTime.AddHours(9), id: 4),
        };

        // Act
        var result = CardSorter.Sort(cards, mode);

        // Assert — no non-favorite may appear before any favorite.
        var favoriteFlags = result.Select(c => c.IsFavorite).ToList();
        var firstNonFavoriteIndex = favoriteFlags.IndexOf(false);
        var lastFavoriteIndex = favoriteFlags.LastIndexOf(true);
        firstNonFavoriteIndex.Should().BeGreaterThan(lastFavoriteIndex,
            "all favorites must precede all non-favorites regardless of sort mode");
        result.Should().HaveCount(4);
    }

    [Theory]
    [MemberData(nameof(AllSortModes))]
    public void Sort_MixedFavorites_KeepsFavoriteGroupSizes(CardSortMode mode)
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "a", isFavorite: true, id: 1),
            Card(name: "b", isFavorite: false, id: 2),
            Card(name: "c", isFavorite: true, id: 3),
        };

        // Act
        var result = CardSorter.Sort(cards, mode);

        // Assert
        result.Take(2).Should().OnlyContain(c => c.IsFavorite);
        result.Skip(2).Should().OnlyContain(c => !c.IsFavorite);
    }

    // ---------------------------------------------------------------------
    // FR-005 — LastAccessed: LastUsed descending within each group
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_LastAccessed_OrdersByLastUsedDescendingWithinEachGroup()
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "FavOld", isFavorite: true, lastUsed: BaseTime.AddHours(1), id: 1),
            Card(name: "FavNew", isFavorite: true, lastUsed: BaseTime.AddHours(9), id: 2),
            Card(name: "PlainOld", isFavorite: false, lastUsed: BaseTime.AddHours(2), id: 3),
            Card(name: "PlainNew", isFavorite: false, lastUsed: BaseTime.AddHours(8), id: 4),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.LastAccessed);

        // Assert
        result.Select(c => c.Name).Should().Equal("FavNew", "FavOld", "PlainNew", "PlainOld");
        var favorites = result.Where(c => c.IsFavorite).Select(c => c.LastUsed);
        var nonFavorites = result.Where(c => !c.IsFavorite).Select(c => c.LastUsed);
        favorites.Should().BeInDescendingOrder();
        nonFavorites.Should().BeInDescendingOrder();
    }

    // ---------------------------------------------------------------------
    // FR-005 — NameAscending: A -> Z within each group
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_NameAscending_OrdersNamesAscendingWithinEachGroup()
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "Charlie", isFavorite: true, id: 1),
            Card(name: "Alpha", isFavorite: true, id: 2),
            Card(name: "Yankee", isFavorite: false, id: 3),
            Card(name: "Mike", isFavorite: false, id: 4),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert
        result.Select(c => c.Name).Should().Equal("Alpha", "Charlie", "Mike", "Yankee");
        result.Where(c => c.IsFavorite).Select(c => c.Name)
            .Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase);
        result.Where(c => !c.IsFavorite).Select(c => c.Name)
            .Should().BeInAscendingOrder(StringComparer.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------------
    // FR-005 — NameDescending: Z -> A within each group
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_NameDescending_OrdersNamesDescendingWithinEachGroup()
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "Alpha", isFavorite: true, id: 1),
            Card(name: "Charlie", isFavorite: true, id: 2),
            Card(name: "Mike", isFavorite: false, id: 3),
            Card(name: "Yankee", isFavorite: false, id: 4),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameDescending);

        // Assert
        result.Select(c => c.Name).Should().Equal("Charlie", "Alpha", "Yankee", "Mike");
        result.Where(c => c.IsFavorite).Select(c => c.Name)
            .Should().BeInDescendingOrder(StringComparer.OrdinalIgnoreCase);
        result.Where(c => !c.IsFavorite).Select(c => c.Name)
            .Should().BeInDescendingOrder(StringComparer.OrdinalIgnoreCase);
    }

    // ---------------------------------------------------------------------
    // FR-005 — Case-insensitive name comparison
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_NameAscending_TreatsCaseInsensitiveNamesAsEquivalentAndKeepsThemAdjacent()
    {
        // Arrange — "apple"/"Apple" must not be split apart by a value that sorts
        // between them case-sensitively (e.g. capital 'B' < lowercase 'a' ordinally).
        var cards = new[]
        {
            Card(name: "apple", isFavorite: false, id: 1),
            Card(name: "Banana", isFavorite: false, id: 2),
            Card(name: "Apple", isFavorite: false, id: 3),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert — both apples adjacent, ahead of Banana; ordering is case-insensitive.
        result.Select(c => c.Name).Should().Equal("apple", "Apple", "Banana");
        var names = result.Select(c => c.Name).ToList();
        Math.Abs(names.IndexOf("apple") - names.IndexOf("Apple")).Should().Be(1,
            "cards differing only by case must be treated as tied and stay adjacent");
    }

    [Fact]
    public void Sort_NameDescending_TreatsCaseInsensitiveNamesAsEquivalentAndKeepsThemAdjacent()
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "apple", isFavorite: false, id: 1),
            Card(name: "Banana", isFavorite: false, id: 2),
            Card(name: "Apple", isFavorite: false, id: 3),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameDescending);

        // Assert — Banana first (desc), then the two apples adjacent, in stable input order.
        result.Select(c => c.Name).Should().Equal("Banana", "apple", "Apple");
        var names = result.Select(c => c.Name).ToList();
        Math.Abs(names.IndexOf("apple") - names.IndexOf("Apple")).Should().Be(1);
    }

    // ---------------------------------------------------------------------
    // FR-008 — Stability (ties keep input relative order)
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_NameAscending_WithIdenticalNames_KeepsInputOrder()
    {
        // Arrange — same name & case; distinguished by Id to assert stable order.
        var cards = new[]
        {
            Card(name: "Duplicate", isFavorite: false, id: 10),
            Card(name: "Duplicate", isFavorite: false, id: 20),
            Card(name: "Duplicate", isFavorite: false, id: 30),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert
        result.Select(c => c.Id).Should().Equal(10, 20, 30);
    }

    [Fact]
    public void Sort_NameAscending_WithCaseOnlyDifference_KeepsInputOrderForTiedNames()
    {
        // Arrange — case-insensitively tied; must resolve by stable ordering (FR-008).
        var cards = new[]
        {
            Card(name: "APPLE", isFavorite: false, id: 1),
            Card(name: "apple", isFavorite: false, id: 2),
            Card(name: "Apple", isFavorite: false, id: 3),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert
        result.Select(c => c.Id).Should().Equal(1, 2, 3);
    }

    [Fact]
    public void Sort_LastAccessed_WithIdenticalLastUsed_KeepsInputOrder()
    {
        // Arrange — same LastUsed; distinguished by Id to assert stable order.
        var tie = BaseTime.AddHours(3);
        var cards = new[]
        {
            Card(name: "A", isFavorite: false, lastUsed: tie, id: 100),
            Card(name: "B", isFavorite: false, lastUsed: tie, id: 200),
            Card(name: "C", isFavorite: false, lastUsed: tie, id: 300),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.LastAccessed);

        // Assert
        result.Select(c => c.Id).Should().Equal(100, 200, 300);
    }

    // ---------------------------------------------------------------------
    // Edge cases
    // ---------------------------------------------------------------------

    [Theory]
    [MemberData(nameof(AllSortModes))]
    public void Sort_EmptyInput_ReturnsEmptyList(CardSortMode mode)
    {
        // Arrange
        var cards = Array.Empty<CustomerCard>();

        // Act
        var result = CardSorter.Sort(cards, mode);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(AllSortModes))]
    public void Sort_NullInput_ReturnsEmptyListWithoutThrowing(CardSortMode mode)
    {
        // Act
        var act = () => CardSorter.Sort(null!, mode);

        // Assert
        act.Should().NotThrow();
        CardSorter.Sort(null!, mode).Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void Sort_SingleCard_ReturnsThatCard()
    {
        // Arrange
        var card = Card(name: "Solo", isFavorite: false, id: 42);

        // Act
        var result = CardSorter.Sort(new[] { card }, CardSortMode.NameAscending);

        // Assert
        result.Should().ContainSingle().Which.Id.Should().Be(42);
    }

    [Fact]
    public void Sort_AllFavorites_AppliesModeAcrossWholeList()
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "Charlie", isFavorite: true, id: 1),
            Card(name: "Alpha", isFavorite: true, id: 2),
            Card(name: "Bravo", isFavorite: true, id: 3),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert
        result.Should().OnlyContain(c => c.IsFavorite);
        result.Select(c => c.Name).Should().Equal("Alpha", "Bravo", "Charlie");
    }

    [Fact]
    public void Sort_NoFavorites_AppliesModeAcrossWholeList()
    {
        // Arrange
        var cards = new[]
        {
            Card(name: "Charlie", isFavorite: false, id: 1),
            Card(name: "Alpha", isFavorite: false, id: 2),
            Card(name: "Bravo", isFavorite: false, id: 3),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert
        result.Should().OnlyContain(c => !c.IsFavorite);
        result.Select(c => c.Name).Should().Equal("Alpha", "Bravo", "Charlie");
    }

    [Fact]
    public void Sort_LastAccessed_CardsWithDefaultLastUsed_SortAfterCardsWithLaterLastUsed()
    {
        // Arrange — a "never explicitly opened" card carries the earliest LastUsed and
        // must appear after cards with a later LastUsed within its group.
        var cards = new[]
        {
            Card(name: "NeverOpened", isFavorite: false, lastUsed: DateTime.MinValue, id: 1),
            Card(name: "RecentlyUsed", isFavorite: false, lastUsed: BaseTime.AddHours(5), id: 2),
        };

        // Act
        var result = CardSorter.Sort(cards, CardSortMode.LastAccessed);

        // Assert
        result.Select(c => c.Name).Should().Equal("RecentlyUsed", "NeverOpened");
    }

    // ---------------------------------------------------------------------
    // Unknown mode -> LastAccessed behaviour
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_UnknownMode_FallsBackToLastAccessedBehaviour()
    {
        // Arrange — a value outside the defined enum members must behave like LastAccessed.
        var unknownMode = (CardSortMode)999;
        var cards = new[]
        {
            Card(name: "FavOld", isFavorite: true, lastUsed: BaseTime.AddHours(1), id: 1),
            Card(name: "FavNew", isFavorite: true, lastUsed: BaseTime.AddHours(9), id: 2),
            Card(name: "PlainOld", isFavorite: false, lastUsed: BaseTime.AddHours(2), id: 3),
            Card(name: "PlainNew", isFavorite: false, lastUsed: BaseTime.AddHours(8), id: 4),
        };

        // Act
        var result = CardSorter.Sort(cards, unknownMode);
        var expected = CardSorter.Sort(cards, CardSortMode.LastAccessed);

        // Assert
        result.Select(c => c.Id).Should().Equal(expected.Select(c => c.Id));
        result.Select(c => c.Name).Should().Equal("FavNew", "FavOld", "PlainNew", "PlainOld");
    }

    // ---------------------------------------------------------------------
    // Sanity: input collection is not mutated
    // ---------------------------------------------------------------------

    [Fact]
    public void Sort_DoesNotMutateInputCollectionOrdering()
    {
        // Arrange
        var cards = new List<CustomerCard>
        {
            Card(name: "Zeta", isFavorite: false, id: 1),
            Card(name: "Alpha", isFavorite: true, id: 2),
        };

        // Act
        CardSorter.Sort(cards, CardSortMode.NameAscending);

        // Assert — original list order is unchanged.
        cards.Select(c => c.Id).Should().Equal(1, 2);
    }
}
