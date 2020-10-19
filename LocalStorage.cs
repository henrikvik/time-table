using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TimeTable
{
    public class LocalStorage
    {
        private readonly IJSRuntime _jsRuntime;
        
        public LocalStorage(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }
        
        public async Task<string?> GetItem(string key)
        {
            return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
        }
        
        public async Task SetItem(string key, string? value)
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value!);
        }
    }
}