﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mijnZorgRooster.Controllers;
using mijnZorgRooster.DAL;
using mijnZorgRooster.Models.DTO;
using mijnZorgRooster.Models.Entities;
using mijnZorgRooster.Services;
using Moq;
using Xunit;



namespace mijnZorgRooster.Tests.Controllers_Testen
{

    public partial class MedewerkerController_Test
    {

        [Fact]
        public async Task IndexTest()
        {
            //Arange
            var mockRepo = new Mock<IUnitOfWork>();
            mockRepo.Setup(repo => repo.MedewerkerRepository.GetAsync())
                .ReturnsAsync(GetMedewerkers());
            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = await controller.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<MedewerkerBasisDto>>(
                viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task DetailTest()
        {
            //Arange
            int MedewerkerID = 1;
            var mockRepo = new Mock<IUnitOfWork>();
            var medewerker = (GetMedewerkers().FirstOrDefault(m => m.MedewerkerID == MedewerkerID));
            mockRepo.Setup(repo => repo.MedewerkerRepository.GetByIdAsync(MedewerkerID))
                .ReturnsAsync(medewerker);
            var mockRepo2 = new Mock<ICalculationsService>();
            mockRepo2.Setup(repo => repo.BerekenLeeftijdInJaren(medewerker.Geboortedatum))
                .Returns(39);
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = await controller.Details(1);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<MedewerkerDetailDto>(
                viewResult.ViewData.Model);

            Assert.Equal(1, model.MedewerkerID);
            Assert.Equal("Francesca", model.Voornaam);
            Assert.Equal(DateTime.Parse("23-06-1979"), model.Geboortedatum);
            Assert.Equal(39, model.LeeftijdInJaren);

        }
        [Fact]
        public async Task DetailTestNotFound()
        {
            //Arange
            var mockRepo = new Mock<IUnitOfWork>();
            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = await controller.Details(null);

            //Assert
            var contentResult = Assert.IsType<NotFoundResult>(result);

        }
        [Fact]
        public async Task CreateTest()
        {
            //Arange
            var mockRepo = new Mock<IUnitOfWork>();
            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = controller.Create();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);
        }
        [Fact]
        public async Task CreateFilledInTest()
        {
            //Arange
            int MedewerkerID = 1;
            var mockRepo = new Mock<IUnitOfWork>();
            var medewerker = (GetMedewerkers().FirstOrDefault(m => m.MedewerkerID == MedewerkerID));
            mockRepo.Setup(repo => repo.MedewerkerRepository.Insert(medewerker));

            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = await controller.Create(medewerker);

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }


        //TODO: Deze werkt wss nog niet omdat er nog geen validatie uitgevoerd wordt op ModelIsValid

        [Fact]
        public async Task CreateFilledInInvalidTest()
        {
            //Arange
            int MedewerkerID = 1;
            var mockRepo = new Mock<IUnitOfWork>();
            var medewerker = (GetMedewerkers().FirstOrDefault(m => m.MedewerkerID == MedewerkerID));
            mockRepo.Setup(repo => repo.MedewerkerRepository.Insert(medewerker));

            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);
            controller.ModelState.AddModelError("Achternaam", "Required");

            //Act
            var result = await controller.Create(null);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(result);

        }
        [Fact]
        public async Task EditTest()
        {
            //Arange
            int MedewerkerID = 1;
            var mockRepo = new Mock<IUnitOfWork>();
            var medewerker = (GetMedewerkers().FirstOrDefault(m => m.MedewerkerID == MedewerkerID));
            var medewerkerRol = GetRollen();
            mockRepo.Setup(repo => repo.MedewerkerRepository.Update(medewerker));
            mockRepo.Setup(repo => repo.MedewerkerRepository.GetMedewerkerMetRollen(MedewerkerID)).ReturnsAsync(MyGetMedewerkerMetRollen());
            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = await controller.Edit(medewerker, medewerkerRol);
            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }
        [Fact]
        public async Task EditTestModelStateNotValid()
        {
            //Arange
            int MedewerkerID = 1;
            var mockRepo = new Mock<IUnitOfWork>();
            var medewerker = (GetMedewerkers().FirstOrDefault(m => m.MedewerkerID == MedewerkerID));
            var medewerkerRol = GetRollen();
            mockRepo.Setup(repo => repo.MedewerkerRepository.Update(medewerker));
            mockRepo.Setup(repo => repo.MedewerkerRepository.GetMedewerkerMetRollen(MedewerkerID)).ReturnsAsync(MyGetMedewerkerMetRollen());
            var mockRepo2 = new Mock<ICalculationsService>();
            var controller = new MedewerkersController(mockRepo2.Object, mockRepo.Object);

            //Act
            var result = await controller.Create(medewerker);
            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
        }
        private List<Medewerker> GetMedewerkers()
        {
            var medewerkers = new List<Medewerker>();
            medewerkers.Add(new Medewerker()
            {
                MedewerkerID = 1,
                Voornaam = "Francesca",
                Geboortedatum = DateTime.Parse("23-06-1979")

            });
            medewerkers.Add(new Medewerker()
            {
                MedewerkerID = 2,
                Voornaam = "Sylvester",
                Geboortedatum = DateTime.Parse("27-12-2002")

            });
            return medewerkers;
        }
        private Medewerker MyGetMedewerkerMetRollen()
        {
            Medewerker m = GetMedewerkers().ElementAt(0);
            List<MedewerkerRol> lmr = new List<MedewerkerRol>();
            lmr.Add(new MedewerkerRol()
            {
                RolId = 1,
                Rol = new Rol()
                {
                    Naam = "Beheerder",
                    RolID = 1
                },
                MedewerkerId = m.MedewerkerID,
                Medewerker = m

            });

            m.MedewerkersRollen = lmr;

            return m;

        }
        private List<int> GetRollen()
        {
            var rollen = new List<int>();
            rollen.Add(1);
            rollen.Add(2);
            return rollen;
        }
    }
}
