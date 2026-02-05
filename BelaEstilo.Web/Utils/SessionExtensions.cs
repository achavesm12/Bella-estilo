using Newtonsoft.Json;

namespace BelaEstilo.Web.Utils
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
            => session.SetString(key, JsonConvert.SerializeObject(value));

        public static T? GetObject<T>(this ISession session, string key)
        {
            var str = session.GetString(key);
            return string.IsNullOrEmpty(str) ? default : JsonConvert.DeserializeObject<T>(str);
        }
    }
}
