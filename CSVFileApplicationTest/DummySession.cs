using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSVFileApplicationTest
{
	public class DummySession : ISession
	{
		private readonly Dictionary<string, byte[]> _session = new();

		public string Id => "TestSession";
		public bool IsAvailable => true;
		public IEnumerable<string> Keys => _session.Keys;

		public void Clear() => _session.Clear();

		public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

		public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

		public void Remove(string key) => _session.Remove(key);

		public void Set(string key, byte[] value) => _session[key] = value;

		public bool TryGetValue(string key, out byte[] value) => _session.TryGetValue(key, out value);
	}
	
}
