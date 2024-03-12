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

        [Fact]
        public void TransferExcessVotesSTV_ShouldRedistributeSurplusVotesProportionally()
        {
            // Arrange
            var originalVoteCounts = new Dictionary<int, int> { { 1, 120 }, { 2, 80 }, { 3, 60 } };
            var voteCounts = new Dictionary<int, int>(originalVoteCounts); // Create a copy of the original voteCounts

            // Simulate voter preferences
            var candidates = Builder<CandidateModel>.CreateListOfSize(3).Build().ToList();
            candidates[0].Votes.Add(new VoteModel { Preferences = new List<VotePreferenceModel> { new VotePreferenceModel { CandidateId = 1, Rank = 1 }, new VotePreferenceModel { CandidateId = 2, Rank = 2 } } });
            candidates[1].Votes.Add(new VoteModel { Preferences = new List<VotePreferenceModel> { new VotePreferenceModel { CandidateId = 2, Rank = 1 }, new VotePreferenceModel { CandidateId = 1, Rank = 2 } } });
            candidates[2].Votes.Add(new VoteModel { Preferences = new List<VotePreferenceModel> { new VotePreferenceModel { CandidateId = 1, Rank = 1 }, new VotePreferenceModel { CandidateId = 3, Rank = 2 } } });

            var electedCandidateId = 1;

            // Act
            VoterCalculationHelper.TransferExcessVotesSTV(voteCounts, candidates, electedCandidateId);

            // Logging vote counts for debugging
            Console.WriteLine("Updated Vote Counts:");
            foreach (var kvp in voteCounts)
            {
                Console.WriteLine($"Candidate {kvp.Key}: {kvp.Value}");
            }

            // Assert
            using (new AssertionScope())
            {
                voteCounts[2].Should().BeGreaterThan(originalVoteCounts[2]); // Votes for candidate 2 should increase
                voteCounts[1].Should().BeLessThan(originalVoteCounts[1]); // Votes for candidate 1 should decrease
            }
        }


        /*
      [Fact]
        public void ElectCandidatesSTV_EliminateCandidateWithFewestVotes()
        {
            // Arrange
            var voteCounts = new Dictionary<int, int>
            {
                { 1, 30 }, 
                { 2, 25 },
                { 3, 15 },
                { 4, 10 },
                { 5, 5 }
            };

            var candidates = Builder<CandidateModel>.CreateListOfSize(5)
                .All().With(c => c.IsElected = false)
                .Build().ToList(); // Convert to List<CandidateModel>

            // Create voters with preferences
            var voters = Builder<VoterModel>.CreateListOfSize(10)
                .All()
                .With(v => v.Votes = new List<VoteModel>()) // Initialize the Votes property
                .With(v => v.PreferenceVotes = new List<VotePreferenceModel>()) // Initialize the PreferenceVotes property
                .With(v => v.IsPending = false)
                .Build();

            foreach (var voter in voters)
            {
                var votePreferences = Builder<VotePreferenceModel>.CreateListOfSize(5)
                    .All()
                    .With(vp => vp.Voters = voter)
                    .With(vp => vp.VoterId = voter.Id)
                    .With(vp => vp.EventId = 1) // Assuming event ID is 1
                    .With((vp, index) => vp.Rank = index + 1) // Rank starts from 1
                    .Build();

                var vote = Builder<VoteModel>.CreateNew()
                    .With(v => v.VotedAtTime = DateTime.UtcNow)
                    .With(v => v.EventId = 1) // Assuming event ID is 1
                    .With(v => v.Voter = voter)
                    .With(v => v.VoterId = voter.Id)
                    .With(v => v.Preferences = votePreferences)
                    .Build();

                voter.Votes.Add(vote);
            }

            var votes = voters.Select(v => v.Votes.First()).ToList(); // Get the first vote of each voter

            // Assign votes to candidates based on voter preferences
            foreach (var vote in votes)
            {
                foreach (var preference in vote.Preferences)
                {
                    if (preference.Rank == 1)
                    {
                        // Increase the vote count for the candidate according to the voter's first preference
                        voteCounts[preference.CandidateId]++;
                        break; // Only consider the voter's first preference
                    }
                }
            }

            HashSet<int> electedCandidates = new HashSet<int>();
            double quota = 120; // Adjusted quota value

            // Act
            VoterCalculationHelper.ElectCandidatesSTV(voteCounts, candidates, electedCandidates, quota);

            // Assert
            electedCandidates.Should().HaveCount(1);
            electedCandidates.Should().Contain(1); // Candidate 1 should be elected

            // Candidate 5 should be eliminated
            candidates.Should().NotContain(c => c.Id == 5);

            // Check if the correct number of candidates are remaining after eliminating candidate 5
            candidates.Should().HaveCount(4);

            // Check if the votes are transferred correctly
            var candidate2 = candidates.First(c => c.Id == 2);
            var candidate3 = candidates.First(c => c.Id == 3);
            var candidate4 = candidates.First(c => c.Id == 4);

            var candidate2Votes = votes.Count(vote => vote.Preferences.Any(p => p.CandidateId == candidate2.Id));
            var candidate3Votes = votes.Count(vote => vote.Preferences.Any(p => p.CandidateId == candidate3.Id));
            var candidate4Votes = votes.Count(vote => vote.Preferences.Any(p => p.CandidateId == candidate4.Id));

            candidate2.Votes.Count.Should().Be(candidate2Votes);
            candidate3.Votes.Count.Should().Be(candidate3Votes);
            candidate4.Votes.Count.Should().Be(candidate4Votes);
        }*/
    }
    


    

