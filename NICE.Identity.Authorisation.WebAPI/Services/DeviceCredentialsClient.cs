using Auth0.ManagementApi;
using Auth0.ManagementApi.Clients;
using Auth0.ManagementApi.Models;
using Auth0.ManagementApi.Paging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// once this is merged: https://github.com/auth0/auth0.net/pull/460 - and a nuget released, we can update the nuget package and remove this file.
/// </summary>
namespace NICE.Identity.Authorisation.WebAPI.Services
{
	/// <summary>
	/// Specifies criteria to use when querying all device credentials.
	/// </summary>
	public class GetDeviceCredentialsRequest
    {
        /// <summary>
        /// Comma-separated list of fields to include or exclude (based on value provided for include_fields) in the result. Leave empty to retrieve all fields.
        /// </summary>
        public string Fields { get; set; }
        /// <summary>
        /// Whether specified fields are to be included (true) or excluded (false).
        /// </summary>
        public bool IncludeFields { get; set; } = true;
        /// <summary>
        /// user_id of the devices to retrieve.
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// client_id of the devices to retrieve.
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Type of credentials to retrieve. Must be `public_key`, `refresh_token` or `rotating_refresh_token`. The property will default to `refresh_token` when paging is requested
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Contains methods to access the /device-credentials endpoints.
    /// </summary>
    public class DeviceCredentialsClient : BaseClient
    {
        readonly JsonConverter[] converters = new JsonConverter[] { new PagedListConverter<DeviceCredential>("device_credentials") };

        /// <summary>
        /// Initializes a new instance of <see cref="DeviceCredentialsClient"/>.
        /// </summary>
        /// <param name="connection"><see cref="IManagementConnection"/> used to make all API calls.</param>
        /// <param name="baseUri"><see cref="Uri"/> of the endpoint to use in making API calls.</param>
        /// <param name="defaultHeaders">Dictionary containing default headers included with every request this client makes.</param>
        public DeviceCredentialsClient(IManagementConnection connection, Uri baseUri, IDictionary<string, string> defaultHeaders)
            : base(connection, baseUri, defaultHeaders)
        {
        }

        /// <summary>
        /// Gets a list of all the device credentials.
        /// </summary>
        /// <param name="request">Specifies criteria to use when querying device credentials.</param>
        /// <param name="pagination">Specifies <see cref="PaginationInfo"/> to use in requesting paged results.</param>
        /// <returns>A list of <see cref="DeviceCredential"/> which conforms to the criteria specified.</returns>
        public Task<IPagedList<DeviceCredential>> GetAllAsync(GetDeviceCredentialsRequest request, PaginationInfo pagination)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (pagination == null)
                throw new ArgumentNullException(nameof(pagination));

            var queryStrings = new Dictionary<string, string>
                {
                    {"fields", request.Fields},
                    {"include_fields", request.IncludeFields.ToString().ToLower()},
                    {"user_id", request.UserId},
                    {"client_id", request.ClientId},
                    {"type", request.Type},
                    {"page", pagination.PageNo.ToString()},
                    {"per_page", pagination.PerPage.ToString()},
                    {"include_totals", pagination.IncludeTotals.ToString().ToLower()}
                };

            return Connection.GetAsync<IPagedList<DeviceCredential>>(BuildUri("device-credentials", queryStrings), DefaultHeaders, converters);
        }

        /// <summary>
        /// Deletes a device credential.
        /// </summary>
        /// <param name="id">The id of the device credential to delete.</param>
        /// <returns>A <see cref="Task"/> that represents the asynchronous delete operation.</returns>
        public Task DeleteAsync(string id)
        {
            return Connection.SendAsync<object>(HttpMethod.Delete, BuildUri($"device-credentials/{EncodePath(id)}"), null, DefaultHeaders);
        }
    }

    internal class PagedListConverter<T> : JsonConverter
    {
        private readonly string _collectionFieldName;
        private readonly bool _collectionInDictionary;

        public PagedListConverter(string collectionFieldName, bool collectionInDictionary = false)
        {
            _collectionFieldName = collectionFieldName;
            _collectionInDictionary = collectionInDictionary;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IPagedList<T>).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject item = JObject.Load(reader);

                if (item[_collectionFieldName] != null)
                {
                    var collection = item[_collectionFieldName].ToObject<IList<T>>(serializer);

                    int length = 0;
                    int limit = 0;
                    int start = 0;
                    int total = 0;
                    if (item["length"] != null)
                        length = item["length"].Value<int>();
                    if (item["limit"] != null)
                        limit = item["limit"].Value<int>();
                    if (item["start"] != null)
                        start = item["start"].Value<int>();
                    if (item["total"] != null)
                        total = item["total"].Value<int>();

                    return new PagedList<T>(collection, new PagingInformation(start, limit, length, total));
                }
                else if (_collectionInDictionary) // Special case to handle User Logs which is returned as a dictionary and not an array
                {
                    List<T> collection = new List<T>();
                    foreach (var kvp in item)
                    {
                        if (kvp.Key != "length")
                        {
                            try
                            {
                                collection.Add(kvp.Value.ToObject<T>());
                            }
                            catch
                            {
                                // Fail silently (for now)
                            }
                        }
                    }

                    return new PagedList<T>(collection);
                }
            }
            else
            {
                JArray array = JArray.Load(reader);

                var collection = array.ToObject<IList<T>>();

                return new PagedList<T>(collection);
            }

            // This should not happen. Perhaps better to throw exception at this point?
            return null;
        }
    }
}
