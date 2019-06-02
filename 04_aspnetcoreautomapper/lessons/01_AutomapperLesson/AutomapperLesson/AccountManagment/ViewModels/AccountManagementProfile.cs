using AutoMapper;
using AutomapperLesson.Domain.AccountManagement;

namespace AutomapperLesson.AccountManagment.ViewModels
{
    // Über Profile lassen sich Automapper-Mappings nach Themenbereich gruppieren was
    // die Übersichtlichkeit deutlich erhöht
    public class AccountManagementProfile : Profile
    {        
        public AccountManagementProfile()
        {
            // Mit ReverseMap kann gleich der umgekehrte Mapping weg ebenfalls definiert werden.
            CreateMap<UserProfile, UserProfileEditModel>().ReverseMap();

            // Für Felder die nicht im Target-Objekt existieren kann man manuelle Mappings definieren
            // Automapper bietet noch viel mehr Möglichkeiten das Mapping-Verhalten zu beeinflussen.
            CreateMap<UserProfile, UserProfileViewModel>()
                .ForMember(m => m.DisplayName, opt => opt.MapFrom(p => $"{p.FirstName} {p.LastName} ({p.Age})"));
        }
    }
}
