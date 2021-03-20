using GoalSystem.Inventario.Backend.DistributedServices.SignalR;
using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Domain.Core.Services;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Domain.Core.Tests
{
    [TestClass]
    public class InventarioItemServiceTests
    {
        private Mock<ILogger<InventarioItemService>> _loggerMocked;
        private Mock<IDataRepository<InventarioItem>> _inventarioItemRepositoryMocked;
        private Mock<IHubContext<ItemInventarioHub>> _signalRContextMocked;
        private IInventarioItemService CreateInventarioService()
        {
            _loggerMocked = new Mock<ILogger<InventarioItemService>>();
            _inventarioItemRepositoryMocked = new Mock<IDataRepository<InventarioItem>>();
            _signalRContextMocked = new Mock<IHubContext<ItemInventarioHub>>();
            Mock<IHubClients> mockClients = new Mock<IHubClients>();
            Mock<Microsoft.AspNetCore.SignalR.IClientProxy> mockClientProxy = new Mock<Microsoft.AspNetCore.SignalR.IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            _signalRContextMocked.Setup(x => x.Clients).Returns(() => mockClients.Object);
            return new InventarioItemService(_loggerMocked.Object, _inventarioItemRepositoryMocked.Object, _signalRContextMocked.Object);
        }

        [TestMethod]
        public void GetAllTestMethod_Ok()
        {
            #region Arrange

            IQueryable<InventarioItem> expected = new List<InventarioItem>()
            {
                new InventarioItem() { Id=Guid.NewGuid(), Nombre="item1", Unidades=500, IsNotificacionExpiradaEnviada=false, FechaCaducidad = DateTime.Today.AddDays(5)  },
                new InventarioItem() { Id=Guid.NewGuid(), Nombre="item2", Unidades=500, IsNotificacionExpiradaEnviada=true, FechaCaducidad = DateTime.Today.AddDays(-5) }
            }.AsQueryable();
            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.AsQueryable()).Returns(expected);

            #endregion

            #region Act

            var actual = service.GetAll().GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count(), expected.Count());
            foreach (var item in actual)
            {
                Assert.IsTrue(expected.Any(x => x.Id == item.Id &&
                                                x.Nombre == item.Nombre &&
                                                x.Unidades == item.Unidades &&
                                                x.IsNotificacionExpiradaEnviada == item.IsNotificacionExpiradaEnviada));
            }
            _inventarioItemRepositoryMocked.Verify(s => s.AsQueryable(), Times.Once);

            #endregion
        }
        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetAllTestMethod_Exception()
        {
            #region Arrange
            
            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.AsQueryable()).Throws(new KeyNotFoundException("Clave primaria no encontrada"));

            #endregion

            #region Act

            service.GetAll().GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        public void GetExpiredTestMethod_Ok()
        {
            #region Arrange

            IEnumerable<InventarioItem> expected = new List<InventarioItem>()
            {
                new InventarioItem() { Id=Guid.NewGuid(), Nombre="item2", Unidades=500, IsNotificacionExpiradaEnviada=true, FechaCaducidad = DateTime.Today.AddDays(-5) }
            };
            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.FindByAsync(It.IsAny<Expression<Func<InventarioItem,bool>>>())).Returns(Task.FromResult(expected));

            #endregion

            #region Act

            var actual = service.GetExpired().GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Count(), expected.Count());
            foreach (var item in actual)
            {
                Assert.IsTrue(expected.Any(x => x.Id == item.Id &&
                                                x.Nombre == item.Nombre &&
                                                x.Unidades == item.Unidades &&
                                                x.IsNotificacionExpiradaEnviada == item.IsNotificacionExpiradaEnviada &&
                                                x.FechaCaducidad < DateTime.UtcNow));
            }
            _inventarioItemRepositoryMocked.Verify(s => s.FindByAsync(It.IsAny<Expression<Func<InventarioItem, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void GetExpiredTestMethod_ReturnNull_Ok()
        {
            #region Arrange

            IEnumerable<InventarioItem> expected = null;
            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.FindByAsync(It.IsAny<Expression<Func<InventarioItem, bool>>>())).Returns(Task.FromResult(expected));

            #endregion

            #region Act

            var actual = service.GetExpired().GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsNull(actual);
            
            _inventarioItemRepositoryMocked.Verify(s => s.FindByAsync(It.IsAny<Expression<Func<InventarioItem, bool>>>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetExpiredTestMethod_Exception()
        {
            #region Arrange

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.FindByAsync(It.IsAny<Expression<Func<InventarioItem, bool>>>())).Throws(new Exception("Error en la cadena de conexión."));

            #endregion

            #region Act

            service.GetExpired().GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        public void InsertItemTestMethod_Ok()
        {
            #region Arrange


            InventarioItem item = new InventarioItem() { Id = Guid.NewGuid(), Nombre = "item2", Unidades = 500, IsNotificacionExpiradaEnviada = true, FechaCaducidad = DateTime.Today.AddDays(-5) };
            
            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Add(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.InsertarItem(item).GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.AreEqual(actual.Id, item.Id);
            
            _inventarioItemRepositoryMocked.Verify(s => s.Add(It.IsAny<InventarioItem>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void InsertItemNullTestMethod_Exception()
        {
            #region Arrange

            InventarioItem item = null;

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Add(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.InsertarItem(item).GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void InsertItemEmptyPrimaryKeyTestMethod_Exception()
        {
            #region Arrange

            InventarioItem item = new InventarioItem() { Id= new Guid() };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Add(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.InsertarItem(item).GetAwaiter().GetResult();

            #endregion           
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InsertItemSaveChangesErrorMethod_Exception()
        {
            #region Arrange

            InventarioItem item = new InventarioItem() { Id = Guid.NewGuid(), Nombre = "item2", Unidades = 500, IsNotificacionExpiradaEnviada = true, FechaCaducidad = DateTime.Today.AddDays(-5) };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Add(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<Exception>();
            #endregion

            #region Act

            var actual = service.InsertarItem(item).GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        public void SacarItemTestMethod_Ok()
        {
            #region Arrange


            InventarioItem item = new InventarioItem() { Id = Guid.NewGuid(), Nombre = "item2", Unidades = 500, IsNotificacionExpiradaEnviada = true, FechaCaducidad = DateTime.Today.AddDays(-5) };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Delete(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
                  
            #endregion

            #region Act

            var actual = service.SacarItem(item).GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsTrue(actual);            

            _inventarioItemRepositoryMocked.Verify(s => s.Delete(It.IsAny<InventarioItem>()), Times.Once);

            #endregion
        }

        [TestMethod]
        public void SacarItemTest_NoEmitEvent_Ok()
        {
            #region Arrange


            InventarioItem item = new InventarioItem() { Id = Guid.NewGuid(), Nombre = "item2", Unidades = 500, IsNotificacionExpiradaEnviada = true, FechaCaducidad = DateTime.Today.AddDays(-5) };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Delete(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(0));

            #endregion

            #region Act

            var actual = service.SacarItem(item).GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsTrue(actual);

            _inventarioItemRepositoryMocked.Verify(s => s.Delete(It.IsAny<InventarioItem>()), Times.Once);
            _signalRContextMocked.Verify(s => s.Clients.All, Times.Never);
            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void SacarItemEmptyPrimaryKeyTestMethod_Exception()
        {
            #region Arrange

            InventarioItem item = new InventarioItem() { Id = new Guid() };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Add(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.SacarItem(item).GetAwaiter().GetResult();

            #endregion           
        }

        [TestMethod]
        public void ActualizarItemTestMethod_Ok()
        {
            #region Arrange


            InventarioItem item = new InventarioItem() { Id = Guid.NewGuid(), Nombre = "item2", Unidades = 500, IsNotificacionExpiradaEnviada = true, FechaCaducidad = DateTime.Today.AddDays(-5) };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Update(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.ActualizarItem(item).GetAwaiter().GetResult();

            #endregion

            #region Assert

            Assert.IsTrue(actual);

            _inventarioItemRepositoryMocked.Verify(s => s.Update(It.IsAny<InventarioItem>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ActualizarItemNullTestMethod_Exception()
        {
            #region Arrange

            InventarioItem item = null;

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Update(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.ActualizarItem(item).GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void ActualizarItemEmptyPrimaryKeyTestMethod_Exception()
        {
            #region Arrange

            InventarioItem item = new InventarioItem() { Id = new Guid() };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Add(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));
            #endregion

            #region Act

            var actual = service.ActualizarItem(item).GetAwaiter().GetResult();

            #endregion           
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ActualizarItemSaveChangesErrorMethod_Exception()
        {
            #region Arrange

            InventarioItem item = new InventarioItem() { Id = Guid.NewGuid(), Nombre = "item2", Unidades = 500, IsNotificacionExpiradaEnviada = true, FechaCaducidad = DateTime.Today.AddDays(-5) };

            IInventarioItemService service = CreateInventarioService();
            _inventarioItemRepositoryMocked.Setup(s => s.Update(It.IsAny<InventarioItem>())).Verifiable();
            _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork).Returns(It.IsAny<IUnitOfWork>());
            _ = _inventarioItemRepositoryMocked.Setup(s => s.UnitOfWork.SaveChangesAsync(It.IsAny<CancellationToken>())).Throws<Exception>();
            #endregion

            #region Act

            var actual = service.InsertarItem(item).GetAwaiter().GetResult();

            #endregion            
        }


    }
}
