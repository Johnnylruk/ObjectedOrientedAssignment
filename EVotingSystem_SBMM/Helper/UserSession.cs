using System.Text.Json;
using Newtonsoft.Json;
using EVotingSystem_SBMM.Models;

namespace EVotingSystem_SBMM.Helper;

public class UserSession : IUserSession
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public void CreateSession(UserModel? userModel, VoterModel? voterModel)
    {
        if (userModel != null)
        {
            string userValue = JsonConvert.SerializeObject(userModel);
            _httpContextAccessor.HttpContext.Session.SetString("userLoggedSession", userValue);
        }
        else if (voterModel != null)
        {
            string voterValue = JsonConvert.SerializeObject(voterModel);
            _httpContextAccessor.HttpContext.Session.SetString("userLoggedSession", voterValue);
        }
    }

    public void RemoveLoginSession()
    {
        _httpContextAccessor.HttpContext.Session.Remove("userLoggedSession");
    }

    public UserModel GetUserSession()
    {
        string userSession = _httpContextAccessor.HttpContext.Session.GetString("userLoggedSession");
        if (string.IsNullOrEmpty(userSession)) return null;
        return JsonConvert.DeserializeObject<UserModel>(userSession);
    }
    
    public VoterModel GetVoterSession()
    {
        string voterSession = _httpContextAccessor.HttpContext.Session.GetString("userLoggedSession");
        if (string.IsNullOrEmpty(voterSession)) return null;
        return JsonConvert.DeserializeObject<VoterModel>(voterSession);
    }
}