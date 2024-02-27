using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace EVotingSystem_SBMM.Helper;

public class LanguageService
{
    private IStringLocalizer _localizer;  
  
    public LanguageService(IStringLocalizerFactory factory)  
    {  
        var type = typeof(ShareResource);  
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);  
        _localizer = factory.Create("SharedResource", assemblyName.Name);  
    }  
    public LocalizedString Getkey(string key)  
    {  
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;  
        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;      
        return _localizer[key];  
    }  
}  
