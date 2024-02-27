using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Helper;


public static class VoterCalculationHelper
{
    public static void ElectCandidatesSTV(Dictionary<int, int> voteCounts, List<CandidateModel> candidates, HashSet<int> electedCandidates, double quota)
    {
        // Base case: Check if all seats have been filled
        if (electedCandidates.Count == candidates.Count)
        {
            return;
        }

        // Find the candidate with the maximum votes
        int maxVoteCount = voteCounts.Values.Max();
        int candidateWithMaxVotesId = voteCounts.FirstOrDefault(x => x.Value == maxVoteCount).Key;

        // Check if any candidate has more votes than the quota
        if (maxVoteCount >= quota)
        {
            
            // Declare the candidate with the maximum votes as a winner
            electedCandidates.Add(candidateWithMaxVotesId);
            candidates.First(c => c.Id == candidateWithMaxVotesId).IsElected = true;

            // Recalculate vote counts after transferring excess votes
            TransferExcessVotesSTV(voteCounts, candidates, candidateWithMaxVotesId);
        }
        else
        {
            // Eliminate the candidate with the fewest votes
            int minVoteCount = voteCounts.Values.Min();
            int candidateWithMinVotesId = voteCounts.FirstOrDefault(x => x.Value == minVoteCount).Key;
            candidates.Remove(candidates.First(c => c.Id == candidateWithMinVotesId));
            voteCounts.Remove(candidateWithMinVotesId);

            // Recursively elect candidates with updated vote counts
            ElectCandidatesSTV(voteCounts, candidates, electedCandidates, quota);
        }
    }
    
    public static void TransferExcessVotesSTV(Dictionary<int, int> voteCounts, List<CandidateModel> candidates, int electedCandidateId)
    {
        // Calculate surplus votes
        int surplusVotes = voteCounts[electedCandidateId] - voteCounts.Values.Min();

        // Transfer surplus votes proportionally to next preferences
        foreach (var voteCount in voteCounts.ToList())
        {
            if (voteCount.Key != electedCandidateId && candidates.Any(c => c.Id == voteCount.Key))
            {
                voteCounts[voteCount.Key] += (int)Math.Floor((double)voteCounts[voteCount.Key] / voteCounts[electedCandidateId] * surplusVotes);
            }
        }

        // Reduce votes for elected candidate to quota
        voteCounts[electedCandidateId] -= surplusVotes;
    }
    
    //PV Calculation helper
    public static int CalculateVotesBasedOnRank(int rank, int totalCandidates)
    {
        // Assign votes inversely proportional to rank
        return totalCandidates - rank + 1;
    }
    public static void ElectCandidatesPV(Dictionary<int, int> voteCounts, List<CandidateModel> candidates, HashSet<int> electedCandidates)
    {
        while (true)
        {
            int totalVotes = voteCounts.Values.Sum();

            // Check if voteCounts is empty
            if (!voteCounts.Any())
            {
                break;
            }

            // If there is only one candidate left, declare them as the winner
            if (candidates.Count == 1)
            {
                var lastCandidateId = candidates[0].Id;
                electedCandidates.Add(lastCandidateId);
                candidates[0].IsElected = true;
                break;
            }

            // Find the candidate(s) with the most votes
            var maxVoteCount = voteCounts.Values.Max();
            var candidatesWithMaxVotes = voteCounts.Where(kv => kv.Value == maxVoteCount).Select(kv => kv.Key).ToList();

            if (maxVoteCount > totalVotes / 2)
            {
                // Declare the candidate(s) with the simple majority as winner(s)
                foreach (var candidateId in candidatesWithMaxVotes)
                {
                    electedCandidates.Add(candidateId);
                    candidates.First(c => c.Id == candidateId).IsElected = true;
                }
                break;
            }

            // Find the candidate with the fewest votes
            var minVoteCount = voteCounts.Values.Min();
            var candidateWithMinVotes = voteCounts.Where(kv => kv.Value == minVoteCount).Select(kv => kv.Key).First();

            // Eliminate the candidate with the fewest votes
            candidates.RemoveAll(c => c.Id == candidateWithMinVotes);
            voteCounts.Remove(candidateWithMinVotes);
        }
    }






}