using EVotingSystem_SBMM.Data;
using EVotingSystem_SBMM.Helper;
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Repository
{
    public class VotersRepository : IVotersRepository
    {
        private readonly EVotingSystemDB _evotingSystem;

        public VotersRepository(EVotingSystemDB evotingSystemDb)
        {
            this._evotingSystem = evotingSystemDb;
        }

        public VoterModel Details(int id)
        {
            return _evotingSystem.Voters.FirstOrDefault(
                x => x.Id == id
                ) ;
        }
        public  List<VoterModel> GetAll()
        {
            return _evotingSystem.Voters.ToList();
        }
        public VoterModel Register(VoterModel voter)
        {
            voter.IsPending = false;
            _evotingSystem.Voters.Add(voter);
            _evotingSystem.SaveChanges();
            return voter;
        }
        public VoterModel GetVoterById(int id)
        {   
            return _evotingSystem.Voters.FirstOrDefault(
                x => x.Id == id );
        }
        public VoterModel UpdateVoter(VoterModel voter)
        {
            VoterModel voterDb = GetVoterById(voter.Id);
            if (voterDb == null) throw new Exception("Error when trying to update voter");

            voterDb.Name = voter.Name;
            voterDb.Email = voter.Email;
            voterDb.Mobile = voter.Mobile;
            voterDb.State = voter.State;
            voterDb.City = voter.City;
            voterDb.Address = voter.Address;
            voterDb.BirthDate = voter.BirthDate;

            _evotingSystem.Voters.Update(voterDb);
            _evotingSystem.SaveChanges();
            return voterDb;
            
        }

        public bool DeleteVoter(int voter)
        {
            VoterModel voterDb = GetVoterById(voter);
            if (voterDb == null) throw new Exception("Error when trying to update voter");

            _evotingSystem.Remove(voterDb);
            _evotingSystem.SaveChanges();
            return true;
        }

        public string GetVoterCity()
        {
            
            var voter = _evotingSystem.Voters.FirstOrDefault();
            if (voter != null)
            {
                return voter.City;
            }
            return null;        
        }
        
        public VoterModel SubmitRequest(VoterModel voter)
        {
            voter.IsPending = true;
            voter.Password = voter.Password.GenerateHash();
            voter.Passport = voter.Passport.GenerateHash();
            _evotingSystem.Voters.Add(voter);
            _evotingSystem.SaveChanges();
            return voter;
        }

        public VoterModel ApproveVoterRequest(VoterModel voter)
        {
            voter.IsPending = false;
            _evotingSystem.Voters.Update(voter);
            _evotingSystem.SaveChanges();
            return voter;
        }
        public VoterModel DenyVoterRequest(VoterModel voter)
        {
            VoterModel voterDb = GetVoterById(voter.Id);
            _evotingSystem.Voters.Remove(voterDb);
            _evotingSystem.SaveChanges();
            return voter;
        }
    }
}
