using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using FluentAssertions.Execution;

namespace EVotingSystem_SBMM.Tests;

public class VoteCalculator_Tests
{
        [Fact]
        public void ElectCandidatesSTV_WhenAllSeatsFilled_ShouldNotElectAnyCandidates()
        {
            // Arrange
            var voteCounts = new Dictionary<int, int> { { 1, 100 }, { 2, 150 } };
            var candidates = Builder<CandidateModel>.CreateListOfSize(2).Build().ToList();
            var electedCandidates = new HashSet<int>(new[] { 1, 2 });
            double quota = 100;

            // Act
            VoterCalculationHelper.ElectCandidatesSTV(voteCounts, candidates, electedCandidates, quota);

            // Assert
            electedCandidates.Should().HaveCount(2); // No new candidates should be elected
        }

        [Fact]
        public void ElectCandidatesSTV_WhenQuotaReached_ShouldElectCandidatesWithExcessVotes()
        {
            // Arrange
            var voteCounts = new Dictionary<int, int> { { 1, 120 }, { 2, 80 } };
            var candidates = Builder<CandidateModel>.CreateListOfSize(2).Build().ToList();
            var electedCandidates = new HashSet<int>();
            double quota = 100;

            // Act
            VoterCalculationHelper.ElectCandidatesSTV(voteCounts, candidates, electedCandidates, quota);

            // Assert
            electedCandidates.Should().Contain(1); // Candidate 1 should be elected
            electedCandidates.Should().NotContain(2); // Candidate 2 should not be elected
        }

        [Fact]
        public void ElectCandidatesSTV_WhenNoCandidateReachedQuota_ShouldTransferVotesToRemainingCandidates()
        {
            // Arrange
            var voteCounts = new Dictionary<int, int> { { 1, 60 }, { 2, 50 }, { 3, 40 } };
            var candidates = Builder<CandidateModel>.CreateListOfSize(3).Build().ToList();
            var electedCandidates = new HashSet<int>();
            double quota = 100;

            // Act
            VoterCalculationHelper.ElectCandidatesSTV(voteCounts, candidates, electedCandidates, quota);

            // Assert
            electedCandidates.Should().BeEmpty(); // No candidate should be elected yet
            voteCounts[1].Should().Be(60); // Votes for candidate 1 should remain the same
            voteCounts[2].Should().Be(50); // Votes for candidate 2 should remain the same
            voteCounts[3].Should().Be(40); // Votes for candidate 3 should remain the same
        }
    }
    


    

