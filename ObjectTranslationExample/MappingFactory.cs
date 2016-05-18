using System.Text.RegularExpressions;
using Glue;
using ObjectTranslationExample.ObjectsToTranslate;

namespace ObjectTranslationExample
{
    public class MappingFactory
    {
        public Mapping<T1, T2> GetMapping<T1, T2>()
        {
            object retMapping = null;

            TypeSwitch.Do(typeof (T1), typeof (T2),
                TypeSwitch.Case<User, UiUser>(() => retMapping = UsertoUiUser),
                TypeSwitch.Case<User, DataUser>(() => retMapping = UserToDataUser)
                );

            if (retMapping == null)
            {
                // error handling, very solution-specific
            }

            return (Mapping<T1, T2>) retMapping;
        }

        private Mapping<User, UiUser> _usertoUiUser;
        private Mapping<User, UiUser> UsertoUiUser 
        {
            get { return _usertoUiUser ?? (
                _usertoUiUser = new Mapping<User, UiUser>(
                    uiuser =>
                    {
                        var addressParts = BreakAddressString(uiuser.Address);
                        return new User
                        {
                            Street = addressParts[0] + " " + addressParts[1],
                            City = addressParts[2],
                            State = addressParts[3],
                            FirstName = uiuser.FirstName,
                            LastName = uiuser.LastName
                        };
                    }, 
                    user => new UiUser
                    {
                        Address = user.Street + " " + user.City + " " + user.State,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    })); 
            }
        }

        private Mapping<User, DataUser> _usertoDataUser;
        private Mapping<User, DataUser> UserToDataUser
        {
            get
            {
                return _usertoDataUser ?? (
                    _usertoDataUser = new Mapping<User, DataUser>(
                        datauser => new User
                        {
                            Street = datauser.StreetNumber + " " + datauser.StreetName,
                            City = datauser.City,
                            State = datauser.State,
                            FirstName = datauser.FirstName,
                            LastName = datauser.LastName
                        },
                        user =>
                        {
                            var addressParts = BreakAddressString(user.Street);
                            return new DataUser
                            {
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                City = user.City,
                                State = user.State,
                                StreetNumber = addressParts[0],
                                StreetName = addressParts[1]
                            };
                        }
                        ));
            }
        }

        private static string[] BreakAddressString(string address)
        {
            var regex = new Regex(@"\s");
            return regex.Split(address);
        }
    }
}