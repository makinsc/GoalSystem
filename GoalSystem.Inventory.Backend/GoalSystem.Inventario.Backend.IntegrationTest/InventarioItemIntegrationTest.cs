using GoalSystem.Inventario.Backend.API;
using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using GoalSystem.Inventario.Backend.Transversal.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GoalSystem.Inventario.Backend.IntegrationTest
{
    public class InventarioItemIntegrationTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly HttpMessageHandler _httpHandler;
        public InventarioItemIntegrationTest(CustomWebApplicationFactory<Startup> fixture)
        {
            _client = fixture.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            _httpHandler = fixture.Server.CreateHandler();//HttpHandler;
        }
        [Fact]
        public async Task GetAllInventarioItem_OK()
        {
            // Arrange
            var request = new HttpRequestMessage(
                HttpMethod.Get, "/api/Inventario");
            // Act: request the /todo route
            var response = await _client.SendAsync(request);
            // Assert: the user is sent to the login page
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);

        }

        [Fact]
        public async Task GetInsertAndGetAllInventarioItem_OK()
        {
            // Arrange
            var request = new HttpRequestMessage(
                HttpMethod.Put, "/api/Inventario");
            var body = new InventarioItemViewModel()
            {
                Id = Guid.NewGuid(),
                Nombre = "Item1 Test",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
            };
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            // Act: lanzamos la petición a la API
            var response = await _client.SendAsync(request);

            //Obtenemos toos los elementos
            var requestGetAll = new HttpRequestMessage(
                HttpMethod.Get, "/api/Inventario");
            // Act: request the /todo route
            var responseGetAll = await _client.SendAsync(requestGetAll);
            var contentGetAllResponse = JsonConvert.DeserializeObject<IEnumerable<InventarioItemViewModel>>(await responseGetAll.Content.ReadAsStringAsync());
            // Assert: Verificamos que todo está correcto
            Assert.Equal(
                HttpStatusCode.OK,
                response.StatusCode);
            Assert.Equal(
                HttpStatusCode.OK,
                responseGetAll.StatusCode);
            Assert.Contains(contentGetAllResponse, item => item.Id == body.Id);
        }

        [Fact]
        public async Task SubscribeAndDeleteInventarioItem_OK()
        {
            #region Arrange

            List<InventarioItemViewModel> listaInserts = new List<InventarioItemViewModel>()
            {
                new InventarioItemViewModel(){
                Id = Guid.NewGuid(),
                    Nombre = "Item A Test",
                    Unidades = 100,
                    FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
                },
                new InventarioItemViewModel(){
                Id = Guid.NewGuid(),
                    Nombre = "Item B Test",
                    Unidades = 100,
                    FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
                },
                new InventarioItemViewModel(){
                Id = Guid.NewGuid(),
                    Nombre = "Item C Test",
                    Unidades = 100,
                    FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
                },
                new InventarioItemViewModel(){
                Id = Guid.NewGuid(),
                    Nombre = "Item D Test",
                    Unidades = 100,
                    FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
                },
                new InventarioItemViewModel(){
                Id = Guid.NewGuid(),
                    Nombre = "Item E Test",
                    Unidades = 100,
                    FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
                },
            };

            List<InventarioItemViewModel> deletedList = new List<InventarioItemViewModel>();
            //Conectamos con el Hub
            var connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl("ws://localhost:8888/Hubs/Inventarioitem", o => o.HttpMessageHandlerFactory = _ => _httpHandler)
                .Build();

            connection.On<DeletedMessageReceived>("ReceiveDeletedMessage", (parameters) =>
            {
                var elemen = parameters as DeletedMessageReceived;
                deletedList.Add(new InventarioItemViewModel() { Id = elemen.Id });
            });

            #endregion

            #region Act

            //Inserto los 5 elementos
            for (int i = 0; i < listaInserts.Count(); i++)
            {
                var request = new HttpRequestMessage(
               HttpMethod.Put, "/api/Inventario");
                var body = listaInserts[i];
                request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
                // Act: lanzamos la petición a la API
                await _client.SendAsync(request);
            }
            //Iniciamos la conexión con el Hub
            await connection.StartAsync();

            for (int i = 0; i < listaInserts.Count(); i++)
            {
                var requestDelete = new HttpRequestMessage(
                HttpMethod.Delete, "/api/Inventario/" + listaInserts[i].Id.ToString());

                // Act: lanzamos la petición a la API
                var response = await _client.SendAsync(requestDelete);
                //Le damos un poco de margen de tiempo a que entren todos los mensajes de SignalR
                Thread.Sleep(1000);
            }
            #endregion

            #region Act
            Assert.Equal(listaInserts.Count(), deletedList.Count());
            for (int i = 0; i < listaInserts.Count(); i++)
            {
                Assert.Contains(deletedList, e => e.Id == listaInserts[i].Id);
            }
            #endregion
        }

        [Fact]
        public async Task SubscribeExpiredInventarioItem_OK()
        {
            #region Arrange
            var request = new HttpRequestMessage(
                HttpMethod.Put, "/api/Inventario");
            var body = new InventarioItemViewModel()
            {
                Id = Guid.NewGuid(),
                Nombre = "Item Expired Test",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(-5).ToString()
            };
            List<InventarioItemViewModel> expiredList = new List<InventarioItemViewModel>();
            //Conectamos con el Hub
            var connection = new HubConnectionBuilder()
                .WithAutomaticReconnect()
                .WithUrl("ws://localhost:8888/Hubs/Inventarioitem", o => o.HttpMessageHandlerFactory = _ => _httpHandler)
                .Build();

            connection.On<ExpiredMessageReceived>("ItemExpired", (parameters) =>
            {
                var elemen = parameters as ExpiredMessageReceived;
                expiredList.Add(new InventarioItemViewModel() { Id = elemen.Id });
            });
            #endregion

            #region Act
            //Iniciamos la conexión con el Hub
            await connection.StartAsync();

            //Inserto un elemento caducado
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            await _client.SendAsync(request);
            //Esperamos 2 minutos para asegurarnos que de tiempo al worker a ejecutarse, ya que se ejecuta cada minuto para limpiar los expirados.
            Thread.Sleep(120000);

            #endregion

            #region Assert
            Assert.Equal(1, expiredList.Count());
            Assert.Equal(body.Id, expiredList[0].Id);
            #endregion
        }
    }
}
