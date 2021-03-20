using AutoMapper;
using GoalSystem.Inventario.Backend.Application.Core.Interfaces;
using GoalSystem.Inventario.Backend.Application.Core.Managers;
using GoalSystem.Inventario.Backend.Application.ViewModels.Cliente;
using GoalSystem.Inventario.Backend.Domain.Core.Interfaces;
using GoalSystem.Inventario.Backend.Infrastructure.Persistence.Models.InventarioItem;
using GoalSystem.Inventario.Backend.Transversal.MappingProfile;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoalSystem.Inventario.Backend.Application.Core.Tests
{
    [TestClass]
    public class InventarioItemManagerTest
    {
        private Mock<ILogger<InventarioItemManager>> _loggerMocked;
        private IMapper _mapper;
        private Mock<IInventarioItemService> _inventarioItemServiceMocked;

        [TestInitialize]
        public void Initialize()
        {
            var config = new MapperConfiguration(opts =>
            {
                opts.AddProfile<MappingProfile>();
            });

            _mapper = config.CreateMapper();
        }

        private IInventarioItemManager CreateInventarioItemManager()
        {
            _loggerMocked = new Mock<ILogger<InventarioItemManager>>();
            _inventarioItemServiceMocked = new Mock<IInventarioItemService>();

            return new InventarioItemManager(_loggerMocked.Object, _mapper, _inventarioItemServiceMocked.Object);
        }

        [TestMethod]
        public void SacarItemByIdMethod_OK()
        {
            #region Arrange

            Guid itemId = Guid.NewGuid();
                          
            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.SacarItem(It.IsAny<Guid>())).Returns(Task.FromResult(true));

            #endregion

            #region Act

            var actual = manager.SacarItem(itemId).Result;

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual);
            _inventarioItemServiceMocked.Verify(s => s.SacarItem(It.IsAny<Guid>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SacarItemByIdMethod_Exception()
        {
            #region Arrange

            Guid itemId = Guid.NewGuid();

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.SacarItem(It.IsAny<Guid>())).Throws(new Exception("Error controlado"));

            #endregion

            #region Act

            manager.SacarItem(itemId).GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        public void SacarItemMethod_OK()
        {
            #region Arrange

            InventarioItemViewModel item = new InventarioItemViewModel()
            {
                Id = Guid.NewGuid(),
                Nombre = "item 1",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
            };

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.SacarItem(It.IsAny<InventarioItem>())).Returns(Task.FromResult(true));

            #endregion

            #region Act

            var actual = manager.SacarItem(item).Result;

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual);
            _inventarioItemServiceMocked.Verify(s => s.SacarItem(It.IsAny<InventarioItem>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void SacarItemMethod_Exception()
        {
            #region Arrange

            InventarioItemViewModel item = new InventarioItemViewModel()
            {
                Id = Guid.NewGuid(),
                Nombre = "item 1",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
            };

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.SacarItem(It.IsAny<InventarioItem>())).Throws(new Exception("Error controlado"));

            #endregion

            #region Act

            var actual = manager.SacarItem(item).GetAwaiter().GetResult();

            #endregion            
        }

        [TestMethod]
        public void InsertarItemMethod_OK()
        {
            #region Arrange

            InventarioItem expected = new InventarioItem()
            {
                Id = Guid.NewGuid(),
                Nombre = "item 1",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(5)
            };
            InventarioItemViewModel itemToInsert = new InventarioItemViewModel()
            {
                Id = Guid.NewGuid(),
                Nombre = "item 1",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(5).ToString()
            };

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.InsertarItem(It.IsAny<InventarioItem>())).Returns(Task.FromResult(expected));

            #endregion

            #region Act

            var actual = manager.InsertarItem(itemToInsert).Result;

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is InventarioItemViewModel);
            Assert.AreEqual(itemToInsert.Id, actual.Id);
            _inventarioItemServiceMocked.Verify(s => s.InsertarItem(It.IsAny<InventarioItem>()), Times.Once);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InsertarItemMethod_Exception()
        {
            #region Arrange

            InventarioItemViewModel itemToInsert = null;

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.InsertarItem(It.IsAny<InventarioItem>())).Throws(new Exception("Error controlado"));

            #endregion

            #region Act

            manager.InsertarItem(itemToInsert).GetAwaiter().GetResult();

            #endregion
        }

        [TestMethod]
        public void GetAllMethod_OK()
        {
            #region Arrange

            IEnumerable<InventarioItem> expectedBD = new List<InventarioItem>()
            {
                new InventarioItem()
                {

                Id = Guid.NewGuid(),
                Nombre = "item 1",
                Unidades = 100,
                FechaCaducidad = DateTime.UtcNow.AddDays(5)
                },
                new InventarioItem()
                {

                Id = Guid.NewGuid(),
                Nombre = "item 2",
                Unidades = 0,
                FechaCaducidad = DateTime.UtcNow.AddDays(-5)
                }
            };

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.GetAll()).Returns(Task.FromResult(expectedBD));

            #endregion

            #region Act

            var actual = manager.GetAll().Result;

            #endregion

            #region Assert

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual is IEnumerable<InventarioItemViewModel>);
            Assert.AreEqual(actual.Count(), expectedBD.Count());
            _inventarioItemServiceMocked.Verify(s => s.GetAll(), Times.Once);
            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void GetAllMethod_Exception()
        {
            #region Arrange

            IInventarioItemManager manager = CreateInventarioItemManager();
            _inventarioItemServiceMocked.Setup(s => s.GetAll()).Throws(new Exception("Error controlado"));

            #endregion

            #region Act

            manager.GetAll().GetAwaiter().GetResult();

            #endregion
            
        }

    }
}
