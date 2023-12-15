using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Emit;
using System.Reflection;
using System.Collections;
using Microsoft.AspNetCore.Authorization;
using LLMServiceHub.Common;

namespace LLMServiceHub.Pages
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.RazorPages.PageModel" />
    [Authorize]
    public class StatusModel : PageModel
    {
        /// <summary>
        /// The cache
        /// </summary>
        private readonly IMemoryCache _cache;

        /// <summary>
        /// Gets or sets the cache keys.
        /// </summary>
        /// <value>
        /// The cache keys.
        /// </value>
        public IEnumerable<string> CacheKeys { get; set; } = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusModel" /> class.
        /// </summary>
        /// <param name="cache">The cache.</param>
        public StatusModel(IMemoryCache cache)
        {
            _cache = cache;
        }
        /// <summary>
        /// Called when [get].
        /// </summary>
        /// <returns></returns>
        public IActionResult OnGet()
        {
            var caches = _cache.GetKeys<string>();
            CacheKeys = caches;

            return Page();        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class MemoryCacheExtensions
    {
        #region Microsoft.Extensions.Caching.Memory_6_OR_OLDER

        /// <summary>
        /// The get entries6
        /// </summary>
        private static readonly Lazy<Func<MemoryCache, object>> GetEntries6 =
            new Lazy<Func<MemoryCache, object>>(() => (Func<MemoryCache, object>)Delegate.CreateDelegate(
                typeof(Func<MemoryCache, object>),
                typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance).GetGetMethod(true),
                throwOnBindFailure: true));

        #endregion

        #region Microsoft.Extensions.Caching.Memory_7_OR_NEWER

        /// <summary>
        /// The get coherent state
        /// </summary>
        private static readonly Lazy<Func<MemoryCache, object>> GetCoherentState =
            new Lazy<Func<MemoryCache, object>>(() =>
                CreateGetter<MemoryCache, object>(typeof(MemoryCache)
                    .GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance)));

        /// <summary>
        /// The get entries7
        /// </summary>
        private static readonly Lazy<Func<object, IDictionary>> GetEntries7 =
            new Lazy<Func<object, IDictionary>>(() =>
                CreateGetter<object, IDictionary>(typeof(MemoryCache)
                    .GetNestedType("CoherentState", BindingFlags.NonPublic)
                    .GetField("_entries", BindingFlags.NonPublic | BindingFlags.Instance)));

        /// <summary>
        /// Creates the getter.
        /// </summary>
        /// <typeparam name="TParam">The type of the parameter.</typeparam>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        private static Func<TParam, TReturn> CreateGetter<TParam, TReturn>(FieldInfo field)
        {
            var methodName = $"{field.ReflectedType.FullName}.get_{field.Name}";
            var method = new DynamicMethod(methodName, typeof(TReturn), new[] { typeof(TParam) }, typeof(TParam), true);
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldfld, field);
            ilGen.Emit(OpCodes.Ret);
            return (Func<TParam, TReturn>)method.CreateDelegate(typeof(Func<TParam, TReturn>));
        }

        #endregion

        /// <summary>
        /// The get entries
        /// </summary>
        private static readonly Func<MemoryCache, IDictionary> GetEntries =
            Assembly.GetAssembly(typeof(MemoryCache)).GetName().Version.Major < 7
                ? (Func<MemoryCache, IDictionary>)(cache => (IDictionary)GetEntries6.Value(cache))
                : cache => GetEntries7.Value(GetCoherentState.Value(cache));

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <param name="memoryCache">The memory cache.</param>
        /// <returns></returns>
        public static ICollection GetKeys(this IMemoryCache memoryCache) =>
            GetEntries((MemoryCache)memoryCache).Keys;

        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="memoryCache">The memory cache.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetKeys<T>(this IMemoryCache memoryCache) =>
            memoryCache.GetKeys().OfType<T>();
    }
}
