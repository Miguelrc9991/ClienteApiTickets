using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ClienteApiTickets.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClienteApiTickets.Services
{


    public class ServiceApiTickets
    {
  
        private Uri UriApi;
        private string UrlApi;
        private MediaTypeWithQualityHeaderValue Header;
        private BlobServiceClient client;

        public ServiceApiTickets(string UrlApi)
        {
            this.UriApi = new Uri(UrlApi);
            this.UrlApi = UrlApi;
            this.Header = new MediaTypeWithQualityHeaderValue("application/json");
        }



        public async Task<string> GetTokenAsync(string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.UrlApi);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                LoginModel model = new LoginModel
                {
                    UserName = username,
                    Password = password
                };
                string json = JsonConvert.SerializeObject(model);
                StringContent content =
                new StringContent(json, Encoding.UTF8, "application/json");
                string request = "/api/login/ValidarUsuario";
                HttpResponseMessage response =
                await client.PostAsync(request, content);
                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    JObject jObject = JObject.Parse(data);
                    string token = jObject.GetValue("response").ToString();
                    return token;
                }
                else
                {
                    return null;
                }
            }
        }

        private async Task<T> CallApiAsync<T>(string request)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = this.UriApi;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                HttpResponseMessage response =
                await client.GetAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    T data = await response.Content.ReadAsAsync<T>();
                    return data;
                }
                else
                {
                    return default(T);
                }
            }
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            string request = "/api/Tickets";
            List<Ticket> tickets =
            await this.CallApiAsync<List<Ticket>>(request);
            return tickets;
        }

        public async Task<Ticket> FindTicketAsync(int idticket)
        {
            string request = "/api/Tickets/" + idticket;
            Ticket ticket =
            await this.CallApiAsync<Ticket>(request);
            return ticket;
        }

        public async Task InsertTicketAsync
(int idticket,int idusuario,DateTime fecha, string importe, string producto,string filename,string storagepath)
        {//hacer el blob
          
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Tickets/NuevoTicket";
                client.BaseAddress = this.UriApi;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                Ticket ticket = new Ticket();
                ticket.IdTicket = idticket;
                ticket.IdUsuario = idusuario;
                ticket.Fecha = fecha;
                ticket.Importe = importe;
                ticket.Producto = producto;
                ticket.Fileanme = filename;
                ticket.StoragePath = storagepath;
                string json = JsonConvert.SerializeObject(ticket);
               
                StringContent content = new StringContent
    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                await client.PostAsync(request, content);
            }
        }
        public async Task InsertUsertAsync
( int idusuario,  string nombre, string Apellidos, string Email, string Username,string password)
        {
            using (HttpClient client = new HttpClient())
            {
                string request = "/api/Tickets/CreateUser";
                client.BaseAddress = this.UriApi;
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(this.Header);
                Usuario user = new Usuario();
                user.IdUsuario = idusuario;
                user.Nombre = nombre;
                user.Apellidos = Apellidos;
                user.Email = Email;
                user.Username = Username;
                user.Password = password;
                string json = JsonConvert.SerializeObject(user);

                StringContent content = new StringContent
    (json, Encoding.UTF8, "application/json");
                HttpResponseMessage response =
                await client.PostAsync(request, content);
            }
        }



        //METODOS DEL BLOB


        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (var container in this.client.GetBlobContainersAsync())
            {
                containers.Add(container.Name);
            }
            return containers;
        }
        public async Task CreateContainerAsync(string nombre)
        {
            await this.client.CreateBlobContainerAsync(nombre.ToLower()
            , Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        }
        public async Task<List<BlobClass>> GetBlobsAsync(string containerName)
        {
            
            BlobContainerClient containerClient =
    this.client.GetBlobContainerClient(containerName);
            List<BlobClass> blobs = new List<BlobClass>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobClass blobClass = new BlobClass
                {
                    Nombre = item.Name,
                    Url = blobClient.Uri.AbsoluteUri
                };
                blobs.Add(blobClass);
            }
            return blobs;
        }
        public async Task UploadBlobAsync(string containerName, string blobName
, Stream stream)
        {

            BlobContainerClient containerClient =
            this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }









    }

}
