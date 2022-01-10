namespace MyWebServer.Http
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class HttpHeaderCollection : IEnumerable<HttpHeader>
    {
        private readonly Dictionary<string, HttpHeader> headers;

        public HttpHeaderCollection()
            => this.headers = new Dictionary<string, HttpHeader>();

        public int Count => this.headers.Count;

        public HttpHeader this[string name] => this.headers[name];
        public void Add(string name, string value)
        {
            var header = new HttpHeader(name, value);

            this.headers.Add(name, header);
        }

        public bool Contain(string key)
            => this.headers.ContainsKey(key);

        public HttpHeader Get(string key)
        {
            if (!this.Contain(key))
            {
                throw new InvalidOperationException($"Header with name '{key}' could not be found");
            }

            return this.headers[key];
        }
        public IEnumerator<HttpHeader> GetEnumerator()
            => this.headers.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => this.GetEnumerator();
    }
}
